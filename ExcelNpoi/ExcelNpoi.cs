using System;
using System.Data;
using System.Xml;
using System.Reflection;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using NPOI;
using NPOI.DDF;
using NPOI.HPSF;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.POIFS;
using NPOI.SS;
using NPOI.SS.UserModel;
using NPOI.Util;
using NPOI.OpenXml4Net;
using NPOI.OpenXmlFormats;
using NPOI.XSSF;
using NPOI.XSSF.UserModel;
using NPOI.XWPF;

namespace ExcelNpoi
{
    /// <summary>
    /// Excel 操作
    /// </summary>
    public class ExcelNpoi
    {
        #region Excel 操作函数

        #region NPOI Excel操作

        #region Excel 97-2003 操作

        #region DataTable 转换
        /// <summary>   
        /// 将DataTable数据导出到Excel表   
        /// </summary>   
        /// <param name="tmpDataTable">要导出的DataTable</param>   
        /// <param name="strFileName">Excel的保存路径及名称</param>   
        public static void DataTabletoExcel9(DataTable tmpDataTable, string strFileName)
        {
            if (!string.IsNullOrEmpty(strFileName) && null != tmpDataTable && tmpDataTable.Rows.Count > 0)
            {
                HSSFWorkbook book = new HSSFWorkbook();
                ISheet sheet = book.CreateSheet(tmpDataTable.TableName);

                IRow row = sheet.CreateRow(0);
                for (int i = 0; i < tmpDataTable.Columns.Count; i++)
                {
                    row.CreateCell(i).SetCellValue(tmpDataTable.Columns[i].ColumnName);
                }
                for (int i = 0; i < tmpDataTable.Rows.Count; i++)
                {
                    IRow row2 = sheet.CreateRow(i + 1);
                    for (int j = 0; j < tmpDataTable.Columns.Count; j++)
                    {
                        row2.CreateCell(j).SetCellValue(Convert.ToString(tmpDataTable.Rows[i][j]));
                    }
                }
                // 写入到客户端  
                using (MemoryStream ms = new MemoryStream())
                {
                    book.Write(ms);
                    using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                    {
                        byte[] data = ms.ToArray();
                        fs.Write(data, 0, data.Length);
                        fs.Flush();
                    }
                    book = null;
                }
            }
        }
        #endregion

        #endregion

