using System.ServiceModel;
using System.Data;
using System.IO;

namespace DataCon
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract(Name = "Servers", Namespace = "DataCon")]
    public interface IOAGetData
    {
        /// <summary>
        /// sql查询语句执行
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <param name="dates">返回数据</param>
        /// <returns>返回信息</returns>
        [OperationContract]
        string GetDates(string strSql, ref DataSet Dates, ref string sqlType);

        /// <summary>
        /// sql更新语句执行
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <returns>返回信息</returns>
        [OperationContract]
        string SetDates(string strSql, string sqlType);

        /// <summary>
        /// 一般Sql查询函数
        /// </summary>
        /// <param name="variables">获取列名</param>
        /// <param name="dbName">获取表名</param>
        /// <param name="VarMen">条件列名</param>
        /// <param name="Values">条件列值</param>
        /// <param name="Equals">条件类型：true--"=" false--"like"</param>
        /// <param name="Andor">并交关系：true--"and" false--"or"</param>
        /// <param name="oredrValues">排列名称</param>
        /// <param name="newDates">返回数据表</param>
        /// <returns>返回信息</returns>
        [OperationContract]
        string DateSelect(string[] variables, string[] dbName, string[] VarMen, string[] Values, bool[] Equals, bool[] Andor, string[] orderValues, ref string sqlType, ref DataSet newDates);

        /// <summary>
        /// sql单条语句插入执行
        /// </summary>
        /// <param name="dbName">数据库名</param>
        /// <param name="columnsName">插入列名</param>
        /// <param name="values">插入列值</param>
        /// <returns>返回信息</returns>
        [OperationContract]
        string DateInsert(string dbName, string[] columnsName, string[] values, string sqlType);

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
        [OperationContract]
        string DateUpdate(string dbName, string[] columnsName, string[] values, string[] VarMen, string[] VarVal, bool[] VEquals, bool[] Andor, string sqlType);

        /// <summary>
        /// sql调用存储过程
        /// </summary>
        /// <param name="strSql">存储过程名称</param>
        /// <param name="ParamConfig">参数名</param>
        /// <param name="sqlType">连接类型</param>
        /// <returns></returns>
        [OperationContract]
        string DateTrans(string strSql, ref DataSet ParamConfig, ref string sqlType);

        /// <summary>
        /// sql调用存储过程返回数据
        /// </summary>
        /// <param name="strSql">存储过程名称</param>
        /// <param name="ParamConfig">参数名</param>
        /// <param name="rnDate">返回数据</param>
        /// <param name="sqlType">连接类型</param>
        /// <returns></returns>
        [OperationContract]
        string ReDateTrans(string strSql, ref DataSet ParamConfig, ref DataSet rnDate, ref string sqlType);

        /// <summary>
        /// 下载函数
        /// </summary>
        /// <param name="Adress">下载地址信息</param>
        /// <returns>下载信息</returns>
        [OperationContract]
        DownFileResult softUpDate(FileAdr Adress);

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
        [OperationContract]
        string MuitDateUpdate(string[] variables, string[] dbName, string[] VarMen, string[] Values, bool[] VEquals, bool[] Andor, string[] orderValues, DataTable upValues);
        
    }

    /// <summary>
    /// 文件数据库位置
    /// </summary>
    [MessageContract]
    public class FileAdr
    {
        /// <summary>
        /// 数据表名称
        /// </summary>
        [MessageHeader]
        public string dbName { get; set; }
        /// <summary>
        /// 数据存储列名
        /// </summary>
        [MessageHeader]
        public string blobCont { get; set; }
        /// <summary>
        /// 数据库存储主键列名
        /// </summary>
        [MessageHeader]
        public string primaryKey { get; set; }
        /// <summary>
        /// 数据库存储主键匹配值
        /// </summary>
        [MessageHeader]
        public string keyValues { get; set; }
        /// <summary>
        /// 数据库连接类型
        /// </summary>
        [MessageHeader]
        public string sqlType { get; set; }
    }

    /// <summary>
    /// 下载返回信息
    /// </summary>
    [MessageContract]
    public class DownFileResult
    {
        [MessageHeader]
        public long FileSize { get; set; }
        [MessageHeader]
        public bool IsSuccess { get; set; }
        [MessageHeader]
        public string Message { get; set; }
        [MessageBodyMember]
        public Stream FileStream { get; set; }
    }
}
