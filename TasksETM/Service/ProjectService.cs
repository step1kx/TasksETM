using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TasksETM.Interfaces;

namespace TasksETM.Service
{
    public class ProjectService : IProjectService
    {
        
        public async Task<IEnumerable<string>> GetAllProjectNamesAsync()
        {
            var result = new List<string>();

            try
            {
                using var conn = new NpgsqlConnection(DatabaseConnection.connString);
                await conn.OpenAsync();

                string query = "SELECT \"ProjectName\" FROM public.\"Projects\" ORDER BY \"ProjectName\"";

                using var cmd = new NpgsqlCommand(query, conn);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Add(reader.GetString(0));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке проектов: {ex.Message}");
            }

            return result;
        }

        public async Task<bool> GetNotifyStatusByProjectNameAndUserLogin(string projectName, string userLogin)
        {
            try
            {
                using (var conn = new NpgsqlConnection(DatabaseConnection.connString))
                {
                    await conn.OpenAsync();

                    var cmd = new NpgsqlCommand(
                        "SELECT \"isNotify\" FROM public.\"ProjectsNotify\" " +
                        "WHERE \"ProjectNameNotify\" = @ProjectName AND \"UserLoginNameNotify\" = @UserLogin", conn);

                    cmd.Parameters.AddWithValue("@ProjectName", projectName);
                    cmd.Parameters.AddWithValue("@UserLogin", userLogin);

                    var result = await cmd.ExecuteScalarAsync();
                    if (result != null && result != DBNull.Value)
                        return (bool)result;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении статуса уведомлений: {ex.Message}");
            }

            return false; // По умолчанию — выключено
        }


        public async Task UpdateNotifyStatusForUserAndProject(string projectName, string userLogin, bool isNotify)
        {
            try
            {
                using (var conn = new NpgsqlConnection(DatabaseConnection.connString))
                {
                    await conn.OpenAsync();

                    // Сначала пробуем UPDATE
                    var updateCmd = new NpgsqlCommand(
                        "UPDATE public.\"ProjectsNotify\" " +
                        "SET \"isNotify\" = @isNotify " +
                        "WHERE \"ProjectNameNotify\" = @ProjectName AND \"UserLoginNameNotify\" = @UserLogin", conn);

                    updateCmd.Parameters.AddWithValue("@ProjectName", projectName);
                    updateCmd.Parameters.AddWithValue("@UserLogin", userLogin);
                    updateCmd.Parameters.AddWithValue("@isNotify", isNotify);

                    int rowsAffected = await updateCmd.ExecuteNonQueryAsync();

                    // Если обновить не удалось — вставляем новую запись
                    if (rowsAffected == 0)
                    {
                        var insertCmd = new NpgsqlCommand(
                            "INSERT INTO public.\"ProjectsNotify\" " +
                            "(\"isNotify\", \"ProjectNameNotify\", \"UserLoginNameNotify\") " +
                            "VALUES (@isNotify, @ProjectName, @UserLogin)", conn);

                        insertCmd.Parameters.AddWithValue("@isNotify", isNotify);
                        insertCmd.Parameters.AddWithValue("@ProjectName", projectName);
                        insertCmd.Parameters.AddWithValue("@UserLogin", userLogin);

                        await insertCmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении статуса уведомлений: {ex.Message}");
            }
        }

    }
}
