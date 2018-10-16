using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Xml;
using Oracle.DataAccess;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace DataCon
{
    public class Servers : IOAGetData
    {

        #region 全局变量
        string publicKeys = "fsd2rmyy";//秘钥
        string error = "";
        public string userid = ""; //服务器连接用户名
        public string userpwd = ""; //服务器连接密码
        public string DatabaseName = ""; //服务器连接数据库名称
        public string ServerIP = ""; //服务器IP
        private GetClass newClass = new GetClass();
        DataSet dataSet = new DataSet();
        SqlConnection SvrCon = null; //SqlServer 打开连接
        OracleConnection OraCon = null; //Oracle 打开连接
        #endregion

        #region 数据库连接定义
        /// <summary>
        /// sql连接设置
        /// </summary>
        /// <param name="SqlType">连接类型</param>
        private string dataConnect(string sqlType)
        {
            try
            {
                string strCon = "";
                switch (sqlType.Trim().ToUpper())
                {
                    case "SQLSERVER":
                        strCon = "Server = " + ServerIP + ";DataBase = " + DatabaseName + ";uid = " + userid + ";pwd = " + userpwd;
                        //strCon = "Server = 200.10.6.248;DataBase = NQCProject;uid = sa;pwd = gwi";
                        SvrCon = new SqlConnection(strCon);
                        break;
                    case "ORACLE":
                        strCon = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + ServerIP +
                        ")(PORT=1521" + "))(CONNECT_DATA=(SERVICE_NAME=" + DatabaseName + ")))" +
                        ";User ID=" + userid +
                        ";Password=" + userpwd + ";";
                        OraCon = new OracleConnection(strCon);
                        break;
                }
                return strCon;
            }
            catch (Exception e)
            {
                string error = e.Message;
                return error;
            }
        }
        #endregion

        #region 纯sql执行函数
        /// <summary>
        /// sql查询执行
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <param name="Dates">返回数据</param>
        /// <returns>返回信息</returns>
        public string GetDates(string strSql, ref DataSet Dates, ref string sqlType)
        {
            try
            {
                error = GetConfig(ref sqlType);
                DataSet dataSet = new DataSet();
                int rowsNum = 0;
                switch (sqlType.Trim().ToUpper())
                {
                    case "SQLSERVER":
                        sqlType = strSql;
                        SvrCon.Open();
                        SqlDataAdapter SvrDataAdp = new SqlDataAdapter(strSql, SvrCon);
                        SqlCommandBuilder SvrBuilder = new SqlCommandBuilder(SvrDataAdp);
                        rowsNum = SvrDataAdp.Fill(Dates);
                        SvrCon.Close();
                        break;
                    case "ORACLE":
                        OraCon.Open();
                        OracleDataAdapter OraDataAdp = new OracleDataAdapter(strSql, OraCon);
                        OracleCommandBuilder OraBuilder = new OracleCommandBuilder(OraDataAdp);
                        rowsNum = OraDataAdp.Fill(Dates);
                        OraCon.Close();
                        break;
                    default: return "找不到所需的数据库连接类型";//找不到所需的数据库连接类型
                }
                return rowsNum.ToString();
            }
            catch (Exception exception)
            {
                return error + exception.Message;
            }
        }
        /// <summary>
        /// sql 更新执行
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <returns>返回信息</returns>
        public string SetDates(string strSql, string sqlType)
        {
            try
            {
                GetConfig(ref sqlType);
                string message;
                object obj;
                switch (sqlType.Trim().ToUpper())
                {
                    case "SQLSERVER":
                        SvrCon.Open();
                        strSql += " SELECT @@IDENTITY AS returnName;";//返回第一行数据
                        SqlDataAdapter SvrDataAdp = new SqlDataAdapter(strSql, SvrCon);
                        SvrDataAdp.UpdateCommand = new SqlCommand(strSql, SvrCon);
                        //SqlCommandBuilder SvrBuilder = new SqlCommandBuilder(SvrDataAdp);
                        obj = SvrDataAdp.UpdateCommand.ExecuteScalar();
                        SvrCon.Close();
                        break;
                    case "ORACLE":
                        OraCon.Open();
                        OracleDataAdapter OraDataAdp = new OracleDataAdapter(strSql, OraCon);
                        OraDataAdp.UpdateCommand = new OracleCommand(strSql, OraCon);
                        //OracleCommandBuilder OraBuilder = new OracleCommandBuilder(OraDataAdp);
                        obj = OraDataAdp.UpdateCommand.ExecuteNonQuery();
                        OraCon.Close();
                        break;
                    default: return "-1";//找不到所需的数据库连接类型
                }
                
                message = obj.ToString();
                return message;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }
        #endregion

        #region 数据库查询函数
        /// <summary>
        /// Sql查询函数
        /// </summary>
        /// <param name="variables">获取列名</param>
        /// <param name="dbName">获取表名</param>
        /// <param name="VarMen">条件列名</param>
        /// <param name="Values">条件列值</param>
        /// <param name="Equals">条件类型：true--"=" false--"like"</param>
        /// <param name="Andor">并交关系：true--"and" false--"or"</param>
        /// <param name="orderValues">排列名称</param>
        /// <param name="newDates">返回数据表</param>
        /// <returns>返回信息</returns>
        public string DateSelect(string[] variables, string[] dbName, string[] VarMen, string[] Values, bool[] Equals, bool[] Andor, string[] orderValues, ref string sqlType, ref DataSet newDates)
        {
            return dateSelect(variables, dbName, VarMen, Values, Equals, Andor, orderValues, ref sqlType, ref newDates);
        }
        private string dateSelect(string[] variables, string[] dbName, string[] VarMen, string[] Values, bool[] VEquals, bool[] Andor, string[] orderValues, ref string sqlType, ref DataSet newDates)
        {
            try
            {
                string sqlSel = dateSelect(variables, dbName, VarMen, Values, VEquals, Andor, orderValues);
                if (string.IsNullOrEmpty(sqlSel)) return "查询列名或数据库名不能为空";
                else return GetDates(sqlSel, ref newDates, ref sqlType);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        /// <summary>
        /// 查询语句生成
        /// </summary>
        /// <param name="variables"></param>
        /// <param name="dbName"></param>
        /// <param name="VarMen"></param>
        /// <param name="Values"></param>
        /// <param name="VEquals"></param>
        /// <param name="Andor"></param>
        /// <param name="orderValues"></param>
        /// <returns></returns>
        private string dateSelect(string[] variables, string[] dbName, string[] VarMen, string[] Values, bool[] VEquals, bool[] Andor, string[] orderValues)
        {
            if (variables != null && dbName != null && variables.Length > 0)
            {
                string SqlSelect = "";
                SqlSelect += "select ";
                SqlSelect += variables[0];
                if (variables.GetLength(0) > 1)
                    for (int i = 1; i < variables.GetLength(0); i++)
                        SqlSelect += "," + variables[i];
                SqlSelect += " from ";
                if (dbName.GetLength(0) > 0)
                    SqlSelect += dbName[0];
                if (dbName.GetLength(0) > 1)
                    for (int i = 1; i < dbName.GetLength(0); i++)
                        SqlSelect += "," + dbName[i];

                if (VarMen != null && VarMen.Length > 0)
                {
                    SqlSelect += " where ";
                    for (int i = 0; i < VarMen.Length; i++)
                    {
                        string tmpSql = "";
                        string tmpMen = "";
                        string tmpVal = "";
                        tmpMen = VarMen[i];
                        tmpVal = Values[i];
                        if (VEquals != null)
                        {
                            switch (VEquals[i])
                            {
                                case false:
                                    tmpSql = tmpMen + " like " + tmpVal;
                                    break;
                                case true:
                                    tmpSql = tmpMen + " = " + tmpVal;
                                    break;
                            }
                            if (Andor != null && i > 0)
                                switch (Andor[i - 1])
                                {
                                    case true:
                                        SqlSelect += " and " + tmpSql;
                                        break;
                                    case false:
                                        SqlSelect += " or " + tmpSql;
                                        break;
                                    default: break;
                                }
                            else
                                SqlSelect += tmpSql;
                        }
                        else
                        {
                            tmpSql = tmpMen + " = " + tmpVal;
                            if (i > 0)
                                SqlSelect += " and " + tmpSql;
                            else
                                SqlSelect += " " + tmpSql;
                        }
                    }
                }
                if (orderValues != null && orderValues.GetLength(0) > 0)
                    for (int i = 0; i < orderValues.GetLength(0); i++)
                        if (i == 0)
                            SqlSelect += " order by " + orderValues[0];
                        else
                            SqlSelect += " , " + orderValues[i];
                return SqlSelect;
            }
            else return "";
        }
        #endregion

        #region 数据库插入函数
        /// <summary>
        /// 数据库插入数据函数
        /// </summary>
        /// <param name="dbName">数据库名称</param>
        /// <param name="columnsName">插入列名</param>
        /// <param name="values">插入值</param>
        /// <returns>返回状态</returns>
        public string DateInsert(string dbName, string[] columnsName, string[] values, string sqlType)
        {
            return dateInsert(dbName, columnsName, values, sqlType);
        }
        /// <summary>
        /// 数据库插入数据函数
        /// </summary>
        /// <param name="dbName">数据库名称</param>
        /// <param name="columnsName">插入列名</param>
        /// <param name="values">插入值</param>
        /// <returns>返回状态</returns>
        private string dateInsert(string dbName, string[] columnsName, string[] values, string sqlType)
        {
            string sign = "se";
            try
            {
                if (values != null && !string.IsNullOrEmpty(dbName) && values.Length > 0)
                {
                    GetConfig(ref sqlType);
                    string strSql = "insert into " + dbName;
                    if (columnsName == null && columnsName.Length <= 0)
                        strSql += " Values ";
                    else
                    {
                        strSql += " ( " + columnsName[0];
                        if (columnsName.GetLength(0) > 1)
                            for (int i = 1; i < columnsName.GetLength(0); i++)
                                strSql += "," + columnsName[i];
                        strSql += " ) Values ";
                    }
                    strSql += "( " + values[0];
                    if (values.GetLength(0) > 1)
                        for (int i = 1; i < values.GetLength(0); i++)
                            strSql += "," + values[i];
                    strSql += ")";
                    string message;
                    object obj;
                    switch (sqlType.Trim().ToUpper())
                    {
                        case "SQLSERVER":
                            SvrCon.Open();
                            strSql += " SELECT @@IDENTITY AS returnName;";//返回第一行数据
                            SqlDataAdapter SvrDataAdp = new SqlDataAdapter(strSql, SvrCon);
                            SvrDataAdp.InsertCommand = new SqlCommand(strSql, SvrCon);
                            //SqlCommandBuilder SvrBuilder = new SqlCommandBuilder(SvrDataAdp);
                            obj = SvrDataAdp.InsertCommand.ExecuteScalar();
                            SvrCon.Close();
                            break;
                        case "ORACLE":
                            OraCon.Open();
                            OracleDataAdapter OraDataAdp = new OracleDataAdapter(strSql, OraCon);
                            OraDataAdp.InsertCommand = new OracleCommand(strSql, OraCon);
                            //OracleCommandBuilder OraBuilder = new OracleCommandBuilder(OraDataAdp);
                            obj = OraDataAdp.InsertCommand.ExecuteNonQuery();
                            OraCon.Close();
                            break;
                        default: return "-1";//找不到所需的数据库连接类型
                    }
                    sign = "3" + strSql;
                    message = obj.ToString();
                    sign = strSql;
                    return message;
                }
                else return "-1";
            }
            catch (Exception ex)
            {
                return sign + ";" + ex.Source + ";" + ex.Message;
            }
        }

        //private string dateInsert(string dbName, string[] columnsName, string[] values, byte[] fileStream, string[] colStream, string sqlType)
        //{

        //}
        #endregion

        #region 数据库更新函数
        /// <summary>
        /// sql单条更新生成
        /// </summary>
        /// <param name="dbName">数据库名称</param>
        /// <param name="columnsName">更新列名</param>
        /// <param name="values">更新列值</param>
        /// <param name="VarMen">条件列名</param>
        /// <param name="VarVal">条件列值</param>
        /// <param name="VEquals">条件类型：true--"=" false--"like"</param>
        /// <param name="Andor">并交关系：true--"and" false--"or"</param>
        /// <returns>返回信息</returns>
        public string DateUpdate(string dbName, string[] columnsName, string[] values, string[] VarMen, string[] VarVal, bool[] VEquals, bool[] Andor, string sqlType)
        {
            return dateUpdate(dbName, columnsName, values, VarMen, VarVal, VEquals, Andor, sqlType);
        }
        private string dateUpdate(string dbName, string[] columnsName, string[] values, string[] VarMen, string[] VarVal, bool[] VEquals, bool[] Andor, string sqlType)
        {
            try
            {
                if (columnsName != null && columnsName.Length > 0 && values != null && values.Length > 0 && !string.IsNullOrEmpty(dbName))
                {
                    string strSql = "update " + dbName + " set ";
                    strSql += columnsName[0] + " = " + values[0];
                    if (columnsName.GetLength(0) > 1 && columnsName.GetLength(0) == values.GetLength(0))
                    {
                        for (int i = 1; i < columnsName.GetLength(0); i++)
                            strSql += " , " + columnsName[i] + " = " + values[i];
                    }
                    if (VarMen != null && VarMen.Length > 0)
                    {
                        strSql += " where ";
                        for (int i = 0; i < VarMen.Length; i++)
                        {
                            string tmpSql = "";
                            string tmpMen = "";
                            string tmpVal = "";
                            tmpMen = VarMen[i];
                            tmpVal = VarVal[i];
                            if (VEquals != null)
                            {
                                switch (VEquals[i])
                                {
                                    case false:
                                        tmpSql = tmpMen + " like " + tmpVal;
                                        break;
                                    case true:
                                        tmpSql = tmpMen + " = " + tmpVal;
                                        break;
                                }
                                if (Andor != null && i > 0)
                                    switch (Andor[i - 1])
                                    {
                                        case true:
                                            strSql += " and " + tmpSql;
                                            break;
                                        case false:
                                            strSql += " or " + tmpSql;
                                            break;
                                        default: break;
                                    }
                                else
                                    strSql += tmpSql;
                            }
                            else
                            {
                                tmpSql = tmpMen + " = " + tmpVal;
                                if (i > 0)
                                    strSql += " and " + tmpSql;
                                else
                                    strSql += " " + tmpSql;
                            }
                        }
                    }
                    return SetDates(strSql, sqlType);
                }
                else return "-1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 数据库多行更新函数
        /// </summary>
        /// <param name="variables">更新列名</param>
        /// <param name="dbName">数据库名称</param>
        /// <param name="VarMen">条件列名</param>
        /// <param name="Values">条件值</param>
        /// <param name="VEquals">条件类型：true--"=" false--"like"</param>
        /// <param name="Andor">并交关系：true--"and" false--"or"</param>
        /// <param name="orderValues">排列名称</param>
        /// <param name="upValues">更新值</param>
        /// <returns></returns>
        public string MuitDateUpdate(string[] variables, string[] dbName, string[] VarMen, string[] Values, bool[] VEquals, bool[] Andor, string[] orderValues, DataTable upValues)
        {
            return muitDateUpdate(variables,  dbName,  VarMen,  Values,  VEquals,  Andor,  orderValues,  upValues);
        }
        private string muitDateUpdate(string[] variables, string[] dbName, string[] VarMen, string[] Values, bool[] VEquals, bool[] Andor, string[] orderValues, DataTable upValues)
        {
            int index = 0;
            try
            {
                string sqlType = "ORACLE";
                GetConfig(ref sqlType);
                OraCon.Open();
                string orgSql = dateSelect(variables, dbName, VarMen, Values, VEquals, Andor, orderValues);
                if (string.IsNullOrEmpty(orgSql)) return "查询列名或数据库名不能为空";
                else
                {
                    OracleDataAdapter OraApt = new OracleDataAdapter(orgSql, OraCon);
                    DataSet ds = new DataSet();
                    OraApt.Fill(ds);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null)
                    {
                        if (upValues != null && upValues.Rows.Count > 0)
                        {
                            ds.Tables[0].Rows.Clear();
                            for (int i = 0; i < upValues.Rows.Count; i++)
                            {
                                DataRow dr = ds.Tables[0].NewRow();
                                for (int j = 0; j < upValues.Columns.Count; j++)
                                {
                                    dr[ds.Tables[0].Columns[j].ColumnName] = upValues.Rows[i][j];
                                }
                                ds.Tables[0].Rows.Add(dr);
                            }
                            OracleCommandBuilder OraComBul = new OracleCommandBuilder(OraApt);
                            OraApt.UpdateBatchSize = 100;
                            index = OraApt.Update(upValues);
                            return index.ToString(); 
                        }
                        else return "更新数据不能为空";
                    }
                    else return "找不到可更改的数据表";
                }
            }
            catch (Exception ex)
            {
                string error = ex.TargetSite + "," + ex.StackTrace + "," + ex.Message;
                return index.ToString();
            }
            finally
            { OraCon.Close(); }
        }
        #endregion

        #region 数据库调用存储过程
        /// <summary>
        /// sql调用存储过程
        /// </summary>
        /// <param name="strSql">存储过程名称</param>
        /// <param name="ParamConfig">参数表</param>
        /// <param name="sqlType">连接类型</param>
        /// <returns>返回错误信息</returns>
        public string DateTrans(string strSql, ref DataSet ParamConfig, ref string sqlType)
        {
            return dateTrans(strSql, ref ParamConfig, ref sqlType);
        }
        private string dateTrans(string strSql, ref DataSet ParamConfig, ref string sqlType)
        {
            try
            {
                error = GetConfig(ref sqlType);
                long num = 0;
                if (string.IsNullOrEmpty(strSql)) return "存储过程名称不能为空！";
                switch (sqlType.Trim().ToUpper())
                {
                    case "SQLSERVER":
                        SvrCon.Open();
                        SqlTransaction sqlTrans = SvrCon.BeginTransaction();
                        SqlCommand svrCmd = new SqlCommand(strSql, SvrCon);
                        svrCmd.Transaction = sqlTrans;
                        svrCmd.CommandType = CommandType.StoredProcedure;
                        if (ParamConfig != null && ParamConfig.Tables[0].Rows.Count > 0)
                            for (int i = 0; i < ParamConfig.Tables[0].Rows.Count; i++)
                            {
                                svrCmd.Parameters.Add(ParamConfig.Tables[0].Rows[i][0].ToString(), (SqlDbType)sqlDateType(ParamConfig.Tables[0].Rows[i][1].ToString(), sqlType)).Value = ParamConfig.Tables[0].Rows[i][2].ToString();
                                svrCmd.Parameters[ParamConfig.Tables[0].Rows[i][0].ToString()].Direction = sqlDirType(ParamConfig.Tables[0].Rows[i][3].ToString());
                            }
                        svrCmd.Parameters.Add("@RETURN_VALUE", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                        sqlTrans.Commit();
                        SqlDataAdapter svrAdp = new SqlDataAdapter(svrCmd);

                        num = svrCmd.ExecuteNonQuery();
                        if (ParamConfig != null && ParamConfig.Tables[0].Rows.Count > 0)
                            for (int i = 0; i < ParamConfig.Tables[0].Rows.Count; i++)
                                ParamConfig.Tables[0].Rows[i][1] = svrCmd.Parameters[ParamConfig.Tables[0].Rows[i][0].ToString()].Value.ToString();

                        svrAdp.Fill(ParamConfig);

                        num = Int32.Parse(svrCmd.Parameters["@RETURN_VALUE"].Value.ToString());
                        return num.ToString();
                    case "ORACLE":
                        OraCon.Open();
                        OracleTransaction oraTrans = OraCon.BeginTransaction();
                        OracleCommand oraCmd = new OracleCommand(strSql, OraCon);
                        oraCmd.Transaction = oraTrans;
                        oraCmd.CommandType = CommandType.StoredProcedure;
                        if (ParamConfig != null && ParamConfig.Tables[0].Rows.Count > 0)
                            for (int i = 0; i < ParamConfig.Tables[0].Rows.Count; i++)
                            {
                                if (ParamConfig.Tables[0].Rows[i][1].ToString().Contains("LIST"))
                                {
                                    string[] ParamTypes = ParamConfig.Tables[0].Rows[i][1].ToString().Split('|');
                                    string[] newValues = ParamConfig.Tables[0].Rows[i][2].ToString().Split('|');

                                    OracleParameter p_in_values = new OracleParameter();
                                    p_in_values.ParameterName = ParamConfig.Tables[0].Rows[i][0].ToString();
                                    p_in_values.OracleDbType = (OracleDbType)sqlDateType(ParamTypes[0], sqlType);
                                    p_in_values.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                                    List<int> strLength = new List<int>();
                                    List<OracleParameterStatus> strStauts = new List<OracleParameterStatus>();
                                    for (int x = 0; x < newValues.Count(); x++)
                                    {
                                        strLength.Add(newValues[x].Length);
                                        strStauts.Add(OracleParameterStatus.Success);
                                    }
                                    p_in_values.ArrayBindSize = strLength.ToArray();
                                    p_in_values.ArrayBindStatus = strStauts.ToArray();
                                    p_in_values.Value = newValues;
                                    p_in_values.Size = newValues.Count();
                                    p_in_values.Direction = sqlDirType(ParamConfig.Tables[0].Rows[i][3].ToString());

                                    oraCmd.Parameters.Add(p_in_values);
                                }
                                else
                                {
                                    oraCmd.Parameters.Add(ParamConfig.Tables[0].Rows[i][0].ToString()
                                        , (OracleDbType)sqlDateType(ParamConfig.Tables[0].Rows[i][1].ToString()
                                        , sqlType), ParamConfig.Tables[0].Rows[i][2].ToString().Length).Value = ParamConfig.Tables[0].Rows[i][2].ToString();
                                    oraCmd.Parameters[ParamConfig.Tables[0].Rows[i][0].ToString()].Direction = sqlDirType(ParamConfig.Tables[0].Rows[i][3].ToString());
                                }
                            }
                        sqlType = OraCon.ConnectionString;
                        num = oraCmd.ExecuteNonQuery();
                        if (ParamConfig != null && ParamConfig.Tables[0].Rows.Count > 0)
                            for (int i = 0; i < ParamConfig.Tables[0].Rows.Count; i++)
                                ParamConfig.Tables[0].Rows[i][2] = oraCmd.Parameters[ParamConfig.Tables[0].Rows[i][0].ToString()].Value + "";
                        //oraTrans.Commit();
                        return error + num.ToString();
                    default: return "-1";//找不到所需的数据库连接类型
                }
            }
            catch (Exception ex)
            {
                error = error + ex.Message;
                return error;
            }
        }

        /// <summary>
        /// sql调用存储过程并返回数据集
        /// </summary>
        /// <param name="strSql">存储过程名称</param>
        /// <param name="ParamConfig">参数表</param>
        /// <param name="rnDate">返回数据集</param>
        /// <param name="sqlType">连接类型</param>
        /// <returns>返回错误信息</returns>
        public string ReDateTrans(string strSql, ref DataSet ParamConfig, ref DataSet rnDate, ref string sqlType)
        {
            return reDateTrans(strSql, ref ParamConfig, ref rnDate, ref sqlType);
        }
        private string reDateTrans(string strSql, ref DataSet ParamConfig, ref DataSet rnDate, ref string sqlType)
        {
            try
            {
                error = GetConfig(ref sqlType);
                long num = 0;
                if (string.IsNullOrEmpty(strSql)) return "存储过程名称不能为空！";
                switch (sqlType.Trim().ToUpper())
                {
                    case "SQLSERVER":
                        SvrCon.Open();
                        SqlTransaction sqlTrans = SvrCon.BeginTransaction();
                        SqlCommand svrCmd = new SqlCommand(strSql, SvrCon);
                        svrCmd.Transaction = sqlTrans;
                        svrCmd.CommandType = CommandType.StoredProcedure;
                        if (ParamConfig != null && ParamConfig.Tables[0].Rows.Count > 0)
                            for (int i = 0; i < ParamConfig.Tables[0].Rows.Count; i++)
                            {
                                svrCmd.Parameters.Add(ParamConfig.Tables[0].Rows[i][0].ToString(), (SqlDbType)sqlDateType(ParamConfig.Tables[0].Rows[i][1].ToString(), sqlType)).Value = ParamConfig.Tables[0].Rows[i][2].ToString();
                                svrCmd.Parameters[ParamConfig.Tables[0].Rows[i][0].ToString()].Direction = sqlDirType(ParamConfig.Tables[0].Rows[i][3].ToString());
                            }
                        svrCmd.Parameters.Add("@RETURN_VALUE", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                        sqlTrans.Commit();
                        SqlDataAdapter svrAdp = new SqlDataAdapter(svrCmd);

                        num = svrCmd.ExecuteNonQuery();
                        if (ParamConfig != null && ParamConfig.Tables[0].Rows.Count > 0)
                            for (int i = 0; i < ParamConfig.Tables[0].Rows.Count; i++)
                                ParamConfig.Tables[0].Rows[i][1] = svrCmd.Parameters[ParamConfig.Tables[0].Rows[i][0].ToString()].Value.ToString();

                        svrAdp.Fill(rnDate);

                        num = Int32.Parse(svrCmd.Parameters["@RETURN_VALUE"].Value.ToString());
                        return num.ToString();
                    case "ORACLE":
                        OraCon.Open();
                        OracleTransaction oraTrans = OraCon.BeginTransaction();
                        OracleCommand oraCmd = new OracleCommand(strSql, OraCon);
                        oraCmd.Transaction = oraTrans;
                        oraCmd.CommandType = CommandType.StoredProcedure;
                        if (ParamConfig != null && ParamConfig.Tables[0].Rows.Count > 0)
                            for (int i = 0; i < ParamConfig.Tables[0].Rows.Count; i++)
                            {
                                if (ParamConfig.Tables[0].Rows[i][1].ToString().Contains("LIST"))
                                {
                                    string[] ParamTypes = ParamConfig.Tables[0].Rows[i][1].ToString().Split('|');
                                    string[] newValues = ParamConfig.Tables[0].Rows[i][2].ToString().Split('|');

                                    OracleParameter p_in_values = new OracleParameter();
                                    p_in_values.ParameterName = ParamConfig.Tables[0].Rows[i][0].ToString();
                                    p_in_values.OracleDbType = (OracleDbType)sqlDateType(ParamTypes[0], sqlType);
                                    p_in_values.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                                    List<int> strLength = new List<int>();
                                    List<OracleParameterStatus> strStauts = new List<OracleParameterStatus>();
                                    for (int x = 0; x < newValues.Count(); x++)
                                    {
                                        strLength.Add(newValues[x].Length);
                                        strStauts.Add(OracleParameterStatus.Success);
                                    }
                                    p_in_values.ArrayBindSize = strLength.ToArray();
                                    p_in_values.ArrayBindStatus = strStauts.ToArray();
                                    p_in_values.Value = newValues;
                                    p_in_values.Size = newValues.Count();
                                    p_in_values.Direction = sqlDirType(ParamConfig.Tables[0].Rows[i][3].ToString());

                                    oraCmd.Parameters.Add(p_in_values);
                                }
                                else
                                {
                                    OracleDbType odt = (OracleDbType)sqlDateType(ParamConfig.Tables[0].Rows[i][1].ToString(), sqlType);
                                    OracleParameter op = oraCmd.Parameters.Add(ParamConfig.Tables[0].Rows[i][0].ToString()
                                        , odt, ParamConfig.Tables[0].Rows[i][2].ToString().Length);
                                    ParameterDirection pd = sqlDirType(ParamConfig.Tables[0].Rows[i][3].ToString());
                                    oraCmd.Parameters[ParamConfig.Tables[0].Rows[i][0].ToString()].Direction = pd;
                                    if(odt != OracleDbType.RefCursor)
                                        op.Value = ParamConfig.Tables[0].Rows[i][2].ToString();
                                }
                            }
                        sqlType = OraCon.ConnectionString;
                        OracleDataAdapter adp = new OracleDataAdapter(oraCmd);
                        adp.Fill(rnDate);
                        if (ParamConfig != null && ParamConfig.Tables[0].Rows.Count > 0)
                            for (int i = 0; i < ParamConfig.Tables[0].Rows.Count; i++)
                                ParamConfig.Tables[0].Rows[i][2] = oraCmd.Parameters[ParamConfig.Tables[0].Rows[i][0].ToString()].Value + "";
                        //oraTrans.Commit();
                        return error + num.ToString();
                    default: return "-1";//找不到所需的数据库连接类型
                }
            }
            catch (Exception ex)
            {
                error = error + ex.Message;
                return error;
            }
        }

        #endregion

        #region 数据库下载函数
        /// <summary>
        /// 数据库byte下载
        /// </summary>
        /// <param name="dbName">数据库名称</param>
        /// <param name="blobCont">获取数据列名</param>
        /// <param name="primaryKey">主键ID</param>
        /// <param name="keyValues">匹配值</param>
        /// <param name="sqlType">连接类型</param>
        /// <returns></returns>
        public DownFileResult softUpDate(FileAdr Adress)
        {
            return SoftUpDate(Adress);
        }
        private DownFileResult SoftUpDate(FileAdr Adress)
        {
            DownFileResult message = new DownFileResult();
            try
            {
                string sqlType = Adress.sqlType;
                Stream Content;
                GetConfig(ref sqlType);
                Byte[] blob = null;
                DataSet dataSet = new DataSet();
                string strSql = "select " + Adress.blobCont + " from " + Adress.dbName + " where " + Adress.primaryKey + " = " + Adress.keyValues;
                long num = 0;
                switch (sqlType.Trim().ToUpper())
                {
                    case "SQLSERVER":
                        SvrCon.Open();
                        SqlCommand svrcmd = new SqlCommand(strSql, SvrCon);
                        SqlDataReader svrsdr = svrcmd.ExecuteReader();
                        SvrCon.Close();
                        bool svrRead = svrsdr.Read();
                        blob = new Byte[(svrsdr.GetBytes(0, 0, null, 0, int.MaxValue))];
                        num = svrsdr.GetBytes(0, 0, blob, 0, blob.Length);
                        svrsdr.Close();
                        if (num > 0)
                        {
                            Content = new MemoryStream(blob);
                            message.FileStream = Content;
                            message.FileSize = Content.Length;
                            message.Message = num.ToString();
                            message.IsSuccess = true;
                        }
                        else
                        {
                            Content = null;
                            message.Message = num.ToString();
                            message.IsSuccess = false;
                        }
                        break;
                    case "ORACLE":
                        OraCon.Open();
                        OracleCommand oracmd = new OracleCommand(strSql, OraCon);
                        OracleDataReader orasdr = oracmd.ExecuteReader();
                        OraCon.Close();
                        bool oraRead = orasdr.Read();
                        blob = new Byte[(orasdr.GetBytes(0, 0, null, 0, int.MaxValue))];
                        num = orasdr.GetBytes(0, 0, blob, 0, blob.Length);
                        orasdr.Close();
                        if (num > 0)
                        {
                            Content = new MemoryStream(blob);
                            message.FileStream = Content;
                            message.FileSize = Content.Length;
                            message.Message = num.ToString();
                            message.IsSuccess = true;
                        }
                        else
                        {
                            Content = null;
                            message.Message = num.ToString();
                            message.IsSuccess = false;
                        }
                        break;
                    default:
                        message.Message = "找不到所需的数据库连接类型";
                        message.IsSuccess = false;
                        break;
                }
                return message;
            }
            catch (Exception e)
            {
                error = e.Message;
                message.IsSuccess = false;
                message.Message = error;
                return message;
            }
        }
        #endregion

        #region 数据库设置获取
        /// <summary>
        /// 获取Xml的服务器设置
        /// </summary>
        private string GetConfig(ref string SqlNode)
        {
            try
            {
                XmlDocument ServerXML = new XmlDocument();
                string XmlPath = GetClass.GetAssemblyPath() + "\\ServerConfig.xml";
                if (File.Exists(XmlPath))
                {
                    ServerXML.Load(XmlPath);
                    XmlNode ServerNode = ServerXML.SelectSingleNode("serverconfig");
                    XmlNode treeNode = ServerNode.SelectSingleNode(SqlNode);
                    if (treeNode != null)
                    {
                        string sqlTypeStr = treeNode.SelectSingleNode("SqlType").InnerText;
                        ServerIP = GetClass.DesDecrypt(publicKeys, treeNode.SelectSingleNode("ServerIP").InnerText);
                        userid = GetClass.DesDecrypt(publicKeys, treeNode.SelectSingleNode("UserName").InnerText);
                        userpwd = GetClass.DesDecrypt(publicKeys, treeNode.SelectSingleNode("UserPwd").InnerText);
                        DatabaseName = GetClass.DesDecrypt(publicKeys, treeNode.SelectSingleNode("DatabaseName").InnerText);
                        SqlNode = sqlTypeStr;
                        return dataConnect(sqlTypeStr.Trim().ToUpper());
                        
                    }
                    else return error = "没输入有效的数据库类型";
                }
                else
                    return "不存在数据库配置文件！";
            }
            catch (Exception ex)
            {
                SqlNode += ex.Message;
                return ex.Message;
            }
        }
        #endregion

        #region 返回数据库类型

        /// <summary>
        /// 返回数据传输类型
        /// </summary>
        /// <param name="str">传输类型名</param>
        /// <returns>返回传输类型</returns>
        private ParameterDirection sqlDirType(string str)
        {
            switch (str.Trim().ToUpper())
            {
                case "INPUTOUTPUT": return ParameterDirection.InputOutput;
                case "OUTPUT": return ParameterDirection.Output;
                case "RETURNVALUE": return ParameterDirection.ReturnValue;
                default: return ParameterDirection.Input;
            }
        }

        /// <summary>
        /// 数据类型
        /// </summary>
        /// <param name="str">数据类型名称</param>
        /// <param name="sqlType">连接类型</param>
        /// <returns>返回数据类型</returns>
        private object sqlDateType(string str, string sqlType)
        {
            switch (sqlType.Trim().ToUpper())
            {
                case "SQLSERVER":
                    switch (str.Trim().ToUpper())
                    {
                        case "BIGINT": return SqlDbType.BigInt;
                        case "BINARY": return SqlDbType.Binary;
                        case "BIT": return SqlDbType.Bit;
                        case "CHAR": return SqlDbType.Char;
                        case "DATE": return SqlDbType.Date;
                        case "DATETIME": return SqlDbType.DateTime;
                        case "DATETIME2": return SqlDbType.DateTime2;
                        case "DATETIMEOFFSET": return SqlDbType.DateTimeOffset;
                        case "DECIMAL": return SqlDbType.Decimal;
                        case "FLOAT": return SqlDbType.Float;
                        case "IMAGE": return SqlDbType.Image;
                        case "INT": return SqlDbType.Int;
                        case "MONEY": return SqlDbType.Money;
                        case "NCHAR": return SqlDbType.NChar;
                        case "NTEXT": return SqlDbType.NText;
                        case "NVARCHAR": return SqlDbType.NVarChar;
                        case "REAL": return SqlDbType.Real;
                        case "SMALLDATETIME": return SqlDbType.SmallDateTime;
                        case "SMALLINT": return SqlDbType.SmallInt;
                        case "SMALLMONEY": return SqlDbType.SmallMoney;
                        case "STRUCTURED": return SqlDbType.Structured;
                        case "TEXT": return SqlDbType.Text;
                        case "TIME": return SqlDbType.Time;
                        case "TIMESTAMP": return SqlDbType.Timestamp;
                        case "TINYINT": return SqlDbType.TinyInt;
                        case "UDT": return SqlDbType.Udt;
                        case "UNIQUEIDENTIFIER": return SqlDbType.UniqueIdentifier;
                        case "VARBINARY": return SqlDbType.VarBinary;
                        case "VARCHAR": return SqlDbType.VarChar;
                        case "VARIANT": return SqlDbType.Variant;
                        case "XML": return SqlDbType.Xml;
                        default: return null;
                    }
                case "ORACLE":
                    switch (str.Trim().ToUpper())
                    {
                        case "ARRAY": return OracleDbType.Array;
                        case "BFILE": return OracleDbType.BFile;
                        case "BINARYDOUBLE": return OracleDbType.BinaryDouble;
                        case "BINARYFLOAT": return OracleDbType.BinaryFloat;
                        case "BLOB": return OracleDbType.Blob;
                        case "BYTE": return OracleDbType.Byte;
                        case "CHAR": return OracleDbType.Char;
                        case "CLOB": return OracleDbType.Clob;
                        case "DATE": return OracleDbType.Date;
                        case "DECIMAL": return OracleDbType.Decimal;
                        case "DOUBLE": return OracleDbType.Double;
                        case "INT16": return OracleDbType.Int16;
                        case "INT32": return OracleDbType.Int32;
                        case "INT64": return OracleDbType.Int64;
                        case "INTERVALDS": return OracleDbType.IntervalDS;
                        case "INTERVALYM": return OracleDbType.IntervalYM;
                        case "LONG": return OracleDbType.Long;
                        case "LONGRAW": return OracleDbType.LongRaw;
                        case "NCHAR": return OracleDbType.NChar;
                        case "NCLOB": return OracleDbType.NClob;
                        case "NVARCHAR2": return OracleDbType.NVarchar2;
                        case "OBJECT": return OracleDbType.Object;
                        case "RAW": return OracleDbType.Raw;
                        case "REF": return OracleDbType.Ref;
                        case "REFCURSOR": return OracleDbType.RefCursor;
                        case "SINGLE": return OracleDbType.Single;
                        case "TIMESTAMP": return OracleDbType.TimeStamp;
                        case "TIMESTAMPLTZ": return OracleDbType.TimeStampLTZ;
                        case "TIMESTAMPTZ": return OracleDbType.TimeStampTZ;
                        case "VARCHAR2": return OracleDbType.Varchar2;
                        case "XMLTYPE": return OracleDbType.XmlType;
                        default: return OracleDbType.Varchar2;
                    }
                default:
                    switch (str.Trim().ToUpper())
                    {
                        case "ANSISTRING": return DbType.AnsiString;
                        case "ANSISTRINGFIXEDLENGTH": return DbType.AnsiStringFixedLength;
                        case "BINARY": return DbType.Binary;
                        case "BOOLEAN": return DbType.Boolean;
                        case "BYTE": return DbType.Byte;
                        case "CURRENCY": return DbType.Currency;
                        case "DATE": return DbType.Date;
                        case "DATETIME": return DbType.DateTime;
                        case "DATETIME2": return DbType.DateTime2;
                        case "DATETIMEOFFSET": return DbType.DateTimeOffset;
                        case "DECIMAL": return DbType.Decimal;
                        case "DOUBLE": return DbType.Double;
                        case "GUID": return DbType.Guid;
                        case "INT16": return DbType.Int16;
                        case "INT32": return DbType.Int32;
                        case "INT64": return DbType.Int64;
                        case "OBJECT": return DbType.Object;
                        case "SBYTE": return DbType.SByte;
                        case "SINGLE": return DbType.Single;
                        case "STRING": return DbType.String;
                        case "STRINGFIXEDLENGTH": return DbType.StringFixedLength;
                        case "TIME": return DbType.Time;
                        case "UINT16": return DbType.UInt16;
                        case "UINT32": return DbType.UInt32;
                        case "UINT64": return DbType.UInt64;
                        case "VARNUMERIC": return DbType.VarNumeric;
                        case "XML": return DbType.Xml;
                        default: return DbType.String;
                    }
            }
        }
        #endregion
    }
}
