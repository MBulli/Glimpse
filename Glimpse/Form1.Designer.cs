namespace Glimpse
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
            this.viewContainer = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // viewContainer
            // 
            this.viewContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewContainer.Location = new System.Drawing.Point(10, 10);
            this.viewContainer.Margin = new System.Windows.Forms.Padding(0);
            this.viewContainer.Name = "viewContainer";
            this.viewContainer.Size = new System.Drawing.Size(514, 502);
            this.viewContainer.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 522);
            this.Controls.Add(this.viewContainer);
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "Glimpse";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel viewContainer;

    }
}

