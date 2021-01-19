using CodeFluff.Extensions.IEnumerable;
using Dapper;
using StrengthIgniter.Dal.Common;
using StrengthIgniter.Models.Mesocycle;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StrengthIgniter.Dal.Mesocycle
{
    public class MesocycleDataAccess : DataAccessBase, IMesocycleDataAccess
    {
        #region CTOR
        public MesocycleDataAccess(DatabaseConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }
        #endregion

        public MesocycleModel Select(int mesocycleId, Guid userReference)
        {
            string sp = "dbo.spMesocycleSelectById";
            object parameters = new
            {
                MesocycleId = mesocycleId,
                UserReference = userReference
            };

            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    return dbConnection.QueryFirstOrDefault<MesocycleModel>(sp, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

        public Tuple<IEnumerable<MesocycleModel>, int> Filter(
            Guid userReference, string searchString,
            DateTime? startDate, DateTime endDate,
            int? offset, int? fetch)
        {
            string sp = "dbo.spMesocycleFilter";
            object parameters = new
            {
                UserReference = userReference,
                SearchString = searchString,
                StartDate = startDate,
                EndDate = endDate,
                Offset = offset,
                Fetch = fetch
            };

            IEnumerable<MesocycleModel> models = new MesocycleModel[] { };
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
                            models = reader.Read<MesocycleModel>().TryToArray();
                            total = reader.ReadSingle<int>();
                        }
                    }
                    return new Tuple<IEnumerable<MesocycleModel>, int>(models, total);
                }
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

        public int Insert(MesocycleModel mesocycle, IDbTransaction dbTransaction = null)
        {
            mesocycle.ValidateAndThrow();

            IDbConnection dbConnection = dbTransaction != null ? dbTransaction.Connection : GetConnection();
            string sp = "dbo.spMesocycleInsert";
            object parameters = new
            {
                Reference = mesocycle.Reference,
                UserReference = mesocycle.UserReference,
                StartDate = mesocycle.StartDate,
                EndDate = mesocycle.EndDate,
                Description = mesocycle.Description,
                Notes = mesocycle.Notes
            };

            try
            {
                int? id = dbConnection.QueryFirstOrDefault<int?>(sp, parameters, dbTransaction, commandType: CommandType.StoredProcedure);
                if (id.HasValue)
                {
                    return id.Value;
                }
                else throw new Exception("Failed to insert mesocycle.");
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

        public void Update(MesocycleModel mesocycle, IDbTransaction dbTransaction = null)
        {
            mesocycle.ValidateAndThrow();

            IDbConnection dbConnection = dbTransaction != null ? dbTransaction.Connection : GetConnection();
            string sp = "dbo.spMesocycleUpdate";
            object parameters = new
            {
                MesocycleId = mesocycle.MesocycleId,
                Reference = mesocycle.Reference,
                UserReference = mesocycle.UserReference,
                StartDate = mesocycle.StartDate,
                EndDate = mesocycle.EndDate,
                Description = mesocycle.Description,
                Notes = mesocycle.Notes
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
        
        public void Delete(
            int mesocycleId, Guid userReference, 
            bool deleteMicrocycles = false, bool deleteRecords = false, 
            IDbTransaction dbTransaction = null)
        {
            IDbConnection dbConnection = dbTransaction != null ? dbTransaction.Connection : GetConnection();
            string sp = "dbo.spMesocycleUpdate";
            object parameters = new
            {
                MesocycleId = mesocycleId,
                UserReference = userReference,
                DeleteMicrocycles = deleteMicrocycles,
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
