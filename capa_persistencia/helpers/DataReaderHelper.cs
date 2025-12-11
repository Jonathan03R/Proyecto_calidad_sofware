using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace capa_persistencia.helpers
{
    public static class DataReaderHelper
    {
        public static int? GetInt(SqlDataReader dr, string column)
            => dr.IsDBNull(dr.GetOrdinal(column)) ? (int?)null : dr.GetInt32(dr.GetOrdinal(column));

        public static decimal? GetDecimal(SqlDataReader dr, string column)
            => dr.IsDBNull(dr.GetOrdinal(column)) ? (decimal?)null : dr.GetDecimal(dr.GetOrdinal(column));

        public static DateTime? GetDate(SqlDataReader dr, string column)
            => dr.IsDBNull(dr.GetOrdinal(column)) ? (DateTime?)null : dr.GetDateTime(dr.GetOrdinal(column));

        public static string GetString(SqlDataReader dr, string column)
            => dr.IsDBNull(dr.GetOrdinal(column)) ? "" : dr.GetString(dr.GetOrdinal(column));

        public static string GetStringNull(SqlDataReader dr, string column)
            => dr.IsDBNull(dr.GetOrdinal(column)) ? null : dr.GetString(dr.GetOrdinal(column));
    }
}