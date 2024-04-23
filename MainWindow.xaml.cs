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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfChat_1._0.Classes;

namespace WpfChat_1._0
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ClientConnect client = new ClientConnect();

        public MainWindow()
        {
            InitializeComponent();
            client.StartConnectAsync();
        }

        private async void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            await client.AuthUserAsync(TxtLogin.Text, TxtPassword.Text, this);
        }

        private void BtnReg_Click(object sender, RoutedEventArgs e)
        {
            RegUserWindow regWindow = new RegUserWindow(client);
            regWindow.Show();
            
        }
    }
}
