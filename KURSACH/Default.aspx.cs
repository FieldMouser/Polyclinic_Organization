using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KURSACH
{
    public partial class _Default : Page
    {

            protected void Page_Load(object sender, EventArgs e)
            {
                // Проверяем, есть ли куки с email пользователя
                HttpCookie emailCookie = Request.Cookies["UserEmail"];
                if (emailCookie != null)
                {
                    string email = emailCookie.Value;

                    // Получаем полное имя пользователя по его email
                    string fullName = GetFullNameByEmail(email);

                    // Выводим приветствие
                    if (!string.IsNullOrEmpty(fullName))
                    {
                        WelcomeMessage.InnerText = "Добро пожаловать, " + fullName + "!";
                    }
                }
            }

            // Метод для получения полного имени пользователя по email
            private string GetFullNameByEmail(string email)
            {
                string fullName = string.Empty;

                // Здесь код для получения полного имени из базы данных по email пользователя
                // Например:
                string query = "SELECT full_name FROM users WHERE email = @Email";

                using (var conn = new Npgsql.NpgsqlConnection("Host=localhost;Username=postgres;Password=123123;Database=polyclinic"))
                {
                    conn.Open();
                    using (var cmd = new Npgsql.NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("Email", email);
                        var result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            fullName = result.ToString();
                        }
                    }
                }

                return fullName;
            }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/ScheduleAppointment.aspx");
        }
    }
}