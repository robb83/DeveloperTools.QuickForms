using DeveloperTools.QuickForms.Grid;
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

        private void buttonShowTodoList_Click(object sender, EventArgs e)
        {
            var todoModel1 = new TodoModel();
            todoModel1.Description = "Hello World #1";
            todoModel1.EventDate = DateTime.Today.AddDays(7);

            var todoModel2 = new TodoModel();
            todoModel2.Description = "Hello World #2";
            todoModel2.EventDate = DateTime.Today.AddDays(7);

            List<TodoModel> datasource = new List<TodoModel>();
            datasource.Add(todoModel1);
            datasource.Add(todoModel2);

            GridForm<TodoModel>.ShowDialog(datasource);
        }

        private void buttonShowAdvancedTodoList_Click(object sender, EventArgs e)
        {
            var todoModel1 = new TodoModel();
            todoModel1.Description = "Hello World #1";
            todoModel1.EventDate = DateTime.Today.AddDays(7);

            var todoModel2 = new TodoModel();
            todoModel2.Description = "Hello World #2";
            todoModel2.EventDate = DateTime.Today.AddDays(7);

            List<TodoModel> datasource = new List<TodoModel>();
            datasource.Add(todoModel1);
            datasource.Add(todoModel2);

            GridForm<TodoModel>.ShowDialog(new AdvancedGridHandler(datasource));
        }

        class AdvancedGridHandler : DefaultGridFormHandler<TodoModel>
        {
            const String ACTION_EXPORT = "Export";
            const String ACTION_DELETE = "Delete";
            const String ACTION_ADD = "Add";

            public AdvancedGridHandler(List<TodoModel> entities)
                :base(entities)
            {
                this.customActions.AddRange(new List<CustomGridAction>
                {
                    new CustomGridAction
                    {
                        Name = ACTION_EXPORT,
                        Text = "Export",
                        SelectionType = CustomGridActionSelectionType.None,
                    },
                    new CustomGridAction
                    {
                        Name = ACTION_DELETE,
                        Text = "Delete Selected Rows",
                        SelectionType = CustomGridActionSelectionType.Multiple
                    },
                    new CustomGridAction
                    {
                        Name = ACTION_ADD,
                        Text = "Create",
                        SelectionType = CustomGridActionSelectionType.None
                    }
                });
            }
                        
            public override void HandleCustomAction(CustomGridAction action, IGrid<TodoModel> grid)
            {
                if (action.Name == ACTION_DELETE)
                {
                    List<TodoModel> selectedItems = grid.GetSelectedRows();
                    foreach(TodoModel model in selectedItems)
                    {
                        this.datasource.Remove(model);
                    }

                    grid.SetDataSource(this.datasource);
                }
                else if (action.Name == ACTION_ADD)
                {
                    var todoModel = new TodoModel();
                    todoModel.Description = "Hello World #" + this.datasource.Count;
                    todoModel.EventDate = DateTime.Today.AddDays(7);

                    if (EditorForm<TodoModel>.ShowDialog(todoModel))
                    {
                        this.datasource.Add(todoModel);
                        grid.SetDataSource(this.datasource);
                    }
                }
                else
                {
                    MessageBox.Show(action.Name);
                }
            }
        }
    }
}
