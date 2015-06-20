namespace DeveloperTools.QuickForms.Sample
{
    partial class MainForm
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
            this.buttonCreateSettings = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonCreateNewProject = new System.Windows.Forms.Button();
            this.buttonCreateTodo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonCreateSettings
            // 
            this.buttonCreateSettings.Location = new System.Drawing.Point(12, 12);
            this.buttonCreateSettings.Name = "buttonCreateSettings";
            this.buttonCreateSettings.Size = new System.Drawing.Size(260, 63);
            this.buttonCreateSettings.TabIndex = 0;
            this.buttonCreateSettings.Text = "Create Config Settings";
            this.buttonCreateSettings.UseVisualStyleBackColor = true;
            this.buttonCreateSettings.Click += new System.EventHandler(this.buttonCreateSettings_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(197, 227);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonCreateNewProject
            // 
            this.buttonCreateNewProject.Location = new System.Drawing.Point(12, 81);
            this.buttonCreateNewProject.Name = "buttonCreateNewProject";
            this.buttonCreateNewProject.Size = new System.Drawing.Size(260, 63);
            this.buttonCreateNewProject.TabIndex = 2;
            this.buttonCreateNewProject.Text = "Generate New Project";
            this.buttonCreateNewProject.UseVisualStyleBackColor = true;
            this.buttonCreateNewProject.Click += new System.EventHandler(this.buttonCreateNewProject_Click);
            // 
            // buttonCreateTodo
            // 
            this.buttonCreateTodo.Location = new System.Drawing.Point(12, 150);
            this.buttonCreateTodo.Name = "buttonCreateTodo";
            this.buttonCreateTodo.Size = new System.Drawing.Size(260, 63);
            this.buttonCreateTodo.TabIndex = 3;
            this.buttonCreateTodo.Text = "Create TODO";
            this.buttonCreateTodo.UseVisualStyleBackColor = true;
            this.buttonCreateTodo.Click += new System.EventHandler(this.buttonCreateTodo_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.buttonCreateTodo);
            this.Controls.Add(this.buttonCreateNewProject);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonCreateSettings);
            this.Name = "MainForm";
            this.Text = "QuickForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCreateSettings;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonCreateNewProject;
        private System.Windows.Forms.Button buttonCreateTodo;
    }
}

