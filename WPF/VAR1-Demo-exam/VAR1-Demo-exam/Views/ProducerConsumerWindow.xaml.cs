using System;
using System.Windows;
using VAR1_Demo_exam.Services;

namespace VAR1_Demo_exam.Views
{
    public partial class ProducerConsumerWindow : Window
    {
        private readonly DataServiceAdo<Zakaz> _zakazService = new DataServiceAdo<Zakaz>();
        private readonly DataServiceAdo<Polzovatel> _polzService = new DataServiceAdo<Polzovatel>();

        public ProducerConsumerWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var current = SessionInfo.CurrentUser;
                ProfileTextBlock.Text = current == null
                    ? "Профиль не загружен"
                    : $"ID: {current.ID_Polz}; {current.Familia} {current.Imya} {current.Otchestvo}";

                var orders = _zakazService.GetAll();
                OrdersGrid.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var wnd = new ProfileEditWindow(SessionInfo.CurrentUser);
                if (wnd.ShowDialog() == true)
                {
                    SessionInfo.CurrentUser = wnd.EditedUser;
                    _polzService.Update(wnd.EditedUser);
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка редактирования профиля: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NewOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var wnd = new ZakazEditWindow(null);
                if (wnd.ShowDialog() == true)
                {
                    _zakazService.Add(wnd.EditedZakaz);
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка создания заявки: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new MainWindow().Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка перехода: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