        #region DataTable 转换
        /// <summary>
        /// 导入Excel文件
        /// </summary>
        /// <param name="filePath">Excel的导入路径及名称</param>
        /// <returns></returns>
        public static DataTable ImportExcelFile(string filePath, bool isExcel9, bool isFirstColumns)
        {
            try
            {
                object book = new object();
                ISheet sheet;
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    if (isExcel9)
                    {
                        book = new HSSFWorkbook(file);
                        sheet = ((HSSFWorkbook)book).GetSheetAt(0);
                    }
                    else
                    {
                        book = new XSSFWorkbook(file);
                        sheet = ((XSSFWorkbook)book).GetSheetAt(0);
                    }
                }
                DataTable dt = new DataTable();
                int colNums = 0;
                for (int i = 0; i < sheet.LastRowNum; i++)
                {
                    IRow ir = sheet.GetRow(i);
                    if (isFirstColumns)
                    {
                        if (i == 0)
                        {
                            for (int x = 0; x < ir.LastCellNum; x++)
                            {
                                dt.Columns.Add(ir.GetCell(x).ToString());
                            }
                        }
                    }
                    else
                    {
                        if (colNums < ir.LastCellNum)
                        {
                            for (int x = colNums; x < ir.LastCellNum; x++)
                            {
                                if (x / 26 == 0)
                                    dt.Columns.Add(Convert.ToChar(((int)'A') + x % 26).ToString());
                                else
                                    dt.Columns.Add(Convert.ToChar(((int)'A') - 1 + x / 26).ToString() + Convert.ToChar(((int)'A') + x % 26).ToString());
                                colNums++;
                            }
                        }
                    }
                    if ((isFirstColumns && i > 0) || !isFirstColumns)
                    {
                        DataRow dr = dt.NewRow();
                        for (int x = 0; x < ir.LastCellNum; x++)
                        {
                            if (x < dt.Columns.Count)
                            {
                                ICell ic = ir.GetCell(x);
                                if (ic == null)
                                    dr[x] = null;
                                else
                                    dr[x] = ic.ToString();
                            }
                        }
                        dt.Rows.Add(dr);
                    }
                }
                return dt;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region DataGridView 转换

        /// <summary>
        /// DataGridView 导出Excel
        /// </summary>
        /// <param name="datagridview">DataGridView源</param>
        /// <param name="strFileName">文件位置</param>
        /// <returns></returns>
        public static void ExportExcelNopi(DataGridView datagridview, string strFileName, bool isExcel9)
        {
            try
            {
                if (!string.IsNullOrEmpty(strFileName) && null != datagridview && datagridview.Rows.Count > 0)
                {
                    object book = new object();
                    string[] orgSheetNames = strFileName.Split('/');
                    string[] sheetNames = orgSheetNames[orgSheetNames.GetLength(0) - 1].Split('\\');
                    string sheetName = sheetNames[sheetNames.GetLength(0) - 1];
                    ISheet sheet;
                    ICellStyle style;
                    IFont font;
                    if (isExcel9)
                    {
                        book = new HSSFWorkbook();
                        sheet = ((HSSFWorkbook)book).CreateSheet(sheetName.Split('.')[0]);
                        style = ((HSSFWorkbook)book).CreateCellStyle();
                        font = ((HSSFWorkbook)book).CreateFont();
                    }
                    else
                    {
                        book = new XSSFWorkbook();
                        sheet = ((XSSFWorkbook)book).CreateSheet(sheetName.Split('.')[0]);
                        style = ((XSSFWorkbook)book).CreateCellStyle();
                        font = ((XSSFWorkbook)book).CreateFont();
                    }
                    IRow row = sheet.CreateRow(0);
                    style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    font.FontHeight = 15 * 15;
                    style.SetFont(font);
                    int displayIndex = 0;
                    for (int i = 0; i < datagridview.Columns.Count; i++)
                    {
                        if (datagridview.Columns[i].Visible)
                        {
                            ICell cell = row.CreateCell(displayIndex);
                            cell.SetCellValue(datagridview.Columns[i].HeaderText);
                            cell.CellStyle = style;
                            sheet.SetColumnWidth(i, datagridview.Columns[i].HeaderText.Length * 700);
                            displayIndex++;
                        }
                    }
                    //sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, displayIndex - 1));
                    for (int i = 0; i < datagridview.Rows.Count; i++)
                    {
                        IRow row2 = sheet.CreateRow(i + 1);
                        displayIndex = 0;
                        for (int j = 0; j < datagridview.Columns.Count; j++)
                        {
                            if (datagridview.Columns[j].Visible)
                            {
                                row2.CreateCell(displayIndex).SetCellValue(Convert.ToString(datagridview.Rows[i].Cells[j].Value));
                                displayIndex++;
                            }
                        }
                    }
                    for (int i = 0; i < sheet.LastRowNum; i++)
                    {
                        try
                        {
                            string value = sheet.GetRow(0).GetCell(i).StringCellValue;
                            if (!string.IsNullOrEmpty(value))
                                sheet.SetColumnWidth(i, value.Length * 600);
                        }
                        catch
                        {
                            int error = i;
                        }
                    }
                    // 写入到客户端  
                    using (MemoryStream ms = new MemoryStream())
                    {
                        if (isExcel9)
                            ((HSSFWorkbook)book).Write(ms);
                        else
                            ((XSSFWorkbook)book).Write(ms);
                        using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                        {
                            byte[] data = ms.ToArray();
                            fs.Write(data, 0, data.Length);
                            fs.Flush();
                        }
                        book = null;
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            finally
            {

            }
        }

        #endregion


        /// <summary>  
        /// 将excel导入到datatable  
        /// </summary>  
        /// <param name="filePath">excel路径</param>  
        /// <param name="isColumnName">第一行是否是列名</param>  
        /// <returns>返回datatable</returns>  
        public static string ExcelToDataTable(string filePath, bool isColumnName,ref DataTable dt)
        {
            DataTable dataTable = null;
            FileStream fs = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            string error = "";
            int startRow = 0;
            try
            {
                using (fs = File.OpenRead(filePath))
                {
                    // 2007版本  
                    if (filePath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本  
                    else if (filePath.IndexOf(".xls") > 0)
                        workbook = new HSSFWorkbook(fs);
                    
                    if (workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet  
                        dataTable = new DataTable();
                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;//总行数  
                            if (rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(0);//第一行  
                                int cellCount = firstRow.LastCellNum;//列数  

                                //构建datatable的列  
                                if (isColumnName)
                                {
                                    startRow = 1;//如果第一行是列名，则从第二行开始读取  
                                    for (int i = firstRow.FirstCellNum; i < cellCount; i++)
                                    {
                                        cell = firstRow.GetCell(i);
                                        if (cell != null)
                                        {
                                            if (cell.StringCellValue != null)
                                            {
                                                column = new DataColumn(cell.StringCellValue);
                                                dataTable.Columns.Add(column);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = firstRow.FirstCellNum; i < cellCount; i++)
                                    {
                                        column = new DataColumn("column" + (i + 1));
                                        dataTable.Columns.Add(column);
                                    }
                                }
                                
                                //填充行  
                                for (int i = startRow; i <= rowCount; i++)
                                {
                                    row = sheet.GetRow(i);
                                    if (row == null) continue;

                                    dataRow = dataTable.NewRow();
                                    for (int j = row.FirstCellNum; j < cellCount; j++)
                                    {
                                        if (j >= 0)
                                        {
                                            cell = row.GetCell(j);
                                            if (cell == null)
                                            {
                                                dataRow[j] = "";
                                            }
                                            else
                                            {
                                                //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)  
                                                switch (cell.CellType)
                                                {
                                                    case CellType.Blank:
                                                        dataRow[j] = "";
                                                        break;
                                                    case CellType.Numeric:
                                                        short format = cell.CellStyle.DataFormat;
                                                        //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理  
                                                        if (format == 14 || format == 31 || format == 57 || format == 58)
                                                            dataRow[j] = cell.DateCellValue;
                                                        else
                                                            dataRow[j] = cell.NumericCellValue;
                                                        break;
                                                    case CellType.String:
                                                        dataRow[j] = cell.StringCellValue;
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }
                    }
                }
                dt = dataTable;
                return "0";
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                error += ex.Message;
                return error;
            }
        }


        #endregion

        #region COM+ 操作Excel
        /// <summary>
        /// Exports a passed datagridview to an Excel worksheet.
        /// If captions is true, grid headers will appear in row 1.
        /// Data will start in row 2.
        /// using System.Reflection;
        /// </summary>
        /// <param name="datagridview"></param>
        /// <param name="captions"></param>
        public static string Export2Excel(DataGridView datagridview, bool captions)
        {
            object objApp_Late;
            object objBook_Late;
            object objBooks_Late;
            object objSheets_Late;
            object objSheet_Late;
            object objRange_Late;
            object[] Parameters;

            List<string> headers = new List<string>();
            List<string> columns = new List<string>();
            List<string> colName = new List<string>();

            int c = 0;
            int m = 0;

            for (c = 0; c < datagridview.Columns.Count; c++)
            {
                for (int j = 0; j < datagridview.Columns.Count; j++)
                {
                    DataGridViewColumn tmpcol = datagridview.Columns[j];
                    if (tmpcol.DisplayIndex == c)
                    {
                        if (tmpcol.Visible) //不显示的隐藏列初始化为tag＝0
                        {
                            int lastAmo = 0;
                            int firstAmo = 0;
                            string columnsSign = "";
                            headers.Add(tmpcol.HeaderText);
                            lastAmo = m % 26 + 65;
                            firstAmo = m / 26 + 64;
                            if (firstAmo < 65)
                                columnsSign = Convert.ToString((char)lastAmo);
                            else
                                columnsSign = Convert.ToString((char)firstAmo) + columnsSign;
                            columns.Add(columnsSign);
                            colName.Add(tmpcol.Name);
                            m++;
                            break;
                        }
                    }
                }
            }

            try
            {
                // Get the class type and instantiate Excel.
                Type objClassType;
                objClassType = Type.GetTypeFromProgID("Excel.Application");
                objApp_Late = Activator.CreateInstance(objClassType);
                //Get the workbooks collection.
                objBooks_Late = objApp_Late.GetType().InvokeMember("Workbooks", BindingFlags.GetProperty, null, objApp_Late, null);
                //Add a new workbook.
                objBook_Late = objBooks_Late.GetType().InvokeMember("Add", BindingFlags.InvokeMethod, null, objBooks_Late, null);
                //Get the worksheets collection.
                objSheets_Late = objBook_Late.GetType().InvokeMember("Worksheets", BindingFlags.GetProperty, null, objBook_Late, null);
                //Get the first worksheet.

                Parameters = new Object[1];
                Parameters[0] = 1;
                objSheet_Late = objSheets_Late.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, objSheets_Late, Parameters);


                if (captions)
                {
                    // Create the headers in the first row of the sheet
                    for (c = 0; c < headers.Count; c++)
                    {
                        //Get a range object that contains cell.
                        Parameters = new Object[2];
                        Parameters[0] = columns[c] + "2";
                        Parameters[1] = Missing.Value;
                        objRange_Late = objSheet_Late.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, objSheet_Late, Parameters);
                        //Write Headers in cell.
                        Parameters = new Object[1];
                        Parameters[0] = headers[c];
                        objRange_Late.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, objRange_Late, Parameters);
                    }
                }

                // Now add the data from the grid to the sheet starting in row 2
                for (int i = 0; i < datagridview.RowCount; i++)
                {
                    c = 0;
                    foreach (string txtCol in colName)
                    {
                        DataGridViewColumn col = datagridview.Columns[txtCol];
                        if (col.Visible)
                        {
                            //Get a range object that contains cell.
                            Parameters = new Object[2];
                            Parameters[0] = columns[c] + Convert.ToString(i + 3);
                            Parameters[1] = Missing.Value;
                            objRange_Late = objSheet_Late.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, objSheet_Late, Parameters);
                            //Write Headers in cell.
                            Parameters = new Object[1];
                            //Parameters[0] = datagridview.Rows[i].Cells[headers[c]].Value.ToString();
                            Parameters[0] = datagridview.Rows[i].Cells[col.Name].Value.ToString();
                            objRange_Late.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, objRange_Late, Parameters);
                            c++;
                        }
                    }
                }

                //Return control of Excel to the user.
                Parameters = new Object[1];
                Parameters[0] = true;
                objApp_Late.GetType().InvokeMember("Visible", BindingFlags.SetProperty, null, objApp_Late, Parameters);
                objApp_Late.GetType().InvokeMember("UserControl", BindingFlags.SetProperty, null, objApp_Late, Parameters);
                return "导出成功!";
            }
            catch (Exception theException)
            {
                String errorMessage;
                errorMessage = "Error: ";
                errorMessage = String.Concat(errorMessage, theException.Message);
                errorMessage = String.Concat(errorMessage, " Line: ");
                errorMessage = String.Concat(errorMessage, theException.Source);

                MessageBox.Show(errorMessage, "Error");
                return errorMessage;
            }
        }
        #endregion

        #endregion
    }

}