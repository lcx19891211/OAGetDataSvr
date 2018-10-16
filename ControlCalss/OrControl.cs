using System;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

namespace ControlClass
{
    #region 复写控件
    /// <summary>
    /// CheckedListBox 选择无底色
    /// </summary>
    public class CheckedListBox : System.Windows.Forms.CheckedListBox
    {
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            DrawItemEventArgs e2 = new DrawItemEventArgs(e.Graphics, e.Font, new Rectangle(e.Bounds.Location, e.Bounds.Size), e.Index, (e.State & DrawItemState.Focus) == DrawItemState.Focus ? DrawItemState.Focus : DrawItemState.None, Color.Black, this.BackColor);
            base.OnDrawItem(e2);
        }
    }

    /// <summary>
    /// DataGridView 自动添加下拉框
    /// </summary>
    public class DataGridViewDrop : System.Windows.Forms.DataGridView
    {
        #region 基础设置值
        #region 公共值
        public List<string> DropListPrimaryKey = new List<string>();//显示下拉框父控件关键字
        public strCollection DropListSelectedValues = new strCollection();//下拉框选中值
        public List<string> DropListEqualValues = new List<string>();//自动匹配条件值数组
                                                                     /// <summary>
                                                                     /// 父控件与下拉框匹配条件值数组，一维为下拉框绑定列名，二维为父控件绑定列名
                                                                     /// </summary>
        public string[,] DropListSetParentValues;
        #endregion

        #region 私有值
        private object tbControl;//编辑模式编辑控件
        private DataGridView DropList = new DataGridView();//下拉框
        private DataTable dropListDataSource = new DataTable();//下拉框数据源
        private DataTable selectDataSource = new DataTable();//匹配的数据源
        private string[,] dropListColumnsBind;//下拉框列名绑定
        private List<string> dropListDisplayColumns = new List<string>();//下拉框显示设置
        private bool isInitTb = false;
        #endregion
        #endregion

        #region 对外部数据绑定
        /// <summary>
        /// 下拉数据源
        /// </summary>
        public DataTable DropListDataSource
        {
            get
            {
                return dropListDataSource;
            }
            set
            {
                dropListDataSource = value;
                DropList.Name = "DropList";
                DropList.AutoGenerateColumns = false;
                DropList.AllowUserToResizeRows = false;
                DropList.AllowUserToAddRows = false;
                DropList.AutoResizeColumns();
                DropList.RowHeadersVisible = false;
                DropList.MultiSelect = false;
                DropList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                DropList.ReadOnly = true;
                DropList.DataSource = dropListDataSource;
                DropList.Refresh();
            }
        }
        /// <summary>
        /// 下拉列绑定
        /// </summary>
        public string[,] DropListColumnsBind
        {
            get
            {
                return dropListColumnsBind;
            }
            set
            {
                dropListColumnsBind = value;
                DropList.DataSource = null;
                if (DropList.Rows.Count > 0)
                    DropList.Rows.Clear();
                if (DropList.Columns.Count > 0)
                    DropList.Columns.Clear();
                LibClass.dgvColumnsBinds(dropListColumnsBind, ref DropList, null, true);
                DropListSelectedValues.ColumnsBind(DropListColumnsBind);
            }
        }
        /// <summary>
        /// 下拉列可显示设置
        /// </summary>
        public List<string> DropListDisplayColumns
        {
            get
            {
                return dropListDisplayColumns;
            }
            set
            {
                dropListDisplayColumns = value;
                LibClass.dgvVisibleBlind(dropListDisplayColumns.ToArray(), ref DropList);
            }
        }
        /// <summary>
        /// 设置下拉框宽度
        /// </summary>
        public int DropListWidth
        {
            set
            {
                DropList.Width = value;
            }
        }
        /// <summary>
        /// 设置下拉框高度
        /// </summary>
        public int DropListHeight
        {
            set
            {
                DropList.Height = value;
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 公共字段初始化
        /// </summary>
        public void DropListPublicValue()
        {
            DropListDataSource = new DataTable();
        }
        /// <summary>
        /// 显示下拉框
        /// </summary>
        private bool DropListShow()
        {
            if (tbControl is TextBox)
            {
                TextBox tb = (TextBox)tbControl;
                //if (DropList.Rows.Count > 0)
                for (int i = 0; i < DropListPrimaryKey.Count; i++)
                {
                    if (this.CurrentCell.OwningColumn.Name == DropListPrimaryKey[i])
                    {
                        Rectangle rect = this.GetCellDisplayRectangle(this.CurrentCell.ColumnIndex, this.CurrentCell.RowIndex, true);
                        addControl(DropList, this, rect);
                        DropList.DataSource = dropListDataSource;
                        DropList.Refresh();
                        string GetPrimaryKey = "";
                        for (int j = 0; j < DropListSetParentValues.GetLength(0); j++)
                        {
                            if (this.CurrentCell.OwningColumn.Name == DropListSetParentValues[j, 1])
                            {
                                GetPrimaryKey = DropListSetParentValues[j, 0];
                                break;
                            }
                        }
                        for (int j = 0; j < DropList.Rows.Count; j++)
                        {

                            if (tb.Text == DropList.Rows[j].Cells[GetPrimaryKey].Value + "")
                            {
                                DropList.Rows[j].Selected = true;
                                DropList.FirstDisplayedScrollingRowIndex = j;
                                break;
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 关闭下拉框
        /// </summary>
        /// <returns></returns>
        private bool DropListClosed()
        {
            try
            {
                DropList.DataSource = null;
                DropList.Parent = null;
                DropList.Visible = false;
                DropList.MouseDoubleClick -= new MouseEventHandler(dgvDateSource_MouseDoubleClick);
                DropList.MouseClick -= new MouseEventHandler(dgvDateSource_MouseClick);
                DropList.KeyDown -= new KeyEventHandler(dgvDateSource_KeyDown);
                DropList.Leave -= new EventHandler(dgvDateSource_Leave);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 重写事件
        /// <summary>
        /// 退出单元格编辑模式
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCancelRowEdit(QuestionEventArgs e)
        {
            base.OnCancelRowEdit(e);
            DropListClosed();
        }
        /// <summary>
        /// 获取焦点自动进入编辑模式
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCellEnter(DataGridViewCellEventArgs e)
        {
            base.OnCellEnter(e);
            this.BeginEdit(true);
        }
        /// <summary>
        /// 单元格显示编辑模式
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEditingControlShowing(DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                base.OnEditingControlShowing(e);
                DropListShow();
                TextBox tb = (TextBox)e.Control;//获取编辑模式的控件
                tbControl = tb;
                if (!isInitTb)
                {
                    tb.GotFocus += new EventHandler(TextBoxGotFocus);
                    tb.TextChanged += new EventHandler(CellsTextChanged);
                    isInitTb = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 单元格失去焦点
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCellLeave(DataGridViewCellEventArgs e)
        {
            base.OnCellLeave(e);
            DropListClosed();
        }
        /// <summary>
        /// 重写键盘输入事件
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            ///屏蔽回车键方向键
            if (keyData == Keys.Enter || keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Left || keyData == Keys.Right)    //监听回车事件 
            {
                if (this.IsCurrentCellInEditMode)
                {
                    if (keyData == Keys.Enter)
                    {
                        CellLeaves();
                        dgvTab(this);
                    }
                    else
                        DropListUpDown(keyData);
                    return true;
                }
            }
            //继续原来base.ProcessCmdKey中的处理 
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region 控件方法函数
        /// <summary>
        /// 按键上下移动
        /// </summary>
        /// <param name="keyData"></param>
        private void DropListUpDown(Keys keyData)
        {
            if (DropList.SelectedRows.Count > 0)
            {
                int index = DropList.SelectedRows[0].Index;
                if (keyData == Keys.Up && DropList.SelectedRows[0].Index > 0)
                {
                    if (DropList.FirstDisplayedScrollingRowIndex == index)
                        DropList.FirstDisplayedScrollingRowIndex = index - 1;
                    DropList.Rows[DropList.SelectedRows[0].Index - 1].Selected = true;
                }
                if (keyData == Keys.Down && DropList.SelectedRows[0].Index < DropList.Rows.Count - 1)
                {
                    int displayRowCount = DropList.DisplayedRowCount(false);
                    if (DropList.FirstDisplayedScrollingRowIndex == index - displayRowCount + 1)
                        DropList.FirstDisplayedScrollingRowIndex = DropList.FirstDisplayedScrollingRowIndex + 1;
                    DropList.Rows[DropList.SelectedRows[0].Index + 1].Selected = true;
                }
            }
            else
            {
                if (this.SelectedCells.Count > 0)
                {
                    int index = this.SelectedCells[0].RowIndex;
                    if (keyData == Keys.Up && this.SelectedCells[0].RowIndex > 0)
                    {

                        this.CurrentCell = this.Rows[index - 1].Cells[this.SelectedCells[0].ColumnIndex];
                        if (this.EndEdit())
                            this.BeginEdit(true);
                        return;
                    }
                    if (keyData == Keys.Down && this.SelectedCells[0].RowIndex < this.Rows.Count - 1)
                    {

                        this.CurrentCell = this.Rows[index + 1].Cells[this.SelectedCells[0].ColumnIndex];
                        if (this.EndEdit())
                            this.BeginEdit(true);
                        return;
                    }
                    if (keyData == Keys.Left && this.SelectedCells[0].ColumnIndex > 0)
                    {

                        this.CurrentCell = this.Rows[index].Cells[this.SelectedCells[0].ColumnIndex - 1];
                        if (this.EndEdit())
                            this.BeginEdit(true);
                        return;
                    }
                    if (keyData == Keys.Right && this.SelectedCells[0].ColumnIndex < this.Columns.Count - 1)
                    {

                        this.CurrentCell = this.Rows[index].Cells[this.SelectedCells[0].ColumnIndex + 1];
                        if (this.EndEdit())
                            this.BeginEdit(true);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 控件添加下拉菜单
        /// </summary>
        /// <param name="control"></param>
        /// <param name="dgv"></param>
        /// <param name="rect"></param>
        private void addControl(Control control, DataGridView dgv, Rectangle rect)
        {
            if (control == null) return;
            control.Text = dgv.CurrentCell.Value + "";
            control.Tag = control.Text;
            control.Left = rect.Left;
            control.Top = rect.Top + dgv.CurrentCell.Size.Height;
            control.Visible = true;
            control.Parent = dgv;
            ///添加动作
            control.MouseDoubleClick += new MouseEventHandler(dgvDateSource_MouseDoubleClick);
            control.MouseClick += new MouseEventHandler(dgvDateSource_MouseClick);
            control.KeyDown += new KeyEventHandler(dgvDateSource_KeyDown);
            control.Leave += new EventHandler(dgvDateSource_Leave);
            control.BringToFront();
            control.Focus();
        }
        /// <summary>
        /// 自动进入下一个单元格编辑模式
        /// </summary>
        /// <param name="sender"></param>
        private void dgvTab(object sender)
        {
            if (sender is DataGridView)
            {
                DataGridView dgv = (DataGridView)sender;

                if (dgv.CurrentRow != null && dgv.CurrentRow.Index >= 0)
                {
                    int OrgIndex = dgv.CurrentCell.RowIndex;
                    int RowIndex = OrgIndex;
                    int ColumnsIndex = dgv.CurrentCell.ColumnIndex;
                    for (int j = RowIndex; j <= dgv.Rows.Count; j++)
                    {
                        for (int i = ColumnsIndex + 1; i <= dgv.Columns.Count; i++)
                        {
                            if (i == dgv.Columns.Count)
                            {
                                ColumnsIndex = 0;
                                break;
                            }
                            if (j == dgv.Rows.Count)
                            {
                                j = 0;
                                break;
                            }
                            if (j + 1 == OrgIndex) return;
                            if (i == ColumnsIndex)
                                break;
                            if (dgv.Columns[i].Visible && !dgv.Columns[i].ReadOnly)
                            {
                                dgv.CurrentCell = dgv.Rows[j].Cells[i];
                                if (dgv.EndEdit())
                                    dgv.BeginEdit(true);
                                return;
                            }
                            if (i + 1 == dgv.Columns.Count)
                            {
                                i = 0;
                                if (j + 1 == dgv.Rows.Count)
                                {
                                    if (!string.IsNullOrEmpty(this.CurrentRow.Cells[0].Value + ""))
                                    {
                                        int index = this.Rows.Add();
                                        for (int x = 0; x < dgv.Columns.Count; x++)
                                        {
                                            if (dgv.Columns[x].Visible && !dgv.Columns[x].ReadOnly)
                                            {
                                                dgv.CurrentCell = dgv.Rows[index].Cells[x];
                                                if (dgv.EndEdit())
                                                    dgv.BeginEdit(true);
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 获取主键信息
        /// </summary>
        private void CellLeaves()
        {
            if (DropList.Visible)
            {
                if (DropList.SelectedRows.Count > 0)
                {
                    for (int j = 0; j < DropListPrimaryKey.Count; j++)
                    {
                        if (this.CurrentCell.OwningColumn.Name == DropListPrimaryKey[j])
                        {
                            DropListSelectedValues = new strCollection();
                            DropListSelectedValues.ColumnsBind(DropListColumnsBind);
                            for (int i = 0; i < DropList.Columns.Count; i++)
                            {
                                int selectIndex = DropList.SelectedRows[0].Index;
                                DropListSelectedValues[i] = DropList.Rows[selectIndex].Cells[i].Value + "";
                            }
                            SetParentValues();
                            return;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 设置父控件获得下拉框匹配值
        /// </summary>
        private void SetParentValues()
        {
            if (DropList.SelectedRows != null && DropList.SelectedRows.Count > 0 && DropListSetParentValues != null)
            {
                for (int i = 0; i < DropListSetParentValues.GetLength(0); i++)
                {
                    this.Rows[this.CurrentRow.Index].Cells[DropListSetParentValues[i, 1]].Value = DropListSelectedValues[DropListSetParentValues[i, 0]];
                }
                TextBox tb = (TextBox)tbControl;
                tb.Text = DropListSelectedValues[this.CurrentCell.OwningColumn.Name];
            }
        }
        #endregion

        #region 动态动作 
        /// <summary>
        /// 文本值匹配数据源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellsTextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Focus() && (dropListDataSource != null || DropListEqualValues != null))
            {
                selectDataSource = DataClass.GetEquals(dropListDataSource, DropListEqualValues, tb.Text.Trim());
                DropList.DataSource = selectDataSource;
                DropList.Refresh();
            }
        }
        /// <summary>
        /// 获取焦点全选文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxGotFocus(object sender, EventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox tb = (TextBox)sender;
                tb.SelectAll();
            }
        }
        /// <summary>
        /// 鼠标双击动作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDateSource_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (sender is DataGridView)
            {
                DataGridView dgv = (DataGridView)sender;
                int rowIndex = new Int32();
                rowIndex = dgv.HitTest(e.X, e.Y).RowIndex;
                if (e.Button == MouseButtons.Right || rowIndex < 0) return;
                dgv.Rows[rowIndex].Selected = true;
                CellLeaves();
            }
        }
        /// <summary>
        /// 鼠标单击动作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDateSource_MouseClick(object sender, MouseEventArgs e)
        {
            if (sender is DataGridView)
            {
                DataGridView dgv = (DataGridView)sender;
                int rowIndex = new Int32();
                rowIndex = dgv.HitTest(e.X, e.Y).RowIndex;
                if (e.Button != MouseButtons.Right || rowIndex < 0) return;
                dgv.Rows[rowIndex].Selected = true;
            }
        }
        /// <summary>
        /// 键盘动作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDateSource_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is DataGridView && e.KeyCode == Keys.Enter)
            {
                DataGridView dgv = (DataGridView)sender;
                int rowIndex = new Int32();
                rowIndex = dgv.SelectedRows[0].Index;
                dgv.Rows[rowIndex].Selected = true;
                CellLeaves();
                this.Focus();
            }
        }
        /// <summary>
        /// 失去焦点动作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDateSource_Leave(object sender, EventArgs e)
        {
            if (sender is DataGridView)
            {
                DataGridView dgv = (DataGridView)sender;
                int rowIndex = new Int32();
                rowIndex = dgv.SelectedRows[0].Index;
                dgv.Rows[rowIndex].Selected = true;
            }
        }
        #endregion
    }
    #endregion
}
