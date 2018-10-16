using System;
using System.Text;
using System.Windows.Forms;

namespace ControlClass
{
    #region 计算类
    /// <summary>
    /// 分值计算 
    /// </summary>
    public class MarkCounts
    {
        int totalPoints = 0;//总分
        public int[] allScores;//分值详细
                               /// <summary>
                               /// 计算总分
                               /// </summary>
        public int Compute()
        {
            totalPoints = 0;
            foreach (int score in allScores)
            {
                totalPoints += score;
            }
            return totalPoints;
        }
        /// <summary>
        /// 重置
        /// </summary>
        public void repeat()
        {
            totalPoints = 0;
        }
    }

    /// <summary>
    /// 拼音类
    /// </summary>
    public class PYSelect
    {
        /// <summary>       
        /// 拼音检索     
        /// </summary>    
        /// <param name="strText"></param> 
        /// <returns></returns>       
        static public string GetChineseSpell(string strText)
        {
            int len = strText.Length;
            string myStr = "";
            for (int i = 0; i < len; i++)
                myStr += getSpell(strText.Substring(i, 1));
            return myStr;
        }

        /// <summary>       
        /// 得到首字母       
        /// </summary> 
        /// <param name="cnChar"></param>       
        /// <returns></returns>       
        static private string getSpell(string cnChar)
        {
            byte[] arrCN = Encoding.Default.GetBytes(cnChar);
            if (arrCN.Length > 1)
            {
                int area = (short)arrCN[0];
                int pos = (short)arrCN[1];
                int code = (area << 8) + pos;
                int[] areacode = { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };
                for (int i = 0; i < 26; i++)
                {
                    int max = 55290;
                    if (i != 25) max = areacode[i + 1];
                    if (areacode[i] <= code && code < max)
                        return Encoding.Default.GetString(new byte[] { (byte)(65 + i) });
                }
                return "";
            }
            else
                return cnChar;

        }
    }
    #endregion

    #region 基础集合类
    /// <summary>
    /// 
    /// </summary>
    public class strCollection
    {
        private string[] Values;//存储数组值
        private string[,] ColumnsName;//匹配列值

        /// <summary>
        /// 字符串索引数组匹配
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string this[string index]
        {
            get
            {
                CheckIsNull();
                for (int i = 0; i < ColumnsName.GetLength(0); i++)
                {
                    if (index == ColumnsName[i, 0] || index == ColumnsName[i, 1])
                    {
                        return Values[i];
                    }
                }
                MessageBox.Show("找不到匹配的值！");
                return null;
            }
            set
            {
                CheckIsNull();
                for (int i = 0; i < ColumnsName.GetLength(0); i++)
                {
                    if (index == ColumnsName[i, 0] || index == ColumnsName[i, 1])
                    {
                        Values[i] = value;
                        return;
                    }
                }
                MessageBox.Show("找不到匹配的值！");
            }
        }

        /// <summary>
        /// 数字索引匹配
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string this[int index]
        {
            get
            {
                CheckIsNull();
                if (index >= Values.GetLength(0))
                {
                    MessageBox.Show("超出索引！");
                    return null;
                }
                return Values[index];
            }
            set
            {
                CheckIsNull();
                if (index >= Values.GetLength(0))
                {
                    MessageBox.Show("超出索引！");
                    return;
                }
                Values[index] = value;
                return;
            }
        }

        /// <summary>
        /// 绑定匹配索引
        /// </summary>
        /// <returns></returns>
        public bool ColumnsBind(string[,] strColumns)
        {
            try
            {
                ColumnsName = strColumns;
                Values = new string[ColumnsName.GetLength(0)];
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查是否为可空
        /// </summary>
        /// <returns></returns>
        private void CheckIsNull()
        {
            if (!(ColumnsName != null && ColumnsName.GetLength(0) > 0))
                throw new Exception("找不到索引值！");
            if (!(Values != null && Values.GetLength(0) > 0))
                throw new Exception("找不到数组值！");
            if (ColumnsName.GetLength(0) != Values.GetLength(0))
                throw new Exception("数组值与索引值不匹配！");
        }
    }
    #endregion
}
