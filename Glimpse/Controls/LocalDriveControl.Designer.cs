namespace Glimpse.Controls
{
    partial class LocalDriveControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.labelDriveName = new System.Windows.Forms.Label();
            this.progressBarFreeSpace = new System.Windows.Forms.ProgressBar();
            this.labelFreeSpace = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.Location = new System.Drawing.Point(3, 3);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(64, 64);
            this.pictureBoxIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxIcon.TabIndex = 0;
            this.pictureBoxIcon.TabStop = false;
            // 
            // labelDriveName
            // 
            this.labelDriveName.AutoSize = true;
            this.labelDriveName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDriveName.Location = new System.Drawing.Point(73, 3);
            this.labelDriveName.Name = "labelDriveName";
            this.labelDriveName.Size = new System.Drawing.Size(113, 20);
            this.labelDriveName.TabIndex = 1;
            this.labelDriveName.Text = "Local drive (X:)";
            // 
            // progressBarFreeSpace
            // 
            this.progressBarFreeSpace.Location = new System.Drawing.Point(77, 26);
            this.progressBarFreeSpace.Name = "progressBarFreeSpace";
            this.progressBarFreeSpace.Size = new System.Drawing.Size(150, 15);
            this.progressBarFreeSpace.TabIndex = 2;
            this.progressBarFreeSpace.Value = 60;
            // 
            // labelFreeSpace
            // 
            this.labelFreeSpace.AutoSize = true;
            this.labelFreeSpace.Location = new System.Drawing.Point(74, 44);
            this.labelFreeSpace.Name = "labelFreeSpace";
            this.labelFreeSpace.Size = new System.Drawing.Size(72, 13);
            this.labelFreeSpace.TabIndex = 3;
            this.labelFreeSpace.Text = "X GB of Y GB";
            // 
            // LocalDriveControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelFreeSpace);
            this.Controls.Add(this.progressBarFreeSpace);
            this.Controls.Add(this.labelDriveName);
            this.Controls.Add(this.pictureBoxIcon);
            this.Name = "LocalDriveControl";
            this.Size = new System.Drawing.Size(234, 70);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.Label labelDriveName;
        private System.Windows.Forms.ProgressBar progressBarFreeSpace;
        private System.Windows.Forms.Label labelFreeSpace;
    }
}
