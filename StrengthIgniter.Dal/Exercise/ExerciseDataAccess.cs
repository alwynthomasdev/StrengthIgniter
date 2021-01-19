using CodeFluff.Extensions.IEnumerable;
using Dapper;
using StrengthIgniter.Dal.Common;
using StrengthIgniter.Models.Exercise;
using System;
using System.Collections.Generic;
using System.Data;

namespace StrengthIgniter.Dal.Exercise
{
    
    public class ExerciseDataAccess : DataAccessBase, IExerciseDataAccess
    {
        #region CTOR
        public ExerciseDataAccess(DatabaseConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }
        #endregion

        public Tuple<IEnumerable<ExerciseModel>, int> Filter(string searchString, Guid userReference, int? offset, int? fetch)
        {
            string sp = "dbo.spExerciseFilter";
            var parameters = new
            {
                UserReference = userReference,
                SearchString = searchString,
                Offset = offset,
                Fetch = fetch
            };

            ExerciseModel[] exercises = null;
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
                            exercises = reader.Read<ExerciseModel>().TryToArray();
                            total = reader.ReadSingle<int>();
                        }
                    }
                    return new Tuple<IEnumerable<ExerciseModel>, int>(exercises, total);
                }
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

        public ExerciseModel Select(int id, Guid userReference)
        {
            string sp = "dbo.spExerciseSelectById";
            object parameters = new
            {
                ExerciseId = id,
                UserReference = userReference
            };

            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    return dbConnection.QueryFirstOrDefault<ExerciseModel>(sp, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

        public int Insert(ExerciseModel exercise, IDbTransaction dbTransaction = null)
        {
            //throw a validation exception if exercise not valid
            exercise.ValidateAndThrow();

            IDbConnection dbConnection = dbTransaction != null ? dbTransaction.Connection : GetConnection();
            string sp = "dbo.spExerciseInsert";
            object parameters = new
            {
                Reference = exercise.Reference != Guid.Empty ? exercise.Reference : Guid.NewGuid(),
                UserReference = exercise.UserReference,
                Name = exercise.Name
            };

            try
            {
                int? exerciseId = dbConnection.QueryFirstOrDefault<int?>(sp, parameters, dbTransaction, commandType: CommandType.StoredProcedure);
                if (exerciseId.HasValue)
                {
                    return exerciseId.Value;
                }
                else throw new Exception("Failed to insert exercise.");
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

        public void Update(ExerciseModel exercise, IDbTransaction dbTransaction = null)
        {
            //throw a validation exception if exercise not valid
            exercise.ValidateAndThrow();

            IDbConnection dbConnection = dbTransaction != null ? dbTransaction.Connection : GetConnection();
            string sp = "dbo.spExerciseUpdate";
            object parameters = new
            {
                ExerciseId = exercise.ExerciseId,
                UserReference = exercise.UserReference,
                Name = exercise.Name
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

        public void Delete(int id, Guid userReference, IDbTransaction dbTransaction = null)
        {
            IDbConnection dbConnection = dbTransaction != null ? dbTransaction.Connection : GetConnection();
            string sp = "dbo.spExerciseDelete";
            object parameters = new
            {
                ExerciseId = id,
                UserReference = userReference,
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
