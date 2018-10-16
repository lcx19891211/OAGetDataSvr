using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfFIleSteamCon
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。

    [ServiceContract]
    public interface ISvcFile
    {
        [OperationContract]
        FileTransferMessageCheck TransferUpFile(FileTransferMessage request);//文件传输

        [OperationContract]
        FileTransferMessageCheck TransferChkFile(FileTransferMessageCheck request);//文件检查

        [OperationContract]
        FileTransferMessageCheck TransferUpLoadFile(FileInformation request);//文件上传
    }

    [Serializable]
    [MessageContract]
    public class FileTransferMessageCheck
    {
        [MessageHeader(MustUnderstand = true)]
        public string reMessage;//返回信息

        [MessageHeader(MustUnderstand = true)]
        public string TimePath;//时间路径

        [MessageHeader(MustUnderstand = true)]
        public string NamePath;//姓名路径

        [MessageHeader(MustUnderstand = true)]
        public string FileName;//文件名称

        [MessageHeader(MustUnderstand = true)]
        public bool isBool;//是否成功

        [MessageHeader(MustUnderstand = true)]
        public long FileLength;//文件长度
    }

    [MessageContract]
    public class FileTransferMessage
    {
        [MessageHeader(MustUnderstand = true)]
        public string FileName;//文件名称

        [MessageBodyMember(Order = 1)]
        public Stream FileData;//文件传输时间
    }

    [MessageContract]
    public class FileInformation
    {
        [MessageHeader(MustUnderstand = true)]
        public long Length { get; set; }

        [MessageHeader(MustUnderstand = true)]
        public string FileName { get; set; }
        
        [MessageHeader(MustUnderstand = true)]
        public long Offset { get; set; }

        [MessageHeader(MustUnderstand = true)]
        public string Error { get; set; }

        [MessageBodyMember(Order = 1)]
        public byte[] Data { get; set; }
    }
}
