using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Npgsql;

namespace KURSACH
{
    public partial class AccountAdmin : System.Web.UI.Page
    {
        private string connectionString = "Host=localhost;Username=postgres;Password=123123;Database=polyclinic";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUsers();
            }
        }

        // Метод для загрузки пользователей
        private void LoadUsers()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                // Обновленный запрос, использующий правильные имена столбцов
                string query = "SELECT u.id, u.full_name, u.email, u.role " +
                               "FROM users u";
                var cmd = new NpgsqlCommand(query, conn);
                var reader = cmd.ExecuteReader();
                DataTable usersTable = new DataTable();
                usersTable.Load(reader);

                UsersGridView.DataSource = usersTable;
                UsersGridView.DataBind();
            }
        }


        // Обработчик для удаления пользователя
        protected void UsersGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                int userId = Convert.ToInt32(e.CommandArgument);
                DeleteUser(userId);
                LoadUsers(); // Обновляем список после удаления
            }
        }

        // Метод для удаления пользователя
        private void DeleteUser(int userId)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                // Вызываем функцию удаления пользователя
                var cmd = new NpgsqlCommand("SELECT delete_user(@UserId)", conn);
                cmd.Parameters.AddWithValue("UserId", userId);
                cmd.ExecuteNonQuery();
            }
        }

        // Обработчик для добавления нового пользователя
        protected void AddUserButton_Click(object sender, EventArgs e)
        {
            string role = RoleDropDown.SelectedValue;
            string fullName = FullNameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordTextBox.Text;

            if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                Response.Write("<script>alert('Пожалуйста, заполните все поля.');</script>");
                return;
            }

            AddNewUser(role, fullName, email, password);

            // Очистка полей после добавления
            FullNameTextBox.Text = string.Empty;
            EmailTextBox.Text = string.Empty;
            PasswordTextBox.Text = string.Empty;

            LoadUsers(); // Обновляем список пользователей
        }

        // Метод для добавления нового пользователя в базу данных
        private void AddNewUser(string role, string fullName, string email, string password)
        {
            string hashedPassword = HashPassword(password);

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                // Вставка нового пользователя в таблицу users
                string insertQuery = "INSERT INTO users (full_name, email, password_hash) VALUES (@FullName, @Email, @Password)";
                var cmd = new NpgsqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("FullName", fullName);
                cmd.Parameters.AddWithValue("Email", email);
                cmd.Parameters.AddWithValue("Password", hashedPassword);
                cmd.ExecuteNonQuery();

                // Получаем ID нового пользователя
                string getUserIdQuery = "SELECT id FROM users WHERE email = @Email";
                var getUserIdCmd = new NpgsqlCommand(getUserIdQuery, conn);
                getUserIdCmd.Parameters.AddWithValue("Email", email);
                int userId = Convert.ToInt32(getUserIdCmd.ExecuteScalar());

                // Вставляем запись в таблицу user_roles для указания роли пользователя
                string insertRoleQuery = "INSERT INTO user_roles (user_id, role_id) VALUES (@UserId, (SELECT id FROM roles WHERE role_name = @Role))";
                var insertRoleCmd = new NpgsqlCommand(insertRoleQuery, conn);
                insertRoleCmd.Parameters.AddWithValue("UserId", userId);
                insertRoleCmd.Parameters.AddWithValue("Role", role);
                insertRoleCmd.ExecuteNonQuery();
            }
        }

        // Метод для хеширования пароля
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
