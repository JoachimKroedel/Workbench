namespace ConceptisPuzzles.Robot
{
    partial class FillAPixRobotView
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
            this.components = new System.ComponentModel.Container();
            this._btnLoadPuzzle = new System.Windows.Forms.Button();
            this._btnResetPuzzle = new System.Windows.Forms.Button();
            this._nudZoomFactor = new System.Windows.Forms.NumericUpDown();
            this._gbxRobot = new System.Windows.Forms.GroupBox();
            this._btnStatisticForm = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this._cbxTypeOfRobot = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this._btnRunInBackground = new System.Windows.Forms.Button();
            this._cbxBehaviourOnError = new System.Windows.Forms.ComboBox();
            this._cbxRunInterations = new System.Windows.Forms.CheckBox();
            this._nudRemainigIterationCount = new System.Windows.Forms.NumericUpDown();
            this._cbxIsInLearningMode = new System.Windows.Forms.CheckBox();
            this._btnMove = new System.Windows.Forms.Button();
            this._cbxDirectionTypes = new System.Windows.Forms.ComboBox();
            this._btnMarkAsNotMarked = new System.Windows.Forms.Button();
            this._btnMarkAsEmpty = new System.Windows.Forms.Button();
            this._cbxAutoDecision = new System.Windows.Forms.CheckBox();
            this._btnMarkAsFilled = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._chkRandomJump = new System.Windows.Forms.CheckBox();
            this._btnJump = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._nudPositionY = new System.Windows.Forms.NumericUpDown();
            this._nudPositionX = new System.Windows.Forms.NumericUpDown();
            this._btnMoveRight = new System.Windows.Forms.Button();
            this._btnMoveLeft = new System.Windows.Forms.Button();
            this._btnMoveDown = new System.Windows.Forms.Button();
            this._btnMoveUp = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this._ddbFieldOfVisionTypes = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this._pbxLookResult = new System.Windows.Forms.PictureBox();
            this._cbxHighlightErrors = new System.Windows.Forms.CheckBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._cbxAutoRefreshPlayground = new System.Windows.Forms.CheckBox();
            this._panelPlayground = new System.Windows.Forms.Panel();
            this._pbxPlayGround = new System.Windows.Forms.PictureBox();
            this._timer = new System.Windows.Forms.Timer(this.components);
            this.fillAPixRobotBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this._nudZoomFactor)).BeginInit();
            this._gbxRobot.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nudRemainigIterationCount)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nudPositionY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._nudPositionX)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbxLookResult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this._panelPlayground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbxPlayGround)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fillAPixRobotBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // _btnLoadPuzzle
            // 
            this._btnLoadPuzzle.Location = new System.Drawing.Point(4, 3);
            this._btnLoadPuzzle.Name = "_btnLoadPuzzle";
            this._btnLoadPuzzle.Size = new System.Drawing.Size(86, 23);
            this._btnLoadPuzzle.TabIndex = 16;
            this._btnLoadPuzzle.Text = "Load Puzzle";
            this._btnLoadPuzzle.UseVisualStyleBackColor = true;
            this._btnLoadPuzzle.Click += new System.EventHandler(this.BtnLoadPuzzle_Click);
            // 
            // _btnResetPuzzle
            // 
            this._btnResetPuzzle.Location = new System.Drawing.Point(144, 3);
            this._btnResetPuzzle.Name = "_btnResetPuzzle";
            this._btnResetPuzzle.Size = new System.Drawing.Size(83, 23);
            this._btnResetPuzzle.TabIndex = 17;
            this._btnResetPuzzle.Text = "Reset Puzzle";
            this._btnResetPuzzle.UseVisualStyleBackColor = true;
            this._btnResetPuzzle.Click += new System.EventHandler(this.BtnResetPuzzle_Click);
            // 
            // _nudZoomFactor
            // 
            this._nudZoomFactor.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this._nudZoomFactor.Location = new System.Drawing.Point(93, 6);
            this._nudZoomFactor.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this._nudZoomFactor.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this._nudZoomFactor.Name = "_nudZoomFactor";
            this._nudZoomFactor.Size = new System.Drawing.Size(46, 20);
            this._nudZoomFactor.TabIndex = 18;
            this._nudZoomFactor.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this._nudZoomFactor.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this._nudZoomFactor.ValueChanged += new System.EventHandler(this.NudZoomFactor_ValueChanged);
            // 
            // _gbxRobot
            // 
            this._gbxRobot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbxRobot.Controls.Add(this.label1);
            this._gbxRobot.Controls.Add(this._cbxTypeOfRobot);
            this._gbxRobot.Controls.Add(this.groupBox4);
            this._gbxRobot.Controls.Add(this.groupBox1);
            this._gbxRobot.Controls.Add(this.groupBox2);
            this._gbxRobot.Enabled = false;
            this._gbxRobot.Location = new System.Drawing.Point(3, 3);
            this._gbxRobot.Name = "_gbxRobot";
            this._gbxRobot.Size = new System.Drawing.Size(343, 366);
            this._gbxRobot.TabIndex = 19;
            this._gbxRobot.TabStop = false;
            this._gbxRobot.Text = "Robot";
            // 
            // _btnStatisticForm
            // 
            this._btnStatisticForm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnStatisticForm.Location = new System.Drawing.Point(249, 515);
            this._btnStatisticForm.Name = "_btnStatisticForm";
            this._btnStatisticForm.Size = new System.Drawing.Size(92, 23);
            this._btnStatisticForm.TabIndex = 26;
            this._btnStatisticForm.Text = "Statistic Form";
            this._btnStatisticForm.UseVisualStyleBackColor = true;
            this._btnStatisticForm.Click += new System.EventHandler(this.BtnStatisticForm_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "Type of Robot:";
            // 
            // _cbxTypeOfRobot
            // 
            this._cbxTypeOfRobot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._cbxTypeOfRobot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbxTypeOfRobot.FormattingEnabled = true;
            this._cbxTypeOfRobot.Items.AddRange(new object[] {
            "Simple Cross Robot",
            "3x3 Robot",
            "Extended Robot"});
            this._cbxTypeOfRobot.Location = new System.Drawing.Point(90, 23);
            this._cbxTypeOfRobot.Name = "_cbxTypeOfRobot";
            this._cbxTypeOfRobot.Size = new System.Drawing.Size(236, 21);
            this._cbxTypeOfRobot.TabIndex = 26;
            this._cbxTypeOfRobot.SelectedIndexChanged += new System.EventHandler(this.CbxTypeOfRobot_SelectedIndexChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this._cbxBehaviourOnError);
            this.groupBox4.Controls.Add(this._cbxRunInterations);
            this.groupBox4.Controls.Add(this._nudRemainigIterationCount);
            this.groupBox4.Controls.Add(this._cbxIsInLearningMode);
            this.groupBox4.Controls.Add(this._btnMove);
            this.groupBox4.Controls.Add(this._cbxDirectionTypes);
            this.groupBox4.Controls.Add(this._btnMarkAsNotMarked);
            this.groupBox4.Controls.Add(this._btnMarkAsEmpty);
            this.groupBox4.Controls.Add(this._cbxAutoDecision);
            this.groupBox4.Controls.Add(this._btnMarkAsFilled);
            this.groupBox4.Location = new System.Drawing.Point(9, 232);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(317, 126);
            this.groupBox4.TabIndex = 16;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Action";
            // 
            // _btnRunInBackground
            // 
            this._btnRunInBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnRunInBackground.Location = new System.Drawing.Point(217, 375);
            this._btnRunInBackground.Name = "_btnRunInBackground";
            this._btnRunInBackground.Size = new System.Drawing.Size(124, 21);
            this._btnRunInBackground.TabIndex = 28;
            this._btnRunInBackground.Text = "Run in Background";
            this._btnRunInBackground.UseVisualStyleBackColor = true;
            this._btnRunInBackground.Click += new System.EventHandler(this.BtnRunInBackground_Click);
            // 
            // _cbxBehaviourOnError
            // 
            this._cbxBehaviourOnError.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbxBehaviourOnError.FormattingEnabled = true;
            this._cbxBehaviourOnError.Items.AddRange(new object[] {
            "Nothing",
            "Undo",
            "Reset"});
            this._cbxBehaviourOnError.Location = new System.Drawing.Point(163, 69);
            this._cbxBehaviourOnError.Name = "_cbxBehaviourOnError";
            this._cbxBehaviourOnError.Size = new System.Drawing.Size(148, 21);
            this._cbxBehaviourOnError.TabIndex = 18;
            // 
            // _cbxRunInterations
            // 
            this._cbxRunInterations.Appearance = System.Windows.Forms.Appearance.Button;
            this._cbxRunInterations.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._cbxRunInterations.Location = new System.Drawing.Point(81, 96);
            this._cbxRunInterations.Name = "_cbxRunInterations";
            this._cbxRunInterations.Size = new System.Drawing.Size(100, 20);
            this._cbxRunInterations.TabIndex = 17;
            this._cbxRunInterations.Text = "Play Simulation";
            this._cbxRunInterations.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._cbxRunInterations.UseVisualStyleBackColor = true;
            this._cbxRunInterations.CheckedChanged += new System.EventHandler(this.ChbRunInterations_CheckedChanged);
            // 
            // _nudRemainigIterationCount
            // 
            this._nudRemainigIterationCount.Location = new System.Drawing.Point(9, 96);
            this._nudRemainigIterationCount.Maximum = new decimal(new int[] {
            500000,
            0,
            0,
            0});
            this._nudRemainigIterationCount.Name = "_nudRemainigIterationCount";
            this._nudRemainigIterationCount.Size = new System.Drawing.Size(66, 20);
            this._nudRemainigIterationCount.TabIndex = 16;
            this._nudRemainigIterationCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this._nudRemainigIterationCount.ThousandsSeparator = true;
            this._nudRemainigIterationCount.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            // 
            // _cbxIsInLearningMode
            // 
            this._cbxIsInLearningMode.AutoSize = true;
            this._cbxIsInLearningMode.Checked = true;
            this._cbxIsInLearningMode.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbxIsInLearningMode.Location = new System.Drawing.Point(9, 69);
            this._cbxIsInLearningMode.Name = "_cbxIsInLearningMode";
            this._cbxIsInLearningMode.Size = new System.Drawing.Size(114, 17);
            this._cbxIsInLearningMode.TabIndex = 15;
            this._cbxIsInLearningMode.Text = "Is in learning mode";
            this._cbxIsInLearningMode.UseVisualStyleBackColor = true;
            // 
            // _btnMove
            // 
            this._btnMove.Location = new System.Drawing.Point(237, 40);
            this._btnMove.Name = "_btnMove";
            this._btnMove.Size = new System.Drawing.Size(75, 23);
            this._btnMove.TabIndex = 14;
            this._btnMove.Text = "Move";
            this._btnMove.UseVisualStyleBackColor = true;
            this._btnMove.Click += new System.EventHandler(this.BtnMove_Click);
            // 
            // _cbxDirectionTypes
            // 
            this._cbxDirectionTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._cbxDirectionTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbxDirectionTypes.FormattingEnabled = true;
            this._cbxDirectionTypes.Location = new System.Drawing.Point(7, 17);
            this._cbxDirectionTypes.Name = "_cbxDirectionTypes";
            this._cbxDirectionTypes.Size = new System.Drawing.Size(193, 21);
            this._cbxDirectionTypes.TabIndex = 13;
            // 
            // _btnMarkAsNotMarked
            // 
            this._btnMarkAsNotMarked.Location = new System.Drawing.Point(160, 40);
            this._btnMarkAsNotMarked.Name = "_btnMarkAsNotMarked";
            this._btnMarkAsNotMarked.Size = new System.Drawing.Size(75, 23);
            this._btnMarkAsNotMarked.TabIndex = 2;
            this._btnMarkAsNotMarked.Text = "Not marked";
            this._btnMarkAsNotMarked.UseVisualStyleBackColor = true;
            this._btnMarkAsNotMarked.Click += new System.EventHandler(this.BtnMarkAsUndefined_Click);
            // 
            // _btnMarkAsEmpty
            // 
            this._btnMarkAsEmpty.Location = new System.Drawing.Point(83, 40);
            this._btnMarkAsEmpty.Name = "_btnMarkAsEmpty";
            this._btnMarkAsEmpty.Size = new System.Drawing.Size(75, 23);
            this._btnMarkAsEmpty.TabIndex = 1;
            this._btnMarkAsEmpty.Text = "Empty";
            this._btnMarkAsEmpty.UseVisualStyleBackColor = true;
            this._btnMarkAsEmpty.Click += new System.EventHandler(this.BtnMarkAsEmpty_Click);
            // 
            // _cbxAutoDecision
            // 
            this._cbxAutoDecision.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._cbxAutoDecision.AutoSize = true;
            this._cbxAutoDecision.Location = new System.Drawing.Point(222, 19);
            this._cbxAutoDecision.Name = "_cbxAutoDecision";
            this._cbxAutoDecision.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this._cbxAutoDecision.Size = new System.Drawing.Size(92, 17);
            this._cbxAutoDecision.TabIndex = 3;
            this._cbxAutoDecision.Text = "Auto Decision";
            this._cbxAutoDecision.UseVisualStyleBackColor = true;
            // 
            // _btnMarkAsFilled
            // 
            this._btnMarkAsFilled.Location = new System.Drawing.Point(6, 40);
            this._btnMarkAsFilled.Name = "_btnMarkAsFilled";
            this._btnMarkAsFilled.Size = new System.Drawing.Size(75, 23);
            this._btnMarkAsFilled.TabIndex = 0;
            this._btnMarkAsFilled.Text = "Filled";
            this._btnMarkAsFilled.UseVisualStyleBackColor = true;
            this._btnMarkAsFilled.Click += new System.EventHandler(this.BtnMarkAsFilled_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._chkRandomJump);
            this.groupBox1.Controls.Add(this._btnJump);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this._nudPositionY);
            this.groupBox1.Controls.Add(this._nudPositionX);
            this.groupBox1.Controls.Add(this._btnMoveRight);
            this.groupBox1.Controls.Add(this._btnMoveLeft);
            this.groupBox1.Controls.Add(this._btnMoveDown);
            this.groupBox1.Controls.Add(this._btnMoveUp);
            this.groupBox1.Location = new System.Drawing.Point(9, 59);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(151, 167);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Movement";
            // 
            // _chkRandomJump
            // 
            this._chkRandomJump.AutoSize = true;
            this._chkRandomJump.Checked = true;
            this._chkRandomJump.CheckState = System.Windows.Forms.CheckState.Checked;
            this._chkRandomJump.Location = new System.Drawing.Point(75, 129);
            this._chkRandomJump.Name = "_chkRandomJump";
            this._chkRandomJump.Size = new System.Drawing.Size(80, 17);
            this._chkRandomJump.TabIndex = 9;
            this._chkRandomJump.Text = "randomized";
            this._chkRandomJump.UseVisualStyleBackColor = true;
            // 
            // _btnJump
            // 
            this._btnJump.Location = new System.Drawing.Point(3, 125);
            this._btnJump.Name = "_btnJump";
            this._btnJump.Size = new System.Drawing.Size(66, 21);
            this._btnJump.TabIndex = 8;
            this._btnJump.Text = "Jump";
            this._btnJump.UseVisualStyleBackColor = true;
            this._btnJump.Click += new System.EventHandler(this.BtnJump_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(78, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Y:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "X:";
            // 
            // _nudPositionY
            // 
            this._nudPositionY.Location = new System.Drawing.Point(101, 99);
            this._nudPositionY.Name = "_nudPositionY";
            this._nudPositionY.Size = new System.Drawing.Size(40, 20);
            this._nudPositionY.TabIndex = 5;
            this._nudPositionY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // _nudPositionX
            // 
            this._nudPositionX.Location = new System.Drawing.Point(29, 99);
            this._nudPositionX.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            -2147483648});
            this._nudPositionX.Name = "_nudPositionX";
            this._nudPositionX.Size = new System.Drawing.Size(40, 20);
            this._nudPositionX.TabIndex = 4;
            this._nudPositionX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // _btnMoveRight
            // 
            this._btnMoveRight.Location = new System.Drawing.Point(101, 38);
            this._btnMoveRight.Name = "_btnMoveRight";
            this._btnMoveRight.Size = new System.Drawing.Size(50, 25);
            this._btnMoveRight.TabIndex = 3;
            this._btnMoveRight.Text = "Right";
            this._btnMoveRight.UseVisualStyleBackColor = true;
            this._btnMoveRight.Click += new System.EventHandler(this.BtnMoveRight_Click);
            // 
            // _btnMoveLeft
            // 
            this._btnMoveLeft.Location = new System.Drawing.Point(3, 38);
            this._btnMoveLeft.Name = "_btnMoveLeft";
            this._btnMoveLeft.Size = new System.Drawing.Size(50, 25);
            this._btnMoveLeft.TabIndex = 2;
            this._btnMoveLeft.Text = "Left";
            this._btnMoveLeft.UseVisualStyleBackColor = true;
            this._btnMoveLeft.Click += new System.EventHandler(this._btnMoveLeft_Click);
            // 
            // _btnMoveDown
            // 
            this._btnMoveDown.Location = new System.Drawing.Point(52, 56);
            this._btnMoveDown.Name = "_btnMoveDown";
            this._btnMoveDown.Size = new System.Drawing.Size(50, 25);
            this._btnMoveDown.TabIndex = 1;
            this._btnMoveDown.Text = "Down";
            this._btnMoveDown.UseVisualStyleBackColor = true;
            this._btnMoveDown.Click += new System.EventHandler(this.BtnMoveDown_Click);
            // 
            // _btnMoveUp
            // 
            this._btnMoveUp.Location = new System.Drawing.Point(52, 25);
            this._btnMoveUp.Name = "_btnMoveUp";
            this._btnMoveUp.Size = new System.Drawing.Size(50, 25);
            this._btnMoveUp.TabIndex = 0;
            this._btnMoveUp.Text = "Up";
            this._btnMoveUp.UseVisualStyleBackColor = true;
            this._btnMoveUp.Click += new System.EventHandler(this.BtnMoveUp_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this._ddbFieldOfVisionTypes);
            this.groupBox2.Controls.Add(this.panel2);
            this.groupBox2.Location = new System.Drawing.Point(166, 59);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(160, 167);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Perception";
            // 
            // _ddbFieldOfVisionTypes
            // 
            this._ddbFieldOfVisionTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._ddbFieldOfVisionTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._ddbFieldOfVisionTypes.FormattingEnabled = true;
            this._ddbFieldOfVisionTypes.Location = new System.Drawing.Point(69, 11);
            this._ddbFieldOfVisionTypes.Name = "_ddbFieldOfVisionTypes";
            this._ddbFieldOfVisionTypes.Size = new System.Drawing.Size(85, 21);
            this._ddbFieldOfVisionTypes.TabIndex = 12;
            this._ddbFieldOfVisionTypes.SelectedIndexChanged += new System.EventHandler(this.DdbFieldOfVisionTypes_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this._pbxLookResult);
            this.panel2.Location = new System.Drawing.Point(6, 38);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(148, 120);
            this.panel2.TabIndex = 11;
            // 
            // _pbxLookResult
            // 
            this._pbxLookResult.BackColor = System.Drawing.Color.Bisque;
            this._pbxLookResult.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this._pbxLookResult.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._pbxLookResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pbxLookResult.Location = new System.Drawing.Point(0, 0);
            this._pbxLookResult.Name = "_pbxLookResult";
            this._pbxLookResult.Size = new System.Drawing.Size(148, 120);
            this._pbxLookResult.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._pbxLookResult.TabIndex = 5;
            this._pbxLookResult.TabStop = false;
            // 
            // _cbxHighlightErrors
            // 
            this._cbxHighlightErrors.AutoSize = true;
            this._cbxHighlightErrors.Checked = true;
            this._cbxHighlightErrors.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbxHighlightErrors.Location = new System.Drawing.Point(229, 7);
            this._cbxHighlightErrors.Name = "_cbxHighlightErrors";
            this._cbxHighlightErrors.Size = new System.Drawing.Size(97, 17);
            this._cbxHighlightErrors.TabIndex = 23;
            this._cbxHighlightErrors.Text = "Highlight Errors";
            this._cbxHighlightErrors.UseVisualStyleBackColor = true;
            this._cbxHighlightErrors.CheckedChanged += new System.EventHandler(this.CbxHighlightErrors_CheckedChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._btnRunInBackground);
            this.splitContainer1.Panel1.Controls.Add(this._btnStatisticForm);
            this.splitContainer1.Panel1.Controls.Add(this._gbxRobot);
            this.splitContainer1.Panel1MinSize = 340;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._cbxAutoRefreshPlayground);
            this.splitContainer1.Panel2.Controls.Add(this._panelPlayground);
            this.splitContainer1.Panel2.Controls.Add(this._cbxHighlightErrors);
            this.splitContainer1.Panel2.Controls.Add(this._btnLoadPuzzle);
            this.splitContainer1.Panel2.Controls.Add(this._btnResetPuzzle);
            this.splitContainer1.Panel2.Controls.Add(this._nudZoomFactor);
            this.splitContainer1.Size = new System.Drawing.Size(1237, 542);
            this.splitContainer1.SplitterDistance = 347;
            this.splitContainer1.TabIndex = 25;
            // 
            // _cbxAutoRefreshPlayground
            // 
            this._cbxAutoRefreshPlayground.AutoSize = true;
            this._cbxAutoRefreshPlayground.Checked = true;
            this._cbxAutoRefreshPlayground.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbxAutoRefreshPlayground.Location = new System.Drawing.Point(325, 7);
            this._cbxAutoRefreshPlayground.Name = "_cbxAutoRefreshPlayground";
            this._cbxAutoRefreshPlayground.Size = new System.Drawing.Size(144, 17);
            this._cbxAutoRefreshPlayground.TabIndex = 24;
            this._cbxAutoRefreshPlayground.Text = "Auto Refresh Playground";
            this._cbxAutoRefreshPlayground.UseVisualStyleBackColor = true;
            this._cbxAutoRefreshPlayground.CheckedChanged += new System.EventHandler(this.CbxAutoRefreshPlayground_CheckedChanged);
            // 
            // _panelPlayground
            // 
            this._panelPlayground.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._panelPlayground.AutoScroll = true;
            this._panelPlayground.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._panelPlayground.Controls.Add(this._pbxPlayGround);
            this._panelPlayground.Location = new System.Drawing.Point(4, 32);
            this._panelPlayground.Name = "_panelPlayground";
            this._panelPlayground.Size = new System.Drawing.Size(879, 507);
            this._panelPlayground.TabIndex = 6;
            // 
            // _pbxPlayGround
            // 
            this._pbxPlayGround.BackColor = System.Drawing.Color.Bisque;
            this._pbxPlayGround.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this._pbxPlayGround.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._pbxPlayGround.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pbxPlayGround.Location = new System.Drawing.Point(0, 0);
            this._pbxPlayGround.Name = "_pbxPlayGround";
            this._pbxPlayGround.Size = new System.Drawing.Size(877, 505);
            this._pbxPlayGround.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this._pbxPlayGround.TabIndex = 4;
            this._pbxPlayGround.TabStop = false;
            // 
            // _timer
            // 
            this._timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // fillAPixRobotBindingSource
            // 
            this.fillAPixRobotBindingSource.DataSource = typeof(FillAPixRobot.RobotBrain);
            // 
            // FillAPixRobotView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1237, 542);
            this.Controls.Add(this.splitContainer1);
            this.MinimumSize = new System.Drawing.Size(742, 369);
            this.Name = "FillAPixRobotView";
            this.Text = "Robot View";
            this.Load += new System.EventHandler(this.FillAPixRobotView_Load);
            ((System.ComponentModel.ISupportInitialize)(this._nudZoomFactor)).EndInit();
            this._gbxRobot.ResumeLayout(false);
            this._gbxRobot.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nudRemainigIterationCount)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nudPositionY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._nudPositionX)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._pbxLookResult)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this._panelPlayground.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._pbxPlayGround)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fillAPixRobotBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _btnLoadPuzzle;
        private System.Windows.Forms.Button _btnResetPuzzle;
        private System.Windows.Forms.NumericUpDown _nudZoomFactor;
        private System.Windows.Forms.GroupBox _gbxRobot;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown _nudPositionY;
        private System.Windows.Forms.NumericUpDown _nudPositionX;
        private System.Windows.Forms.Button _btnMoveRight;
        private System.Windows.Forms.Button _btnMoveLeft;
        private System.Windows.Forms.Button _btnMoveDown;
        private System.Windows.Forms.Button _btnMoveUp;
        private System.Windows.Forms.CheckBox _cbxAutoDecision;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox _ddbFieldOfVisionTypes;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox _pbxLookResult;
        private System.Windows.Forms.CheckBox _cbxHighlightErrors;
        private System.Windows.Forms.BindingSource fillAPixRobotBindingSource;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel _panelPlayground;
        private System.Windows.Forms.PictureBox _pbxPlayGround;
        private System.Windows.Forms.Timer _timer;
        private System.Windows.Forms.CheckBox _chkRandomJump;
        private System.Windows.Forms.Button _btnJump;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button _btnMarkAsNotMarked;
        private System.Windows.Forms.Button _btnMarkAsEmpty;
        private System.Windows.Forms.Button _btnMarkAsFilled;
        private System.Windows.Forms.ComboBox _cbxDirectionTypes;
        private System.Windows.Forms.Button _btnMove;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox _cbxTypeOfRobot;
        private System.Windows.Forms.CheckBox _cbxIsInLearningMode;
        private System.Windows.Forms.CheckBox _cbxRunInterations;
        private System.Windows.Forms.NumericUpDown _nudRemainigIterationCount;
        private System.Windows.Forms.CheckBox _cbxAutoRefreshPlayground;
        private System.Windows.Forms.Button _btnStatisticForm;
        private System.Windows.Forms.ComboBox _cbxBehaviourOnError;
        private System.Windows.Forms.Button _btnRunInBackground;
    }
}