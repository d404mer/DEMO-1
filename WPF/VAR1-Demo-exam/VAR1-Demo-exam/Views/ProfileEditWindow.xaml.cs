using System;
using System.Windows;

namespace VAR1_Demo_exam.Views
{
    public partial class ProfileEditWindow : Window
    {
        private readonly Polzovatel _source;
        public Polzovatel EditedUser { get; private set; }

        public ProfileEditWindow(Polzovatel user)
        {
            try
            {
                InitializeComponent();
                _source = user;
                LoadFromUser(user);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка открытия профиля: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadFromUser(Polzovatel user)
        {
            try
            {
                if (user == null)
                {
                    return;
                }

                FamiliaTextBox.Text = user.Familia;
                ImyaTextBox.Text = user.Imya;
                OtchestvoTextBox.Text = user.Otchestvo;
                PochtaTextBox.Text = user.Pochta;
                TelefonTextBox.Text = user.Telephon;
                AdresTextBox.Text = user.Adres;
                ParolTextBox.Text = user.Parol;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка заполнения формы: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_source == null)
                {
                    MessageBox.Show("Нельзя редактировать пустой профиль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                EditedUser = new Polzovatel
                {
                    ID_Polz = _source.ID_Polz,
                    Familia = FamiliaTextBox.Text,
                    Imya = ImyaTextBox.Text,
                    Otchestvo = OtchestvoTextBox.Text,
                    Pochta = PochtaTextBox.Text,
                    Telephon = TelefonTextBox.Text,
                    Adres = AdresTextBox.Text,
                    Parol = ParolTextBox.Text,
                    DataRoz = _source.DataRoz,
                    Foto = _source.Foto,
                    Stazh = _source.Stazh,
                    Strana = _source.Strana
                };

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения профиля: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
