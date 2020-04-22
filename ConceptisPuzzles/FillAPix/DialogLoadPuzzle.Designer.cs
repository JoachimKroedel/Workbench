namespace ConceptisPuzzles
{
    partial class DialogLoadPuzzle
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._txtDirectory = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._btnChangeDirectory = new System.Windows.Forms.Button();
            this._dataGridView = new System.Windows.Forms.DataGridView();
            this._colFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colSaveDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._btnLoad = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this._cbxFilterState = new System.Windows.Forms.ComboBox();
            this._folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this._dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // _txtDirectory
            // 
            this._txtDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._txtDirectory.Location = new System.Drawing.Point(70, 12);
            this._txtDirectory.Name = "_txtDirectory";
            this._txtDirectory.ReadOnly = true;
            this._txtDirectory.Size = new System.Drawing.Size(588, 20);
            this._txtDirectory.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Directory:";
            // 
            // _btnChangeDirectory
            // 
            this._btnChangeDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnChangeDirectory.Location = new System.Drawing.Point(664, 10);
            this._btnChangeDirectory.Name = "_btnChangeDirectory";
            this._btnChangeDirectory.Size = new System.Drawing.Size(31, 23);
            this._btnChangeDirectory.TabIndex = 2;
            this._btnChangeDirectory.Text = "...";
            this._btnChangeDirectory.UseVisualStyleBackColor = true;
            this._btnChangeDirectory.Click += new System.EventHandler(this._btnChangeDirectory_Click);
            // 
            // _dataGridView
            // 
            this._dataGridView.AllowUserToAddRows = false;
            this._dataGridView.AllowUserToDeleteRows = false;
            this._dataGridView.AllowUserToResizeRows = false;
            this._dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this._dataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this._dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._colFileName,
            this._colDate,
            this._colIndex,
            this._colSize,
            this._colLevel,
            this._colState,
            this._colSaveDate});
            this._dataGridView.Location = new System.Drawing.Point(12, 66);
            this._dataGridView.MultiSelect = false;
            this._dataGridView.Name = "_dataGridView";
            this._dataGridView.RowHeadersVisible = false;
            this._dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dataGridView.ShowCellErrors = false;
            this._dataGridView.ShowCellToolTips = false;
            this._dataGridView.ShowEditingIcon = false;
            this._dataGridView.ShowRowErrors = false;
            this._dataGridView.Size = new System.Drawing.Size(683, 369);
            this._dataGridView.TabIndex = 3;
            this._dataGridView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._dataGridView_MouseDoubleClick);
            // 
            // _colFileName
            // 
            this._colFileName.HeaderText = "Filename";
            this._colFileName.Name = "_colFileName";
            this._colFileName.ReadOnly = true;
            this._colFileName.Visible = false;
            this._colFileName.Width = 55;
            // 
            // _colDate
            // 
            this._colDate.HeaderText = "Date";
            this._colDate.MinimumWidth = 80;
            this._colDate.Name = "_colDate";
            this._colDate.ReadOnly = true;
            this._colDate.Width = 80;
            // 
            // _colIndex
            // 
            this._colIndex.HeaderText = "Index";
            this._colIndex.MinimumWidth = 80;
            this._colIndex.Name = "_colIndex";
            this._colIndex.ReadOnly = true;
            this._colIndex.Width = 80;
            // 
            // _colSize
            // 
            this._colSize.HeaderText = "Size";
            this._colSize.MinimumWidth = 80;
            this._colSize.Name = "_colSize";
            this._colSize.ReadOnly = true;
            this._colSize.Width = 80;
            // 
            // _colLevel
            // 
            this._colLevel.HeaderText = "Level";
            this._colLevel.MinimumWidth = 80;
            this._colLevel.Name = "_colLevel";
            this._colLevel.ReadOnly = true;
            this._colLevel.Width = 80;
            // 
            // _colState
            // 
            this._colState.HeaderText = "State";
            this._colState.MinimumWidth = 80;
            this._colState.Name = "_colState";
            this._colState.ReadOnly = true;
            this._colState.Width = 80;
            // 
            // _colSaveDate
            // 
            this._colSaveDate.HeaderText = "Saved";
            this._colSaveDate.MinimumWidth = 90;
            this._colSaveDate.Name = "_colSaveDate";
            this._colSaveDate.Width = 90;
            // 
            // _btnLoad
            // 
            this._btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnLoad.Location = new System.Drawing.Point(620, 441);
            this._btnLoad.Name = "_btnLoad";
            this._btnLoad.Size = new System.Drawing.Size(75, 23);
            this._btnLoad.TabIndex = 4;
            this._btnLoad.Text = "Load";
            this._btnLoad.UseVisualStyleBackColor = true;
            this._btnLoad.Click += new System.EventHandler(this._btnLoad_Click);
            // 
            // _btnCancel
            // 
            this._btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnCancel.Location = new System.Drawing.Point(539, 441);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(75, 23);
            this._btnCancel.TabIndex = 5;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.UseVisualStyleBackColor = true;
            this._btnCancel.Click += new System.EventHandler(this._btnCancel_Click);
            // 
            // _cbxFilterState
            // 
            this._cbxFilterState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbxFilterState.FormattingEnabled = true;
            this._cbxFilterState.Location = new System.Drawing.Point(333, 42);
            this._cbxFilterState.Name = "_cbxFilterState";
            this._cbxFilterState.Size = new System.Drawing.Size(82, 21);
            this._cbxFilterState.TabIndex = 6;
            this._cbxFilterState.SelectedIndexChanged += new System.EventHandler(this._cbxFilterState_SelectedIndexChanged);
            // 
            // DialogLoadPuzzle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(707, 476);
            this.Controls.Add(this._cbxFilterState);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnLoad);
            this.Controls.Add(this._dataGridView);
            this.Controls.Add(this._btnChangeDirectory);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._txtDirectory);
            this.Name = "DialogLoadPuzzle";
            this.Text = "DialogLoadPuzzle";
            this.Load += new System.EventHandler(this.DialogLoadPuzzle_Load);
            ((System.ComponentModel.ISupportInitialize)(this._dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _txtDirectory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _btnChangeDirectory;
        private System.Windows.Forms.DataGridView _dataGridView;
        private System.Windows.Forms.Button _btnLoad;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.ComboBox _cbxFilterState;
        private System.Windows.Forms.DataGridViewTextBoxColumn _colFileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn _colDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn _colIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn _colSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn _colLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn _colState;
        private System.Windows.Forms.DataGridViewTextBoxColumn _colSaveDate;
        private System.Windows.Forms.FolderBrowserDialog _folderBrowserDialog;
    }
}