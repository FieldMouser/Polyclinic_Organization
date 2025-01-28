using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Npgsql;

namespace KURSACH
{
    public partial class ScheduleAppointment : System.Web.UI.Page
    {
        private string connectionString = "Host=localhost;Username=postgres;Password=123123;Database=polyclinic";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDoctors();
            }
        }

        // Загрузка списка врачей
        private void LoadDoctors()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id, full_name FROM doctors";
                var cmd = new NpgsqlCommand(query, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DoctorDropDown.Items.Add(new ListItem(reader["full_name"].ToString(), reader["id"].ToString()));
                }
            }
        }

        // Когда выбран врач, загружаем доступное время
        protected void DoctorDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            int doctorId = Convert.ToInt32(DoctorDropDown.SelectedValue);
            if (doctorId > 0)
            {
                // Загружаем доступное время для выбранного врача
                LoadAvailableTimes(doctorId);
            }
        }

        // Загрузка доступных интервалов времени для выбранного врача
        private void LoadAvailableTimes(int doctorId)
        {
            // Получаем выбранную дату
            DateTime selectedDate = AppointmentCalendar.SelectedDate;

            // Проверяем, выбрана ли дата
            if (selectedDate == DateTime.MinValue)
            {
                TimeDropDown.Items.Clear();
                TimeDropDown.Items.Add(new ListItem("Выберите дату", "0"));
                return;
            }

            TimeDropDown.Items.Clear();
            TimeDropDown.Items.Add(new ListItem("Выберите время", "0"));

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT start_time, end_time FROM doctor_schedule WHERE doctor_id = @doctorId AND day_of_week = @dayOfWeek";
                var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("doctorId", doctorId);
                cmd.Parameters.AddWithValue("dayOfWeek", selectedDate.DayOfWeek.ToString());

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    TimeSpan startTime = (TimeSpan)reader["start_time"];
                    TimeSpan endTime = (TimeSpan)reader["end_time"];

                    for (TimeSpan time = startTime; time < endTime; time = time.Add(TimeSpan.FromMinutes(15)))
                    {
                        string timeSlot = time.ToString(@"hh\:mm");
                        TimeDropDown.Items.Add(new ListItem(timeSlot, timeSlot));
                    }
                }
            }
        }

        // Когда выбран день на календаре
        protected void AppointmentCalendar_SelectionChanged(object sender, EventArgs e)
        {
            // Когда дата изменяется, загружаем доступные интервалы времени для этого дня
            int doctorId = Convert.ToInt32(DoctorDropDown.SelectedValue);
            if (doctorId > 0)
            {
                LoadAvailableTimes(doctorId);
            }
        }

        // Кнопка для записи на прием
        protected void BookAppointmentButton_Click(object sender, EventArgs e)
        {
            int doctorId = Convert.ToInt32(DoctorDropDown.SelectedValue);
            string selectedTime = TimeDropDown.SelectedValue;
            DateTime selectedDate = AppointmentCalendar.SelectedDate;
            string reason = ComplaintTextBox.Text.Trim();

            // Извлекаем email из куки
            string email = Request.Cookies["UserEmail"]?.Value;

            if (string.IsNullOrEmpty(email))
            {
                Response.Write("<script>alert('Ошибка: пользователь не найден.');</script>");
                return;
            }

            // Получаем user_id по email
            int userId = GetUserIdByEmail(email);

            if (userId == 0)
            {
                Response.Write("<script>alert('Не удалось найти пользователя. Пожалуйста, убедитесь, что вы вошли в систему.');</script>");
                return;
            }

            // Получаем patient_id по user_id
            int patientId = GetPatientIdByUserId(userId);

            if (patientId == 0)
            {
                Response.Write("<script>alert('Не удалось найти информацию о пациенте. Пожалуйста, убедитесь, что вы зарегистрированы.');</script>");
                return;
            }

            // Проверка, что все данные заполнены
            if (doctorId == 0 || selectedTime == "0" || selectedDate == DateTime.MinValue || string.IsNullOrEmpty(reason))
            {
                Response.Write("<script>alert('Пожалуйста, заполните все поля.');</script>");
                return;
            }

            // Формируем полный запрос для записи на прием
            DateTime appointmentDate = selectedDate.Add(TimeSpan.Parse(selectedTime));

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                // Проверка, занят ли выбранный интервал времени
                string checkQuery = "SELECT COUNT(*) FROM appointments WHERE doctor_id = @doctorId AND appointment_date = @appointmentDate";
                var checkCmd = new NpgsqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("doctorId", doctorId);
                checkCmd.Parameters.AddWithValue("appointmentDate", appointmentDate);
                int appointmentCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (appointmentCount > 0)
                {
                    // Если время занято
                    Response.Write("<script>alert('Выбранное время уже занято. Пожалуйста, выберите другое.');</script>");
                }
                else
                {
                    // Записываем прием в базу данных
                    string insertQuery = "INSERT INTO appointments (doctor_id, patient_id, appointment_date, reason) VALUES (@doctorId, @patientId, @appointmentDate, @reason)";
                    var insertCmd = new NpgsqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("doctorId", doctorId);
                    insertCmd.Parameters.AddWithValue("patientId", patientId); // Используем найденный patientId
                    insertCmd.Parameters.AddWithValue("appointmentDate", appointmentDate);
                    insertCmd.Parameters.AddWithValue("reason", reason);

                    insertCmd.ExecuteNonQuery();

                    Response.Write("<script>alert('Вы успешно записаны на прием!');</script>");
                }
            }
        }

        // Метод для получения user_id по email
        private int GetUserIdByEmail(string email)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id FROM users WHERE email = @Email";
                var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("Email", email);

                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0; // Если email не найден, возвращаем 0
            }
        }

        // Метод для получения patient_id по user_id
        private int GetPatientIdByUserId(int userId)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id FROM patients WHERE user_id = @userId";
                var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("userId", userId);

                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0; // Если пациент не найден, возвращаем 0
            }
        }
    }
}
