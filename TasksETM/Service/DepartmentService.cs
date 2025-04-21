using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
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

                string query = "SELECT \"departmentName\" FROM public.\"Users\" ORDER BY \"departmentName\"";

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
    }
}
