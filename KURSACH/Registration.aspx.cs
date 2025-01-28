using System;
using System.Data;
using System.Web.UI;
using Npgsql;

namespace KURSACH
{
    public partial class Registration : System.Web.UI.Page
    {
        private string connectionString = "Host=localhost;Username=postgres;Password=123123;Database=polyclinic";

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Register_btn_Click(object sender, EventArgs e)
        {
            string fullName = Full_Name_tb.Text.Trim();  // Получаем имя
            string email = Email_tb.Text.Trim();         // Получаем email
            string password = Password_tb.Text.Trim();   // Получаем пароль
            string passwordRepeat = Password_repeat_tb.Text.Trim(); // Повтор пароля

            // Проверка на совпадение паролей
            if (password != passwordRepeat)
            {
                // Уведомление о несовпадении паролей
                Response.Write("<script>alert('Пароли не совпадают. Пожалуйста, попробуйте снова.');</script>");
                return;
            }

            string hashedPassword = HashPassword(password);  // Хешируем пароль

            // Проверка на существующий email
            if (IsEmailExist(email))
            {
                // Уведомление о существующем email
                Response.Write("<script>alert('Пользователь с таким email уже существует.');</script>");
                return;
            }

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                // Вставляем нового пользователя в таблицу users
                string insertUserQuery = "INSERT INTO users (full_name, email, password_hash, role) " +
                                         "VALUES (@fullName, @email, @password, 'Patient') RETURNING id;";
                var cmd = new NpgsqlCommand(insertUserQuery, conn);
                cmd.Parameters.AddWithValue("fullName", fullName);
                cmd.Parameters.AddWithValue("email", email);
                cmd.Parameters.AddWithValue("password", hashedPassword);

                // Получаем id нового пользователя
                int newUserId = (int)cmd.ExecuteScalar();

                // Вставляем запись в таблицу patients с данными пациента
                string insertPatientQuery = "INSERT INTO patients (user_id, full_name, address, phone) " +
                                            "VALUES (@userId, @fullName, @address, @phone)";
                var patientCmd = new NpgsqlCommand(insertPatientQuery, conn);
                patientCmd.Parameters.AddWithValue("userId", newUserId);
                patientCmd.Parameters.AddWithValue("fullName", fullName);
                patientCmd.Parameters.AddWithValue("address", Address_tb.Text.Trim()); // Получаем адрес
                patientCmd.Parameters.AddWithValue("phone", Phone_tb.Text.Trim());     // Получаем телефон

                patientCmd.ExecuteNonQuery(); // Выполняем вставку данных пациента

                // Вставляем роль "Пациент" в таблицу user_roles
                string insertRoleQuery = "INSERT INTO user_roles (user_id, role_id) VALUES (@userId, 3);"; // 2 - это id роли 'Пациент'
                var roleCmd = new NpgsqlCommand(insertRoleQuery, conn);
                roleCmd.Parameters.AddWithValue("userId", newUserId);
                roleCmd.ExecuteNonQuery();
            }

            // Уведомление пользователя о успешной регистрации
            Response.Redirect("~/Login.aspx");
        }

        private string HashPassword(string password)
        {
            using (System.Security.Cryptography.SHA256 sha256Hash = System.Security.Cryptography.SHA256.Create())
            {
                // Преобразуем строку в массив байтов и вычисляем хеш
                byte[] bytes = sha256Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                // Преобразуем массив байтов в строку
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                foreach (byte t in bytes)
                {
                    builder.Append(t.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Метод для проверки наличия email в базе данных
        private bool IsEmailExist(string email)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT COUNT(*) FROM users WHERE email = @Email", conn))
                {
                    cmd.Parameters.AddWithValue("Email", email);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
    }
}
