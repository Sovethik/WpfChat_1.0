using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfChat_1._0.Pages;
using WpfChat_1._0.Classes;
using System.Windows.Media;
using WpfChat_1._0.Properties;

namespace WpfChat_1._0.Classes
{
    public class ClientConnect : Page
    {
        string UserName;

        NetworkStream networkStream;
        StreamReader Reader;
        StreamWriter Writer;
        int countFile = 0;



        public async Task StartConnectAsync()
        {
            string host = "26.237.33.146";
            int port = 500;
            TcpClient client = new TcpClient();


            try
            {
                client.Connect(host, port);
                networkStream = client.GetStream();
                Reader = new StreamReader(networkStream);
                Writer = new StreamWriter(networkStream);

                if (Reader is null || Writer is null)
                    return;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        public async Task ReciveDataAsync(ListBox lbChat, Chat chatPage)
        {
            try
            {
                while (true)
                {
                    string typeData = await Reader.ReadLineAsync();
                    switch (typeData)
                    {
                        case "message":
                            await ReciveMessageAsync(lbChat);
                            break;
                        case "file":
                            await ReciveFileAsync(lbChat, chatPage);
                            break;

                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        public async Task ReciveMessageAsync(ListBox lbChat)
        {
            string message = await Reader.ReadLineAsync();
            TextBlock textBlock = new TextBlock();
            textBlock.Margin = new Thickness(2);
            textBlock.Foreground = new SolidColorBrush(Colors.White);
            textBlock.FontFamily = new FontFamily("Comic Sans MS");

            Border border = new Border();
            border.CornerRadius = new CornerRadius(10);
            border.Background = new SolidColorBrush(Color.FromRgb(94, 94, 94));
            textBlock.Text = message;
            border.Child = textBlock;

            lbChat.Items.Add(border);
        }

        //Получение файла
        public async Task ReciveFileAsync(ListBox lbChat, Chat chatPage)
        {
            string loginRecive = await Reader.ReadLineAsync();

            byte[] fileSize = new byte[sizeof(int)];

            await networkStream.ReadAsync(fileSize, 0, fileSize.Length);
            int sizeToInt = BitConverter.ToInt32(fileSize, 0);

            byte[] fileData = new byte[sizeToInt];

            await networkStream.ReadAsync(fileData, 0, fileData.Length);

            string formatFile = ".jpeg";

            if (fileData[4] == 0x66 && fileData[5] == 0x74 && fileData[6] == 0x79
                            && fileData[7] == 0x70)
                formatFile = ".MP4";

            

            try
            {

                switch(formatFile)
                {
                    case ".jpeg":
                        File.WriteAllBytes($"{Environment.CurrentDirectory}/ReciveFile/Image/{loginRecive}_{countFile}{formatFile}", fileData);

                        StackPanel stackPanel = new StackPanel();
                        stackPanel.Orientation = Orientation.Horizontal;

                        TextBlock textBlock = new TextBlock();
                        textBlock.FontSize = 14;
                        textBlock.Text = loginRecive + ": ";
                        textBlock.Foreground = new SolidColorBrush(Colors.White);

                        stackPanel.Children.Add(textBlock);

                        Image newImage = new Image();
                        newImage.Source = new BitmapImage(new Uri($"{Environment.CurrentDirectory}/ReciveFile/Image/{loginRecive}_{countFile}{formatFile}"));
                        newImage.Height = 80;
                        newImage.Name = loginRecive + "_" + countFile;

                        Button button = new Button();
                        button.Style = (Style)Resources["Button_OpenFile"];
                        button.Content = newImage;
                        button.Click += chatPage.Button_Click;
                        stackPanel.Children.Add(button);

                        lbChat.Items.Add(stackPanel);
                        break;
                    case ".MP4":


                        File.WriteAllBytes($"{Environment.CurrentDirectory}/ReciveFile/Video/{loginRecive}_{countFile}{formatFile}", fileData);
                        MediaElement videoElement = new MediaElement();
                        videoElement.Name = loginRecive + "_" + countFile;


                        button = new Button();
                        button.Style = (Style)Resources["Button_OpenFile"];
                        button.Content = videoElement;
                        button.Click += chatPage.Button_Click;

                        textBlock = new TextBlock();
                        textBlock.FontSize = 16;
                        textBlock.Text = loginRecive + ": ";
                        textBlock.Foreground = new SolidColorBrush(Colors.White);

                        stackPanel = new StackPanel();
                        stackPanel.Children.Add(textBlock);
                        stackPanel.Children.Add(button);

                        videoElement.Source = new Uri($"{Environment.CurrentDirectory}/ReciveFile/Video/{loginRecive}_{countFile}{formatFile}");
                        videoElement.LoadedBehavior = MediaState.Manual;
                        videoElement.Play();
                        videoElement.Height = 150;



                        lbChat.Items.Add(stackPanel);
                        break;
                }

                countFile++;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

        }

        //Отправить файл
        public async Task EnterFileAsync(string PATH_FILE, ListBox lbChat, Chat chatPage)
        {
            await Writer.WriteLineAsync("file");
            await Writer.FlushAsync();

            await Task.Delay(1000);

            byte[] fileData = File.ReadAllBytes(PATH_FILE);
            byte[] sizeFile = BitConverter.GetBytes(fileData.Length);



            await networkStream.WriteAsync(sizeFile, 0, sizeFile.Length);
            await networkStream.FlushAsync();

            await Task.Delay(1000);

            await networkStream.WriteAsync(fileData, 0, fileData.Length);
            await networkStream.FlushAsync();

            string extension = Path.GetExtension(PATH_FILE);
            string FileName = Path.GetFileName(PATH_FILE);

            switch (extension)
            {
                case ".jpg":

                    File.WriteAllBytes($"{Environment.CurrentDirectory}/ReciveFile/Image/{UserName}_{countFile}{extension}", fileData);


                    StackPanel stackPanel = new StackPanel();
                    stackPanel.Orientation = Orientation.Horizontal;

                    TextBlock textBlock = new TextBlock();
                    textBlock.FontSize = 16;
                    textBlock.Text = "Вы: ";
                    textBlock.Foreground = new SolidColorBrush(Colors.White);

                    stackPanel.Children.Add(textBlock);


                    Image newImage = new Image();
                    newImage.Source = new BitmapImage(new Uri(PATH_FILE));
                    newImage.Height = 80;
                    newImage.Name = UserName + "_" + countFile.ToString();

                    Button button = new Button();
                    button.Style = (Style)Resources["Button_OpenFile"];
                    button.Content = newImage;
                    button.Click += chatPage.Button_Click;
                    stackPanel.Children.Add(button);

                    lbChat.Items.Add(stackPanel);

                    break;
                case ".MP4":

                    File.WriteAllBytes($"{Environment.CurrentDirectory}/ReciveFile/Video/{UserName}_{countFile}{extension}", fileData);

                    stackPanel = new StackPanel();
                    stackPanel.Orientation = Orientation.Horizontal;

                    MediaElement videoElement = new MediaElement();
                    videoElement.Name = UserName + "_" + countFile;

                    textBlock = new TextBlock();
                    textBlock.FontSize = 16;
                    textBlock.Text = "Вы: ";
                    textBlock.Foreground = new SolidColorBrush(Colors.White);

                    videoElement.Source = new Uri(PATH_FILE);
                    videoElement.LoadedBehavior = MediaState.Manual;
                    videoElement.Play();
                    videoElement.Height = 150;

                    button = new Button();
                    button.Style = (Style)Resources["Button_OpenFile"];
                    button.Content = videoElement;
                    button.Click += chatPage.Button_Click;

                    stackPanel.Children.Add(textBlock);
                    stackPanel.Children.Add(button);

                    lbChat.Items.Add(stackPanel);

                    break;

            } 
            



        }

        


        

        public async Task EnterMessage(string message)
        {
            try
            {
                await Writer.WriteLineAsync("message");
                await Writer.FlushAsync();
                await Writer.WriteLineAsync(message);
                await Writer.FlushAsync();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        public async Task AuthUserAsync(string login, string password, MainWindow mainWindow)
        {
            byte[] confirmation = new byte[1] { 1 };

            await networkStream.WriteAsync(confirmation, 0, confirmation.Length);

            await Writer.WriteLineAsync(login);
            await Writer.FlushAsync();
            await Writer.WriteLineAsync(password);
            await Writer.FlushAsync();

            byte[] result = new byte[1];
            await networkStream.ReadAsync(result, 0, result.Length);

            if (result[0] == 1)
            {
                MainMenu mainMenu = new MainMenu(this);
                mainMenu.Show();
                UserName = login;
                mainWindow.Close();
            }
            else
            {
                MessageBox.Show("Логин или пароль не верны");
            }
        }

        public async Task RegUserAsync(string login, string password, string firstName, string lastName, RegUserWindow regWindow)
        {

            byte[] confirmation = new byte[1] { 0 };
            await networkStream.WriteAsync(confirmation, 0, confirmation.Length);
            await networkStream.FlushAsync();

            await Writer.WriteLineAsync(login);
            await Writer.FlushAsync();

            await Writer.WriteLineAsync(password);
            await Writer.FlushAsync();

            await Writer.WriteLineAsync(firstName);
            await Writer.FlushAsync();

            await Writer.WriteLineAsync(lastName);
            await Writer.FlushAsync();

            byte[] answerServer = new byte[1];
            await networkStream.ReadAsync(answerServer, 0, answerServer.Length);

            if (answerServer[0] == 1)
            {
                MessageBox.Show("Регистрация прошла успешно!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                regWindow.Close();
            }
            else if (answerServer[0] == 0)
            {
                MessageBox.Show("Пользователь с таким никнеймом уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("На сервер произошел сбой", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        

    }
}
