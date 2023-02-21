
namespace CopyReportTemplateToOtherWorkspaces
{
    partial class CopyReportTemplateForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonCopyReportTemplateThroughRestApi = new System.Windows.Forms.Button();
            this.buttonCopyReportTemplateThroughNetApi = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonCopyReportTemplateThroughRestApi
            // 
            this.buttonCopyReportTemplateThroughRestApi.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCopyReportTemplateThroughRestApi.Location = new System.Drawing.Point(115, 150);
            this.buttonCopyReportTemplateThroughRestApi.Margin = new System.Windows.Forms.Padding(1);
            this.buttonCopyReportTemplateThroughRestApi.Name = "buttonCopyReportTemplateThroughRestApi";
            this.buttonCopyReportTemplateThroughRestApi.Size = new System.Drawing.Size(150, 50);
            this.buttonCopyReportTemplateThroughRestApi.TabIndex = 0;
            this.buttonCopyReportTemplateThroughRestApi.Text = "Copy Template through REST API";
            this.buttonCopyReportTemplateThroughRestApi.UseVisualStyleBackColor = true;
            this.buttonCopyReportTemplateThroughRestApi.Click += new System.EventHandler(this.CopyReportTemplateThroughRestApi);
            // 
            // buttonCopyReportTemplateThroughNetApi
            // 
            this.buttonCopyReportTemplateThroughNetApi.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCopyReportTemplateThroughNetApi.Location = new System.Drawing.Point(285, 150);
            this.buttonCopyReportTemplateThroughNetApi.Margin = new System.Windows.Forms.Padding(1);
            this.buttonCopyReportTemplateThroughNetApi.Name = "buttonCopyReportTemplateThroughNetApi";
            this.buttonCopyReportTemplateThroughNetApi.Size = new System.Drawing.Size(150, 50);
            this.buttonCopyReportTemplateThroughNetApi.TabIndex = 1;
            this.buttonCopyReportTemplateThroughNetApi.Text = "Copy Template through .NET API";
            this.buttonCopyReportTemplateThroughNetApi.UseVisualStyleBackColor = true;
            this.buttonCopyReportTemplateThroughNetApi.Click += new System.EventHandler(this.CopyReportTemplateThroughNetApi);
            // 
            // CopyReportTemplateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 350);
            this.Controls.Add(this.buttonCopyReportTemplateThroughNetApi);
            this.Controls.Add(this.buttonCopyReportTemplateThroughRestApi);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CopyReportTemplateForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Copy Report Template to other Server Workspaces";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCopyReportTemplateThroughRestApi;
        private System.Windows.Forms.Button buttonCopyReportTemplateThroughNetApi;
    }
}

