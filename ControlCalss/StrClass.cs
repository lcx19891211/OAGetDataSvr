using System.Collections.Generic;
using System.Xml;

namespace ControlClass
{
    public class StrClass
    {
        #region 字符串操作函数
        /// <summary>
        /// 字符串加引号
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string formatStr(string values)
        {
            return "'" + values + "'";
        }
        /// <summary>
        /// 字符串加%号
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string formatSqlStr(string values)
        {
            return "%" + values + "%";
        }
        public static double lengthStr(int values, int size)
        {
            return values * size * 35 * 0.039370078740157;
        }
        #endregion

        #region 字符串转换
        /// <summary>
        /// dgvSql查询
        /// </summary>
        /// <param name="varMen"></param>
        /// <param name="values"></param>
        /// <param name="andor"></param>
        /// <returns></returns>
        public static string SqlFormat(string[] varMen, string[] values, bool andor)
        {
            string SqlSelect = "";
            if (varMen != null && values != null && values.GetLength(0) > 0 && varMen.GetLength(0) == values.GetLength(0))
                SqlSelect += varMen[0] + " like " + formatStr(values[0]);
            if (varMen != null && values != null && values.GetLength(0) > 0 && varMen.GetLength(0) == values.GetLength(0))
                for (int i = 1; i < values.GetLength(0); i++)
                    if (andor)
                        SqlSelect += " and " + varMen[i] + " like " + formatStr(values[i]);
                    else
                        SqlSelect += " or " + varMen[i] + " like " + formatStr(values[i]);
            return SqlSelect;
        }
        /// <summary>
        /// 字符串数组转换XML
        /// </summary>
        /// <param name="aList">输入字符串数组</param>
        /// <param name="nodeName">参数名</param>
        /// <param name="nodeAttrb">参数值</param>
        /// <returns>返回xml字符串</returns>
        public static string arrayToXML(string[] aList, string rootName, string nodeName, string nodeAttrb)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement rootXe = doc.CreateElement(rootName);
            XmlNode xn = doc.AppendChild(rootXe);
            for (int i = 0; i < aList.Length; i++)
            {
                XmlElement xe = doc.CreateElement(nodeName);
                XmlAttribute xab = doc.CreateAttribute(nodeAttrb);
                xab.InnerText = aList[i];
                xe.Attributes.Append(xab);
                xn.AppendChild(xe);
            }

            return doc.InnerXml;
        }
        /// <summary>
        /// 字符串转换数组
        /// </summary>
        /// <param name="values">输入字符串</param>
        /// <param name="spChr">分隔符号</param>
        /// <returns>返回数组</returns>
        public static string[] getString(string values, char spChr)
        {
            return values.Split(spChr);
        }

        #endregion

        #region 数组转换
        /// <summary>
        /// 数组序列化
        /// </summary>
        /// <param name="x">输入数组</param>
        /// <returns>输出字符串</returns>
        public static string ListToString(List<string> x)
        {
            string retrunValues = "";
            if (x.Count > 0)
            {
                for (int i = 0; i < x.Count; i++)
                    if (i == 0)
                        retrunValues = x[i];
                    else
                        retrunValues += "|" + x[i];
            }
            return retrunValues;
        }
        #endregion
    }
}
