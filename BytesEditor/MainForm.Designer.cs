namespace BytesEditor
{
    partial class BytesEditor
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.treeView = new System.Windows.Forms.TreeView();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.btnResetPath = new System.Windows.Forms.Button();
            this.cmbBytes = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbLanguage = new System.Windows.Forms.ComboBox();
            this.lblMsg = new System.Windows.Forms.Label();
            this.btnDataLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.RowGridMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ColumnGridMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lockStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.RowGridMenuStrip.SuspendLayout();
            this.ColumnGridMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.treeView);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.dataGridView);
            this.splitContainer3.Size = new System.Drawing.Size(1329, 946);
            this.splitContainer3.SplitterDistance = 314;
            this.splitContainer3.TabIndex = 6;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(314, 946);
            this.treeView.TabIndex = 2;
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // dataGridView
            // 
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowTemplate.Height = 23;
            this.dataGridView.Size = new System.Drawing.Size(1011, 946);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseDown);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.btnResetPath);
            this.splitContainer2.Panel1.Controls.Add(this.cmbBytes);
            this.splitContainer2.Panel1.Controls.Add(this.label2);
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            this.splitContainer2.Panel1.Controls.Add(this.cmbLanguage);
            this.splitContainer2.Panel1.Controls.Add(this.lblMsg);
            this.splitContainer2.Panel1.Controls.Add(this.btnDataLoad);
            this.splitContainer2.Panel1.Controls.Add(this.btnSave);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(1329, 1005);
            this.splitContainer2.SplitterDistance = 55;
            this.splitContainer2.TabIndex = 5;
            // 
            // btnResetPath
            // 
            this.btnResetPath.Location = new System.Drawing.Point(174, 12);
            this.btnResetPath.Name = "btnResetPath";
            this.btnResetPath.Size = new System.Drawing.Size(75, 23);
            this.btnResetPath.TabIndex = 11;
            this.btnResetPath.Text = "Reset Path";
            this.btnResetPath.UseVisualStyleBackColor = true;
            this.btnResetPath.Click += new System.EventHandler(this.btnResetPath_Click);
            // 
            // cmbBytes
            // 
            this.cmbBytes.FormattingEnabled = true;
            this.cmbBytes.Location = new System.Drawing.Point(508, 14);
            this.cmbBytes.Name = "cmbBytes";
            this.cmbBytes.Size = new System.Drawing.Size(121, 20);
            this.cmbBytes.TabIndex = 10;
            this.cmbBytes.SelectedIndexChanged += new System.EventHandler(this.cmbBytes_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(457, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "Bytes :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(255, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "Language :";
            // 
            // cmbLanguage
            // 
            this.cmbLanguage.FormattingEnabled = true;
            this.cmbLanguage.Location = new System.Drawing.Point(330, 14);
            this.cmbLanguage.Name = "cmbLanguage";
            this.cmbLanguage.Size = new System.Drawing.Size(121, 20);
            this.cmbLanguage.TabIndex = 7;
            this.cmbLanguage.SelectedIndexChanged += new System.EventHandler(this.cmbLanguage_SelectedIndexChanged);
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.Location = new System.Drawing.Point(640, 17);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(38, 12);
            this.lblMsg.TabIndex = 6;
            this.lblMsg.Text = "label1";
            // 
            // btnDataLoad
            // 
            this.btnDataLoad.Location = new System.Drawing.Point(12, 12);
            this.btnDataLoad.Name = "btnDataLoad";
            this.btnDataLoad.Size = new System.Drawing.Size(75, 23);
            this.btnDataLoad.TabIndex = 4;
            this.btnDataLoad.Text = "&Load";
            this.btnDataLoad.UseVisualStyleBackColor = true;
            this.btnDataLoad.Click += new System.EventHandler(this.btnDataLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(93, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // RowGridMenuStrip
            // 
            this.RowGridMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteStripMenuItem});
            this.RowGridMenuStrip.Name = "contextMenuStrip1";
            this.RowGridMenuStrip.Size = new System.Drawing.Size(109, 26);
            this.RowGridMenuStrip.Text = "Delete";
            // 
            // deleteStripMenuItem
            // 
            this.deleteStripMenuItem.Name = "deleteStripMenuItem";
            this.deleteStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.deleteStripMenuItem.Text = "&Delete";
            this.deleteStripMenuItem.Click += new System.EventHandler(this.deleteStripMenuItem_Click);
            // 
            // ColumnGridMenuStrip
            // 
            this.ColumnGridMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lockStripMenuItem});
            this.ColumnGridMenuStrip.Name = "contextMenuStrip1";
            this.ColumnGridMenuStrip.Size = new System.Drawing.Size(153, 48);
            this.ColumnGridMenuStrip.Text = "Delete";
            // 
            // lockStripMenuItem
            // 
            this.lockStripMenuItem.Name = "lockStripMenuItem";
            this.lockStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.lockStripMenuItem.Text = "&Locked";
            this.lockStripMenuItem.Click += new System.EventHandler(this.lockStripMenuItem_Click);
            // 
            // BytesEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1329, 1005);
            this.Controls.Add(this.splitContainer2);
            this.Name = "BytesEditor";
            this.Text = "BytesEditor";
            this.MaximumSizeChanged += new System.EventHandler(this.BytesEditor_MaximumSizeChanged);
            this.MinimumSizeChanged += new System.EventHandler(this.BytesEditor_MinimumSizeChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BytesEditor_FormClosing);
            this.Load += new System.EventHandler(this.BytesEditor_Load);
            this.ResizeEnd += new System.EventHandler(this.BytesEditor_ResizeEnd);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.RowGridMenuStrip.ResumeLayout(false);
            this.ColumnGridMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button btnDataLoad;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.ComboBox cmbLanguage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbBytes;
        private System.Windows.Forms.ContextMenuStrip RowGridMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem deleteStripMenuItem;
        private System.Windows.Forms.Button btnResetPath;
        private System.Windows.Forms.ContextMenuStrip ColumnGridMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem lockStripMenuItem;
    }
}

