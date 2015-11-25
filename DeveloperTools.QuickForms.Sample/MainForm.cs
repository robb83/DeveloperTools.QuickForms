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

        class AdvancedGridHandler : IGridFormHandler<TodoModel>
        {
            List<TodoModel> entities;
            List<CustomGridAction> actions;

            public AdvancedGridHandler(List<TodoModel> entities)
            {
                this.entities = entities;
                this.actions = new List<CustomGridAction>
                {
                    new CustomGridAction
                    {
                        Name = "Export",
                        SelectionType = CustomGridActionSelectionType.None,
                        Text = "Export"
                    },
                    new CustomGridAction
                    {
                        Name = "DeleteSelectedRows",
                        SelectionType = CustomGridActionSelectionType.Multiple,
                        Text = "Delete Selected Rows"
                    },
                    new CustomGridAction
                    {
                        Name = "CreateNew",
                        SelectionType = CustomGridActionSelectionType.None,
                        Text = "CreateNew"
                    }
                };
            }

            public void BeginEdit(TodoModel model)
            {
                
            }

            public void CancelEdit(TodoModel model)
            {
                
            }

            public void EndEdit(TodoModel model)
            {

            }

            public bool CanEdit(TodoModel model)
            {
                return true;
            }

            public List<CustomGridAction> GetCustomActions()
            {
                return this.actions;
            }

            public List<TodoModel> GetData()
            {
                return this.entities;
            }

            public void HandleCustomAction(CustomGridAction action, IGrid<TodoModel> grid)
            {
                if (action.Name == this.actions[1].Name)
                {
                    List<TodoModel> selectedItems = grid.GetSelectedRows();
                    foreach(TodoModel model in selectedItems)
                    {
                        this.entities.Remove(model);
                    }

                    grid.SetDataSource(this.entities);
                }
                else if (action.Name == this.actions[2].Name)
                {
                    var todoModel = new TodoModel();
                    todoModel.Description = "Hello World #" + this.entities.Count;
                    todoModel.EventDate = DateTime.Today.AddDays(7);

                    if (EditorForm<TodoModel>.ShowDialog(todoModel))
                    {
                        this.entities.Add(todoModel);
                        grid.SetDataSource(this.entities);
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
