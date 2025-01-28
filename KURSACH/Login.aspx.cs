using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Npgsql;

namespace KURSACH
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        private string connectionString = "Host=localhost;Username=postgres;Password=123123;Database=polyclinic";
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Login_btn_Click(object sender, EventArgs e)
        {
            string email = Email_tb.Text.Trim();
            string password = Password_tb.Text.Trim();

            // Проверка, что email и пароль не пустые
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                // Можно отобразить ошибку (например, через Label или JavaScript)
                return;
            }

            // Проверка, существует ли такой пользователь и правильный ли пароль
            if (ValidateUser(email, password))
            {
                // Создаем куки перед редиректом
                HttpCookie userCookie = new HttpCookie("isLoggedIn", "true"); // Сохраняем, что пользователь вошел
                userCookie.Expires = DateTime.Now.AddDays(30); // Кука будет храниться 30 дней
                Response.Cookies.Add(userCookie);

                // Сохраняем также email или ID пользователя в куки, если необходимо
                HttpCookie emailCookie = new HttpCookie("UserEmail", email);
                emailCookie.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Add(emailCookie);

                // После того как куки добавлены, выполняем редирект
                Response.Redirect("~/Default.aspx");
            }
            else
            {
                // Отображаем ошибку
                Error_lbl.Text = "Неверный email или пароль.";
                Error_lbl.Visible = true;
            }

        }

        // Метод для проверки пользователя в базе данных
        private bool ValidateUser(string email, string password)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT password_hash FROM users WHERE email = @Email", conn))
                {
                    cmd.Parameters.AddWithValue("Email", email);

                    // Получаем хеш пароля из базы данных
                    var result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        string storedPasswordHash = result.ToString();
                        // Сравниваем хеши паролей
                        if (VerifyPassword(password, storedPasswordHash))
                        {
                            return true; // Пользователь найден и пароль совпадает
                        }
                    }
                    return false; // Логин или пароль неверные
                }
            }
        }

        // Метод для сравнения введенного пароля с хешированным паролем
        private bool VerifyPassword(string password, string storedHash)
        {
            // Хешируем введенный пароль и сравниваем с сохраненным хешем
            string passwordHash = HashPassword(password);
            return storedHash == passwordHash;
        }

        // Метод для хеширования пароля
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Преобразуем строку в массив байтов и вычисляем хеш
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Преобразуем массив байтов в строку
                StringBuilder builder = new StringBuilder();
                foreach (byte t in bytes)
                {
                    builder.Append(t.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}