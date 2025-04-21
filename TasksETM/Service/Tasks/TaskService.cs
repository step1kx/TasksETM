using IssuingTasksETM.Interfaces;
using Npgsql;
using System.Data;
using TasksETM.Interfaces.ITasks;
using TasksETM.Models;

namespace TasksETM.Service.Tasks
{
    internal class TaskService : ITaskService
    {
        private readonly string _connectionString;

        public TaskService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<DataTable> GetTasksByProjectAsync(string projectName)
        {
            int projectNumber;

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string projectQuery = "SELECT \"ProjectNumber\" FROM public.\"Projects\" WHERE \"ProjectName\" = @ProjectName";

                using (var projectCmd = new NpgsqlCommand(projectQuery, conn))
                {
                    projectCmd.Parameters.AddWithValue("@ProjectName", projectName);
                    var result = await projectCmd.ExecuteScalarAsync();

                    if (result == null)
                        throw new Exception("Проект не найден.");

                    projectNumber = Convert.ToInt32(result);
                }
            }

            string taskQuery = "SELECT t.* " +
                               "FROM public.\"Tasks\" t " +
                               "JOIN public.\"Projects\" p ON t.\"PK_ProjectNumber\" = p.\"ProjectNumber\" " +
                               "WHERE t.\"PK_ProjectNumber\" = @ProjectNumber";

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(taskQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@ProjectNumber", projectNumber);

                    using (var adapter = new NpgsqlDataAdapter(cmd))
                    {
                        var table = new DataTable();
                        adapter.Fill(table);
                        return table;
                    }
                }
            }
        }
    }
}
