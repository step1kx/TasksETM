using System.Data;
using System.Windows.Controls;


namespace IssuingTasksETM.Interfaces
{
    public interface IDatabaseConnection
    {
        bool Connected();
        bool Disconnected();
        bool IsConnected();
        DataTable ExecuteQuery(string query);
        public bool FillProjects(ComboBox comboBox);

        public bool CheckerDBConn();
        public bool FillDepartmentName(ComboBox comboBox);
    }
}
