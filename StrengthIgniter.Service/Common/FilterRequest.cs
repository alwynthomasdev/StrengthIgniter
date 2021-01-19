using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.Common
{
    public class FilterRequest : RequestBase
    {
        //TODO: is this needed publicly ???
        private const int DEFAULT_PAGE_LENGTH = 20;

        public string SearchString { get; set; }
        public Guid UserReference { get; set; }
        public int? PageNo { get; set; }
        public int? PageLength { get; set; }


        //TODO: revise this ???
        public int? GetPageOffset()
        {
            int? offset = null;
            if (PageNo.HasValue && PageLength.HasValue)
            {
                offset = 0;
                if (PageNo.Value > 1)
                {
                    if (PageLength.Value == 0)
                    {
                        //use default
                        PageLength = DEFAULT_PAGE_LENGTH;
                    }
                    offset = (PageNo - 1) * PageLength;
                }
            }
            return offset;
        }

    }



}
