
namespace ShowAllWorkspaces
{
    partial class ShowAllWorkspacesForm
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
            this.buttonShowWorkspacesThroughRestApi = new System.Windows.Forms.Button();
            this.buttonShowWorkspacesThroughNetApi = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonShowWorkspacesThroughRestApi
            // 
            this.buttonShowWorkspacesThroughRestApi.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonShowWorkspacesThroughRestApi.Location = new System.Drawing.Point(115, 150);
            this.buttonShowWorkspacesThroughRestApi.Margin = new System.Windows.Forms.Padding(1);
            this.buttonShowWorkspacesThroughRestApi.Name = "buttonShowWorkspacesThroughRestApi";
            this.buttonShowWorkspacesThroughRestApi.Size = new System.Drawing.Size(150, 50);
            this.buttonShowWorkspacesThroughRestApi.TabIndex = 0;
            this.buttonShowWorkspacesThroughRestApi.Text = "Show Workspaces through REST API";
            this.buttonShowWorkspacesThroughRestApi.UseVisualStyleBackColor = true;
            this.buttonShowWorkspacesThroughRestApi.Click += new System.EventHandler(this.ShowWorkspacesThroughRestApi);
            // 
            // buttonShowWorkspacesThroughNetApi
            // 
            this.buttonShowWorkspacesThroughNetApi.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonShowWorkspacesThroughNetApi.Location = new System.Drawing.Point(285, 150);
            this.buttonShowWorkspacesThroughNetApi.Margin = new System.Windows.Forms.Padding(1);
            this.buttonShowWorkspacesThroughNetApi.Name = "buttonShowWorkspacesThroughNetApi";
            this.buttonShowWorkspacesThroughNetApi.Size = new System.Drawing.Size(150, 50);
            this.buttonShowWorkspacesThroughNetApi.TabIndex = 1;
            this.buttonShowWorkspacesThroughNetApi.Text = "Show Workspaces through .NET API";
            this.buttonShowWorkspacesThroughNetApi.UseVisualStyleBackColor = true;
            this.buttonShowWorkspacesThroughNetApi.Click += new System.EventHandler(this.ShowWorkspacesThroughNetApi);
            // 
            // ShowAllWorkspacesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 350);
            this.Controls.Add(this.buttonShowWorkspacesThroughNetApi);
            this.Controls.Add(this.buttonShowWorkspacesThroughRestApi);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ShowAllWorkspacesForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Show Stimulsoft Server Workspaces";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonShowWorkspacesThroughRestApi;
        private System.Windows.Forms.Button buttonShowWorkspacesThroughNetApi;
    }
}

