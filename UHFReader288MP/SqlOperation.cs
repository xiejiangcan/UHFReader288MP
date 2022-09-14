using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHFReader288MPDemo;

namespace UHFReader288MP
{
    class SqlOperation
    {
        public string Addr { get; set; }
        public int Port { get; set; }
        public string Account { get; set; }
        public string PassWd { get; set; }
        public string DbName { get; set; }
        public string TableName { get; set; }

        SqlConnection conn = new SqlConnection();

        private static SqlOperation instance = null;
        private static object obj = new object();

        /// <summary>
        /// 单例实现
        /// </summary>
        public static SqlOperation Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new SqlOperation();
                        }
                    }
                }
                return instance;
            }
        }

        private SqlOperation()
        {

        }

        public bool ConnectSql()
        {
            string conStr = string.Format("server = {0}; user = {1}; database = {2}; port = {3}; password = {4}", Addr, Account, DbName, Port, PassWd);
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                Log.WriteError(ex.Message);
                return false;
            }

            return true;
        }

        public bool ExecuteReader(string cmdStr)
        {
            SqlCommand cmd = new SqlCommand(cmdStr, conn);
            bool hasItem = false;
            try
            {
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Log.WriteLog(rdr[0] + " " + rdr[1]);
                    hasItem = true;
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                Log.WriteError(ex.Message);
                return false;
            }
            return hasItem;
        }

        public bool ExecuteNonQuery(string cmdStr)
        {
            SqlCommand cmd = new SqlCommand(cmdStr, conn);
            bool result = false;
            try
            {
                int res = cmd.ExecuteNonQuery();
                result = (res != -1);
            }
            catch (Exception ex)
            {
                Log.WriteError(ex.Message);
                return false;
            }

            return result;
        }

        public object ExecuteScalar(string cmdStr)
        {
            object res = null;
            SqlCommand cmd = new SqlCommand(cmdStr, conn);
            try
            {
                res = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Log.WriteError(ex.Message);
                return res;
            }

            return res;
        }
    }
}
