using System;
using System.Collections.Generic;
using System.Data;

namespace ControlClass
{
    public class DataClass
    {
        #region DataTable操作
        /// <summary>
        /// 返回匹配数据表
        /// </summary>
        /// <param name="OrgData">原始数据表</param>
        /// <param name="EqualsValues">匹配关键字</param>
        /// <param name="GetValues">匹配值</param>
        /// <returns>匹配数据表</returns>
        public static DataTable GetEquals(DataTable OrgData, List<string> EqualsValues, string GetValues)
        {
            DataTable DataSelect = OrgData.Clone();
            DataTable dt = OrgData.Copy();
            DataView dv = dt.DefaultView;
            string sqlselect = "";
            try
            {
                string Value = StrClass.formatSqlStr(GetValues);
                string[] Values = new string[EqualsValues.Count];
                for (int i = 0; i < Values.GetLength(0); i++)
                    Values[i] = Value;
                sqlselect = StrClass.SqlFormat(EqualsValues.ToArray(), Values, false);
                dv.RowFilter = sqlselect;
                dv.Sort = EqualsValues[0] + " asc";
                DataSelect = dv.ToTable().Copy();
                return DataSelect;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return null;
            }
        }

        #region DataTable转换函数
        /// <summary>
        /// 数据源转换数据表函数
        /// </summary>
        /// <param name="dtColumns">新增数据列名</param>
        /// <param name="dtValues">新增数据值</param>
        /// <param name="dtSource">原数据源</param>
        /// <returns>返回改变数据表</returns>
        public System.Data.DataTable DTBinds(string[] dtColumns, string[,] dtValues, System.Data.DataTable dtSource)
        {
            return dtBinds(dtColumns, dtValues, dtSource);
        }
        private System.Data.DataTable dtBinds(string[] dtColumns, string[,] dtValues, System.Data.DataTable dtSource)
        {
            System.Data.DataTable dt = new System.Data.DataTable();//返回数据表
            System.Data.DataTable dtBase = new System.Data.DataTable();//原数据源
                                                                       //添加新增数据值至返回数据表
            if (dtColumns != null && dtColumns.GetLength(0) > 0)
            {
                for (int i = 0; i < dtColumns.GetLength(0); i++)
                    dt.Columns.Add(dtColumns[i], typeof(string));
                if (dtValues != null && dtValues.Length > 0)
                    for (int i = 0; i < dtValues.GetLength(0); i++)
                    {
                        DataRow dr = dt.NewRow();
                        for (int j = 0; j < dtValues.GetLength(1); j++)
                            dr[dtColumns[j]] = dtValues[i, j];
                        dt.Rows.Add(dr);
                    }
            }
            //复制原数据源至返回数据源
            if (dtSource != null)
            {
                dtBase = dtSource;
                for (int i = 0; i < dtBase.Rows.Count; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr = dtBase.Rows[i];
                    dt.Rows.Add(dr.ItemArray);
                }
            }
            return dt;
        }
        /// <summary>
        /// 数据源转换数据表函数单行增加
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
        public DataRow DRBinds(string[] values, string[] columnsName, System.Data.DataTable dtSource)
        {
            return drBinds(values, columnsName, dtSource);
        }
        public DataRow drBinds(string[] values, string[] columnsName, System.Data.DataTable dtSource)
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

        #endregion
    }
}
