using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace DeveloperTools.QuickForms
{
    public interface IGrid<T>
    {
        List<T> GetSelectedRows();
        void SetDataSource(List<T> datasource);        
    }

    public interface IGridFormHandler<T>
    {
        //TODO: column visible
        //TODO: additional columns, computed columns

        bool CanEdit(T model);
        void BeginEdit(T model);
        void EndEdit(T model);
        void CancelEdit(T model);
        void HandleCustomAction(CustomGridAction action, IGrid<T> grid);
        List<T> GetData();
        List<CustomGridAction> GetCustomActions();
    }

    public class DefaultGridFormHandler<T> : IGridFormHandler<T>
    {
        List<T> datasource;

        public DefaultGridFormHandler(List<T> datasource)
        {
            this.datasource = datasource;
        }

        public void BeginEdit(T model)
        {
            
        }

        public void CancelEdit(T model)
        {
            
        }

        public bool CanEdit(T model)
        {
            return true;
        }

        public void EndEdit(T model)
        {
            
        }

        public List<CustomGridAction> GetCustomActions()
        {
            return null;
        }

        public List<T> GetData()
        {
            return this.datasource;
        }

        public void HandleCustomAction(CustomGridAction action, IGrid<T> grid)
        {
            
        }
    }

    public enum CustomGridActionSelectionType { None, Single, Multiple }

    public class CustomGridAction
    {
        public String Name { get; set; }
        public String Text { get; set; }
        public CustomGridActionSelectionType SelectionType { get; set; }
    }

    public class GridForm<T> : Form, IGrid<T>
        where T : class
    {
        const int WIDTH = 480;
        const int HEIGHT = 80;
        const String BUTTON_OK = "OK";
        const String BUTTON_CANCEL = "Cancel";

        Type entityType;
        PropertyInfo[] properties;
        DataGridView dataGridView;
        IGridFormHandler<T> handler;
        List<CustomGridAction> customActions;
        Dictionary<String, Button> customActionMappings;
        BindingSource bindingSource;

        public GridForm(IGridFormHandler<T> handler)
        {
            this.handler = handler;
            this.entityType = typeof(T);
            this.properties = this.entityType.GetProperties();
            this.customActionMappings = new Dictionary<String, Button>();

            Initialization();

            this.bindingSource.DataSource = this.handler.GetData();
        }

        private void Initialization()
        {
            int row = 0;

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

            for (int i = 0; i < this.properties.Length; ++row, ++i)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.DataPropertyName = this.properties[i].Name;
                column.HeaderText = this.properties[i].Name;
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                dataGridView.Columns.Add(column);
            }
            this.Controls.Add(dataGridView);

            // generate action handlers
            FlowLayoutPanel panelForButtons = new FlowLayoutPanel();
            panelForButtons.Dock = DockStyle.Bottom;
            panelForButtons.FlowDirection = FlowDirection.LeftToRight;
            panelForButtons.AutoSize = true;
            this.Controls.Add(panelForButtons);

            // generate OK button
            panelForButtons.Controls.Add(GenerateButtonControl(BUTTON_OK, DialogResult.OK));

            this.customActions = this.handler.GetCustomActions();
            if (this.customActions != null)
            {
                foreach(CustomGridAction action in this.customActions)
                {
                    Button button = GenerateButtonControl(action.Text, DialogResult.None, action.Name);
                    this.customActionMappings.Add(action.Name, button);
                    panelForButtons.Controls.Add(button);

                    button.Click += Button_Click;
                }
                RefreshCustomActionButtons();
            }
        }

        private void DataGridView_SelectionChanged(object sender, EventArgs e)
        {
            RefreshCustomActionButtons();
        }

        private void DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = this.dataGridView.Rows[e.RowIndex];
            T model = (T)row.DataBoundItem;

            EditRowModel(model);
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button != null && this.customActions != null)
            {
                String tag = button.Tag as String;

                foreach(CustomGridAction action in this.customActions)
                {                    
                    if (action.Name == tag)
                    {
                        this.handler.HandleCustomAction(action, this);
                        this.dataGridView.Update();
                        break;
                    }
                }
            }
        }

        private void RefreshCustomActionButtons()
        {
            int selectedRows = this.dataGridView.SelectedRows.Count;

            if (this.customActions != null)
            {
                foreach (CustomGridAction action in this.customActions)
                {
                    Button button;
                    if (!this.customActionMappings.TryGetValue(action.Name, out button))
                    {
                        continue;
                    }

                    if (action.SelectionType == CustomGridActionSelectionType.Single)
                    {
                        button.Enabled = selectedRows == 1;
                    }
                    else if (action.SelectionType == CustomGridActionSelectionType.Multiple)
                    {
                        button.Enabled = selectedRows > 0;
                    }
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

        private static Button GenerateButtonControl(String text, DialogResult dialogResult, String tag = null)
        {
            Button button = new Button();
            button.Text = text;
            button.DialogResult = dialogResult;
            button.Tag = tag;

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

    }
}
