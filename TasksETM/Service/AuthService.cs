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
    public class AuthService : IAuthService
    {
        private readonly string _connectionString;

        public AuthService(string connectionsString)
        {
            _connectionString = connectionsString;
        }

        public async Task<bool> LoginAsync(string departmentName, string password)
        {
            try
            {
                using var conn = new NpgsqlConnection(DatabaseConnection.connString);
                await conn.OpenAsync();

                string query = "SELECT COUNT(*) FROM public.\"Users\" WHERE \"departmentName\" = @departmentName AND \"password\" = @password";
                using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@departmentName", departmentName);
                cmd.Parameters.AddWithValue("@password", password);

                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result) > 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Что-то пошло не так {ex.Message}");
                return false;
            }
        }
    }
}
