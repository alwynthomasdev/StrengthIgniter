using Microsoft.Extensions.Logging;
using StrengthIgniter.Core.Data;
using StrengthIgniter.Core.Data.Infrastructure;
using StrengthIgniter.Core.Models;
using StrengthIgniter.Core.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace StrengthIgniter.Core.Services
{
    public interface IRecordService
    {
        RecordModel GetByIdAndUser(int recordId, Guid userReference);
        void SaveRecord(RecordModel record);
        void DeleteRecord(int recordId, Guid userReference);
    }

    public class RecordService : ServiceBase, IRecordService
    {
        #region CTOR
        IRecordDataAccess _RecordDal;
        IExerciseDataAccess _ExerciseDal;

        public RecordService(
            IRecordDataAccess recordDal,
            IExerciseDataAccess exerciseDal,
            //
            IAuditEventDataAccess auditEventDal,
            ILogger<RecordService> logger,
            DatabaseConnectionFactory dbConnectionFactory
        )
            : base(auditEventDal, logger, dbConnectionFactory.GetConnection)
        {
            _RecordDal = recordDal;
            _ExerciseDal = exerciseDal;
        }
        #endregion

        public void SaveRecord(RecordModel record)
        {
            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    dbConnection.Open();
                    using (IDbTransaction dbTransaction= dbConnection.BeginTransaction())
                    {
                        if (record.RecordId > 0)
                        {
                            _RecordDal.Update(dbConnection, dbTransaction, record);
                            CreateAuditEvent(dbConnection, dbTransaction, AuditEventType.RecordUpdate, record.UserReference.Value, "Record Updated", new AuditEventItemModel[] {
                                new AuditEventItemModel{ Key ="RecordId", Value=record.RecordId.ToString() }
                            });
                        }
                        else
                        {
                            record.CreatedDateTimeUtc = DateTime.UtcNow;
                            ExerciseModel exercise = _ExerciseDal.GetByReference(record.ExerciseReference.Value);
                            record.ExerciseId = exercise.ExerciseId;

                            int id = _RecordDal.Insert(dbConnection, dbTransaction, record);
                            CreateAuditEvent(dbConnection, dbTransaction, AuditEventType.RecordInsert, record.UserReference.Value, "Record Inserted", new AuditEventItemModel[] {
                                new AuditEventItemModel{ Key ="RecordId", Value=id.ToString() }
                            });
                        }

                        dbTransaction.Commit();
                    }
                }
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, new { record });
                throw serviceException;
            }
        }

        public void DeleteRecord(int recordId, Guid userReference)
        {
            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    dbConnection.Open();
                    using (IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                    {
                        _RecordDal.Delete(dbConnection, dbTransaction, recordId, userReference);
                        CreateAuditEvent(dbConnection, dbTransaction, AuditEventType.RecordDelete, userReference, "Record deleted", new AuditEventItemModel[] {
                            new AuditEventItemModel{ Key="RecordId", Value = recordId.ToString() }
                        });
                        dbTransaction.Commit();
                    }
                }
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, new { recordId, userReference });
                throw serviceException;
            }
        }

        public RecordModel GetByIdAndUser(int recordId, Guid userReference)
        {
            try
            {
                return _RecordDal.GetByIdAndUser(recordId, userReference);
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, new { recordId, userReference });
                throw serviceException;
            }
        }

    }
}
