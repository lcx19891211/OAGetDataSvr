using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace ControlClass
{
    public class LibClass
    {
        #region Datagridview 操作函数
        /// <summary>
        /// dgv第一列加checkbox
        /// </summary>
        /// <param name="dgv">datagridview名称</param>
        public static void AddCheckBoxToDataGridView(ref DataGridView dgv)
        {
            DataGridViewCheckBoxColumn FirstCells = new DataGridViewCheckBoxColumn();
            FirstCells.Name = "dgvCellsCheck";
            FirstCells.DataPropertyName = "dgvCellsCheck";
            FirstCells.HeaderText = "";
            FirstCells.ReadOnly = false;
            FirstCells.Width = 60;
            dgv.Columns.Add(FirstCells);
            System.Windows.Forms.CheckBox ckBox = new System.Windows.Forms.CheckBox();
            ckBox.Text = "全选";
            ckBox.Parent = dgv;//设置checkbox父控件为DataGridView
            ckBox.Checked = false;
            System.Drawing.Rectangle rect = dgv.GetCellDisplayRectangle(0, -1, true);
            ckBox.Size = new System.Drawing.Size(dgv.Columns[0].Width - 10, 18);
            ckBox.Location = new Point(rect.Location.X + 5, rect.Location.Y + 10);
            ckBox.CheckedChanged += new EventHandler(ckBox_CheckedChanged);
            dgv.Controls.Add(ckBox);
            ckBox.BringToFront();
        }
        /// <summary>
        /// DataGridView列名显示设置
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="columnsName"></param>
        /// <param name="columnsTitle"></param>
        public static void dgvHeaderTitleBinds(DataGridView dgv, string[] columnsName, string[] columnsTitle)
        {
            if (dgv.Columns.Count > 0 && columnsName != null && columnsTitle != null && columnsName.GetLength(0) == columnsTitle.GetLength(0))
            {
                for (int i = 0; i < columnsName.GetLength(0); i++)
                    dgv.Columns[columnsName[i]].HeaderText = columnsTitle[i];
            }
        }
        /// <summary>
        /// DataGridView列可见性设置
        /// </summary>
        /// <param name="columnsName">列隐藏名</param>
        /// <param name="dgv">返回DataGridView</param>
        public static void dgvVisibleBlind(string[] columnsName, ref DataGridView dgv)
        {
            if (dgv.Columns.Count > 0 && columnsName != null)
            {
                for (int i = 0; i < dgv.Columns.Count; i++)
                    dgv.Columns[i].Visible = true;
                for (int i = 0; i < columnsName.GetLength(0); i++)
                    dgv.Columns[columnsName[i]].Visible = false;
            }
        }
        /// <summary>
        /// DataGridView列宽度重置
        /// </summary>
        /// <param name="columnsName"></param>
        /// <param name="dgv"></param>
        public static void dgvColumnsSizeBinds(string[] columnsName, int[] Size, ref DataGridView dgv)
        {
            if (columnsName.Length > 0 && columnsName.Length == Size.Length)
            {
                for (int i = 0; i < columnsName.GetLength(0); i++)
                {
                    dgv.Columns[columnsName[i]].Width = Size[i];
                }
            }
        }
        /// <summary>
        /// DataGridView列数据源绑定
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="columnsName"></param>
        /// <param name="columnsTitle"></param>
        public static void dgvDataSourseBlind(ref DataGridView dgv, DataTable dt, bool isCheck)
        {
            dgv.Rows.Clear();
            if (dt != null && dt.Rows.Count > 0)
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string[] values = new string[dgv.Columns.Count];
                    for (int x = 0; x < dgv.Columns.Count; x++)
                    {
                        try
                        {
                            if (isCheck && x == 0)
                                values[x] = Convert.ToString(false);
                            else
                                values[x] = dt.Rows[i][dgv.Columns[x].Name] + "";
                        }
                        catch { }
                    }
                    dgv.Rows.Add(values);
                }
        }
        /// <summary>
        /// DataGridView列初始化
        /// </summary>
        /// <param name="columnsName"></param>
        /// <param name="dgv"></param>
        /// <param name="colName"></param>
        /// <param name="isClear"></param>
        public static void dgvColumnsBinds(string[,] columnsName, ref DataGridView dgv, string[] colName,bool isClear)
        {
            if(isClear)
                dgv.Columns.Clear();
            if (columnsName != null && columnsName.Length > 0)
            {
                for (int i = 0; i < columnsName.GetLength(0); i++)
                {
                    DataGridViewTextBoxColumn dgvTBC = new DataGridViewTextBoxColumn();
                    dgvTBC.Name = columnsName[i, 0];
                    dgvTBC.DataPropertyName = columnsName[i, 0];
                    dgvTBC.HeaderText = columnsName[i, 1];
                    dgvTBC.HeaderCell.Style.Font = new Font(new FontFamily("宋体"), 12);
                    dgvTBC.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvTBC.CellTemplate.Style.Font = new Font(new FontFamily("宋体"), 12);
                    dgv.Columns.Add(dgvTBC);
                    if (colName != null && colName.GetLength(0) > 0)
                    {
                        for (int j = 0; j < colName.GetLength(0); j++)
                        {
                            if (dgvTBC.Name == colName[j])
                                dgvTBC.ReadOnly = false;
                            else
                                dgvTBC.ReadOnly = true;
                        }
                    }
                    else
                    {
                        dgvTBC.ReadOnly = true;
                    }
                }
            }
        }
        /// <summary>
        /// 添加全选反选动作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ckBox_CheckedChanged(object sender, EventArgs e)
        {
            DataGridView newdgv = (DataGridView)((CheckBox)sender).Parent;
            for (int i = 0; i < newdgv.Rows.Count; i++)
            {
                newdgv.Rows[i].Cells[0].Value = ((System.Windows.Forms.CheckBox)sender).Checked;
                newdgv.Rows[i].Cells[3].Selected = true;
            }
            newdgv.EndEdit();
        }
        #endregion

        #region 控件函数

        /// <summary>
        /// Combobox与DataTable绑定
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="cbb"></param>
        public static void ComboBoxDtBind(DataTable dt, ComboBox cbb, string displayName, string valueName, bool isDelFirst)
        {
            if (dt == null || dt.Rows.Count <= 0)
            {
                dt = new DataTable();
                dt.Columns.Add("type", typeof(int));
                dt.Columns.Add("Name", typeof(string));
                DataRow dr = dt.NewRow();

                dr["type"] = "100";
                dr["Name"] = "连接数据库失败";
                dt.Rows.Add(dr);

                cbb.DataSource = dt;
                cbb.DisplayMember = "Name";
                cbb.ValueMember = "type";
            }
            else
            {
                if (isDelFirst)
                    dt.Rows[dt.Rows.Count - 1].Delete();
                cbb.DataSource = dt;
                cbb.DisplayMember = displayName;
                cbb.ValueMember = valueName;
                if (cbb.Items.Count > 0)
                    cbb.SelectedIndex = 0;
            }
        }
        #endregion
    }

}
