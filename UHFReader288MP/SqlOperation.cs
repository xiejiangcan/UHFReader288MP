using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHFReader288MPDemo;

namespace UHFReader288MP
{
    /// <summary>
    /// 两个表格：
    /// CREATE TABLE 'base_epc'(
    /// 'id' int(11) NOT NULL AUTO_INCREAMENT,
    /// 'number' varchar(255) DEFAULT '' COMMENT '电子标签的epc编号',
    /// 'type' varchar(30) DEFAULT NULL '标签类型'，
    /// 'status' tinyint(2) DEFAULT '1' COMMENT '状态 1:正常 2:禁用',
    /// 'created_at' datetime DEFAULT NULL,
    /// PRIMARY KEY ('id')
    /// ) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8 COMMENT='基础电子标签表';
    /// 
    /// CREATE TABLE 'base_epc_log'(
    /// 'id' int(11) NOT NULL AUTO_INCREMENT,
    /// 'epc_id' int(10) DEFAULT 0 COMMENT '电子标签的EPC主键ID',
    /// 'type' varchar(30) DEFAULT 0 COMMENT '扫描类型 1出库 2入库',
    /// 'create_time' int(10) DEFAULT 0 COMMENT '执行时间戳',
    /// PRIMARY KEY ('id'),
    /// KEY 'idx_base_epc_epc_id' ('epc_id') USING BTREE
    /// ) ENGINE_InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=uft8 COMMENT='出库入库记录表'
    /// </summary>
    class SqlOperation
    {
        public string Addr { get; set; }
        public int Port { get; set; }
        public string Account { get; set; }
        public string PassWd { get; set; }
        public string DbName { get; set; }
        public string BaseTableName { get; set; }
        public string LogTableName { get; set; }

        MySqlConnection conn;

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
            string conStr = string.Format("server={0};port=3306;database={1};user={2};password={3}", Addr,DbName,Account,PassWd);
            try
            {
                conn = new MySqlConnection(conStr);
                if(conn.State != System.Data.ConnectionState.Open)
                    conn.Open();
            }
            catch (Exception ex)
            {
                Log.WriteError(ex.Message);
                return false;
            }

            return true;
        }

        public void EpcRecordInsert(string epc)
        {
            if (EpcRecordExist(epc))
            {
                //string time = new DateTimeOffset(DateTime.UtcNow).ToString();
                string cmd = string.Format("INSERT INTO {0} (number, create_time) VALUES (\"{1}\", now())", BaseTableName, epc);
                string res = MySQL_Insdelupd(cmd);
                if (!res.Contains("success"))
                {
                    throw new Exception(res);
                }
            }
        }

        public void EpcLabelInsert(string epc, int type)
        {
            switch (type)
            {
                case 0:
                    return;
                case 1:
                    if (!EpcLabelOutputExist(epc, ref type))
                    {
                        return;
                    }
                    break;
                case 2:
                    if (!EpcLabelInputExist(epc, ref type))
                    {
                        return;
                    }
                    break;
                default:
                    break;
            }

            // INSERT INTO ali_base_epc_log (epc_id, type, create_time) VALUES ({epc_id}, {type}, {create_time})
            long time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            string cmd = string.Format("INSERT INTO {0} (epc_id, type, create_time) VALUES (\"{1}\", {2}, {3})", LogTableName, epc, type, time);
            string res = MySQL_Insdelupd(cmd);
            if (!res.Contains("success"))
            {
                throw new Exception(res);
            }
        }

        public bool EpcLabelInputExist(string epc, ref int type)
        {
            string str = string.Format("SELECT * from {0} WHERE epc_id = \"{1}\"", LogTableName, epc);
            string record;
            DataSet ds = MySQL_Select(str, out record);
            if (record.Equals("success"))
            {
                if (ds.Tables[0].Rows.Count == 0)
                    return true;
                if (ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1]["type"].ToString() == "1")
                {
                    type = 3;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new Exception(record);
            }
        }

        public bool EpcRecordExist(string epc)
        {
            string str = string.Format("SELECT * from {0} WHERE number = \"{1}\"", BaseTableName, epc);
            string record;
            DataSet ds = MySQL_Select(str, out record);
            if (record.Equals("success"))
            {
                if (ds.Tables[0].Rows.Count == 0)
                    return true;
                else
                    return false;
            }
            else
            {
                throw new Exception(record);
            }
        }

        public bool EpcLabelOutputExist(string epc, ref int type)
        {
            string str = string.Format("SELECT * from {0} WHERE epc_id = \"{1}\"", LogTableName, epc);
            string record;
            DataSet ds = MySQL_Select(str, out record);
            if (record.Equals("success"))
            {
                if (ds.Tables[0].Rows.Count == 0)
                    return false;
                string lastType = ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1]["type"].ToString();
                if (lastType == "2" || lastType == "3")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new Exception(record);
            }
        }

        /// <summary>
        /// MySQL insert,delete,update
        /// </summary>
        /// <param name="sql">insert,delete,update statement</param>
        /// <returns>Success:success + Number of affected rows; Fail:reason</returns>
        public string MySQL_Insdelupd(string sql)
        {
            try
            {
                int num = 0;
                if (conn == null)
                {
                    return "Please open the database connection first";
                }
                if (conn.State == ConnectionState.Open)
                {
                    MySqlCommand sqlCommand = new MySqlCommand(sql, conn);
                    num = sqlCommand.ExecuteNonQuery();
                }
                else
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        return "Database connection closed";
                    }
                    if (conn.State == ConnectionState.Broken)
                    {
                        return "Database connection is destroyed";
                    }
                    if (conn.State == ConnectionState.Connecting)
                    {
                        return "The database is in connection";
                    }
                }
                return "success" + num;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        /// <summary>
        /// MySQL select
        /// </summary>
        /// <param name="sql">select statement</param>
        /// <param name="record">Success:success; Fail:reason</param>
        /// <returns>select result</returns>
        public DataSet MySQL_Select(string sql, out string record)
        {
            try
            {
                //储存数据的工具初始化
                DataSet dataSet = new DataSet();
                if (conn == null)
                {
                    record = "Please open the database connection first";
                    return dataSet;
                }
                if (conn.State == ConnectionState.Open)
                {
                    MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(sql, conn);
                    sqlDataAdapter.Fill(dataSet);
                    sqlDataAdapter.Dispose();
                    record = "success";
                    return dataSet;
                }
                if (conn.State == ConnectionState.Closed)
                {
                    record = "Database connection closed";
                    return dataSet;
                }
                if (conn.State == ConnectionState.Broken)
                {
                    record = "Database connection is destroyed";
                    return dataSet;
                }
                if (conn.State == ConnectionState.Connecting)
                {
                    record = "The database is in connection";
                    return dataSet;
                }
                record = "ERROR";
                return dataSet;
            }
            catch (Exception ex)
            {
                DataSet dataSet = new DataSet();
                record = ex.Message.ToString();
                return dataSet;
            }
        }
    }
}
