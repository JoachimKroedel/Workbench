namespace ConceptisPuzzles.Robot
{
    partial class RobotBrainInfoForm
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
            this._btnShowActionMemory = new System.Windows.Forms.Button();
            this._txtInfoOutput = new System.Windows.Forms.RichTextBox();
            this._cbxShowDifferentUnits = new System.Windows.Forms.CheckBox();
            this._cbxShowNegativeFeedbackUnits = new System.Windows.Forms.CheckBox();
            this._cbxShowPositveFeedbackUnits = new System.Windows.Forms.CheckBox();
            this._cbxShowNegativeFeedbackPattern3x3 = new System.Windows.Forms.CheckBox();
            this._cbxShowNoDifferencePattern3x3 = new System.Windows.Forms.CheckBox();
            this._cbxShowNoDifferencePattern1x1 = new System.Windows.Forms.CheckBox();
            this._cbxShowNoDifferentUnits = new System.Windows.Forms.CheckBox();
            this._cbxClearBefore = new System.Windows.Forms.CheckBox();
            this._cbxShowRemovedNegativeUnitCount = new System.Windows.Forms.CheckBox();
            this._cbxShowNegativeFeedbackUnitsCount = new System.Windows.Forms.CheckBox();
            this._cbxShowPositiveFeedbackUnitsCount = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _btnShowActionMemory
            // 
            this._btnShowActionMemory.Location = new System.Drawing.Point(12, 12);
            this._btnShowActionMemory.Name = "_btnShowActionMemory";
            this._btnShowActionMemory.Size = new System.Drawing.Size(127, 23);
            this._btnShowActionMemory.TabIndex = 0;
            this._btnShowActionMemory.Text = "Show action memory";
            this._btnShowActionMemory.UseVisualStyleBackColor = true;
            this._btnShowActionMemory.Click += new System.EventHandler(this._btnShowActionMemory_Click);
            // 
            // _txtInfoOutput
            // 
            this._txtInfoOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._txtInfoOutput.Location = new System.Drawing.Point(4, 62);
            this._txtInfoOutput.Name = "_txtInfoOutput";
            this._txtInfoOutput.Size = new System.Drawing.Size(1208, 394);
            this._txtInfoOutput.TabIndex = 1;
            this._txtInfoOutput.Text = "";
            // 
            // _cbxShowDifferentUnits
            // 
            this._cbxShowDifferentUnits.AutoSize = true;
            this._cbxShowDifferentUnits.Location = new System.Drawing.Point(145, 16);
            this._cbxShowDifferentUnits.Name = "_cbxShowDifferentUnits";
            this._cbxShowDifferentUnits.Size = new System.Drawing.Size(93, 17);
            this._cbxShowDifferentUnits.TabIndex = 2;
            this._cbxShowDifferentUnits.Text = "Different Units";
            this._cbxShowDifferentUnits.UseVisualStyleBackColor = true;
            // 
            // _cbxShowNegativeFeedbackUnits
            // 
            this._cbxShowNegativeFeedbackUnits.AutoSize = true;
            this._cbxShowNegativeFeedbackUnits.Checked = true;
            this._cbxShowNegativeFeedbackUnits.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbxShowNegativeFeedbackUnits.Location = new System.Drawing.Point(430, 16);
            this._cbxShowNegativeFeedbackUnits.Name = "_cbxShowNegativeFeedbackUnits";
            this._cbxShowNegativeFeedbackUnits.Size = new System.Drawing.Size(147, 17);
            this._cbxShowNegativeFeedbackUnits.TabIndex = 3;
            this._cbxShowNegativeFeedbackUnits.Text = "Negative Feedback Units";
            this._cbxShowNegativeFeedbackUnits.UseVisualStyleBackColor = true;
            // 
            // _cbxShowPositveFeedbackUnits
            // 
            this._cbxShowPositveFeedbackUnits.AutoSize = true;
            this._cbxShowPositveFeedbackUnits.Checked = true;
            this._cbxShowPositveFeedbackUnits.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbxShowPositveFeedbackUnits.Location = new System.Drawing.Point(430, 39);
            this._cbxShowPositveFeedbackUnits.Name = "_cbxShowPositveFeedbackUnits";
            this._cbxShowPositveFeedbackUnits.Size = new System.Drawing.Size(139, 17);
            this._cbxShowPositveFeedbackUnits.TabIndex = 4;
            this._cbxShowPositveFeedbackUnits.Text = "Positve Feedback Units";
            this._cbxShowPositveFeedbackUnits.UseVisualStyleBackColor = true;
            // 
            // _cbxShowNegativeFeedbackPattern3x3
            // 
            this._cbxShowNegativeFeedbackPattern3x3.AutoSize = true;
            this._cbxShowNegativeFeedbackPattern3x3.Location = new System.Drawing.Point(596, 16);
            this._cbxShowNegativeFeedbackPattern3x3.Name = "_cbxShowNegativeFeedbackPattern3x3";
            this._cbxShowNegativeFeedbackPattern3x3.Size = new System.Drawing.Size(157, 17);
            this._cbxShowNegativeFeedbackPattern3x3.TabIndex = 5;
            this._cbxShowNegativeFeedbackPattern3x3.Text = "Negative Feedback Pattern";
            this._cbxShowNegativeFeedbackPattern3x3.UseVisualStyleBackColor = true;
            // 
            // _cbxShowNoDifferencePattern3x3
            // 
            this._cbxShowNoDifferencePattern3x3.AutoSize = true;
            this._cbxShowNoDifferencePattern3x3.Location = new System.Drawing.Point(266, 39);
            this._cbxShowNoDifferencePattern3x3.Name = "_cbxShowNoDifferencePattern3x3";
            this._cbxShowNoDifferencePattern3x3.Size = new System.Drawing.Size(149, 17);
            this._cbxShowNoDifferencePattern3x3.TabIndex = 6;
            this._cbxShowNoDifferencePattern3x3.Text = "No Difference Pattern 3x3";
            this._cbxShowNoDifferencePattern3x3.UseVisualStyleBackColor = true;
            // 
            // _cbxShowNoDifferencePattern1x1
            // 
            this._cbxShowNoDifferencePattern1x1.AutoSize = true;
            this._cbxShowNoDifferencePattern1x1.Location = new System.Drawing.Point(266, 16);
            this._cbxShowNoDifferencePattern1x1.Name = "_cbxShowNoDifferencePattern1x1";
            this._cbxShowNoDifferencePattern1x1.Size = new System.Drawing.Size(149, 17);
            this._cbxShowNoDifferencePattern1x1.TabIndex = 7;
            this._cbxShowNoDifferencePattern1x1.Text = "No Difference Pattern 1x1";
            this._cbxShowNoDifferencePattern1x1.UseVisualStyleBackColor = true;
            // 
            // _cbxShowNoDifferentUnits
            // 
            this._cbxShowNoDifferentUnits.AutoSize = true;
            this._cbxShowNoDifferentUnits.Location = new System.Drawing.Point(145, 39);
            this._cbxShowNoDifferentUnits.Name = "_cbxShowNoDifferentUnits";
            this._cbxShowNoDifferentUnits.Size = new System.Drawing.Size(110, 17);
            this._cbxShowNoDifferentUnits.TabIndex = 8;
            this._cbxShowNoDifferentUnits.Text = "No Different Units";
            this._cbxShowNoDifferentUnits.UseVisualStyleBackColor = true;
            // 
            // _cbxClearBefore
            // 
            this._cbxClearBefore.AutoSize = true;
            this._cbxClearBefore.Checked = true;
            this._cbxClearBefore.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbxClearBefore.Location = new System.Drawing.Point(12, 41);
            this._cbxClearBefore.Name = "_cbxClearBefore";
            this._cbxClearBefore.Size = new System.Drawing.Size(83, 17);
            this._cbxClearBefore.TabIndex = 9;
            this._cbxClearBefore.Text = "Clear before";
            this._cbxClearBefore.UseVisualStyleBackColor = true;
            // 
            // _cbxShowRemovedNegativeUnitCount
            // 
            this._cbxShowRemovedNegativeUnitCount.AutoSize = true;
            this._cbxShowRemovedNegativeUnitCount.Location = new System.Drawing.Point(968, 16);
            this._cbxShowRemovedNegativeUnitCount.Name = "_cbxShowRemovedNegativeUnitCount";
            this._cbxShowRemovedNegativeUnitCount.Size = new System.Drawing.Size(171, 17);
            this._cbxShowRemovedNegativeUnitCount.TabIndex = 11;
            this._cbxShowRemovedNegativeUnitCount.Text = "Removed Negative Unit Count";
            this._cbxShowRemovedNegativeUnitCount.UseVisualStyleBackColor = true;
            // 
            // _cbxShowNegativeFeedbackUnitsCount
            // 
            this._cbxShowNegativeFeedbackUnitsCount.AutoSize = true;
            this._cbxShowNegativeFeedbackUnitsCount.Location = new System.Drawing.Point(773, 16);
            this._cbxShowNegativeFeedbackUnitsCount.Name = "_cbxShowNegativeFeedbackUnitsCount";
            this._cbxShowNegativeFeedbackUnitsCount.Size = new System.Drawing.Size(178, 17);
            this._cbxShowNegativeFeedbackUnitsCount.TabIndex = 10;
            this._cbxShowNegativeFeedbackUnitsCount.Text = "Negative Feedback Units Count";
            this._cbxShowNegativeFeedbackUnitsCount.UseVisualStyleBackColor = true;
            // 
            // _cbxShowPositiveFeedbackUnitsCount
            // 
            this._cbxShowPositiveFeedbackUnitsCount.AutoSize = true;
            this._cbxShowPositiveFeedbackUnitsCount.Location = new System.Drawing.Point(773, 39);
            this._cbxShowPositiveFeedbackUnitsCount.Name = "_cbxShowPositiveFeedbackUnitsCount";
            this._cbxShowPositiveFeedbackUnitsCount.Size = new System.Drawing.Size(172, 17);
            this._cbxShowPositiveFeedbackUnitsCount.TabIndex = 12;
            this._cbxShowPositiveFeedbackUnitsCount.Text = "Positive Feedback Units Count";
            this._cbxShowPositiveFeedbackUnitsCount.UseVisualStyleBackColor = true;
            // 
            // RobotBrainInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1214, 461);
            this.Controls.Add(this._cbxShowPositiveFeedbackUnitsCount);
            this.Controls.Add(this._cbxShowRemovedNegativeUnitCount);
            this.Controls.Add(this._cbxShowNegativeFeedbackUnitsCount);
            this.Controls.Add(this._cbxClearBefore);
            this.Controls.Add(this._cbxShowNoDifferentUnits);
            this.Controls.Add(this._cbxShowNoDifferencePattern1x1);
            this.Controls.Add(this._cbxShowNoDifferencePattern3x3);
            this.Controls.Add(this._cbxShowNegativeFeedbackPattern3x3);
            this.Controls.Add(this._cbxShowPositveFeedbackUnits);
            this.Controls.Add(this._cbxShowNegativeFeedbackUnits);
            this.Controls.Add(this._cbxShowDifferentUnits);
            this.Controls.Add(this._txtInfoOutput);
            this.Controls.Add(this._btnShowActionMemory);
            this.Name = "RobotBrainInfoForm";
            this.Text = "RobotTestForm";
            this.Load += new System.EventHandler(this.RobotTestForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _btnShowActionMemory;
        private System.Windows.Forms.RichTextBox _txtInfoOutput;
        private System.Windows.Forms.CheckBox _cbxShowDifferentUnits;
        private System.Windows.Forms.CheckBox _cbxShowNegativeFeedbackUnits;
        private System.Windows.Forms.CheckBox _cbxShowPositveFeedbackUnits;
        private System.Windows.Forms.CheckBox _cbxShowNegativeFeedbackPattern3x3;
        private System.Windows.Forms.CheckBox _cbxShowNoDifferencePattern3x3;
        private System.Windows.Forms.CheckBox _cbxShowNoDifferencePattern1x1;
        private System.Windows.Forms.CheckBox _cbxShowNoDifferentUnits;
        private System.Windows.Forms.CheckBox _cbxClearBefore;
        private System.Windows.Forms.CheckBox _cbxShowRemovedNegativeUnitCount;
        private System.Windows.Forms.CheckBox _cbxShowNegativeFeedbackUnitsCount;
        private System.Windows.Forms.CheckBox _cbxShowPositiveFeedbackUnitsCount;
    }
}