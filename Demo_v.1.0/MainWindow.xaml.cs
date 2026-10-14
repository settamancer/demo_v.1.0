using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace Demo_v._1._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Auth_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль. Они обязательны к заполнению", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = new DemoDBEntities())
            {
                var user = await context.Users
                    .Where(u => u.login == login)
                    .FirstOrDefaultAsync();

                if (user == null) 
                {
                    MessageBox.Show("Пользователь не найден, обратитесь к системному администратору", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (user.IsBlocked == true)
                {
                    MessageBox.Show("Вы заблокированы, обратитесь к системному администратору", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (user.password == password) 
                {
                    user.IsBlocked = false;
                    user.FailedLoginAttempts = 0;
                    await context.SaveChangesAsync();
                    MessageBox.Show("Вы успешно авторизовались!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                

                if (user.role == "Admin")
                {
                    AdminWindow adminWindow = new AdminWindow();
                    adminWindow.Show();
                }
                else
                {
                    UserWindow userWindow = new UserWindow();
                    userWindow.Show();
                }
                
                }
                else 
                {
                    user.FailedLoginAttempts++;

                    
                    if (user.FailedLoginAttempts >= 3)
                    {
                        user.IsBlocked = true;
                        MessageBox.Show("Вы были заблокированы!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);
                        await context.SaveChangesAsync();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show($"Неверный пароль. Осталось попыток: {3 - user.FailedLoginAttempts}");
                    }
                    await context.SaveChangesAsync();
                    
                    txtLogin.Clear();
                    txtPassword.Clear(); 
                
                }
            }
        }
    }
}
