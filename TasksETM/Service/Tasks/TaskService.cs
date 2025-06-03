using IssuingTasksETM.Interfaces;
using Npgsql;
using System.Data;
using System.Windows;
using TasksETM.Interfaces.ITasks;
using TasksETM.Models;

namespace TasksETM.Service.Tasks
{
    public class TaskService : ITaskService
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
                SELECT 
                    t.""TaskNumber"", 
                    t.""FromDepart"", 
                    t.""ToDepart"", 
                    t.""AcceptedDepart"",
                    t.""TaskCompleted"",
                    t.""ScreenShot"", 
                    t.""TaskView"", 
                    t.""TaskDescription"", 
                    t.""TaskDate"", 
                    t.""TaskDeadLine"",
                    (SELECT ""IsAssigned"" FROM public.""TaskAssignments"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'AR') AS ""IsAR"",
                    (SELECT ""IsAssigned"" FROM public.""TaskAssignments"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'VK') AS ""IsVK"",
                    (SELECT ""IsAssigned"" FROM public.""TaskAssignments"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'OV') AS ""IsOV"",
                    (SELECT ""IsAssigned"" FROM public.""TaskAssignments"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'SS') AS ""IsSS"",
                    (SELECT ""IsAssigned"" FROM public.""TaskAssignments"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'ES') AS ""IsES"",
                    (SELECT ""IsAssigned"" FROM public.""TaskAssignments"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'GIP') As ""IsGIP"",
                    (SELECT ""IsCompleted"" FROM public.""TaskCompleted"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'AR') AS ""IsARCompl"",
                    (SELECT ""IsCompleted"" FROM public.""TaskCompleted"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'VK') AS ""IsVKCompl"",
                    (SELECT ""IsCompleted"" FROM public.""TaskCompleted"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'OV') AS ""IsOVCompl"",
                    (SELECT ""IsCompleted"" FROM public.""TaskCompleted"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'SS') AS ""IsSSCompl"",
                    (SELECT ""IsCompleted"" FROM public.""TaskCompleted"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'ES') AS ""IsESCompl"",
                    (SELECT ""IsCompleted"" FROM public.""TaskCompleted"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'GIP') As ""IsGIPCompl""
                FROM public.""Tasks"" t
                WHERE t.""PK_ProjectNumber"" = (
                    SELECT ""ProjectNumber"" FROM public.""Projects"" WHERE ""ProjectName"" = @projectname
                )
                AND t.""FromDepart"" IS NOT NULL 
                AND t.""ToDepart"" IS NOT NULL 
                AND t.""TaskDescription"" IS NOT NULL
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
                                    Accepted = reader["AcceptedDepart"] is DBNull ? null : (int)reader["AcceptedDepart"] == 1,
                                    Completed = reader["TaskCompleted"] is DBNull ? null : (int)reader["TaskCompleted"] == 1,
                                    ScreenshotPath = reader["ScreenShot"] is DBNull ? new byte[0] : (byte[])reader["ScreenShot"],
                                    TaskView = reader["TaskView"] is DBNull ? string.Empty : reader["TaskView"]!.ToString()!,
                                    TaskDescription = reader["TaskDescription"] is DBNull ? string.Empty : reader["TaskDescription"]!.ToString()!,
                                    TaskDate = reader["TaskDate"] is DBNull ? string.Empty : reader["TaskDate"]!.ToString()!,
                                    TaskDeadline = reader["TaskDeadLine"] is DBNull ? string.Empty : reader["TaskDeadLine"]!.ToString()!,
                                    IsAR = reader["IsAR"] is DBNull ? null : (bool)reader["IsAR"],
                                    IsVK = reader["IsVK"] is DBNull ? null : (bool)reader["IsVK"],
                                    IsOV = reader["IsOV"] is DBNull ? null : (bool)reader["IsOV"],
                                    IsSS = reader["IsSS"] is DBNull ? null : (bool)reader["IsSS"],
                                    IsES = reader["IsES"] is DBNull ? null : (bool)reader["IsES"],
                                    IsGIP = reader["IsGIP"] is DBNull ? null : (bool)reader["IsGIP"],
                                    IsARCompl = reader["IsARCompl"] is DBNull ? null : (bool)reader["IsARCompl"],
                                    IsVKCompl = reader["IsVKCompl"] is DBNull ? null : (bool)reader["IsVKCompl"],
                                    IsOVCompl = reader["IsOVCompl"] is DBNull ? null : (bool)reader["IsOVCompl"],
                                    IsSSCompl = reader["IsSSCompl"] is DBNull ? null : (bool)reader["IsSSCompl"],
                                    IsESCompl = reader["IsESCompl"] is DBNull ? null : (bool)reader["IsESCompl"],
                                    IsGIPCompl = reader["IsGIPCompl"] is DBNull ? null : (bool)reader["IsGIPCompl"],
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

        public async Task UpdateTaskAssignmentsAsync(int taskNumber, bool isAR, bool isVK, bool isOV, bool isSS, bool isES, bool isGIP)
        {
            try
            {
                using (var conn = new NpgsqlConnection(DatabaseConnection.connString))
                {
                    await conn.OpenAsync();

                    var sections = new[] { ("AR", isAR), ("VK", isVK), ("OV", isOV), ("SS", isSS), ("ES", isES), ("GIP", isGIP) };
                    foreach (var (section, isAssigned) in sections)
                    {
                        var updateCommand = new NpgsqlCommand(
                            "UPDATE public.\"TaskAssignments\" " +
                            "SET \"IsAssigned\" = @IsAssigned " +
                            "WHERE \"TaskNumber\" = @TaskNumber AND \"Section\" = @Section", conn);
                        updateCommand.Parameters.AddWithValue("@TaskNumber", taskNumber);
                        updateCommand.Parameters.AddWithValue("@Section", section);
                        updateCommand.Parameters.AddWithValue("@IsAssigned", isAssigned);
                        int rowsAffected = await updateCommand.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            var insertCommand = new NpgsqlCommand(
                                "INSERT INTO public.\"TaskAssignments\" (\"TaskNumber\", \"Section\", \"IsAssigned\") " +
                                "VALUES (@TaskNumber, @Section, @IsAssigned)", conn);
                            insertCommand.Parameters.AddWithValue("@TaskNumber", taskNumber);
                            insertCommand.Parameters.AddWithValue("@Section", section);
                            insertCommand.Parameters.AddWithValue("@IsAssigned", isAssigned);
                            await insertCommand.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении задания: {ex.Message}");
            }
        }

        public async Task UpdateTaskCompletedAsync(int taskNumber, bool isARCompl, bool isVKCompl, bool isOVCompl, bool isSSCompl, bool isESCompl, bool isGIPCompl)
        {
            try
            {
                using (var conn = new NpgsqlConnection(DatabaseConnection.connString))
                {
                    await conn.OpenAsync();

                    var sections = new[] { ("AR", isARCompl), ("VK", isVKCompl), ("OV", isOVCompl), ("SS", isSSCompl), ("ES", isESCompl), ("GIP", isGIPCompl) };
                    foreach (var (section, isCompleted) in sections)
                    {
                        var updateCommand = new NpgsqlCommand(
                            "UPDATE public.\"TaskCompleted\" " +
                            "SET \"IsCompleted\" = @IsCompleted " +
                            "WHERE \"TaskNumber\" = @TaskNumber AND \"Section\" = @Section", conn);
                        updateCommand.Parameters.AddWithValue("@TaskNumber", taskNumber);
                        updateCommand.Parameters.AddWithValue("@Section", section);
                        updateCommand.Parameters.AddWithValue("@IsCompleted", isCompleted);
                        int rowsAffected = await updateCommand.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            var insertCommand = new NpgsqlCommand(
                                "INSERT INTO public.\"TaskCompleted\" (\"TaskNumber\", \"Section\", \"IsCompleted\") " +
                                "VALUES (@TaskNumber, @Section, @IsCompleted)", conn);
                            insertCommand.Parameters.AddWithValue("@TaskNumber", taskNumber);
                            insertCommand.Parameters.AddWithValue("@Section", section);
                            insertCommand.Parameters.AddWithValue("@IsCompleted", isCompleted);
                            await insertCommand.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении задания: {ex.Message}");
            }
        }



    }
}
