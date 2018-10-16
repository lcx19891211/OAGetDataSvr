using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfSteam
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“Service1”。
    public class WcfSteamService : IWcfSteamService
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="FMessage"></param>
        public void GetDataFile(FileAdr FMessage)
        {
            DownFileResult DFR = new DownFileResult();
            DFR.IsSuccess = false;
            DFR.Message = "传输成功！";
            ICallBackInterface CallBackChannel = OperationContext.Current.GetCallbackChannel<ICallBackInterface>();
            CallBackChannel.UploadFile(DFR);

        }
    }
}
