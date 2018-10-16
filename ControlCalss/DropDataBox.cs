using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ControlClass
{
    public partial class DropDataBox : UserControl
    {
        #region 全局变量
        public DataTable dataSource = new DataTable(); //连接数据源
        public DataTable DataSelect = new DataTable(); //缓存数据表
        public string NText = ""; //控件显示值
        public string deptname = ""; //部门名称
        public string deptid = ""; //部门id
        public object values = new object(); //查询列名
        public object mainColValues = new object(); //查询输出值列名
        public object mainCelValues = new object(); //查询输出值
        public string focusNext = ""; //下一焦点名称
        public bool isnull = false;
        #endregion

        /// <summary>
        /// 控件初始化
        /// </summary>
        public DropDataBox()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint
                     | ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);//透明设置
            txtboxdate.AutoGenerateColumns = false;//取消自动生成列
        }

        #region 控件调用函数
        /// <summary>
        /// DataGridView列初始化
        /// </summary>
        /// <param name="columnsName"></param>
        /// <param name="dgv"></param>
        public void dgvColumnsBinds(string[,] columnsName, int[] width)
        {
            txtboxdate.Columns.Clear();
            if (columnsName.Length > 0)
            {
                for (int i = 0; i < columnsName.GetLength(0); i++)
                {
                    DataGridViewTextBoxColumn dgvTBC = new DataGridViewTextBoxColumn();
                    dgvTBC.Name = columnsName[i, 0];
                    dgvTBC.DataPropertyName = columnsName[i, 0];
                    dgvTBC.HeaderText = columnsName[i, 1];
                    dgvTBC.Width = width[i];
                    txtboxdate.Columns.Add(dgvTBC);
                }
            }
        }
        /// <summary>
        /// 控件显示大小改变
        /// </summary>
        private void sizeChange()
        {
            if (!string.IsNullOrEmpty(txtbox.Text.Trim()))
            {
                this.Height = 173;//显示值不为空，显示下拉框
            }
            NText = txtbox.Text;
            if (isnull)
            {
                this.Width = 330;
                txtboxdate.Width = 325;
                txtboxdate.Height = 150;
                ValueSelect();
            }
            else
                if (dataSource == null)
                ValueSelect();//连接数据源为空，选择默认
            else
            {
                this.Width = 400;
                txtboxdate.Width = 400;
                txtboxdate.Height = 150;
                if (mainColValues is string[])
                    ValueSelect((string[])mainColValues);
            }
        }
        /// <summary>
        /// 查找匹配值
        /// </summary>
        private void ValueSelect()
        {
            try
            {
                DataSelect.Clear();//清除缓存
                string sqlselect = "";
                string[] varMen = { "deptid_chr", "deptname_vchr", "pycode_chr" };
                string Value = formatSqlStr(txtbox.Text);
                string[] values = { Value, Value, Value };
                sqlselect = StrClass.SqlFormat(varMen, values, false);
                DataRow[] z = dataSource.Select(sqlselect);
                for (int i = 0; i < z.GetLength(0); i++)
                    DataSelect.ImportRow(z[i]);
                txtboxdate.DataSource = DataSelect;
                if (txtboxdate.Rows.Count > 0 && txtboxdate.SelectedRows.Count < 1)
                    txtboxdate.Rows[0].Selected = true;
            }
            catch { }
        }
        /// <summary>
        /// 查找匹配数据
        /// </summary>
        /// <param name="strValmen">条件值</param>
        private void ValueSelect(object strValmen)
        {
            DataSelect.Clear();
            string sqlselect = "";
            try
            {
                string[] varMen = (string[])strValmen;
                string Value = formatSqlStr(txtbox.Text);
                string[] values = new string[varMen.GetLength(0)];
                for (int i = 0; i < values.GetLength(0); i++)
                    values[i] = Value;
                sqlselect = StrClass.SqlFormat(varMen, values, false);
                DataRow[] z = dataSource.Select(sqlselect, varMen[0] + " asc");
                int znum = z.GetLength(0);
                for (int i = 0; i < znum; i++)
                    DataSelect.ImportRow(z[i]);
                txtboxdate.DataSource = DataSelect;
                if (txtboxdate.Rows.Count > 0 && txtboxdate.SelectedRows.Count < 1)
                    txtboxdate.Rows[0].Selected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查找失败，错误代码：" + ex.Message);
            }
        }
        /// <summary>
        /// 控件加载函数
        /// </summary>
        public void txtDataBoxLoad()
        {
            try
            {
                DataSelect = dataSource.Clone();//缓存复制数据源表结构
                txtboxdate.DataSource = dataSource; //连接数据源
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }
        /// <summary>
        /// txtboxdate获取数据
        /// </summary>
        /// <param name="rowIndex">txtboxdate行数</param>
        private void txtboxdateGet(int rowIndex)
        {
            txtboxdate.Rows[rowIndex].Selected = true;
            if (dataSource == null || isnull)
            {
                deptid = txtboxdate.Rows[rowIndex].Cells["deptid_chr"].Value + "";
                deptname = txtboxdate.Rows[rowIndex].Cells["deptname_vchr"].Value + "";
                txtbox.Text = txtboxdate.Rows[rowIndex].Cells["deptname_vchr"].Value + "";
            }
            else
            {
                if (mainColValues is string[])
                {
                    string[] ColumnsValues = (string[])mainColValues;
                    if (((string[])mainColValues).GetLength(0) > 0)
                    {
                        int valLength = ColumnsValues.GetLength(0);
                        if (valLength > 1)
                        {
                            string[] newCelValues = new string[valLength];
                            for (int i = 0; i < valLength; i++)
                            {
                                string x = ColumnsValues[i];
                                newCelValues[i] = txtboxdate.Rows[rowIndex].Cells[x].Value + "";
                            }
                            mainCelValues = newCelValues;
                        }
                        txtbox.Text = txtboxdate.Rows[rowIndex].Cells[ColumnsValues[0]].Value + "";
                    }
                }
            }
            sizeLeave();
            txtbox.SelectAll();
        }

        #endregion

        #region dgvDateSource控件动作
        /// <summary>
        /// 双击选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDateSource_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int rowIndex = new Int32();
            rowIndex = txtboxdate.HitTest(e.X, e.Y).RowIndex;
            if (e.Button != MouseButtons.Left || rowIndex < 0) return;
            txtboxdateGet(rowIndex);
        }
        #endregion

        #region txtboxdate控件动作
        /// <summary>
        /// txtboxdate键盘动作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtboxdate_KeyDown(object sender, KeyEventArgs e)
        {
            //回车获取改变值
            if (e.KeyCode == Keys.Enter)
            {
                int rowIndex = new Int32();
                rowIndex = txtboxdate.SelectedRows[0].Index;
                txtboxdate.Rows[rowIndex].Selected = true;
                txtboxdateGet(rowIndex);
            }
            //下键选择下一行
            if (e.KeyCode == Keys.Down)
                if (txtboxdate.SelectedCells[0].RowIndex < txtboxdate.Rows.Count - 1 && txtboxdate.SelectedRows[0].Index + 1 < txtboxdate.Rows.Count - 1)
                {
                    txtboxdate.FirstDisplayedScrollingRowIndex = txtboxdate.SelectedRows[0].Index + 1;
                    txtboxdate.Rows[txtboxdate.SelectedRows[0].Index + 1].Selected = true;
                    txtboxdate.Rows[txtboxdate.SelectedRows[0].Index].Selected = false;
                }
            //上键选择上一行
            if (e.KeyCode == Keys.Up)
                if (txtboxdate.SelectedCells[0].RowIndex > 0 && txtboxdate.SelectedRows[0].Index - 1 > 0)
                {
                    txtboxdate.FirstDisplayedScrollingRowIndex = txtboxdate.SelectedRows[0].Index - 1;
                    txtboxdate.Rows[txtboxdate.SelectedRows[0].Index - 1].Selected = true;
                    txtboxdate.Rows[txtboxdate.SelectedRows[0].Index].Selected = false;
                }
        }
        /// <summary>
        /// 缩小显示区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDataBox_Leave(object sender, EventArgs e)
        {
            this.Height = 23;
        }

        /// <summary>
        /// 单击选择数据行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtboxdate_MouseClick(object sender, MouseEventArgs e)
        {
            int rowIndex = new Int32();
            rowIndex = txtboxdate.HitTest(e.X, e.Y).RowIndex;
            if (e.Button != MouseButtons.Left || rowIndex < 0) return;
            txtboxdate.Rows[rowIndex].Selected = true;
        }
        #endregion

        #region txtbox控件动作
        /// <summary>
        /// 显示值改变动作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_TextChanged(object sender, EventArgs e)
        {
            if (txtbox.Focused)
            {
                sizeChange();
            }
            else
            {
                NText = txtbox.Text;
                if (isnull)
                {
                    ValueSelect();
                }
                else
                    if (dataSource == null)
                    ValueSelect();//连接数据源为空，选择默认
                else
                {
                    if (mainColValues is string[])
                        ValueSelect((string[])mainColValues);
                }
            }
            if (string.IsNullOrEmpty(txtbox.Text.Trim()))
            {
                deptid = string.Empty;
                deptname = string.Empty;
            }
        }
        /// <summary>
        /// txtCode键盘动作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_KeyDown(object sender, KeyEventArgs e)
        {
            //回车获取改变值
            if (e.KeyCode == Keys.Enter)
            {
                int rowIndex = new Int32();
                if (txtboxdate.SelectedRows.Count > 0)
                {
                    rowIndex = txtboxdate.SelectedRows[0].Index;
                    txtboxdateGet(rowIndex);
                }
            }
            //下键选择下一行
            if (e.KeyCode == Keys.Down)
                if (txtboxdate.SelectedCells[0].RowIndex < txtboxdate.Rows.Count)
                {
                    int x = txtboxdate.SelectedRows[0].Index;
                    if (x < txtboxdate.Rows.Count && x + 1 < txtboxdate.Rows.Count)
                    {
                        txtboxdate.FirstDisplayedScrollingRowIndex = x + 1;
                        txtboxdate.Rows[x + 1].Selected = true;
                        txtboxdate.Rows[x].Selected = false;
                    }
                }
            //上键选择上一行
            if (e.KeyCode == Keys.Up)
                if (txtboxdate.SelectedCells[0].RowIndex >= 0)
                {
                    int x = txtboxdate.SelectedRows[0].Index;
                    if (x > 0 && x - 1 >= 0)
                    {
                        txtboxdate.FirstDisplayedScrollingRowIndex = x - 1;
                        txtboxdate.Rows[x - 1].Selected = true;
                        txtboxdate.Rows[x].Selected = false;
                    }
                }
        }
        /// <summary>
        /// 获取焦点下拉
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_Enter(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtbox.Text) && txtbox.Focused)
            {
                sizeChange();
            }
        }
        #endregion

        #region 应用函数
        /// <summary>
        /// 焦点选择下一项
        /// </summary>
        public void sizeLeave()
        {
            Size x = new Size();
            x.Width = 120;
            x.Height = 23;
            this.Size = x;
            for (int i = 0; i < this.Parent.Controls.Count; i++)
                if (this.Parent.Controls[i].Name == focusNext)
                {
                    this.Parent.Controls[i].Focus();
                    return;
                }
        }
        /// <summary>
        /// 字符串添加单引号
        /// </summary>
        /// <param name="values">字符串值</param>
        /// <returns>返回改变值</returns>
        public string formatStr(string values)
        {
            return "'" + values + "'";
        }
        /// <summary>
        /// 字符串添加百分号
        /// </summary>
        /// <param name="values">字符串值</param>
        /// <returns>返回改变值</returns>
        public string formatSqlStr(string values)
        {
            return "%" + values + "%";
        }
        /// <summary>
        /// DataGridView列可见性设置
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="columnsName"></param>
        /// <param name="columnsTitle"></param>
        public void dgvVisibleBlind(DataGridView dgv, string[] columnsName)
        {
            if (dgv.Columns.Count > 0 && columnsName != null)
            {
                for (int i = 0; i < columnsName.GetLength(0); i++)
                    dgv.Columns[columnsName[i]].Visible = false;
            }
        }
        #endregion
    }
}
