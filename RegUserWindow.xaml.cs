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
using WpfChat_1._0.Classes;

namespace WpfChat_1._0
{
    /// <summary>
    /// Логика взаимодействия для RegUserWindow.xaml
    /// </summary>
    public partial class RegUserWindow : Window
    {
        ClientConnect clientConnect;

        public RegUserWindow(ClientConnect _clientConnect)
        {
            InitializeComponent();
            clientConnect = _clientConnect;
        }

        private async void BtnReg_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder error = new StringBuilder();


            if (txtLogin.Text.Length < 3)
            {
                error.Append("Логин не может быть короче трех символов\n");
                MessageBox.Show(error.ToString());
                return;
            }

            if (txtPassword.Text.Length < 3)
                error.Append("Пароль не может быть короче трех символов\n");

            if (Char.IsDigit(txtLogin.Text[0]))
               error.Append("Логин не может начинатся с цифры\n");

            if (txtFirstName.Text == "")
                error.Append("Заполните имя\n");

            if (txtLastName.Text == "")
                error.Append("Заполните фамилию");

            if(error.Length > 0)
            {
                MessageBox.Show(error.ToString());
                return;
            }
           


           await clientConnect.RegUserAsync(txtLogin.Text, txtPassword.Text, txtFirstName.Text, txtLastName.Text, this);
        }
    }
}
