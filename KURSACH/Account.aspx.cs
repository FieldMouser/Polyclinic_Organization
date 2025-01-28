using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Npgsql;

namespace KURSACH
{
    public partial class Account : System.Web.UI.Page
    {
        private string connectionString = "Host=localhost;Username=postgres;Password=123123;Database=polyclinic";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Получаем пользователя из куки или сессии
                var currentUser = GetCurrentUser();

                if (currentUser != null)
                {
                    // Приветствие пользователя
                    WelcomeLabel.Text += currentUser.FullName;

                    if (currentUser.Role == "Patient")
                    {
                        // Показываем панель для пациента
                        PatientInfoPanel.Visible = true;

                        // Загружаем информацию для пациента
                        LoadPatientInfo(currentUser.Email);
                    }
                    else
                    {
                        // Панель для других ролей
                        OtherRolesInfoPanel.Visible = true;
                    }
                }
                else
                {
                    // Если пользователя нет в куках или сессии, редиректим на страницу входа
                    Response.Redirect("Login.aspx");
                }
            }
        }

        // Метод для получения текущего пользователя
        private User GetCurrentUser()
        {
            var userCookie = Request.Cookies["UserEmail"];
            if (userCookie != null)
            {
                string email = userCookie.Value;
                return GetUserByEmail(email);
            }
            return null;
        }

        // Метод для получения информации о пользователе по email
        private User GetUserByEmail(string email)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT u.id, u.full_name, ur.role_id, r.role_name " +
                               "FROM users u " +
                               "JOIN user_roles ur ON u.id = ur.user_id " +
                               "JOIN roles r ON ur.role_id = r.id " +
                               "WHERE u.email = @Email";
                var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("Email", email);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new User
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        FullName = reader["full_name"].ToString(),
                        Role = reader["role_name"].ToString(),
                        Email = email // Добавляем email в объект User
                    };
                }
            }
            return null;
        }

        // Метод для загрузки информации о пациенте по его email
        private void LoadPatientInfo(string email)
        {
            int patientId = GetPatientIdByEmail(email);

            if (patientId > 0)
            {
                // Загружаем диагнозы, лечение, записи и тесты
                LoadDiagnosisAndTreatment(patientId);
                LoadAppointments(patientId);
                LoadMedicalTests(patientId);
            }
            else
            {
                // Если ID пациента не найден, выводим сообщение
                Response.Write("<script>alert('Не удалось найти информацию о пациенте. Пожалуйста, убедитесь, что вы вошли в систему.');</script>");
            }
        }

        // Метод для получения ID пациента по email
        private int GetPatientIdByEmail(string email)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id FROM patients WHERE user_id = (SELECT id FROM users WHERE email = @Email)";
                var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("Email", email);
                var result = cmd.ExecuteScalar();

                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        // Метод для загрузки диагнозов и лечения
        private void LoadDiagnosisAndTreatment(int patientId)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT mh.diagnosis, mh.treatment " +
                               "FROM medical_histories mh " +
                               "WHERE mh.patient_id = @PatientId";
                var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("PatientId", patientId);
                var reader = cmd.ExecuteReader();

                string diagnosis = string.Empty;
                string treatment = string.Empty;

                if (reader.Read())
                {
                    diagnosis = reader["diagnosis"].ToString();
                    treatment = reader["treatment"].ToString();
                }

                // Отображаем данные
                DiagnosisLabel.Text = diagnosis;
                TreatmentLabel.Text = treatment;
            }
        }

        // Метод для загрузки записей на прием
        private void LoadAppointments(int patientId)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT a.appointment_date, d.full_name AS doctor_name " +
                               "FROM appointments a " +
                               "JOIN doctors d ON a.doctor_id = d.id " +
                               "WHERE a.patient_id = @PatientId";
                var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("PatientId", patientId);
                var reader = cmd.ExecuteReader();

                var appointments = new DataTable();
                appointments.Columns.Add("Дата приема");
                appointments.Columns.Add("Врач");

                while (reader.Read())
                {
                    var appointmentRow = appointments.NewRow();
                    appointmentRow["Дата приема"] = reader["appointment_date"];
                    appointmentRow["Врач"] = reader["doctor_name"];
                    appointments.Rows.Add(appointmentRow);
                }

                AppointmentsGrid.DataSource = appointments;
                AppointmentsGrid.DataBind();
            }
        }

        // Метод для загрузки медицинских тестов
        private void LoadMedicalTests(int patientId)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT mt.test_name, mt.test_date " +
                               "FROM medical_tests mt " +
                               "WHERE mt.patient_id = @PatientId";
                var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("PatientId", patientId);
                var reader = cmd.ExecuteReader();

                var tests = new DataTable();
                tests.Columns.Add("Тест");
                tests.Columns.Add("Дата теста");

                while (reader.Read())
                {
                    var testRow = tests.NewRow();
                    testRow["Тест"] = reader["test_name"];
                    testRow["Дата теста"] = reader["test_date"];
                    tests.Rows.Add(testRow);
                }

                MedicalTestsGrid.DataSource = tests;
                MedicalTestsGrid.DataBind();
            }
        }
    }

    // Класс для пользователя (можно расширить)
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Email { get; set; } // Добавляем поле Email
    }
}
