using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfFIleSteamCon
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    public class Service1 : ISvcFile
    {
        long fileLimitLength = 3000;//文件限制
        /// <summary>
        /// 检查文件是否可传输
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FileTransferMessageCheck TransferChkFile(FileTransferMessageCheck request)
        {
            string MainPath = GetClass.GetPathConfig(ref request.reMessage);
            if (string.IsNullOrEmpty(MainPath))
            {
                request.isBool = false;
                return request;
            }
            else
            {
                if (request.FileName.Contains(".jpg") && request.FileLength > fileLimitLength)
                {
                    string DirTimes = MainPath + "\\" + request.TimePath;
                    string DirNames = DirTimes + "\\" + request.NamePath;
                    string FileNamePath = DirNames + "\\" + request.FileName;
                    if (!Directory.Exists(DirTimes))
                    {
                        request.reMessage = FileNamePath;
                        request.NamePath = DirNames;
                        request.TimePath = DirTimes;
                        request.isBool = true;
                        Directory.CreateDirectory(DirTimes);
                    }
                    if (!Directory.Exists(DirNames))
                    {
                        request.reMessage = FileNamePath;
                        request.NamePath = DirNames;
                        request.TimePath = DirTimes;
                        request.isBool = true;
                        Directory.CreateDirectory(DirNames);
                    }
                    if (!File.Exists(FileNamePath))
                    {
                        request.reMessage = FileNamePath;
                        request.NamePath = DirNames;
                        request.TimePath = DirTimes;
                        request.isBool = true;
                        return request;
                    }
                    else
                    {
                        FileInfo info = new FileInfo(FileNamePath);
                        if(info.Length > fileLimitLength)
                        {
                            request.reMessage = "File Is ALive";
                            request.NamePath = DirNames;
                            request.TimePath = DirTimes;
                            request.isBool = false;
                            return request;
                        }
                        else
                        {
                            request.reMessage = FileNamePath;
                            request.NamePath = DirNames;
                            request.TimePath = DirTimes;
                            request.isBool = true;
                            return request;
                        }
                    }
                }
                else
                {
                    request.reMessage = "Picture Type Or Length Limit";
                    request.isBool = false;
                    return request;
                }
            }
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="request"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public FileTransferMessageCheck TransferUpFile(FileTransferMessage request)
        {
            FileTransferMessageCheck FTMC = new FileTransferMessageCheck();
            string MainPath = GetClass.GetPathConfig(ref FTMC.reMessage);
            if (string.IsNullOrEmpty(MainPath))
            {
                FTMC.reMessage = "Null XML";
                FTMC.isBool = false;
                return FTMC;
            }
            else
            {
                if (!File.Exists(request.FileName))
                {
                    FTMC.isBool = GetClass.FileSave(request.FileName, request.FileData, ref FTMC.reMessage);
                    return FTMC;
                }
                else
                {
                    FileInfo info = new FileInfo(request.FileName);
                    if (info.Length > fileLimitLength)
                    {
                        FTMC.reMessage = "File Is ALive";
                        FTMC.isBool = false;
                        return FTMC;
                    }
                    else
                    {
                        FTMC.isBool = GetClass.FileSave(request.FileName, request.FileData, ref FTMC.reMessage);
                        return FTMC;
                    }
                }
            }
        }
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FileTransferMessageCheck TransferUpLoadFile(FileInformation request)
        {
            FileTransferMessageCheck FTMC = new FileTransferMessageCheck();
            try
            {
                FTMC.isBool = GetClass.FileUpLoad(request);
                FTMC.reMessage = "success";
                return FTMC;
            }
            catch (Exception ex)
            {
                FTMC.reMessage = ex.Message;
                return FTMC;
            }
        }

    }
}
