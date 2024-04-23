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

namespace WpfChat_1._0.Pages
{
    /// <summary>
    /// Логика взаимодействия для OpenFile.xaml
    /// </summary>
    public partial class OpenFile : Page
    {
        ClientConnect clientConnect;
        bool isPauseVideo = false;

        public OpenFile(ClientConnect _clientConnect, string pathFile)
        {
            InitializeComponent();
            clientConnect = _clientConnect;



            string extension = System.IO.Path.GetExtension(pathFile);

            switch (extension)
            {
                case ".jpg":
                    Image image = new Image();
                    image.Source = new BitmapImage(new Uri(pathFile));
                    image.Height = image.Source.Height;
                    grid.Children.Add(image);
                    break;
                case ".MP4":
                    MediaElement videoElement = new MediaElement();
                    videoElement.Source = new Uri(pathFile);
                    videoElement.LoadedBehavior = MediaState.Manual;
                    videoElement.Play();

                    Button button = new Button();
                    ResourceDictionary resources = new ResourceDictionary() { Source = new Uri("pack://application:,,,/DictionaryStyle.xaml") };
                    button.Style = (Style)resources["Button_OpenFile"];
                    button.Content = videoElement;
                    button.Click += Button_Click;

                    grid.Children.Add(button);
                    break;
            }
            
        }

        

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            MediaElement videoElement = (sender as Button).Content as MediaElement;

            if(videoElement != null)
            {
                if(!isPauseVideo)
                {
                    videoElement.Pause();
                    isPauseVideo = true;
                }
                else
                {
                    videoElement.Play();
                    isPauseVideo = false;
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
