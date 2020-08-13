using CodeFluff.Extensions.IEnumerable;
using Microsoft.Extensions.Logging;
using StrengthIgniter.Core.Data;
using StrengthIgniter.Core.Data.Infrastructure;
using StrengthIgniter.Core.Models;
using StrengthIgniter.Core.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace StrengthIgniter.Core.Services
{

    public interface IExerciseService
    {
        IEnumerable<RecordModel> GetBestRecordPerDay(Guid exerciseReference, Guid userReference);
    }

    public class ExerciseService : ServiceBase, IExerciseService
    {
        #region CTOR
        IExerciseDataAccess _ExerciseDal;
        IRecordDataAccess _RecordDal;

        public ExerciseService(
            IExerciseDataAccess exerciseDal,
            IRecordDataAccess recordDal,
            //
            IAuditEventDataAccess auditEventDal,
            ILoggerFactory loggerFactory,
            DatabaseConnectionFactory dbConnectionFactory
        )
            : base(auditEventDal, loggerFactory.CreateLogger(typeof(RecordImportService)), dbConnectionFactory.GetConnection)
        {
            _ExerciseDal = exerciseDal;
            _RecordDal = recordDal;
        }
        #endregion

        public IEnumerable<RecordModel> GetBestRecordPerDay(Guid exerciseReference, Guid userReference)
        {
            IEnumerable<RecordModel> records = _RecordDal.GetByExerciseAndUser(exerciseReference, userReference);
            if(records.HasItems())
            {
                //get the best lift for each day
                return records
                    .GroupBy(x => GetIso8601WeekOfYear(x.Date))
                    .Select(grp => grp.OrderByDescending(rec => rec.e1RM).FirstOrDefault() );
            }
            return records;//empty
        }

        //TODO: move this 
        private int GetIso8601WeekOfYear(DateTime dt)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(dt);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                dt = dt.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

    }
}
