using IssuingTasksETM.Interfaces;
using Npgsql;
using System.Data;
using System.Windows;
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

        public async Task<List<TaskModel>> GetTasksByProjectAsync(string projectName)
        {
            var tasks = new List<TaskModel>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(@"
                        SELECT ""TaskNumber"", ""FromDepart"", ""ToDepart"", ""AcceptedDepart"", ""TaskCompleted"", 
                        ""ScreenShot"", ""TaskView"", ""TaskDescription"", ""TaskDate"", ""TaskDeadLine""
                        FROM ""Tasks""
                        WHERE ""PK_ProjectNumber"" = (
                            SELECT ""ProjectNumber"" FROM ""Projects"" WHERE ""ProjectName"" = @projectname
                        )
                        ", conn))
                {
                    cmd.Parameters.AddWithValue("projectname", projectName);
                    try
                    {


                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var task = new TaskModel
                                {
                                    TaskNumber = reader.GetInt32(reader.GetOrdinal("TaskNumber")),

                                    FromDepart = reader["FromDepart"] is DBNull ? string.Empty : reader["FromDepart"]!.ToString()!,
                                    ToDepart = reader["ToDepart"] is DBNull ? string.Empty : reader["ToDepart"]!.ToString()!,

                                    Accepted = reader["AcceptedDepart"] is DBNull
                                                ? null
                                                : (int)reader["AcceptedDepart"] == 1,


                                    TaskCompleted = reader["TaskCompleted"] is DBNull ? null : (int)reader["TaskCompleted"] == 1,

                                    ScreenshotPath = reader["ScreenShot"] is DBNull ? new byte[0] : (byte[])reader["ScreenShot"],

                                    TaskView = reader["TaskView"] is DBNull ? string.Empty : reader["TaskView"]!.ToString()!,
                                    TaskDescription = reader["TaskDescription"] is DBNull ? string.Empty : reader["TaskDescription"]!.ToString()!,
                                    TaskDate = reader["TaskDate"] is DBNull ? string.Empty : reader["TaskDate"]!.ToString()!,
                                    TaskDeadline = reader["TaskDeadLine"] is DBNull ? string.Empty : reader["TaskDeadLine"]!.ToString()!
                                };

                                tasks.Add(task);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}");
                    }
                }
            }

            return tasks;
        }

    }
}
