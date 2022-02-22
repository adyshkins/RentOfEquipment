using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RentOfEquipment.Windows
{
    /// <summary>
    /// Логика взаимодействия для EmployeeWindow.xaml
    /// </summary>
    public partial class EmployeeWindow : Window
    {
        List<string> listSort = new List<string>()
        {
            "По умолчанию",
            "По фамилии",
            "По имени",
            "По электронной почте",
            "По должности"
        };
        public EmployeeWindow()
        {
            InitializeComponent();
            Filter();
            cmbSort.ItemsSource = listSort;
            cmbSort.SelectedIndex = 0;
        }

        private void Filter()
        {
            List<EF.Employee> listEmployee = new List<EF.Employee>();

            listEmployee = ClassHelper.AppData.Context.Employee.ToList();

            // поиск
            listEmployee = listEmployee.
                Where(i => i.LastName.ToLower().Contains(txtSearch.Text.ToLower())
                || i.FirstName.ToLower().Contains(txtSearch.Text.ToLower())
                || i.MiddleName.ToLower().Contains(txtSearch.Text.ToLower())
                || i.FIO.ToLower().Contains(txtSearch.Text.ToLower())
                || i.Phone.ToLower().Contains(txtSearch.Text.ToLower())
                || i.Email.ToLower().Contains(txtSearch.Text.ToLower())).
                ToList();

            // сортировка

            switch (cmbSort.SelectedIndex)
            {
                case 0:
                    listEmployee = listEmployee.OrderBy(i => i.Id).ToList();
                    break;

                case 1:
                    listEmployee = listEmployee.OrderBy(i => i.LastName).ToList();
                    break;

                case 2:
                    listEmployee = listEmployee.OrderBy(i => i.FirstName).ToList();
                    break;

                case 3:
                    listEmployee = listEmployee.OrderBy(i => i.Email).ToList();
                    break;

                case 4:
                    listEmployee = listEmployee.OrderBy(i => i.IdRole).ToList();
                    break;

                default:
                    listEmployee = listEmployee.OrderBy(i => i.Id).ToList();
                    break;
            }

            lvEmployee.ItemsSource = listEmployee;
        }

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            AddEmployeeWindow addEmployeeWindow = new AddEmployeeWindow();
            addEmployeeWindow.ShowDialog();
            Filter();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void lvEmployee_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                var resClick = MessageBox.Show("Удалить пользователя?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (resClick == MessageBoxResult.No)
                {
                    return;
                }

                try
                {
                    if (lvEmployee.SelectedItem is EF.Employee)
                    {
                        var empl = lvEmployee.SelectedItem as EF.Employee;
                        ClassHelper.AppData.Context.Employee.Remove(empl);
                        ClassHelper.AppData.Context.SaveChanges();
                        MessageBox.Show("Пользователь успешно удален", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                        Filter();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }

            }
        }

        private void lvEmployee_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvEmployee.SelectedItem is EF.Employee)
            {
                var empl = lvEmployee.SelectedItem as EF.Employee;

                AddEmployeeWindow addEmployeeWindow = new AddEmployeeWindow(empl);
                addEmployeeWindow.ShowDialog();
                Filter();

            }
        }
    }
}
