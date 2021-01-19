using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace StrengthIgniter.Dal.Common
{
    public static class DataAccessExtentions
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> collection)
        {
            DataTable dtResult = new DataTable();
            Type typeOfT = typeof(T);
            PropertyInfo[] aryPropertyInfo = typeOfT.GetProperties();

            // define the data tables columns
            foreach (PropertyInfo pi in aryPropertyInfo)
            {
                dtResult.Columns.Add(pi.Name, pi.PropertyType);
            }

            // add each item in the collection to the data table
            foreach (T item in collection)
            {
                DataRow dr = dtResult.NewRow();

                // map item properties to data table columns
                foreach (PropertyInfo pi in aryPropertyInfo)
                {
                    object value = pi.GetValue(item);
                    if (value != null)
                    {
                        dr[pi.Name] = value;
                    }
                }

                dtResult.Rows.Add(dr);
            }

            return dtResult;
        }

    }
}
