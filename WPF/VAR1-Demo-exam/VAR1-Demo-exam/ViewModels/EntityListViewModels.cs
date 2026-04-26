using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using VAR1_Demo_exam.Services;

namespace VAR1_Demo_exam.ViewModels
{
    public abstract class EntityListViewModel<T> : BaseViewModel where T : class, new()
    {
        private readonly DataServiceAdo<T> _service = new DataServiceAdo<T>();
        private T _selectedItem;

        public ObservableCollection<T> Items { get; } = new ObservableCollection<T>();

        public T SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand ReloadCommand { get; }

        protected EntityListViewModel()
        {
            ReloadCommand = new RelayCommand(Load);
        }

        public virtual void Load()
        {
            try
            {
                Items.Clear();
                foreach (var item in _service.GetAll())
                {
                    Items.Add(item);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка загрузки списка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class PolzovatelListViewModel : EntityListViewModel<Polzovatel> { }
    public class PolListViewModel : EntityListViewModel<Pol> { }
    public class FirmaListViewModel : EntityListViewModel<Firma> { }
    public class RolListViewModel : EntityListViewModel<Rol> { }
    public class RolPolzListViewModel : EntityListViewModel<Rol_Polz> { }
    public class ZakazListViewModel : EntityListViewModel<Zakaz> { }

    public class ProfileViewModel : BaseViewModel
    {
        private readonly DataServiceAdo<Polzovatel> _service = new DataServiceAdo<Polzovatel>();
        private Polzovatel _currentProfile;

        public Polzovatel CurrentProfile
        {
            get => _currentProfile;
            set
            {
                _currentProfile = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand SaveProfileCommand { get; }

        public ProfileViewModel()
        {
            CurrentProfile = SessionInfo.CurrentUser;
            SaveProfileCommand = new RelayCommand(Save);
        }

        private void Save()
        {
            try
            {
                if (CurrentProfile == null)
                {
                    return;
                }

                _service.Update(CurrentProfile);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка сохранения профиля: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class OrdersAdminViewModel : BaseViewModel
    {
        private readonly DataServiceAdo<Zakaz> _zakazService = new DataServiceAdo<Zakaz>();
        public ObservableCollection<Zakaz> Orders { get; } = new ObservableCollection<Zakaz>();

        private Zakaz _selectedOrder;
        public Zakaz SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand LoadOrdersCommand { get; }
        public RelayCommand DeleteOrderCommand { get; }

        public OrdersAdminViewModel()
        {
            LoadOrdersCommand = new RelayCommand(LoadOrders);
            DeleteOrderCommand = new RelayCommand(DeleteSelectedOrder, () => SelectedOrder != null);
        }

        public void LoadOrders()
        {
            try
            {
                Orders.Clear();
                foreach (var order in _zakazService.GetAll().OrderByDescending(x => x.ID_Zakaz))
                {
                    Orders.Add(order);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка загрузки заказов: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteSelectedOrder()
        {
            try
            {
                if (SelectedOrder == null)
                {
                    return;
                }

                _zakazService.Delete(SelectedOrder);
                LoadOrders();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка удаления заказа: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
