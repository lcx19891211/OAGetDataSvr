using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;

namespace WcfSteam
{

    /// <summary>
    /// 获取数据传输
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ICallBackInterface))]
    public interface IWcfSteamService
    {
        [OperationContract(IsOneWay = true)]
        void GetDataFile(FileAdr FMessage);
    }
    /// <summary>
    /// 向客户端推送下载数据
    /// </summary>
    public interface ICallBackInterface
    {
        [OperationContract(IsOneWay = true)]
        void UploadFile(DownFileResult DFR);
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
