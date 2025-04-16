using System.Data;


namespace IssuingTasksETM.Interfaces
{
    public interface IDatabaseConnection
    {
        bool Connected();
        bool Disconnected();
        bool IsConnected();
        DataTable ExecuteQuery(string query);
    }
}
