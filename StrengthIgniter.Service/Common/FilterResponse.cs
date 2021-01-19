using StrengthIgniter.Models.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.Common
{
    public class FilterResponse<TModel>
        where TModel : ModelBase
    {

        public FilterResponse()
        {
            //reponse defaults
            Results = new List<TModel>();
            TotalMatches = 0;
        }
        public FilterResponse(IEnumerable<TModel> results, int totalMatches)
        {
            Results = results;
            TotalMatches = totalMatches;
        }

        public IEnumerable<TModel> Results { get; }
        public int TotalMatches { get; }
    }
}
