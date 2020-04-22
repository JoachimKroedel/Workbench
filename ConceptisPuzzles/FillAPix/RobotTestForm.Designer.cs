namespace ConceptisPuzzles.Robot
{
    partial class RobotTestForm
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
            this._btnLoadMemory = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _btnLoadMemory
            // 
            this._btnLoadMemory.Location = new System.Drawing.Point(12, 12);
            this._btnLoadMemory.Name = "_btnLoadMemory";
            this._btnLoadMemory.Size = new System.Drawing.Size(123, 23);
            this._btnLoadMemory.TabIndex = 0;
            this._btnLoadMemory.Text = "Load memory";
            this._btnLoadMemory.UseVisualStyleBackColor = true;
            this._btnLoadMemory.Click += new System.EventHandler(this._btnLoadMemory_Click);
            // 
            // RobotTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(855, 512);
            this.Controls.Add(this._btnLoadMemory);
            this.Name = "RobotTestForm";
            this.Text = "RobotTestForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _btnLoadMemory;
    }
}