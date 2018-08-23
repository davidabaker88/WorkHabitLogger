namespace Behaivoir_Logger
{
    partial class Menu
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
            this.amBtn = new System.Windows.Forms.Button();
            this.pmBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // amBtn
            // 
            this.amBtn.Location = new System.Drawing.Point(112, 52);
            this.amBtn.Name = "amBtn";
            this.amBtn.Size = new System.Drawing.Size(75, 23);
            this.amBtn.TabIndex = 0;
            this.amBtn.Text = "AM";
            this.amBtn.UseVisualStyleBackColor = true;
            this.amBtn.Click += new System.EventHandler(this.amBtn_Click);
            // 
            // pmBtn
            // 
            this.pmBtn.Location = new System.Drawing.Point(112, 129);
            this.pmBtn.Name = "pmBtn";
            this.pmBtn.Size = new System.Drawing.Size(75, 23);
            this.pmBtn.TabIndex = 1;
            this.pmBtn.Text = "PM";
            this.pmBtn.UseVisualStyleBackColor = true;
            this.pmBtn.Click += new System.EventHandler(this.pmBtn_Click);
            // 
            // Menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.pmBtn);
            this.Controls.Add(this.amBtn);
            this.Name = "Menu";
            this.Text = "Menu";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button amBtn;
        private System.Windows.Forms.Button pmBtn;
    }
}