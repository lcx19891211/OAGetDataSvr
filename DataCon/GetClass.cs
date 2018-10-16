using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Globalization;
using System.Xml;
using System.Data;


namespace DataCon
{
    #region 通用方法函数
    class GetClass
    {
        #region 系统安全用函数

        #region DES加解密
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="pubKey">公匙</param>
        /// <param name="input">加密文</param>
        /// <returns></returns>
        public static string DesEncrypt(string pubKey, string input)
        {
            var des = new DESCryptoServiceProvider();
            var bytes = Encoding.UTF8.GetBytes(input);
            des.Key = Encoding.ASCII.GetBytes(pubKey);
            des.IV = Encoding.ASCII.GetBytes(pubKey);
            using (var ms = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytes, 0, bytes.Length);
                    cryptoStream.FlushFinalBlock();
                }
                var data = ms.ToArray();
                return BitConverter.ToString(data).Replace("-", "");
            }
        }
        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="pubKey">公匙</param>
        /// <param name="input">解密文</param>
        /// <returns></returns>
        public static string DesDecrypt(string pubKey, string input)
        {
            var des = new DESCryptoServiceProvider();
            var bytes = Hex2Bytes(input);
            des.Key = Encoding.ASCII.GetBytes(pubKey);
            des.IV = Encoding.ASCII.GetBytes(pubKey);
            using (var ms = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytes, 0, bytes.Length);
                    cryptoStream.FlushFinalBlock();
                }
                var data = ms.ToArray();
                return Encoding.UTF8.GetString(data);
            }
        }
        #endregion

        #region sql检测
        /// <summary>
        /// sql防注入
        /// </summary>
        /// <param name="Str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
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
        #endregion

        #endregion

        #region 数值转化
        /// <summary>
        /// 将16进制数组转换成字节
        /// </summary>
        /// <param name="input">转换字节</param>
        /// <returns>16进制数组</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static byte[] Hex2Bytes(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;
            var offset = input.Length % 2;
            if (offset == 1) input = "0" + input;
            int i;
            var list = new List<byte>();
            for (i = 0; i < input.Length; i += 2)
            {
                var temp = input.Substring(i, 2);
                byte bv;
                var success = byte.TryParse(temp, NumberStyles.HexNumber, null, out bv);
                if (!success) throw new ArgumentOutOfRangeException();
                list.Add(bv);
            }
            return list.ToArray();
        }
        #endregion

        #region 文件程序路径类方法
        /// <summary>
        /// 获取dll调用路径
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyPath()
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

        #region 格式化字符
        public static string[] getString(string values)
        {
            return values.Split('|');
        }

        public static string formatStr(string values)
        {
            return "'" + values + "'";
        }

        public static string formatSqlStr(string values)
        {
            return "%" + values + "%";
        }

        public static string formatbool(bool values)
        {
            if (values) return "是";
            else return "否";
        }
        #endregion

        #region DataTable 转换函数
        /// <summary>
        /// 数据源转换数据表函数
        /// </summary>
        /// <param name="dtColumns">新增数据列名</param>
        /// <param name="dtValues">新增数据值</param>
        /// <param name="dtSource">原数据源</param>
        /// <returns>返回改变数据表</returns>
        public static System.Data.DataTable DTBinds(string dtColumns, string[] dtValues, System.Data.DataTable dtSource)
        {
            return dtBinds(dtColumns, dtValues, dtSource);
        }
        private static System.Data.DataTable dtBinds(string dtColumns, string[] dtValues, System.Data.DataTable dtSource)
        {
            System.Data.DataTable dt = new System.Data.DataTable();//返回数据表
            System.Data.DataTable dtBase = new System.Data.DataTable();//原数据源
            //添加新增数据值至返回数据表
            if (dtColumns != null)
            {
                for (int i = 0; i < dtSource.Columns.Count; i++)
                    dt.Columns.Add(dtSource.Columns[i].ColumnName, typeof(string));
                bool isNewColumns = false;
                for (int j = 0; j < dt.Columns.Count; j++)
                    if (dt.Columns[j].ColumnName == dtColumns)
                        isNewColumns = true;
                if (!isNewColumns)
                    dt.Columns.Add(dtColumns, typeof(string));
                if (dtValues != null && dtValues.Length > 0)
                    for (int i = 0; i < dtValues.GetLength(0); i++)
                    {
                        DataRow dr = dt.NewRow();
                        for (int j = 0; j < dtSource.Columns.Count; j++)
                            dr[dtSource.Columns[j].ColumnName] = dtSource.Rows[i][dtSource.Columns[j].ColumnName];
                        dr[dtColumns] = dtValues[i];
                        dt.Rows.Add(dr);
                    }
            }
            return dt;
        }
        /// <summary>
        /// 数据源新增数据行函数
        /// </summary>
        /// <param name="values">数据值</param>
        /// <param name="columnsName">数据列名</param>
        /// <param name="dtSource">对应数据表</param>
        /// <returns>返回新数据行</returns>
        public static DataRow DRBinds(string[] values, string[] columnsName, System.Data.DataTable dtSource)
        {
            return drBinds(values, columnsName, dtSource);
        }
        private static DataRow drBinds(string[] values, string[] columnsName, System.Data.DataTable dtSource)
        {
            if (dtSource != null && values != null && values.GetLength(0) > 0)
            {
                DataRow dr = dtSource.NewRow();//赋予数据行对应数据表结构
                if (columnsName != null && columnsName.GetLength(0) > 0 && columnsName.GetLength(0) == values.GetLength(0))
                    for (int i = 0; i < columnsName.GetLength(0); i++)
                        dr[columnsName[i]] = values[i];
                else
                    for (int i = 0; i < values.GetLength(0); i++)
                        dr[dtSource.Columns[i].ColumnName] = values[i];
                return dr;
            }
            else return null;
        }
        #endregion

    }
    #endregion

    #region 拼音转化
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
}
