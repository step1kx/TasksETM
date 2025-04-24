using Npgsql;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
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

        public async Task CreateTaskAsync(TaskModel taskModel)
        {
            try
            {
                using (var conn = new NpgsqlConnection(DatabaseConnection.connString))
                {
                    await conn.OpenAsync();

                    var projectNumberCommand = new NpgsqlCommand(
                        "SELECT \"ProjectNumber\" FROM public.\"Projects\" WHERE \"ProjectName\" = @ProjectName", conn);
                    projectNumberCommand.Parameters.AddWithValue("@ProjectName", _selectedProject);
                    var projectNumber = Convert.ToInt32(await projectNumberCommand.ExecuteScalarAsync());

                    var createCommand = new NpgsqlCommand(
                        "INSERT INTO public.\"Tasks\" (\"FromDepart\", \"ToDepart\", \"AcceptedDepart\", \"TaskCompleted\", " +
                        "\"ScreenShot\", \"TaskView\", \"TaskDescription\", " +
                        "\"PK_ProjectNumber\", \"TaskDate\", \"TaskDeadLine\") " +
                        "VALUES (@FromDepart, @ToDepart, 0, 0, @ScreenShot, @TaskView, @TaskDescription, @ProjectNumber, @TaskDate, @TaskDeadline) " +
                        "RETURNING \"TaskNumber\"", conn);

                    createCommand.Parameters.AddWithValue("@FromDepart", taskModel.FromDepart);
                    createCommand.Parameters.AddWithValue("@ToDepart", taskModel.ToDepart);
                    createCommand.Parameters.AddWithValue("@ScreenShot", taskModel.ScreenshotPath ?? (object)DBNull.Value);
                    createCommand.Parameters.AddWithValue("@TaskDescription", taskModel.TaskDescription);
                    createCommand.Parameters.AddWithValue("@TaskView", taskModel.TaskView);
                    createCommand.Parameters.AddWithValue("@ProjectNumber", projectNumber);
                    createCommand.Parameters.AddWithValue("@TaskDate", taskModel.TaskDate);
                    createCommand.Parameters.AddWithValue("@TaskDeadline", taskModel.TaskDeadline);

                    var taskNumber = Convert.ToInt32(await createCommand.ExecuteScalarAsync());

                    var sections = new[] { "AR", "VK", "OV", "SS", "ES" };
                    foreach (var section in sections)
                    {
                        var insertAssignmentCommand = new NpgsqlCommand(
                            "INSERT INTO public.\"TaskAssignments\" (\"TaskNumber\", \"Section\", \"IsAssigned\") " +
                            "VALUES (@TaskNumber, @Section, @IsAssigned)", conn);
                        insertAssignmentCommand.Parameters.AddWithValue("@TaskNumber", taskNumber);
                        insertAssignmentCommand.Parameters.AddWithValue("@Section", section);
                        insertAssignmentCommand.Parameters.AddWithValue("@IsAssigned", false);
                        await insertAssignmentCommand.ExecuteNonQueryAsync();
                    }

                    var selectCommand = new NpgsqlCommand(
                        "SELECT t.*, " +
                        "(SELECT \"IsAssigned\" FROM public.\"TaskAssignments\" WHERE \"TaskNumber\" = t.\"TaskNumber\" AND \"Section\" = 'AR') AS \"IsAR\", " +
                        "(SELECT \"IsAssigned\" FROM public.\"TaskAssignments\" WHERE \"TaskNumber\" = t.\"TaskNumber\" AND \"Section\" = 'VK') AS \"IsVK\", " +
                        "(SELECT \"IsAssigned\" FROM public.\"TaskAssignments\" WHERE \"TaskNumber\" = t.\"TaskNumber\" AND \"Section\" = 'OV') AS \"IsOV\", " +
                        "(SELECT \"IsAssigned\" FROM public.\"TaskAssignments\" WHERE \"TaskNumber\" = t.\"TaskNumber\" AND \"Section\" = 'SS') AS \"IsSS\", " +
                        "(SELECT \"IsAssigned\" FROM public.\"TaskAssignments\" WHERE \"TaskNumber\" = t.\"TaskNumber\" AND \"Section\" = 'ES') AS \"IsES\" " +
                        "FROM public.\"Tasks\" t " +
                        "JOIN public.\"Projects\" p ON t.\"PK_ProjectNumber\" = p.\"ProjectNumber\" " +
                        "WHERE p.\"ProjectNumber\" = @ProjectNumber " +
                        "AND t.\"FromDepart\" IS NOT NULL " +
                        "AND t.\"ToDepart\" IS NOT NULL " +
                        "AND t.\"TaskDescription\" IS NOT NULL", conn);
                    selectCommand.Parameters.AddWithValue("@ProjectNumber", projectNumber);

                    var dt = new DataTable("public.\"Tasks\"");
                    using (var adapter = new NpgsqlDataAdapter(selectCommand))
                    {
                        await Task.Run(() => adapter.Fill(dt));
                    }

                    TaskCreates?.Invoke(this, dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Словили ошибку при создании задания: {ex.Message}");
            }
        }
    }
}