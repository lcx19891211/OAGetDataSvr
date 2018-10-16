using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ControlClass
{
    public class JudgeClass
    {
        #region 判断函数
        /// <summary>
        /// 输入限制
        /// </summary>
        /// <param name="e"></param>
        public static bool CheckKeys(string str, KeyPressEventArgs e, bool isdot)
        {
            e.Handled = true;
            if (e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar == (char)8 || e.KeyChar == (char)46)
            {
                if (isdot && !(System.Text.RegularExpressions.Regex.IsMatch(str.ToString(), @"[.]") && e.KeyChar == (char)46))
                    e.Handled = false;
                if (e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar == (char)8)
                    e.Handled = false;
            }
            return e.Handled;
        }
        /// <summary>
        /// sql防注入
        /// </summary>
        /// <param name="Str">检查字符串</param>
        /// <returns>返回判断</returns>
        public static bool ProcessSqlStr(string Str)
        {
            string SqlStr = "and|exec|insert|select|delete|update|count|*|chr|mid|master|truncate|char|declare";
            bool ReturnValue = true;
            try
            {
                if (Str != "")
                {
                    string[] anySqlStr = SqlStr.Split('|');
                    foreach (string ss in anySqlStr)
                    {
                        if (Str.IndexOf(ss) >= 0)
                            ReturnValue = false;
                    }
                }
            }
            catch { ReturnValue = false; }
            return ReturnValue;
        }

        /// <summary>
        /// 是否数字类型
        /// </summary>
        /// <param name="str">检查字符串</param>
        /// <returns>返回判断</returns>
        public static bool IsNumber(string strNumber)
        {
            Regex objNotNumberPattern = new Regex("[^0-9.-]");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");

            return !objNotNumberPattern.IsMatch(strNumber) &&
                   !objTwoDotPattern.IsMatch(strNumber) &&
                   !objTwoMinusPattern.IsMatch(strNumber) &&
                   objNumberPattern.IsMatch(strNumber);
        }
        #endregion
    }
}
