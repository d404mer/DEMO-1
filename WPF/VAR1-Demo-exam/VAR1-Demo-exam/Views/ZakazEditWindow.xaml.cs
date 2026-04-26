using System;
using System.Linq;
using System.Windows;
using VAR1_Demo_exam.Services;

namespace VAR1_Demo_exam.Views
{
    public partial class ZakazEditWindow : Window
    {
        private readonly Zakaz _source;
        public Zakaz EditedZakaz { get; private set; }

        public ZakazEditWindow(Zakaz zakaz)
        {
            try
            {
                InitializeComponent();
                _source = zakaz;

                if (_source != null)
                {
                    DateOtprPicker.SelectedDate = _source.DataOtpr;
                    DniVPutiTextBox.Text = _source.DniVPuti.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка открытия заявки: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(DniVPutiTextBox.Text, out var days) || days < 0)
                {
                    MessageBox.Show("Введите корректное значение дней в пути.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var service = new DataServiceAdo<Zakaz>();
                var nextId = service.GetAll().Select(z => z.ID_Zakaz).DefaultIfEmpty(0).Max() + 1;

                EditedZakaz = new Zakaz
                {
                    ID_Zakaz = _source?.ID_Zakaz ?? nextId,
                    DataOtpr = DateOtprPicker.SelectedDate ?? DateTime.Now,
                    DniVPuti = days
                };

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения заявки: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
