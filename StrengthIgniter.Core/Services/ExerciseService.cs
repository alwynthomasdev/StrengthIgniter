using CodeFluff.Extensions.IEnumerable;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Ocsp;
using StrengthIgniter.Core.Data;
using StrengthIgniter.Core.Data.Infrastructure;
using StrengthIgniter.Core.Models;
using StrengthIgniter.Core.Services.Infrastructure;
using StrengthIgniter.Core.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StrengthIgniter.Core.Services
{

    public interface IExerciseService
    {
        ExerciseModel GetExercise(Guid reference, Guid userReference);
        FilterResponse<ExerciseModel> FilterExercises(FilterRequest request);
        FilterResponse<RecordModel> FilterExerciseRecords(ExerciseRecordSearchRequest request);
        //TODO: rename this or make it obsolete
        IEnumerable<RecordModel> GetBestRecordPerDay(Guid exerciseReference, Guid userReference);

        ExerciseModel SaveExercise(ExerciseModel model);
        void DeleteExercise(Guid reference, Guid userReference);
    }

    public class ExerciseService : ServiceBase, IExerciseService
    {
        #region CTOR
        private readonly IExerciseDataAccess _ExerciseDal;
        private readonly IRecordDataAccess _RecordDal;
        private readonly IPaginationUtility _PaginationUtility;

        public ExerciseService(
            IExerciseDataAccess exerciseDal,
            IRecordDataAccess recordDal,
            IPaginationUtility paginationUtility,
            //
            IAuditEventDataAccess auditEventDal,
            ILogger<ExerciseService> logger,
            DatabaseConnectionFactory dbConnectionFactory
        )
            : base(auditEventDal, logger, dbConnectionFactory.GetConnection)
        {
            _ExerciseDal = exerciseDal;
            _RecordDal = recordDal;
            _PaginationUtility = paginationUtility;
        }
        #endregion

        public FilterResponse<ExerciseModel> FilterExercises(FilterRequest request)
        {
            try
            {
                int? offset = _PaginationUtility.GetPageOffset(request.PageNo, request.PageLength);
                Tuple<IEnumerable<ExerciseModel>, int> result = _ExerciseDal.Filter(request.SearchString, request.UserReference, offset, request.PageLength);

                if (result != null)
                {
                     return new FilterResponse<ExerciseModel>(result.Item1, result.Item2);
                }
                else return new FilterResponse<ExerciseModel>();
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, request);
                throw serviceException;
            }
        }

        public FilterResponse<RecordModel> FilterExerciseRecords(ExerciseRecordSearchRequest request)
        {
            try
            {
                int? offset = _PaginationUtility.GetPageOffset(request.PageNo, request.PageLength);
                Tuple<IEnumerable<RecordModel>, int> result = _RecordDal.FilterByExercise(request.ExerciseReference, request.UserReference, offset, request.PageLength);

                if (result != null)
                {
                    return new FilterResponse<RecordModel>(result.Item1, result.Item2);
                }
                else return new FilterResponse<RecordModel>();
            }
            catch (Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, request);
                throw serviceException;
            }
        }

        [Obsolete]
        public IEnumerable<RecordModel> GetBestRecordPerDay(Guid exerciseReference, Guid userReference)
        {
            try
            {
                IEnumerable<RecordModel> records = _RecordDal.SelectByExercise(exerciseReference, userReference);
                if (records.HasItems())
                {
                    //get the best lift for each day
                    return records
                        .GroupBy(x => GetWeekOfYear(x.Date))
                        .Select(grp => grp.OrderByDescending(rec => rec.e1RM).FirstOrDefault())
                        .OrderBy(x => x.Date);
                }
                return records;//empty
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, new { exerciseReference, userReference });
                throw serviceException;
            }
        }

        public ExerciseModel GetExercise(Guid reference, Guid userReference)
        {
            try
            {
                return _ExerciseDal.Select(reference, userReference);
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, new { reference });
                throw serviceException;
            }
        }

        public ExerciseModel SaveExercise(ExerciseModel exercise)
        {
            try
            {
                ExercieModelValidator validatior = new ExercieModelValidator();
                validatior.ValidateAndThrow(exercise);

                using (IDbConnection dbConnection = GetConnection())
                {
                    dbConnection.Open();
                    using (IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                    {
                        if (exercise.Reference == Guid.Empty)
                        {
                            exercise.Reference = Guid.NewGuid();
                            _ExerciseDal.Insert(dbConnection, dbTransaction, exercise);
                            CreateAuditEvent(dbConnection, dbTransaction, AuditEventType.ExerciseInsert, exercise.UserReference, "Exercise created.", new AuditEventItemModel[] {
                                new AuditEventItemModel { Key="Reference", Value=exercise.Reference.ToString() }
                            });
                        }
                        else
                        {
                            _ExerciseDal.Update(dbConnection, dbTransaction, exercise);
                            CreateAuditEvent(dbConnection, dbTransaction, AuditEventType.ExerciseUpdate, exercise.UserReference, "Exercise updated.", new AuditEventItemModel[] {
                                new AuditEventItemModel { Key="Reference", Value=exercise.Reference.ToString() }
                            });
                        }
                        dbTransaction.Commit();
                        return exercise;
                    }
                }
            }
            catch (Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, exercise);
                throw serviceException;
            }
        }

        public void DeleteExercise(Guid reference, Guid userReference)
        {
            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    dbConnection.Open();
                    using (IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                    {
                        _ExerciseDal.Delete(dbConnection, dbTransaction, reference, userReference);
                        CreateAuditEvent(AuditEventType.ExerciseDelete, userReference, "Exercise deleted.", new AuditEventItemModel[] {
                            new AuditEventItemModel { Key="Reference", Value=reference.ToString() }
                        });
                        dbTransaction.Commit();
                    }
                }
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, new { reference, userReference });
                throw serviceException;
            }
        }

        #region Private Helpers

        //TODO: should this be in utility / helper ???
        private int GetWeekOfYear(DateTime dt)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(dt);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                dt = dt.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        #endregion

    }

    #region ExerciseService Models

    public class ExerciseRecordSearchRequest : FilterRequest
    {
        public Guid ExerciseReference { get; set; }
    }

    #endregion

}
