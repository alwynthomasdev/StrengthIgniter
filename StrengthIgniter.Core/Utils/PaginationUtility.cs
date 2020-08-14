using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Core.Utils
{
    public interface IPaginationUtility
    {
        int? GetPageOffset(int? pageNo, int? pageLength);
    }

    public class PaginationUtility : IPaginationUtility
    {
        public const int DEFAULT_PAGE_LENGTH = 20;

        public int? GetPageOffset(int? pageNo, int? pageLength)
        {
            int? offset = null;
            if (pageNo.HasValue && pageLength.HasValue)
            {
                offset = 0;
                if (pageNo.Value > 1)
                {
                    if (pageLength.Value == 0)
                    {
                        //use default
                        pageLength = DEFAULT_PAGE_LENGTH;
                    }
                    offset = (pageNo - 1) * pageLength;
                }
            }
            return offset;
        }
    }
}
