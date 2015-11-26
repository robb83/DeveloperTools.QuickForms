using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace DeveloperTools.QuickForms.Grid
{
    public class DefaultGridFormHandler<T> : IGridFormHandler<T>
        where T : class
    {
        protected List<T> datasource;
        protected List<CustomGridAction> customActions;
        protected PropertyInfo[] properties;
        protected Type entityType;

        public DefaultGridFormHandler()
            : this(new List<T>())
        {
        }

        public DefaultGridFormHandler(List<T> datasource)
        {
            this.entityType = typeof(T);
            this.properties = this.entityType.GetProperties();
            this.datasource = datasource;
            this.customActions = new List<CustomGridAction>();
        }

        public virtual void BeginEdit(T model)
        {

        }

        public virtual void CancelEdit(T model)
        {

        }

        public virtual bool CanEdit(T model)
        {
            return true;
        }

        public virtual void EndEdit(T model)
        {

        }
        
        public virtual List<T> GetData()
        {
            return this.datasource;
        }

        public virtual void HandleCustomAction(CustomGridAction action, IGrid<T> grid)
        {

        }

        public virtual void Setup(IGrid<T> grid)
        {
            for (int p = 0; p < this.properties.Length; ++p)
            {
                String propertyName = this.properties[p].Name;
                String text = this.properties[p].Name;

                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.DataPropertyName = propertyName;
                column.HeaderText = text;
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                grid.AddColumn(column);
            }

            foreach(CustomGridAction action in this.customActions)
            {
                grid.AddCustomAction(action);
            }

            grid.SetDataSource(this.GetData());
        }

        public virtual void ShowDialog()
        {
            GridForm<T> gridForm = new GridForm<T>(this);
            gridForm.ShowDialog();
        }
    }
}
