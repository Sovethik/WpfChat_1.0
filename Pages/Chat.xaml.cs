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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfChat_1._0.Classes;

namespace WpfChat_1._0.Pages
{
    /// <summary>
    /// Логика взаимодействия для Chat.xaml
    /// </summary>
    public partial class Chat : Page
    {
        static string PATH_FILE;

        ClientConnect clientConnect;
        public Chat(ClientConnect _clientConnect)
        {
            InitializeComponent();
            clientConnect = _clientConnect;
            clientConnect.ReciveDataAsync(lbChat, this);
        }

        private async void TxtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await clientConnect.EnterMessage(TxtMessage.Text);
                
                TextBlock textBlock = new TextBlock();
                textBlock.Margin = new Thickness(2);
                textBlock.Foreground = new SolidColorBrush(Colors.White);
                textBlock.FontFamily = new FontFamily("Comic Sans MS");

                Border border = new Border();
                border.CornerRadius = new CornerRadius(10);
                border.Background = new SolidColorBrush(Color.FromRgb(120, 68, 133));
                textBlock.Text = "Вы: " + TxtMessage.Text;
                border.Child = textBlock;
                border.HorizontalAlignment = HorizontalAlignment.Right;

                Grid grid = new Grid();
                grid.HorizontalAlignment = HorizontalAlignment.Stretch;
                grid.Children.Add(border);

                lbChat.Items.Add(grid);
                TxtMessage.Text = "";
            }
        }

        private async void btnSelectedFile_Click(object sender, RoutedEventArgs e)
        {
            PATH_FILE = ToKnowNameFile();
            if (PATH_FILE == "")
                return;
            await clientConnect.EnterFileAsync(PATH_FILE, lbChat, this);
        }

        private string ToKnowNameFile()
        {
            string copy = Directory.GetCurrentDirectory();
            copy = copy.Substring(0, copy.Length - 9) + @"Documets\";

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = "c:";
            ofd.Filter = "jpeg files (*.jpg)|*.jpg|All fiels (*.*)|*.*";
            ofd.FilterIndex = 2;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == true)
            {
                var str = ofd.FileName.Split(new[] { '\\' }).Last();
                File.Copy(ofd.FileName, System.IO.Path.Combine(copy, str), true);
                string name = ofd.SafeFileName;

                return copy + name;
            }
            else
            {
                return "";
            }
           
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            Image newImage = ((sender as Button).Content as Image);
            string pathFile = "";

            if (newImage != null)
            {
                pathFile = $"{Environment.CurrentDirectory}/ReciveFile/Image/{newImage.Name}.jpg";

            }
            else
            {
                MediaElement videoElement = (sender as Button).Content as MediaElement;
                if(videoElement != null)
                {
                    pathFile = $"{Environment.CurrentDirectory}/ReciveFile/Video/{videoElement.Name}.MP4";

                }
            }

            if(pathFile != "")
                NavigationService.Navigate(new OpenFile(clientConnect, pathFile));
            

        }


    }
}
