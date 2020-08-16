using CodeFluff.Extensions.IEnumerable;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Ocsp;
using StrengthIgniter.Core.Data;
using StrengthIgniter.Core.Data.Infrastructure;
using StrengthIgniter.Core.Models;
using StrengthIgniter.Core.Services.Infrastructure;
using StrengthIgniter.Core.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StrengthIgniter.Core.Services
{

    public interface IExerciseService
    {
        IEnumerable<RecordModel> GetBestRecordPerDay(Guid exerciseReference, Guid userReference);
        ExerciseSearchResponse Search(ExerciseSearchRequest request);
        ExerciseRecordsResponse GetPagedExerciseRecords(ExerciseRecordsRequest request);
        ExerciseModel GetByReference(Guid reference);
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

        public ExerciseSearchResponse Search(ExerciseSearchRequest request)
        {
            try
            {
                int? offset = _PaginationUtility.GetPageOffset(request.PageNo, request.PageLength);
                Tuple<IEnumerable<ExerciseModel>, int> result = _ExerciseDal.Search(request.ExerciseName, offset, request.PageLength);

                ExerciseSearchResponse response = new ExerciseSearchResponse();

                if (result != null)
                {
                    response.Exercises = result.Item1;
                    response.TotalMatches = result.Item2;
                }

                return response;
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, request);
                throw serviceException;
            }
        }

        public ExerciseRecordsResponse GetPagedExerciseRecords(ExerciseRecordsRequest request)
        {
            try
            {
                int? offset = _PaginationUtility.GetPageOffset(request.PageNo, request.PageLength);
                Tuple<IEnumerable<RecordModel>, int> result = _RecordDal.GetByExerciseAndUser(request.ExerciseReference, request.UserReference, offset, request.PageLength);

                ExerciseRecordsResponse response = new ExerciseRecordsResponse();

                if (result != null)
                {
                    response.Records = result.Item1;
                    response.TotalMatches = result.Item2;
                }

                return response;
            }
            catch (Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, request);
                throw serviceException;
            }
        }

        public IEnumerable<RecordModel> GetBestRecordPerDay(Guid exerciseReference, Guid userReference)
        {
            try
            {
                IEnumerable<RecordModel> records = _RecordDal.GetByExerciseAndUser(exerciseReference, userReference);
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

        public ExerciseModel GetByReference(Guid reference)
        {
            try
            {
                return _ExerciseDal.GetByReference(reference);
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, new { reference });
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

    public class ExerciseSearchRequest
    {
        public string ExerciseName { get; set; }
        public int PageNo { get; set; }
        public int PageLength { get; set; }
    }

    public class ExerciseSearchResponse
    {
        public IEnumerable<ExerciseModel> Exercises { get; internal set; }
        public int TotalMatches { get; internal set; }
    }

    public class ExerciseRecordsRequest
    {
        public Guid ExerciseReference { get; set; }
        public Guid UserReference { get; set; }
        public int PageNo { get; set; }
        public int PageLength { get; set; }
    }

    public class ExerciseRecordsResponse
    {
        public IEnumerable<RecordModel> Records { get; internal set; }
        public int TotalMatches { get; internal set; }
    }

    #endregion

}
