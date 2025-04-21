using IssuingTasksETM.Interfaces;
using Npgsql;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace TasksETM.Service
{
    public class DatabaseConnection : IDatabaseConnection
    {
        public static string connString = "Server=192.168.0.171; Port=5432; User Id=User; Password=123; Database=postgres";
        private NpgsqlConnection connection;

        public bool Connected()
        {
            try
            {
                connection = new NpgsqlConnection(connString);
                connection.Open();
                return true;
            }
            catch (NpgsqlException exp)
            {
                MessageBox.Show($"Ошибка в подключении к базе данных:\n{exp.Message}");
                return false;
            }
        }

        public bool Disconnected()
        {
            try
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                    return true;
                }
                return false;
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Ошибка отключения от базы данных: {ex.Message}");
                return false;
            }
        }

        public bool IsConnected()
        {
            return connection != null && connection.State == ConnectionState.Open;
        }

        public bool CheckerDBConn()
        {
            if (!IsConnected())
            {
                if (!Connected())
                {
                    MessageBox.Show("Нет подключения к базе данных!");
                    return false;
                }
            }
            return true;
        }

        public DataTable ExecuteQuery(string query)
        {
            if (!CheckerDBConn())
            {
                throw new Exception("Нет активного подключения к базе данных!");
            }

            DataTable result = new DataTable();
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd))
                {
                    adapter.Fill(result);
                }
                return result;
            }
            catch (NpgsqlException exp)
            {
                throw new Exception($"Ошибка выполнения запроса: {exp.Message}");
            }
        }

        public bool LoginDepartment(string departmentName, string password)
        {
            if (!CheckerDBConn())
            {
                return false;
            }

            try
            {
                string query = "SELECT COUNT(*) FROM public.\"Users\" WHERE \"departmentName\" = @departmentName AND \"password\" = @password";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@departmentName", departmentName);
                    cmd.Parameters.AddWithValue("@password", password);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка входа: {ex.Message}");
                return false;
            }
        }

        public bool FillProjects(ComboBox comboBox)
        {
            if (!CheckerDBConn())
            {
                return false;
            }

            try
            {
                string query = "SELECT \"ProjectName\" FROM public.\"Projects\" ORDER BY \"ProjectName\"";
                DataTable result = ExecuteQuery(query);

                comboBox.Items.Clear();

                foreach (DataRow row in result.Rows)
                {
                    comboBox.Items.Add(row["ProjectName"].ToString());
                }

                if (comboBox.Items.Count > 0)
                {
                    comboBox.SelectedIndex = 0;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при заполнении проектов: {ex.Message}");
                return false;
            }
        }

        public bool FillDepartmentName(ComboBox comboBox)
        {
            if (!CheckerDBConn())
            {
                return false;
            }

            try
            {
                string query = "SELECT \"departmentName\" FROM public.\"Users\" ORDER BY \"departmentName\"";
                DataTable result = ExecuteQuery(query);

                comboBox.Items.Clear();

                foreach (DataRow row in result.Rows)
                {
                    comboBox.Items.Add(row["departmentName"].ToString());
                }

                if (comboBox.Items.Count > 0)
                {
                    comboBox.SelectedIndex = 0;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при заполнении имен отделов: {ex.Message}");
                return false;
            }
        }
    }
}