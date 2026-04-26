using System.Windows;
using VAR1_Demo_exam.Services;
using VAR1_Demo_exam.Views;

namespace VAR1_Demo_exam
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var auth = new AuthService();
                if (!auth.Login(IdNumberTextBox.Text, PasswordBox.Password, out var error))
                {
                    MessageBox.Show(error, "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Window nextWindow = null;
                if (SessionInfo.Roles.Contains(AppRole.Director))
                {
                    nextWindow = new DirectorWindow();
                }
                else if (SessionInfo.Roles.Contains(AppRole.Manager))
                {
                    nextWindow = new ManagerWindow();
                }
                else if (SessionInfo.Roles.Contains(AppRole.Producer) || SessionInfo.Roles.Contains(AppRole.Consumer))
                {
                    nextWindow = new ProducerConsumerWindow();
                }
                else if (SessionInfo.Roles.Contains(AppRole.Carrier))
                {
                    nextWindow = new CarrierWindow();
                }

                if (nextWindow == null)
                {
                    MessageBox.Show("Для пользователя не назначена поддерживаемая роль.", "Вход невозможен", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                nextWindow.Show();
                Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка при входе: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
