using CodeFluff.Extensions.IEnumerable;
using Dapper;
using StrengthIgniter.Dal.Common;
using StrengthIgniter.Models.Record;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StrengthIgniter.Dal.Record
{
    

    public class RecordDataAccess : DataAccessBase, IRecordDataAccess
    {
        #region CTOR
        public RecordDataAccess(DatabaseConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }
        #endregion

        public IEnumerable<RecordModel> Select(int recordId, Guid userReference)
        {
            string sp = "spRecordSelectById";
            object parameters = new
            {
                RecordId = recordId,
                UserReference = userReference
            };

            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    return dbConnection.Query<RecordModel>(sp, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

        public Tuple<IEnumerable<RecordModel>, int> Filter(
            Guid userReference, 
            int? exerciseId, 
            DateTime? startDate, DateTime endDate,
            int? mesocycleId, int? microcycleId,
            int? offset, int? fetch)
        {
            string sp = "dbo.spRecordFilter";
            object parameters = new
            {
                UserReference = userReference,
                ExerciseId = exerciseId,
                StartDate = startDate,
                EndDate = endDate,
                MesocycleId = mesocycleId,
                MicrocycleId = microcycleId,
                Offset = offset,
                Fetch = fetch
            };

            IEnumerable<RecordModel> records = new RecordModel[] { };
            int total = 0;

            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    dbConnection.Open();
                    using (SqlMapper.GridReader reader = dbConnection.QueryMultiple(sp, parameters, commandType: CommandType.StoredProcedure))
                    {
                        if (reader != null)
                        {
                            records = reader.Read<RecordModel>().TryToArray();
                            total = reader.ReadSingle<int>();
                        }
                    }
                    return new Tuple<IEnumerable<RecordModel>, int>(records, total);
                }
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

        public int Insert(RecordModel record, IDbTransaction dbTransaction = null)
        {
            string sp = "dbo.spRecordInsert";
            object parameters = new
            {
                UserReference = record.UserReference,
                Reference = record.Reference,
                ExerciseId = record.ExerciseId,
                Date = record.Date,
                Reps = record.Reps,
                SetReference = record.SetReference,
                SetOrdinal = record.SetOrdinal,
                WeightKg = record.WeightKg,
                BodyweightKg = record.BodyweightKg,
                RPE = record.RPE,
                Notes = record.Notes,
                MesocycleId = record.MesocycleId,
                MicrocycleId = record.MicrocycleId
            };

            try
            {

                return ManageConnection<int>((con, trn) => {

                    int? recordId = con.QuerySingle<int?>(sp, parameters, trn, commandType: CommandType.StoredProcedure);
                    if (recordId.HasValue)
                    {
                        return recordId.Value;
                    }
                    else throw new Exception("Failed to insert record.");

                }, dbTransaction);

            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

        public void Update(RecordModel record, IDbTransaction dbTransaction = null)
        {
            string sp = "dbo.spRecordUpdate";
            object parameters = new
            {
                RecordId = record.RecordId,
                UserReference = record.UserReference,
                ExerciseId = record.ExerciseId,
                Date = record.Date,
                Reps = record.Reps,
                SetReference = record.SetReference,
                SetOrdinal = record.SetOrdinal,
                WeightKg = record.WeightKg,
                BodyweightKg = record.BodyweightKg,
                RPE = record.RPE,
                Notes = record.Notes
            };

            try
            {

                ManageConnection((con, trn) => {

                    con.Execute(sp, parameters, trn, commandType: CommandType.StoredProcedure);

                }, dbTransaction);

            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

        public void Delete(int recordId, Guid userReference, IDbTransaction dbTransaction = null)
        {
            string sp = "dbo.spRecordDelete";
            object parameters = new
            {
                RecordId = recordId,
                UserReference = userReference
            };

            try
            {
                ManageConnection((con, trn) => {

                    con.Execute(sp, parameters, trn, commandType: CommandType.StoredProcedure);

                }, dbTransaction);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }


    }
}
