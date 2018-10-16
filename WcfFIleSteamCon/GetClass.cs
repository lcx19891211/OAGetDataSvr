using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace WcfFIleSteamCon
{
    public class GetClass
    {
        #region 文件程序路径类方法
        /// <summary>
        /// 获取dll调用路径
        /// </summary>
        /// <returns></returns>
        private static string GetAssemblyPath()
        {
            string _CodeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

            _CodeBase = _CodeBase.Substring(8, _CodeBase.Length - 8);    // 8是 file:// 的长度

            string[] arrSection = _CodeBase.Split(new char[] { '/' });

            string _FolderPath = "";
            for (int i = 0; i < arrSection.Length - 1; i++)
            {
                _FolderPath += arrSection[i] + "/";
            }

            return _FolderPath;
        }
        #endregion

        #region 设置获取
        /// <summary>
        /// 获取Xml的服务器设置
        /// </summary>
        public static string GetPathConfig(ref string error)
        {
            try
            {
                XmlDocument ServerXML = new XmlDocument();
                string XmlPath = GetClass.GetAssemblyPath() + "Config.xml";
                if (File.Exists(XmlPath))
                {
                    error += XmlPath;
                    ServerXML.Load(XmlPath);
                    error += "Config";
                    XmlNode ServerNode = ServerXML.SelectSingleNode("config");
                    error += "mainPath";
                    XmlNode treeNode = ServerNode.SelectSingleNode("mainPath");
                    if (treeNode != null)
                    {
                        error += ";" + treeNode.InnerText;
                        return treeNode.InnerText;
                    }
                    else
                    {
                        error += "Null Path";
                        return string.Empty;
                    }
                }
                else
                {
                    error += "Null Xml";
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                error += ex.Message;
                return string.Empty;
            }
        }
        #endregion

        #region 
        public static bool FileSave(string FilePath, Stream SourceStream, ref string error)
        {
            try
            {
                int fileSize = 0;
                FileStream targetStream = null;
                using (targetStream = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    //定义文件缓冲区
                    const int bufferLen = 4096;
                    byte[] buffer = new byte[bufferLen];
                    int count = 0;

                    while ((count = SourceStream.Read(buffer, 0, bufferLen)) > 0)
                    {
                        targetStream.Write(buffer, 0, count);
                        fileSize += count;
                    }
                    targetStream.Close();
                    SourceStream.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public static bool FileUpLoad(FileInformation fileInfo)
        {

            try
            {
                if (fileInfo == null)
                    return false;
                var savePath = fileInfo.FileName;
                using (var fs = new FileStream(savePath, FileMode.OpenOrCreate))
                {
                    long offset = fileInfo.Offset;
                    using (var write = new BinaryWriter(fs))
                    {
                        write.Seek((int)offset, SeekOrigin.Begin);
                        write.Write(fileInfo.Data);
                        fileInfo.Offset = fs.Length;
                    }
                }
                return true;
            }
            catch 
            {
                return false;
            }
        }
        #endregion
    }
}