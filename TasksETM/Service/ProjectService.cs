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
    }
}
