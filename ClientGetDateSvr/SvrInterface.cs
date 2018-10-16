using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ClientGetDateSvr
{
    public class SvrInterface
    {
        private SvrOpr GetConfig = new SvrOpr();
        public string url = "http://200.10.6.250:9999/Servers.svc";

        #region 初始化链接
        /// <summary>
        /// 创建新链接
        /// </summary>
        public string CreateNewConn(ref IOAGetData proxy)
        {
            try
            {
                string error = "";
                GetConfig.GetConfig();
                if (GetConfig.uriList != null && GetConfig.uriList.Count > 0)
                {
                    for (int i = 0; i < GetConfig.uriList.Count; i++)
                    {
                        url = GetConfig.uriList[i];
                        bool isCon = getUri(ref proxy, ref error);
                        if (isCon)
                            return "0";
                    }
                    return "无法连接到服务器: " + error;
                }
                else
                {
                    proxy = SvrOpr.CreateServiceByUrl<IOAGetData>(url);
                    return "找不到服务器连接设置，请确保ServerConfig.xml文件存在！现将以默认服务器IP连接。" + url;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        public string closeCon(IOAGetData proxy)
        {
            try
            {
                IClientChannel x = (IClientChannel)proxy;
                x.Close();
                return x.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 创建wcf连接
        /// </summary>
        /// <returns>是否创建成功</returns>
        private bool getUri(ref IOAGetData proxy,ref string error)
        {
            proxy = SvrOpr.CreateServiceByUrl<IOAGetData>(url);
            DataSet dt = new DataSet();
            string mes = "";
            try
            {
                proxy.GetDates("", ref dt, ref mes);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
        #endregion
    }
}
