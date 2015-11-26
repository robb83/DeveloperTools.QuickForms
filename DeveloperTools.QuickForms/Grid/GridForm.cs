using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace DeveloperTools.QuickForms.Grid
{
    public class GridForm<T> : Form, IGrid<T>
        where T:class
    {
        const int WIDTH = 480;
        const int HEIGHT = 80;
        const String BUTTON_OK = "OK";
        const String BUTTON_CANCEL = "Cancel";

        Type entityType;
        IGridFormHandler<T> handler;
        DataGridView dataGridView;
        List<CustomGridActionButton> customActions;
        BindingSource bindingSource;
        FlowLayoutPanel panelForButtons;

        public GridForm(IGridFormHandler<T> handler)
        {
            this.entityType = typeof(T);
            this.customActions = new List<CustomGridActionButton>();
            this.handler = handler;

            Initialization();

            this.handler.Setup(this);
        }

        private void Initialization()
        {
            // form initialization
            this.Text = entityType.FullName;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AutoSize = true;
            this.MinimumSize = new System.Drawing.Size(WIDTH, HEIGHT);
            this.Size = new System.Drawing.Size(WIDTH, HEIGHT);

            bindingSource = new BindingSource();
            bindingSource.DataSource = typeof(T);

            // editor controls initialization
            dataGridView = new DataGridView();
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.AutoSize = true;
            dataGridView.AutoGenerateColumns = false;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.ReadOnly = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.CellDoubleClick += DataGridView_CellDoubleClick;
            dataGridView.SelectionChanged += DataGridView_SelectionChanged;
            dataGridView.DataSource = bindingSource;

            this.Controls.Add(dataGridView);

            // generate action handlers
            panelForButtons = new FlowLayoutPanel();
            panelForButtons.Dock = DockStyle.Bottom;
            panelForButtons.FlowDirection = FlowDirection.LeftToRight;
            panelForButtons.AutoSize = true;
            this.Controls.Add(panelForButtons);

            // generate OK button
            panelForButtons.Controls.Add(GenerateButtonControl(BUTTON_OK, DialogResult.OK));
        }

        private void DataGridView_SelectionChanged(object sender, EventArgs e)
        {
            RefreshCustomActionButtons();
        }

        private void DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {            
            DataGridViewRow row = this.dataGridView.Rows[e.RowIndex];
            T model = row.DataBoundItem as T;
            if (model != null)
            {
                EditRowModel(model);
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button != null && button.Tag != null)
            {
                CustomGridAction action = button.Tag as CustomGridAction;
                if (action != null)
                {
                    this.handler.HandleCustomAction(action, this);
                    this.dataGridView.Update();
                }
            }
        }

        private void RefreshCustomActionButtons()
        {
            int selectedRows = this.dataGridView.SelectedRows.Count;

            foreach (CustomGridActionButton actionButton in this.customActions)
            {
                if (actionButton.Action.SelectionType == CustomGridActionSelectionType.Single)
                {
                    actionButton.Button.Enabled = selectedRows == 1;
                }
                else if (actionButton.Action.SelectionType == CustomGridActionSelectionType.Multiple)
                {
                    actionButton.Button.Enabled = selectedRows > 0;
                }
            }
        }

        private void EditRowModel(T model)
        {
            if (this.handler.CanEdit(model))
            {
                this.handler.BeginEdit(model);

                if (EditorForm<T>.ShowDialog(model))
                {
                    this.handler.EndEdit(model);
                    this.dataGridView.Update();
                }
                else
                {
                    this.handler.CancelEdit(model);
                }
            }
        }

        public List<T> GetSelectedRows()
        {
            List<T> result = new List<T>();
            DataGridViewSelectedRowCollection rows = this.dataGridView.SelectedRows;

            foreach(DataGridViewRow row in rows)
            {
                result.Add((T)row.DataBoundItem);
            }

            return result;
        }

        public void SetDataSource(List<T> datasource)
        {
            this.bindingSource.DataSource = null;
            this.bindingSource.DataSource = datasource;
        }

        public void AddCustomAction(CustomGridAction action)
        {
            Button button = GenerateButtonControl(action.Text, DialogResult.None, action);
            button.Click += Button_Click;

            this.customActions.Add(new CustomGridActionButton
            {
                Action = action,
                Button = button
            });

            panelForButtons.Controls.Add(button);

            RefreshCustomActionButtons();
        }

        public void AddColumn(DataGridViewColumn column)
        {
            dataGridView.Columns.Add(column);
        }

        private static Button GenerateButtonControl(String text, DialogResult dialogResult, CustomGridAction action = null)
        {
            Button button = new Button();
            button.Text = text;
            button.DialogResult = dialogResult;
            button.Tag = action;

            return button;
        }

        public static void ShowDialog(List<T> entities)
        {
            IGridFormHandler<T> gridHandler = new DefaultGridFormHandler<T>(entities);

            GridForm<T> gridForm = new GridForm<T>(gridHandler);
            gridForm.ShowDialog();
        }

        public static void ShowDialog(IGridFormHandler<T> handler)
        {
            GridForm<T> gridForm = new GridForm<T>(handler);
            gridForm.ShowDialog();
        }
        
        class CustomGridActionButton
        {
            public CustomGridAction Action;
            public Button Button;
        }
    }
}
