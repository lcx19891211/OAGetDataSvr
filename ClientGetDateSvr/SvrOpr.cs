using System;
using System.Xml;
using System.Collections.Generic;
using System.ServiceModel;
using System.IO;

namespace ClientGetDateSvr
{
    class SvrOpr
    {
        public List<string> uriList = new List<string>();
        /// <summary>
        /// 获取Xml的服务器设置
        /// </summary>
        public void GetConfig()
        {
            try
            {
                XmlDocument OracleServerXML = new XmlDocument();
                string XmlPath = System.IO.Directory.GetCurrentDirectory() + "\\ServerConfig.xml";
                if (File.Exists(XmlPath))
                {
                    OracleServerXML.Load(XmlPath);
                    XmlNode ServerNode = OracleServerXML.SelectSingleNode("serverconfig");
                    uriList = new List<string>();
                    for (int i = 0; i < ServerNode.ChildNodes.Count; i++)
                    {
                        XmlNode x = ServerNode.ChildNodes[i];
                        if (x.Name == "ServerIP")
                            uriList.Add(x.InnerText);
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        #region WCF服务工厂
        public static T CreateServiceByUrl<T>(string url)
        {
            return CreateServiceByUrl<T>(url, "basicHttpBinding");
        }
        public static T CreateServiceByUrl<T>(string url, string bing)
        {
            try
            {
                if (string.IsNullOrEmpty(url)) throw new NotSupportedException("This url is not Null or Empty!");
                EndpointAddress address = new EndpointAddress(url);
                System.ServiceModel.Channels.Binding binding = CreateBinding(bing);
                ChannelFactory<T> factory = new ChannelFactory<T>(binding, address);
                return factory.CreateChannel();

            }
            catch (Exception ex)
            {
                throw new Exception("无法获取到正常的服务器配置，请检查ServerConfig.xml中的配置，" + ex.Message);
            }
        }
        #endregion

        #region 创建传输协议
        /// <summary>
        /// 创建传输协议
        /// </summary>
        /// <param name="binding">传输协议名称</param>
        /// <returns></returns>
        private static System.ServiceModel.Channels.Binding CreateBinding(string binding)
        {
            System.ServiceModel.Channels.Binding bindinginstance = null;
            if (binding.ToLower() == "basichttpbinding")
            {
                BasicHttpBinding ws = new BasicHttpBinding();
                ws.MaxBufferSize = 2147483647;
                ws.MaxBufferPoolSize = 2147483647;
                ws.MaxReceivedMessageSize = 2147483647;
                ws.ReaderQuotas.MaxStringContentLength = 2147483647;
                ws.CloseTimeout = new TimeSpan(0, 30, 0);
                ws.OpenTimeout = new TimeSpan(0, 30, 0);
                ws.ReceiveTimeout = new TimeSpan(0, 30, 0);
                ws.SendTimeout = new TimeSpan(0, 30, 0);
                bindinginstance = ws;
            }
            else if (binding.ToLower() == "nettcpbinding")
            {
                NetTcpBinding ws = new NetTcpBinding();
                ws.MaxReceivedMessageSize = 65535000;
                ws.Security.Mode = SecurityMode.None;
                bindinginstance = ws;
            }
            else if (binding.ToLower() == "wshttpbinding")
            {
                WSHttpBinding ws = new WSHttpBinding(SecurityMode.None);
                ws.MaxReceivedMessageSize = 65535000;
                ws.Security.Message.ClientCredentialType = System.ServiceModel.MessageCredentialType.Windows;
                ws.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Windows;
                bindinginstance = ws;
            }
            return bindinginstance;
        }
        #endregion

    }
}
