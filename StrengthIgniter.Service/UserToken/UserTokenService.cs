using Microsoft.Extensions.Logging;
using StrengthIgniter.Dal.AuditEvent;
using StrengthIgniter.Dal.Common;
using StrengthIgniter.Dal.UserToken;
using StrengthIgniter.Models.UserToken;
using StrengthIgniter.Service.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.UserToken
{
    public class UserTokenService : DataServiceBase, IUserTokenService
    {
        private readonly UserTokenServiceConfig _Config;
        private readonly IUserTokenDataAccess _UserTokenDataAccess;

        public UserTokenService(
            UserTokenServiceConfig config,
            IUserTokenDataAccess userTokenDataAccess,
            //
            IDataAccessTransactionProvider transactionProvider,
            IAuditEventDataAccess auditEventDataAccess,
            //
            ILogger logger
        )
            :base(transactionProvider, auditEventDataAccess, logger)
        {
            _Config = config;
            _UserTokenDataAccess = userTokenDataAccess;
        }

        public Guid Create(Guid userReference, string purposeCode)
        {
            try
            {
                UserTokenModel userTokenModel = CreateUserToken(userReference, purposeCode, _Config.TokenExpiryHours);
                _UserTokenDataAccess.Insert(userTokenModel);
                return userTokenModel.Reference;
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, "Create", new { userReference, purposeCode });
                throw serviceException;
            }
        }

        public Guid Create(Guid userReference, string purposeCode, IDataAccessTransaction dataAccessTransaction)
        {
            try
            {
                UserTokenModel userTokenModel = CreateUserToken(userReference, purposeCode, _Config.TokenExpiryHours);
                _UserTokenDataAccess.Insert(userTokenModel, dataAccessTransaction.DbTransaction);
                return userTokenModel.Reference;
            }
            catch (Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, "Create", new { userReference, purposeCode });
                throw serviceException;
            }
        }

        public void DeleteToken(Guid tokenReference)
        {
            try
            {
                _UserTokenDataAccess.Delete(tokenReference);
            }
            catch(Exception ex)
            {
                ThrowServiceException(ex, "DeleteToken", new { tokenReference });
            }
        }

        public ValidateTokenResponse Validate(Guid tokenReference, string purposeCode)
        {
            try
            {
                UserTokenModel userToken = _UserTokenDataAccess.SelectByReference(tokenReference);
                if(userToken != null)
                {
                    if(userToken.PurposeCode == purposeCode)
                    {
                        if (!userToken.HasExpired())
                        {
                            // Token Passed All Checks...

                            // Delete the token
                            _UserTokenDataAccess.Delete(tokenReference);

                            return new ValidateTokenResponse 
                            { 
                                UserReference = userToken.UserReference,
                                ResponseTpe = ValidateTokenResponseTypeEnum.Valid 
                            };
                        }
                        else // Token Has Expired
                        {
                            LogInfo($"A UserToken with reference '{tokenReference}' failed to validate due to its expiry date/time of '{userToken.ExpiryDateTimeUtc}'.");
                            return new ValidateTokenResponse { ResponseTpe = ValidateTokenResponseTypeEnum.Expired };
                        }
                    }
                    else // Token Purpose Mismatch
                    {
                        LogWarning($"A UserToken with reference '{tokenReference}' failed to validate due to purpose mismatch of token purpose '{userToken.PurposeCode}' used for purpose '{purposeCode}'.");
                        return new ValidateTokenResponse { ResponseTpe = ValidateTokenResponseTypeEnum.InvalidPurpose };
                    }
                }
                else // Token Not Found
                {
                    LogWarning($"A UserToken with reference '{tokenReference}' was not found.");
                    return new ValidateTokenResponse { ResponseTpe = ValidateTokenResponseTypeEnum.NotFound };
                }
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, "Validate", new { tokenReference });
                throw serviceException;
            }
        }

        // Private Methods ...

        private UserTokenModel CreateUserToken(Guid userReference, string purpose, int expiryHours = 12)
        {
            return new UserTokenModel
            {
                UserReference = userReference,
                PurposeCode = purpose,
                Reference = Guid.NewGuid(),
                IssuedDateTimeUtc = DateTime.UtcNow,
                ExpiryDateTimeUtc = DateTime.UtcNow.AddHours(expiryHours)
            };
        }
    }
}
