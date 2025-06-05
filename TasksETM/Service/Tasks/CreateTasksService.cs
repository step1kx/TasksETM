using Npgsql;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TasksETM.Interfaces.ITasks;
using TasksETM.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TasksETM.Service.Tasks
{
    public class CreateTasksService : ICreateTasksService
    {
        private readonly string _selectedProject;
        public event EventHandler<DataTable> TaskCreates;

        public CreateTasksService(string selectedProject)
        {
            _selectedProject = selectedProject;
        }

        public async Task LoadTasksForProjectAsync(string selectedProject)
        {
            try
            {
                using (var conn = new NpgsqlConnection(DatabaseConnection.connString))
                {
                    int projectNumber;
                    using (var projectNumberCommand = new NpgsqlCommand(
                        "SELECT \"ProjectNumber\" FROM public.\"Projects\" WHERE \"ProjectName\" = @ProjectName", conn))
                    {
                        projectNumberCommand.Parameters.AddWithValue("@ProjectName", _selectedProject);
                        projectNumber = Convert.ToInt32(await projectNumberCommand.ExecuteScalarAsync());
                    }

                    var selectCommand = new NpgsqlCommand(@"
                    SELECT t.*,
                        (SELECT ""IsAssigned"" FROM public.""TaskAssignments"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'AR') AS ""IsAR"",
                        (SELECT ""IsAssigned"" FROM public.""TaskAssignments"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'VK') AS ""IsVK"",
                        (SELECT ""IsAssigned"" FROM public.""TaskAssignments"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'OV') AS ""IsOV"",
                        (SELECT ""IsAssigned"" FROM public.""TaskAssignments"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'SS') AS ""IsSS"",
                        (SELECT ""IsAssigned"" FROM public.""TaskAssignments"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'ES') AS ""IsES"",
                        (SELECT ""IsAssigned"" FROM public.""TaskAssignments"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'GIP') AS ""IsGIP"",
                        (SELECT ""IsCompleted"" FROM public.""TaskCompleted"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'AR') AS ""IsARCompl"",
                        (SELECT ""IsCompleted"" FROM public.""TaskCompleted"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'VK') AS ""IsVKCompl"",
                        (SELECT ""IsCompleted"" FROM public.""TaskCompleted"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'OV') AS ""IsOVCompl"",
                        (SELECT ""IsCompleted"" FROM public.""TaskCompleted"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'SS') AS ""IsSSCompl"",
                        (SELECT ""IsCompleted"" FROM public.""TaskCompleted"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'ES') AS ""IsESCompl"",
                        (SELECT ""IsCompleted"" FROM public.""TaskCompleted"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'GIP') AS ""IsGIPCompl""
                    FROM public.""Tasks"" t
                    JOIN public.""Projects"" p ON t.""PK_ProjectNumber"" = p.""ProjectNumber""
                    WHERE p.""ProjectNumber"" = @ProjectNumber
                        AND t.""FromDepart"" IS NOT NULL
                        AND t.""ToDepart"" IS NOT NULL
                        AND t.""TaskDescription"" IS NOT NULL", conn);

                    selectCommand.Parameters.AddWithValue("@ProjectNumber", projectNumber);

                    var dt = new DataTable("public.\"Tasks\"");
                    using (var adapter = new NpgsqlDataAdapter(selectCommand))
                    {
                        await Task.Run(() => adapter.Fill(dt));
                    }

                    TaskCreates?.Invoke(this, dt);
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"Ошибка при отображении задач: {ex.Message}");
                throw;
            }

        }

        public async Task CreateTaskAsync(TaskModel taskModel, CheckBox section)
        {
            using (var conn = new NpgsqlConnection(DatabaseConnection.connString))
            {
                try
                {
                    await conn.OpenAsync();

                    int projectNumber;
                    using (var projectNumberCommand = new NpgsqlCommand(
                        "SELECT \"ProjectNumber\" FROM public.\"Projects\" WHERE \"ProjectName\" = @ProjectName", conn))
                    {
                        projectNumberCommand.Parameters.AddWithValue("@ProjectName", _selectedProject);
                        projectNumber = Convert.ToInt32(await projectNumberCommand.ExecuteScalarAsync());
                    }

                    int taskNumber;
                    using (var createCommand = new NpgsqlCommand(@"
                    INSERT INTO public.""Tasks"" (
                        ""FromDepart"", ""ToDepart"", ""AcceptedDepart"", ""TaskCompleted"",
                        ""ScreenShot"", ""TaskView"", ""TaskDescription"",
                        ""PK_ProjectNumber"", ""TaskDate"", ""TaskDeadLine"")
                    VALUES (
                        @FromDepart, @ToDepart, 0, 0,
                        @ScreenShot, @TaskView, @TaskDescription,
                        @ProjectNumber, @TaskDate, @TaskDeadline)
                    RETURNING ""TaskNumber""", conn))
                    {
                        createCommand.Parameters.AddWithValue("@FromDepart", taskModel.FromDepart);
                        createCommand.Parameters.AddWithValue("@ToDepart", taskModel.ToDepart);
                        createCommand.Parameters.AddWithValue("@ScreenShot", taskModel.ScreenshotPath ?? (object)DBNull.Value);
                        createCommand.Parameters.AddWithValue("@TaskView", taskModel.TaskView);
                        createCommand.Parameters.AddWithValue("@TaskDescription", taskModel.TaskDescription);
                        createCommand.Parameters.AddWithValue("@ProjectNumber", projectNumber);
                        createCommand.Parameters.AddWithValue("@TaskDate", taskModel.TaskDate);
                        createCommand.Parameters.AddWithValue("@TaskDeadline", taskModel.TaskDeadline);

                        taskNumber = Convert.ToInt32(await createCommand.ExecuteScalarAsync());
                    }

                    var sections = new[] { "AR", "VK", "OV", "SS", "ES", "GIP" };

                    foreach(var sectionName in sections)
                    {
                        // TaskAssignments
                        using (var insertAssignment = new NpgsqlCommand(@"
                        INSERT INTO public.""TaskAssignments"" (""TaskNumber"", ""Section"", ""IsAssigned"")
                        VALUES (@TaskNumber, @Section, @IsAssigned)", conn))
                        {
                                    insertAssignment.Parameters.AddWithValue("@TaskNumber", taskNumber);
                                    insertAssignment.Parameters.AddWithValue("@Section", sectionName);
                                    insertAssignment.Parameters.AddWithValue("@IsAssigned", false);
                                    await insertAssignment.ExecuteNonQueryAsync();
                        }

                        // TaskCompleted
                        using (var insertCompleted = new NpgsqlCommand(@"
                        INSERT INTO public.""TaskCompleted"" (""TaskNumber"", ""Section"", ""IsCompleted"")
                        VALUES (@TaskNumber, @Section, @IsCompleted)", conn))
                        {
                                    insertCompleted.Parameters.AddWithValue("@TaskNumber", taskNumber);
                                    insertCompleted.Parameters.AddWithValue("@Section", sectionName);
                                    insertCompleted.Parameters.AddWithValue("@IsCompleted", false);
                                    await insertCompleted.ExecuteNonQueryAsync();
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при создании задачи: {ex.Message}");
                    throw;
                }
            }
        }

    }
}