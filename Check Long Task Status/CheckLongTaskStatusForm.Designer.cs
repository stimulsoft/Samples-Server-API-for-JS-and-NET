namespace CheckLongTaskStatus
{
    partial class CheckLongTaskStatusForm
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
            this.buttonCheckStatusThroughRestApi = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonCheckStatusThroughRestApi
            // 
            this.buttonCheckStatusThroughRestApi.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCheckStatusThroughRestApi.Location = new System.Drawing.Point(200, 150);
            this.buttonCheckStatusThroughRestApi.Margin = new System.Windows.Forms.Padding(1);
            this.buttonCheckStatusThroughRestApi.Name = "buttonCheckStatusThroughRestApi";
            this.buttonCheckStatusThroughRestApi.Size = new System.Drawing.Size(150, 50);
            this.buttonCheckStatusThroughRestApi.TabIndex = 0;
            this.buttonCheckStatusThroughRestApi.Text = "Check Status through REST API";
            this.buttonCheckStatusThroughRestApi.UseVisualStyleBackColor = true;
            this.buttonCheckStatusThroughRestApi.Click += new System.EventHandler(this.CheckStatusThroughRestApi);
            // 
            // CheckLongTaskStatusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 350);
            this.Controls.Add(this.buttonCheckStatusThroughRestApi);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CheckLongTaskStatusForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Check Status of Long Task on Stimulsoft Server";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCheckStatusThroughRestApi;
    }
}