using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using VAR1_Demo_exam.Services;

namespace VAR1_Demo_exam.Views
{
    public partial class ManagerWindow : Window
    {
        private readonly DataServiceAdo<Polzovatel> _userService = new DataServiceAdo<Polzovatel>();
        private readonly DataServiceAdo<Zakaz> _zakazService = new DataServiceAdo<Zakaz>();
        private List<Polzovatel> _allUsers = new List<Polzovatel>();

        public ManagerWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                ProfileTextBlock.Text = BuildUserText(SessionInfo.CurrentUser);
                _allUsers = _userService.GetAll();
                UsersGrid.ItemsSource = _allUsers;
                OrdersGrid.ItemsSource = _zakazService.GetAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static string BuildUserText(Polzovatel user)
        {
            if (user == null)
            {
                return "Профиль не загружен";
            }

            return $"ID: {user.ID_Polz}; ФИО: {user.Familia} {user.Imya} {user.Otchestvo}; Почта: {user.Pochta}";
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var wnd = new ProfileEditWindow(SessionInfo.CurrentUser);
                if (wnd.ShowDialog() == true)
                {
                    SessionInfo.CurrentUser = wnd.EditedUser;
                    _userService.Update(wnd.EditedUser);
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка редактирования профиля: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                var text = (SearchTextBox.Text ?? string.Empty).Trim().ToLowerInvariant();
                if (string.IsNullOrEmpty(text))
                {
                    UsersGrid.ItemsSource = _allUsers;
                    return;
                }

                UsersGrid.ItemsSource = _allUsers.Where(u =>
                    $"{u.ID_Polz} {u.Familia} {u.Imya} {u.Otchestvo} {u.Pochta}".ToLowerInvariant().Contains(text)).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка поиска: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AssignCarrier_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Назначение перевозчика выполняется через таблицу связей заказа и пользователей в БД.",
                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ReloadOrders_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OrdersGrid.ItemsSource = _zakazService.GetAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления заказов: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var auth = new MainWindow();
                auth.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка выхода: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
