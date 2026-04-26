using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using VAR1_Demo_exam.Services;

namespace VAR1_Demo_exam.Views
{
    public partial class DirectorWindow : Window
    {
        private readonly DataServiceAdo<Polzovatel> _userService = new DataServiceAdo<Polzovatel>();
        private readonly DataServiceAdo<Zakaz> _zakazService = new DataServiceAdo<Zakaz>();
        private List<Polzovatel> _allUsers = new List<Polzovatel>();

        public DirectorWindow()
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

                _allUsers = _userService.GetAll();
                var orders = _zakazService.GetAll();
                UsersGrid.ItemsSource = _allUsers;
                OrdersGrid.ItemsSource = orders;
                ReportTextBlock.Text = $"Всего пользователей: {_allUsers.Count}; Всего заказов: {orders.Count}.";
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
                new MainWindow().Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка выхода: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
