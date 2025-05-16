using Notify.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Notify.Models;
using TasksETMCommon;    

namespace Notify.NotificationService
{
    public class TaskService : ITaskService
    {
        public readonly string connectionString = "Server=192.168.0.171; Port=5432; User Id=User; Password=123; Database=postgres";

        public async Task<List<TaskModel>> GetTasksByUserAsync(string currentLogin)
        {
            var tasks = new List<TaskModel>();

            using (var conn = new NpgsqlConnection(connectionString))
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
                    p.""ProjectName"",
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
                JOIN public.""Project"" p ON p.""ProjectNumber"" = t.""PK_ProjectNumber""
                WHERE t.""ToDepart"" IN (
                    SELECT ""departmentName"" FROM public.""Users"" WHERE ""Login"" = @login
                )
                AND t.""FromDepart"" IS NOT NULL 
                AND t.""ToDepart"" IS NOT NULL 
                AND t.""TaskDescription"" IS NOT NULL
                ", conn))
                {
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
    }
}

