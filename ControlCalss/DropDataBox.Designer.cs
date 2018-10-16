namespace ControlClass
{
    partial class DropDataBox
    {/// <summary> 
     /// 必需的设计器变量。
     /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.txtbox = new System.Windows.Forms.TextBox();
            this.txtboxdate = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.txtboxdate)).BeginInit();
            this.SuspendLayout();
            // 
            // txtbox
            // 
            this.txtbox.Location = new System.Drawing.Point(0, 0);
            this.txtbox.Name = "txtbox";
            this.txtbox.Size = new System.Drawing.Size(120, 21);
            this.txtbox.TabIndex = 0;
            this.txtbox.TextChanged += new System.EventHandler(this.txtbox_TextChanged);
            this.txtbox.Enter += new System.EventHandler(this.txtbox_Enter);
            this.txtbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtbox_KeyDown);
            // 
            // txtboxdate
            // 
            this.txtboxdate.AllowUserToAddRows = false;
            this.txtboxdate.AllowUserToDeleteRows = false;
            this.txtboxdate.AllowUserToResizeRows = false;
            this.txtboxdate.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.txtboxdate.Location = new System.Drawing.Point(0, 23);
            this.txtboxdate.Name = "txtboxdate";
            this.txtboxdate.ReadOnly = true;
            this.txtboxdate.RowHeadersVisible = false;
            this.txtboxdate.RowTemplate.Height = 23;
            this.txtboxdate.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.txtboxdate.Size = new System.Drawing.Size(120, 150);
            this.txtboxdate.TabIndex = 1;
            this.txtboxdate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtboxdate_KeyDown);
            this.txtboxdate.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtboxdate_MouseClick);
            this.txtboxdate.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dgvDateSource_MouseDoubleClick);
            // 
            // txtDataBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.Controls.Add(this.txtboxdate);
            this.Controls.Add(this.txtbox);
            this.Name = "txtDataBox";
            this.Size = new System.Drawing.Size(122, 23);
            this.Leave += new System.EventHandler(this.txtDataBox_Leave);
            ((System.ComponentModel.ISupportInitialize)(this.txtboxdate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox txtbox;
        public System.Windows.Forms.DataGridView txtboxdate;
    }
}
