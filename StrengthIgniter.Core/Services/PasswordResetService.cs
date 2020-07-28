using CodeFluff.Extensions.IEnumerable;
using Microsoft.Extensions.Logging;
using StrengthIgniter.Core.Data;
using StrengthIgniter.Core.Data.Infrastructure;
using StrengthIgniter.Core.Models;
using StrengthIgniter.Core.Services.Infrastructure;
using StrengthIgniter.Core.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StrengthIgniter.Core.Services
{
    public interface IPasswordResetService
    {
        void RequestPasswordReset(string emailAddress);
        PasswordResetResponse GetSecurityQuestion(Guid passwordResetToken);
        PasswordResetResponse ResetPassword(ResetPasswordRequest request);
    }

    public class PasswordResetService : ServiceBase, IPasswordResetService
    {
        #region CTOR
        private readonly PasswordResetServiceConfig _Config;
        private readonly IUserDataAccess _UserDal;
        private readonly IHashUtility _HashUtility;
        private readonly IEmailUtility _EmailUtility;
        private readonly ITemplateUtility _TemplateUtility;

        public PasswordResetService(
            PasswordResetServiceConfig config,
            IUserDataAccess userDal,
            IHashUtility hashUtility,
            IEmailUtility emailUtility,
            ITemplateUtility templateUtility,
            //
            ILoggerFactory loggerFactory,
            IAuditEventDataAccess auditEventDataAccess,
            DatabaseConnectionFactory dbConnectionFactory
        )
            : base(auditEventDataAccess, loggerFactory.CreateLogger(typeof(PasswordResetService)), dbConnectionFactory.GetConnection)
        {
            _UserDal = userDal;
            _HashUtility = hashUtility;
            _EmailUtility = emailUtility;
            _TemplateUtility = templateUtility;
            _Config = config;
        }
        #endregion

        public void RequestPasswordReset(string emailAddress)
        {
            try
            {
                UserModel user = _UserDal.GetByEmailAddress(emailAddress);
                if (user != null)
                {
                    UserTokenModel userToken = new UserTokenModel
                    {
                        TokenReference = Guid.NewGuid(),
                        PurposeCode = "PasswordReset",
                        ExpiryDateTimeUtc = DateTime.UtcNow.AddHours(_Config.PasswordResetTokenExpiryHours),
                        IssuedDateTimeUtc = DateTime.UtcNow
                    };

                    string passwordResetUrl = string.Format(_Config.PasswordResetBaseUrl, userToken.TokenReference.ToString());
                    int auditId = 9;

                    using (IDbConnection dbConnection = GetConnection())
                    {
                        using (IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                        {
                            _UserDal.CreateUserToken(dbConnection, dbTransaction, user.Reference, userToken);
                            auditId = CreateTokenAudit(dbConnection, dbTransaction, user.UserId, userToken);

                            dbTransaction.Commit();
                        }
                    }

                    SendPasswordResetEmail(user, passwordResetUrl, auditId);
                }
                else //no account found with the email address
                {
                    SendNoAccountFoundEmail(emailAddress);
                }
            }
            catch (Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, new { emailAddress = emailAddress });
                throw serviceException;
            }
        }

        public PasswordResetResponse GetSecurityQuestion(Guid passwordResetToken)
        {
            try
            {
                PasswordResetResponse response = new PasswordResetResponse { PasswordResetToken = passwordResetToken, ResponseType = PasswordResetResponseType.PasswordResetTokenInvalid };
                UserModel user = _UserDal.GetByToken(passwordResetToken);
                if (user != null)
                {
                    //this token should definitely exist other wise gt by token would nt work
                    UserTokenModel userToken = user.Tokens.Where(x => x.TokenReference == passwordResetToken).First();
                    if (userToken.PurposeCode == "PasswordReset")
                    {
                        //if the token has expired
                        if (userToken.ExpiryDateTimeUtc < DateTime.UtcNow)
                        {
                            LogInfo($"Password reset attempted using expired token '{passwordResetToken}' for user with reference '{user.Reference}'.");
                            response.ResponseType = PasswordResetResponseType.PasswordResetTokenExpired;
                        }
                        else
                        {
                            UserSecurityQuestionAnswerModel question = GetRandomQuestion(user.SecurityQuestions.ToArray());
                            if (question != null)
                            {
                                response.UserSecurityQuestionAnswerReference = question.Reference;
                                response.QuestionText = question.QuestionText;
                                response.ResponseType = PasswordResetResponseType.PasswordResetTokenValid;
                            }
                            else// if you are unable to get another 
                            {
                                LogWarning($"User with reference '{user.Reference}' has failed to answer their secret questions too many times.");
                                response.ResponseType = PasswordResetResponseType.PasswordResetAttemptsMaxed;
                            }
                        }
                    }
                    else//incorrect purpose
                    {
                        LogInfo($"Password reset attempted using token '{passwordResetToken}' with purpose '{userToken.PurposeCode}' for user with reference '{user.Reference}'.");
                    }
                }
                else//could not find user token
                {
                    LogInfo($"Password reset attempt failed with token '{passwordResetToken}'.");
                }
                return response;
            }
            catch (Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, new { passwordResetToken = passwordResetToken });
                throw serviceException;
            }
        }

        public PasswordResetResponse ResetPassword(ResetPasswordRequest request)
        {
            try
            {
                UserModel user = _UserDal.GetByToken(request.PasswordResetToken);
                if (user != null)
                {
                    //token purpose re-check!
                    UserTokenModel token = user.Tokens.Where(x => x.TokenReference == request.PasswordResetToken).FirstOrDefault();
                    if (token.PurposeCode != "PasswordReset")
                    {
                        return new PasswordResetResponse
                        {
                            PasswordResetToken = request.PasswordResetToken,
                            ResponseType = PasswordResetResponseType.PasswordResetTokenInvalid
                        };
                    }

                    UserSecurityQuestionAnswerModel question = user.SecurityQuestions
                        .Where(x => x.Reference == request.PasswordResetToken)
                        .FirstOrDefault();

                    if (question != null)
                    {
                        PasswordResetResponse response;

                        using (IDbConnection dbConnection = GetConnection())
                        {
                            using (IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                            {
                                if (_HashUtility.Validate(request.SecurityQuestionAnswer, question.AnswerHash))
                                {
                                    string passwordHash = _HashUtility.Generate(request.NewPassword);

                                    _UserDal.UpdateSecurityQuestionAttempts(dbConnection, dbTransaction, user.UserId, null);
                                    _UserDal.UpdatePassword(dbConnection, dbTransaction, user.UserId, passwordHash);
                                    //might as well make sure this is set
                                    _UserDal.UpdateRegistrationValidated(dbConnection, dbTransaction, user.UserId);

                                    CreateAuditEvent(dbConnection, dbTransaction, AuditEventType.PasswordResetSuccess, user.UserId, "", null);
                                    response = new PasswordResetResponse
                                    {
                                        ResponseType = PasswordResetResponseType.PasswordReset
                                    };
                                }
                                else//failed to answer the question correctly
                                {
                                    _UserDal.UpdateSecurityQuestionAttempts(dbConnection, dbTransaction, user.UserId, question.FailedAnswerAttemptCount + 1);
                                    CreateAuditEvent(dbConnection, dbTransaction, AuditEventType.PasswordResetAttemptFailed, user.UserId, "", null);

                                    UserSecurityQuestionAnswerModel newQuestion = GetNewRandomQuestion(question, user.SecurityQuestions.ToArray());
                                    if (newQuestion != null)
                                    {
                                        response = new PasswordResetResponse
                                        {
                                            PasswordResetToken = request.PasswordResetToken,
                                            ResponseType = PasswordResetResponseType.PasswordResetAttemptFailed,
                                            UserSecurityQuestionAnswerReference = newQuestion.Reference,
                                            QuestionText = newQuestion.QuestionText,
                                            //SecurityQuestionFailedAttempts = newQuestion.FailedAttempts
                                        };
                                    }
                                    else//no more questions to ask
                                    {
                                        LogWarning($"User with reference '{user.Reference}' has failed to answer their Security questions too many times.");
                                        response = new PasswordResetResponse
                                        {
                                            PasswordResetToken = request.PasswordResetToken,
                                            ResponseType = PasswordResetResponseType.PasswordResetAttemptsMaxed
                                        };
                                    }
                                }

                                dbTransaction.Commit();
                            } // end transaction
                        } // end conection

                        return response;
                    }
                    else//could not get the question with the given uid
                    {
                        LogInfo($"Could not find Security question with unique id '{request.UserSecurityQuestionAnswerReference}' for user with reference '{user.Reference}'.");
                        return new PasswordResetResponse
                        {
                            PasswordResetToken = request.PasswordResetToken,
                            ResponseType = PasswordResetResponseType.PasswordResetAttemptFailed
                        };
                    }
                }
                else//could not get user with token
                {
                    LogInfo($"Password reset attempt failed with token '{request.PasswordResetToken}'.");
                    return new PasswordResetResponse
                    {
                        PasswordResetToken = request.PasswordResetToken,
                        ResponseType = PasswordResetResponseType.PasswordResetAttemptFailed
                    };
                }
            }
            catch (Exception ex)
            {
                ServiceException serviceException =  CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, request);
                throw serviceException;
            }
        }

        #region Private Methods

        private UserSecurityQuestionAnswerModel GetRandomQuestion(UserSecurityQuestionAnswerModel[] questions)
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            questions = questions.Where(x => x.FailedAnswerAttemptCount == questions.Select(y => y.FailedAnswerAttemptCount).Min()).TryToArray();
            if (questions.HasItems())
            {
                return questions[rnd.Next(0, questions.Length - 1)];
            }
            else return null;
        }

        private UserSecurityQuestionAnswerModel GetNewRandomQuestion(UserSecurityQuestionAnswerModel current, UserSecurityQuestionAnswerModel[] questions)
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            questions = questions.Where(x => x.Reference != current.Reference).TryToArray();
            if (questions != null)
            {
                questions = questions.Where(x => x.FailedAnswerAttemptCount == questions.Select(y => y.FailedAnswerAttemptCount).Min() && x.FailedAnswerAttemptCount < _Config.SecurityQuestionAnswerMaxAttempts).TryToArray();
                if (questions.HasItems())
                {
                    return questions[rnd.Next(0, questions.Length - 1)];
                }
            }
            return null;
        }

        private int CreateTokenAudit(IDbConnection dbConnection, IDbTransaction dbTransaction, int userId, UserTokenModel userToken)
        {
            IEnumerable<AuditEventItemModel> auditItems = new AuditEventItemModel[]
            {
                new AuditEventItemModel{ Key = "TokenReference", Value = userToken.TokenReference.ToString() },
                new AuditEventItemModel{ Key = "PurposeCode", Value = userToken.PurposeCode },
            };
            return CreateAuditEvent(dbConnection, dbTransaction, AuditEventType.TokenCreated, userId, "", auditItems);
        }

        private void SendPasswordResetEmail(UserModel user, string url, int relatedAuditId)
        {
            string messageBody = _TemplateUtility.Parse(_Config.PasswordResetEmailTemplatePath, new
            {
                Subject = _Config.PasswordResetEmailSubject,
                Username = user.Name,
                Url = url
            });

            EmailMessageModel msg = new EmailMessageModel
            {
                To = new string[] { user.EmailAddress },
                Subject = _Config.PasswordResetEmailSubject,
                Body = messageBody,
                IsHtml = true,
            };

            _EmailUtility.Send(msg);
            CreateEmailAuditEvent(msg, user.UserId, relatedAuditId);
        }

        private void SendNoAccountFoundEmail(string emailAddress)
        {
            string messageBody = _TemplateUtility.Parse(_Config.NoAccountFoundTemplatePath, new
            {
                Subject = _Config.NoAccountFoundEmailSubject,
            });

            EmailMessageModel msg = new EmailMessageModel
            {
                To = new string[] { emailAddress },
                Subject = _Config.NoAccountFoundEmailSubject,
                Body = messageBody,
                IsHtml = true,
            };
            _EmailUtility.Send(msg);
            CreateEmailAuditEvent(msg, null);
        }

        #endregion

    }

    #region Password Reset Service Models

    public class PasswordResetServiceConfig
    {
        public string PasswordResetEmailSubject { get; set; }
        public string PasswordResetEmailTemplatePath { get; set; }

        public string NoAccountFoundEmailSubject { get; set; }
        public string NoAccountFoundTemplatePath { get; set; }

        public int PasswordResetTokenExpiryHours { get; set; }
        public string PasswordResetBaseUrl { get; set; }
        public int SecurityQuestionAnswerMaxAttempts { get; set; }
    }

    public class ResetPasswordRequest
    {
        public Guid PasswordResetToken { get; set; }

        public Guid UserSecurityQuestionAnswerReference { get; set; }
        public string SecurityQuestionAnswer { get; set; }

        public string NewPassword { get; set; }
    }

    public class PasswordResetResponse
    {
        public Guid PasswordResetToken { get; internal set; }
        public PasswordResetResponseType ResponseType { get; internal set; }

        public Guid UserSecurityQuestionAnswerReference { get; internal set; }
        public string QuestionText { get; internal set; }
        //public int SecurityQuestionFailedAttempts { get; internal set; }
    }

    public enum PasswordResetResponseType
    {
        PasswordReset = 1,
        PasswordResetAttemptFailed = -1,
        PasswordResetAttemptsMaxed = -999,//no more security questions to ask

        PasswordResetTokenValid = 2,
        PasswordResetTokenInvalid = -2,
        PasswordResetTokenExpired = -3
    }

    #endregion

}
