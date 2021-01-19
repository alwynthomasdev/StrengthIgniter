using CodeFluff.Extensions.IEnumerable;
using Dapper;
using StrengthIgniter.Dal.Common;
using StrengthIgniter.Models.Microcycle;
using System;
using System.Collections.Generic;
using System.Data;

namespace StrengthIgniter.Dal.Microcycle
{
    

    public class MicrocycleDataAccess : DataAccessBase, IMicrocycleDataAccess
    {
        #region CTOR
        public MicrocycleDataAccess(DatabaseConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }
        #endregion

        public MicrocycleModel Select(int microcycleId, Guid userReference)
        {
            string sp = "dbo.spMicrocycleSelectById";
            object parameters = new
            {
                MicrocycleId = microcycleId,
                UserReference = userReference
            };

            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    return dbConnection.QueryFirstOrDefault<MicrocycleModel>(sp, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

        public Tuple<IEnumerable<MicrocycleModel>, int> Filter(
            Guid userReference, string searchString,
            DateTime? startDate, DateTime endDate,
            int? offset, int? fetch)
        {
            string sp = "dbo.spMicrocycleFilter";
            object parameters = new
            {
                UserReference = userReference,
                SearchString = searchString,
                StartDate = startDate,
                EndDate = endDate,
                Offset = offset,
                Fetch = fetch
            };

            IEnumerable<MicrocycleModel> models = new MicrocycleModel[] { };
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
                            models = reader.Read<MicrocycleModel>().TryToArray();
                            total = reader.ReadSingle<int>();
                        }
                    }
                    return new Tuple<IEnumerable<MicrocycleModel>, int>(models, total);
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

        public int Insert(MicrocycleModel microcycle, IDbTransaction dbTransaction = null)
        {
            microcycle.ValidateAndThrow();

            IDbConnection dbConnection = dbTransaction != null ? dbTransaction.Connection : GetConnection();
            string sp = "dbo.spMicrocycleInsert";
            object parameters = new
            {
                Reference = microcycle.Reference,
                UserReference = microcycle.UserReference,
                StartDate = microcycle.StartDate,
                EndDate = microcycle.EndDate,
                Description = microcycle.Description,
                Notes = microcycle.Notes,
                MesocycleId = microcycle.MesocycleId
            };

            try
            {
                int? id = dbConnection.QueryFirstOrDefault<int?>(sp, parameters, dbTransaction, commandType: CommandType.StoredProcedure);
                if (id.HasValue)
                {
                    return id.Value;
                }
                else throw new Exception("Failed to insert microcycle.");
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

        public void Update(MicrocycleModel microcycle, IDbTransaction dbTransaction = null)
        {
            microcycle.ValidateAndThrow();

            IDbConnection dbConnection = dbTransaction != null ? dbTransaction.Connection : GetConnection();
            string sp = "dbo.spMicrocycleUpdate";
            object parameters = new
            {
                MicrocycleId = microcycle.MesocycleId,
                Reference = microcycle.Reference,
                UserReference = microcycle.UserReference,
                StartDate = microcycle.StartDate,
                EndDate = microcycle.EndDate,
                Description = microcycle.Description,
                Notes = microcycle.Notes,
                MesocycleId = microcycle.MesocycleId
            };

            try
            {
                dbConnection.Execute(sp, parameters, dbTransaction, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

        public void Delete(int microcycleId, Guid userReference, bool deleteRecords = false, IDbTransaction dbTransaction = null)
        {
            IDbConnection dbConnection = dbTransaction != null ? dbTransaction.Connection : GetConnection();
            string sp = "dbo.spMicrocycleDelete";
            object parameters = new
            {
                MicrocycleId = microcycleId,
                UserReference = userReference,
                DeleteRecords = deleteRecords
            };

            try
            {
                dbConnection.Execute(sp, parameters, dbTransaction, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

    }

}
