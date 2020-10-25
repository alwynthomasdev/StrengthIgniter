using StrengthIgniter.Core.Models.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Core.Services.Infrastructure
{
    public class FilterRequest
    {
        public string SearchString { get; set; }
        public Guid UserReference { get; set; }
        public int PageNo { get; set; }
        public int PageLength { get; set; }
    }

    public class FilterResponse<T>
        where T : ModelBase
    {

        public FilterResponse()
        {
            //reponse defaults
            Results = new List<T>();
            TotalMatches = 0;
        }
        public FilterResponse(IEnumerable<T> results, int totalMatches)
        {
            Results = results;
            TotalMatches = totalMatches;
        }

        public IEnumerable<T> Results { get; }
        public int TotalMatches { get; }
    }
}
