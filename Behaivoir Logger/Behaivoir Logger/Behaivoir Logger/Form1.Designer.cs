namespace Behaivoir_Logger
{
    partial class Form1
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
            this.backBtn = new System.Windows.Forms.Button();
            this.snapshotBtn = new System.Windows.Forms.Button();
            this.submitBtn = new System.Windows.Forms.Button();
            this.pullReportBtn = new System.Windows.Forms.Button();
            this.fromTextBox = new System.Windows.Forms.TextBox();
            this.fromLabel = new System.Windows.Forms.Label();
            this.dateLabel1 = new System.Windows.Forms.Label();
            this.dateLabel2 = new System.Windows.Forms.Label();
            this.toLabel = new System.Windows.Forms.Label();
            this.toTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // backBtn
            // 
            this.backBtn.Location = new System.Drawing.Point(30, 13);
            this.backBtn.Name = "backBtn";
            this.backBtn.Size = new System.Drawing.Size(84, 40);
            this.backBtn.TabIndex = 0;
            this.backBtn.Text = "Switch To PM";
            this.backBtn.UseVisualStyleBackColor = true;
            this.backBtn.Click += new System.EventHandler(this.backBtn_Click);
            // 
            // snapshotBtn
            // 
            this.snapshotBtn.Location = new System.Drawing.Point(177, 13);
            this.snapshotBtn.Name = "snapshotBtn";
            this.snapshotBtn.Size = new System.Drawing.Size(104, 40);
            this.snapshotBtn.TabIndex = 1;
            this.snapshotBtn.Text = "Snapshot";
            this.snapshotBtn.UseVisualStyleBackColor = true;
            this.snapshotBtn.Click += new System.EventHandler(this.snapshotBtn_Click);
            // 
            // submitBtn
            // 
            this.submitBtn.Location = new System.Drawing.Point(320, 13);
            this.submitBtn.Name = "submitBtn";
            this.submitBtn.Size = new System.Drawing.Size(97, 40);
            this.submitBtn.TabIndex = 2;
            this.submitBtn.Text = "Submit";
            this.submitBtn.UseVisualStyleBackColor = true;
            this.submitBtn.Visible = false;
            this.submitBtn.Click += new System.EventHandler(this.submitBtn_Click);
            // 
            // pullReportBtn
            // 
            this.pullReportBtn.Location = new System.Drawing.Point(498, 13);
            this.pullReportBtn.Name = "pullReportBtn";
            this.pullReportBtn.Size = new System.Drawing.Size(103, 40);
            this.pullReportBtn.TabIndex = 3;
            this.pullReportBtn.Text = "Pull Report";
            this.pullReportBtn.UseVisualStyleBackColor = true;
            this.pullReportBtn.Visible = false;
            this.pullReportBtn.Click += new System.EventHandler(this.pullReportBtn_Click);
            // 
            // fromTextBox
            // 
            this.fromTextBox.Location = new System.Drawing.Point(607, 43);
            this.fromTextBox.Name = "fromTextBox";
            this.fromTextBox.Size = new System.Drawing.Size(66, 20);
            this.fromTextBox.TabIndex = 4;
            this.fromTextBox.Visible = false;
            // 
            // fromLabel
            // 
            this.fromLabel.AutoSize = true;
            this.fromLabel.Location = new System.Drawing.Point(619, 9);
            this.fromLabel.Name = "fromLabel";
            this.fromLabel.Size = new System.Drawing.Size(33, 13);
            this.fromLabel.TabIndex = 5;
            this.fromLabel.Text = "From:";
            this.fromLabel.Visible = false;
            // 
            // dateLabel1
            // 
            this.dateLabel1.AutoSize = true;
            this.dateLabel1.Location = new System.Drawing.Point(607, 27);
            this.dateLabel1.Name = "dateLabel1";
            this.dateLabel1.Size = new System.Drawing.Size(55, 13);
            this.dateLabel1.TabIndex = 6;
            this.dateLabel1.Text = "mm/dd/yy";
            this.dateLabel1.Visible = false;
            // 
            // dateLabel2
            // 
            this.dateLabel2.AutoSize = true;
            this.dateLabel2.Location = new System.Drawing.Point(683, 27);
            this.dateLabel2.Name = "dateLabel2";
            this.dateLabel2.Size = new System.Drawing.Size(55, 13);
            this.dateLabel2.TabIndex = 9;
            this.dateLabel2.Text = "mm/dd/yy";
            this.dateLabel2.Visible = false;
            // 
            // toLabel
            // 
            this.toLabel.AutoSize = true;
            this.toLabel.Location = new System.Drawing.Point(695, 9);
            this.toLabel.Name = "toLabel";
            this.toLabel.Size = new System.Drawing.Size(23, 13);
            this.toLabel.TabIndex = 8;
            this.toLabel.Text = "To:";
            this.toLabel.Visible = false;
            // 
            // toTextBox
            // 
            this.toTextBox.Location = new System.Drawing.Point(683, 43);
            this.toTextBox.Name = "toTextBox";
            this.toTextBox.Size = new System.Drawing.Size(66, 20);
            this.toTextBox.TabIndex = 7;
            this.toTextBox.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 489);
            this.Controls.Add(this.dateLabel2);
            this.Controls.Add(this.toLabel);
            this.Controls.Add(this.toTextBox);
            this.Controls.Add(this.dateLabel1);
            this.Controls.Add(this.fromLabel);
            this.Controls.Add(this.fromTextBox);
            this.Controls.Add(this.pullReportBtn);
            this.Controls.Add(this.submitBtn);
            this.Controls.Add(this.snapshotBtn);
            this.Controls.Add(this.backBtn);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button backBtn;
        private System.Windows.Forms.Button snapshotBtn;
        private System.Windows.Forms.Button submitBtn;
        private System.Windows.Forms.Button pullReportBtn;
        private System.Windows.Forms.TextBox fromTextBox;
        private System.Windows.Forms.Label fromLabel;
        private System.Windows.Forms.Label dateLabel1;
        private System.Windows.Forms.Label dateLabel2;
        private System.Windows.Forms.Label toLabel;
        private System.Windows.Forms.TextBox toTextBox;

    }
}

