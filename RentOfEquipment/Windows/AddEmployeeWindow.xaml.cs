using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для AddEmployeeWindow.xaml
    /// </summary>
    public partial class AddEmployeeWindow : Window
    {
        bool isEdit = false;
        EF.Employee editEmployee = new EF.Employee();
        string pathPhoto = null;

        public AddEmployeeWindow()
        {
            InitializeComponent();
            cmbRole.ItemsSource = ClassHelper.AppData.Context.Role.ToList();
            cmbRole.DisplayMemberPath = "NameRole";
            cmbRole.SelectedIndex = 1;

            isEdit = false;
        }

        public AddEmployeeWindow(EF.Employee employee)
        {
            InitializeComponent();
            cmbRole.ItemsSource = ClassHelper.AppData.Context.Role.ToList();
            cmbRole.DisplayMemberPath = "NameRole";

            txtLName.Text = employee.LastName;
            txtFName.Text = employee.FirstName;
            txtMName.Text = employee.MiddleName;
            txtPhone.Text = employee.Phone;
            txtEmail.Text = employee.Email;

            // ?*@?*.?*
            txtLogin.Text = employee.Login;
            txtPassword.Password = employee.Password;

            cmbRole.SelectedIndex = employee.IdRole - 1;

            if (employee.Photo != null)
            {
                using (MemoryStream stream = new MemoryStream(employee.Photo))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();

                    photoUser.Source = bitmapImage;
                }

            }

            tbTitle.Text = "Изменение данных работника";
            btnAdd.Content = "Сохранить";

            isEdit = true;

            editEmployee = employee;
        }

            private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //Валидация

            if (string.IsNullOrWhiteSpace(txtLName.Text))
            {
                MessageBox.Show("Поле ФАМИЛИЯ не должно быть пустым","Ошибка",MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFName.Text))
            {
                MessageBox.Show("Поле ИМЯ не должно быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Поле ТЕЛЕФОН не должно быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                MessageBox.Show("Поле ЛОГИН не должно быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Поле ПАРОЛЬ не должно быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (txtPhone.Text.Length > 12)
            {
                MessageBox.Show("Превышено максимальное количество символов в поле ТЕЛЕФОН", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //if (!Int32.TryParse(txtPhone.Text, out int res))
            //{
            //    MessageBox.Show("Недопустимые символы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}

            if (isEdit) // Изменение пользователя
            {
                // обработка случайного нажатия

                var resClick = MessageBox.Show("Изменить пользователя?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (resClick == MessageBoxResult.No)
                {
                    return;
                }

                try
                {
                    editEmployee.LastName = txtLName.Text;
                    editEmployee.FirstName = txtFName.Text;
                    editEmployee.MiddleName = txtMName.Text;
                    editEmployee.Phone = txtPhone.Text;
                    editEmployee.Email = txtEmail.Text;

                    // ?*@?*.?*

                    editEmployee.IdRole = (cmbRole.SelectedItem as EF.Role).Id;
                    editEmployee.Login = txtLogin.Text;
                    editEmployee.Password = txtPassword.Password;

                    if (pathPhoto != null)
                    {
                        editEmployee.Photo = File.ReadAllBytes(pathPhoto);
                    }

                    ClassHelper.AppData.Context.SaveChanges();

                    MessageBox.Show("Пользователь изменен");

                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            else    // Добавление пользователя

            {
                // обработка случайного нажатия

                var resClick = MessageBox.Show("Добавить пользователя?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (resClick == MessageBoxResult.No)
                {
                    return;
                }
             
                try
                {

                    EF.Employee newEmployee = new EF.Employee();

                    newEmployee.LastName = txtLName.Text;
                    newEmployee.FirstName = txtFName.Text;
                    newEmployee.MiddleName = txtMName.Text;
                    newEmployee.Phone = txtPhone.Text;
                    newEmployee.Email = txtEmail.Text;

                    // ?*@?*.?*
                    //
                    newEmployee.IdRole = (cmbRole.SelectedItem as EF.Role).Id;
                    newEmployee.Login = txtLogin.Text;
                    newEmployee.Password = txtPassword.Password;

                    if (pathPhoto != null)
                    {
                        editEmployee.Photo = File.ReadAllBytes(pathPhoto);
                    }



                    ClassHelper.AppData.Context.Employee.Add(newEmployee);

                    ClassHelper.AppData.Context.Employee.Add(new EF.Employee() 
                    {
                        Login = txtLogin.Text,
                        Photo = File.ReadAllBytes(pathPhoto),

                    });

                    ClassHelper.AppData.Context.SaveChanges();

                    MessageBox.Show("Пользователь добавлен");

                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }

        }

        private void btnChoosePhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == true)
            {
                photoUser.Source = new BitmapImage(new Uri(openFile.FileName));
                pathPhoto = openFile.FileName;
            }
        }
    }
}

//////