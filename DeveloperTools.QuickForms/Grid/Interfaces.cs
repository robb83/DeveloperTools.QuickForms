using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DeveloperTools.QuickForms.Grid
{

    public enum CustomGridActionSelectionType { None, Single, Multiple }

    public interface IGrid<T>
    {
        List<T> GetSelectedRows();
        void SetDataSource(List<T> datasource);
        void AddCustomAction(CustomGridAction action);
        void AddColumn(DataGridViewColumn column);
    }

    public interface IGridFormHandler<T>
    {
        //TODO: column visible
        //TODO: additional columns, computed columns

        void Setup(IGrid<T> grid);
        bool CanEdit(T model);
        void BeginEdit(T model);
        void EndEdit(T model);
        void CancelEdit(T model);
        void HandleCustomAction(CustomGridAction action, IGrid<T> grid);
        List<T> GetData();
    }

    public class CustomGridAction
    {
        public String Name { get; set; }
        public String Text { get; set; }
        public CustomGridActionSelectionType SelectionType { get; set; }
    }
}
