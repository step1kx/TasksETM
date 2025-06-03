using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TasksETM.Interfaces;

namespace TasksETM.Service
{
    public class DepartmentService : IDepartmentService
    {
        public async Task<IEnumerable<string>> GetDepartmentNamesAsync()
        {
            var result = new List<string>();

            try
            {
                using var conn = new NpgsqlConnection(DatabaseConnection.connString);
                await conn.OpenAsync();

                string query = "SELECT \"departmentName\" FROM public.\"Departments\" ORDER BY \"departmentName\"";

                using var cmd = new NpgsqlCommand(query, conn);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Add(reader.GetString(0));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке отделов: {ex.Message}");
            }

            return result;
        }

        public async Task<IEnumerable<string>> GetTaskStatusAsync()
        {
            var result = new List<string>();

            try
            {
                using var conn = new NpgsqlConnection(DatabaseConnection.connString);
                await conn.OpenAsync();

                string query = "SELECT \"TaskStatus\" FROM public.\"CompetedStatus\" ORDER BY \"TaskStatus\"";

                using var cmd = new NpgsqlCommand(query, conn);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Add(reader.GetString(0));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке отделов: {ex.Message}");
            }

            return result;
        }

        public async Task<IEnumerable<string>> GetUsersSurnamesByDepartmentAsync(string departmentName)
        {
            var result = new List<string>();
            try
            {
                using var conn = new NpgsqlConnection(DatabaseConnection.connString);
                await conn.OpenAsync();

                string queryDept = "SELECT \"id\" FROM public.\"Departments\" WHERE \"departmentName\" = @depName LIMIT 1";
                int? departmentId = null;
                using (var cmdDept = new NpgsqlCommand(queryDept, conn))
                {
                    cmdDept.Parameters.AddWithValue("depName", departmentName);
                    var res = await cmdDept.ExecuteScalarAsync();
                    if (res != null)
                        departmentId = Convert.ToInt32(res);
                }

                if (departmentId == null)
                    return result;

                string queryUsers = "SELECT \"userName\" FROM public.\"Users\" WHERE \"departmentid\" = @depId ORDER BY \"userName\"";
                using (var cmdUsers = new NpgsqlCommand(queryUsers, conn))
                {
                    cmdUsers.Parameters.AddWithValue("depId", departmentId.Value);
                    using var reader = await cmdUsers.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        result.Add(reader.GetString(0));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке фамилий по отделу: {ex.Message}");
            }
            return result;
        }
    }
}
