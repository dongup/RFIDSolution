using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;


namespace RFIDSolution.Shared.Utils
{
    public static class EFCoreExtension
    {
        public static DataTable GetDataTable(this DbContext context, string sqlQuery,
                                     params object[] prms)
        {

            sqlQuery += " ";
            sqlQuery += string.Join(", ", prms.Select(x => $"'{x}'"));
            //Console.WriteLine($"[{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}]: Executing query: {sqlQuery}");

            DataTable dataTable = new DataTable();
            DbConnection connection = context.Database.GetDbConnection();
            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
            using (var cmd = dbFactory.CreateCommand())
            {
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlQuery;
                using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                {
                    adapter.SelectCommand = cmd;
                    adapter.Fill(dataTable);
                }
            }
            //Console.WriteLine($"[{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}]: Executed query: {sqlQuery}");

            return dataTable;
        }

        public static int ExecuteNonQuery(this DbContext context, string sqlQuery, params object[] prms)
        {
            int rowEffected = 0;
            sqlQuery += " ";
            sqlQuery += string.Join(", ", prms.Select(x => $"'{x}'"));

            DbConnection connection = context.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
            using (DbCommand cmd = dbFactory.CreateCommand())
            {
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlQuery;
                rowEffected = cmd.ExecuteNonQuery();
            }

            //var stats = connection.RetrieveStatistics();
            //var firstCommandExecutionTimeInMs = (long)stats["ExecutionTime"];

            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
            return rowEffected;
        }

        public static object ExecuteScalar(this DbContext context, string sqlQuery, params object[] prms)
        {
            object result = 0;
            sqlQuery += " ";
            sqlQuery += string.Join(", ", prms.Select(x => $"'{x}'"));

            DbConnection connection = context.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
            using (DbCommand cmd = dbFactory.CreateCommand())
            {
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlQuery;
                result = cmd.ExecuteScalar();
            }

            //var stats = connection.RetrieveStatistics();
            //var firstCommandExecutionTimeInMs = (long)stats["ExecutionTime"];

            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
            return result;
        }

        public static string ToJsonString(this DataTable dt)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return JsonConvert.SerializeObject(rows);
        }
    }
}
