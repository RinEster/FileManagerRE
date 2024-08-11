using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FileManagerRE
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string currentPath;

        public MainWindow()
        {
            InitializeComponent();
            currentPath = @"C:\";
            LoadFilesAndDirectories(currentPath);
            
        }

        /// <summary>
        /// загрузка директорий и файлов
        /// </summary>
        /// <param name="path">текущий путь</param>
        private void LoadFilesAndDirectories(string path)
        {
            allFileList.Items.Clear();
            filePath.Text = path;

            try
            {               
                foreach (var dir in Directory.GetDirectories(path))
                {
                    var directoryInfo = new DirectoryInfo(dir);
                    if ((directoryInfo.Attributes & FileAttributes.Hidden)==0)
                    {
                        allFileList.Items.Add(new DirectoryInfo(dir).Name);
                    }                    
                }

                foreach (var file in Directory.GetFiles(path))
                {
                    var fileInfo = new FileInfo(file);
                    if ((fileInfo.Attributes & FileAttributes.Hidden) == 0)
                    {
                        allFileList.Items.Add(new FileInfo(file).Name);
                    }                      
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
               
        /// <summary>
        /// обработчик кнопки Back
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            var parentDir = Directory.GetParent(currentPath);
            if (parentDir != null)
            {
                currentPath = parentDir.FullName;
                LoadFilesAndDirectories(currentPath);
            }
        }

        /// <summary>
        /// обработчик двойного нажатия на элемент списка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void allFileList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (allFileList.SelectedItem != null)
            {
                string selectedItem = allFileList.SelectedItem.ToString();
                string selectedPath = Path.Combine(currentPath, selectedItem);

                if (Directory.Exists(selectedPath))
                {
                    currentPath = selectedPath;
                    LoadFilesAndDirectories(currentPath);
                }
                else if (File.Exists(selectedPath))
                {
                    FileInfo fileInfo = new FileInfo(selectedPath);
                    PreviewFile(selectedPath);
                    string properties = $"Имя: {fileInfo.Name}\n" +
                                        $"Полный путь: {fileInfo.FullName}\n" +
                                        $"Размер: {fileInfo.Length} байт\n" +
                                        $"Дата создания: {fileInfo.CreationTime}\n" +
                                        $"Дата последнего доступа: {fileInfo.LastAccessTime}\n" +
                                        $"Дата последнего изменения: {fileInfo.LastWriteTime}\n" +
                                        $"Атрибуты: {fileInfo.Attributes}";

                    infoFile.Text = properties;
                }
                
            }            
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            infoFile.Text = "";
        }

        /// <summary>
        /// предварительный просмотр
        /// </summary>
        /// <param name="filePath"></param>
        private void PreviewFile(string filePath)
        {
            previewPanel.Children.Clear(); 

            string extension = Path.GetExtension(filePath).ToLower();

            if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp")
            {                
                Image image = new Image();
                image.Source = new BitmapImage(new Uri(filePath));
                image.Width = 450; 
                image.Height = 450;
                previewPanel.Children.Add(image);
            }
            else if (extension == ".txt")
            {
                TextBox textBox = new TextBox();
                textBox.Text = File.ReadAllText(filePath);
                textBox.IsReadOnly = true;
                previewPanel.Children.Add(textBox);
            }
            else
            {                
                TextBlock textBlock = new TextBlock();
                textBlock.Text = "Предварительный просмотр не поддерживается для этого типа файла.";
                previewPanel.Children.Add(textBlock);
            }
        }
    }
}
