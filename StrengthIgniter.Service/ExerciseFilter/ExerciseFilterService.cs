using Microsoft.Extensions.Logging;
using StrengthIgniter.Dal.AuditEvent;
using StrengthIgniter.Dal.Common;
using StrengthIgniter.Dal.Exercise;
using StrengthIgniter.Models.Exercise;
using StrengthIgniter.Service.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.ExerciseFilter
{
    public class ExerciseFilterService : DataServiceBase, IExerciseFilterService
    {
        private readonly IExerciseDataAccess _ExerciseDataAccess;

        public ExerciseFilterService(
            IExerciseDataAccess exerciseDataAccess,
            //
            IDataAccessTransactionProvider transactionProvider,
            IAuditEventDataAccess auditEventDataAccess,
            //
            ILogger logger
        )
            : base(transactionProvider, auditEventDataAccess, logger)
        {
            _ExerciseDataAccess = exerciseDataAccess;
        }

        public FilterResponse<ExerciseModel> Filter(FilterRequest request)
        {
            try
            {
                Tuple<IEnumerable<ExerciseModel>, int> results = 
                    _ExerciseDataAccess.Filter(request.SearchString, request.UserReference, request.GetPageOffset(), request.PageLength);

                if (results != null)
                {
                    return new FilterResponse<ExerciseModel>(results.Item1, results.Item2);
                }
                // return empty filter response
                else return new FilterResponse<ExerciseModel>();

            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, "Filter", request);
                throw serviceException;
            }
        }
    }
}
