using System;
using System.Data;
using System.Collections.Generic;

namespace ControlClass
{
    #region 通用函数库
    /// <summary>
    /// 通用函数库
    /// </summary>
    public class IOClass
    {
        #region 操作系统文件操作
        /// 获取dll调用路径
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyPath()
        {
            string _CodeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

            _CodeBase = _CodeBase.Substring(8, _CodeBase.Length - 8);    // 8是文件名的长度

            string[] arrSection = _CodeBase.Split(new char[] { '/' });

            string _FolderPath = "";
            for (int i = 0; i < arrSection.Length - 1; i++)
            {
                _FolderPath += arrSection[i] + "/";
            }

            return _FolderPath;
        }
        #endregion
    }
    #endregion


}


