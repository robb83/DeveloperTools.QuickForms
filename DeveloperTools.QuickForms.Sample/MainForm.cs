using DeveloperTools.QuickForms.Sample.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DeveloperTools.QuickForms.Sample
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonCreateSettings_Click(object sender, EventArgs e)
        {
            var configModel = new ConfigModel();
            configModel.GenerateConstants = true;

            if (EditorForm<ConfigModel>.ShowDialog(configModel))
            {
                //TODO: do some action here
            }
            else
            {
                // canceled, nothing to do
            }
        }

        private void buttonCreateNewProject_Click(object sender, EventArgs e)
        {
            var projectModel = new ProjectModel();

            if (EditorForm<ProjectModel>.ShowDialog(projectModel))
            {
                //TODO: do some action here
            }
            else
            {
                // canceled, nothing to do
            }
        }

        private void buttonCreateTodo_Click(object sender, EventArgs e)
        {
            var todoModel = new TodoModel();
            todoModel.Description = "Hello World";
            todoModel.EventDate = DateTime.Today.AddDays(7);

            if (EditorForm<TodoModel>.ShowDialog(todoModel))
            {
                //TODO: do some action here
            }
            else
            {
                // canceled, nothing to do
            }
        }
    }
}
