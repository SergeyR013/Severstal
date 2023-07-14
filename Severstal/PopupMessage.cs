using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Severstal
{
    class PopupMessage
    {
        public void ShowPopupMessage(string message, int duration, Color color)
        {
            // Создаем форму без рамки и заголовка
            Form popupForm = new Form
            {
                Size = new Size(100, 50),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.None,
                Text = string.Empty,
                BackColor = color
            };

            // Создаем метку с текстом сообщения
            Label label = new Label
            {
                Text = message,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Добавляем метку на форму
            popupForm.Controls.Add(label);

            // Устанавливаем таймер
            Timer timer = new Timer
            {
                Interval = duration
            };

            timer.Tick += (sender, e) =>
            {
                // Закрываем форму и останавливаем таймер
                popupForm.Close();
                timer.Stop();
            };

            // Запускаем таймер
            timer.Start();

            // Отображаем всплывающее окно
            popupForm.ShowDialog();
        }
    }
}
