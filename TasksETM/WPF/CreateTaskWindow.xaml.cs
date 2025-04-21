using IssuingTasksETM.Interfaces;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TasksETM.Interfaces;
using TasksETM.Models;
using TasksETM.Service;

namespace IssuingTasksETM.WPF
{
    /// <summary>
    /// Логика взаимодействия для CreateTaskWindow.xaml
    /// </summary>
    public partial class CreateTaskWindow : Window
    {
        private string ImagePath { get; set; }
        private readonly TaskWindow _taskWindow;
        private readonly IDatabaseConnection _dbConnection;
        private readonly ImageService _imageManager;
        private readonly IDepartmentService _departmentService;
        private readonly IProjectService _projectService;
        private readonly IAuthService _authService;

        public static string loggedInUser = UserSession.Login;
        public CreateTaskWindow(string selectedProject, 
            IDatabaseConnection dbConnection, 
            IDepartmentService departmentService, 
            IProjectService projectService,
            IAuthService authService)
        {
            InitializeComponent();
            _taskWindow = new TaskWindow(selectedProject, dbConnection, departmentService, projectService, authService);
            _dbConnection = new DatabaseConnection();
            _departmentService = new DepartmentService();
            _projectService = new ProjectService();
            _authService = new AuthService(DatabaseConnection.connString);
            _imageManager = new ImageService();
            FillComboBoxAsync();
        }

        private async Task FillComboBoxAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(UserSession.Login)) 
                {
                    MessageBox.Show("Логин не был инициализирован.");
                    return;
                }

                string loggedInUser = UserSession.Login;
                MessageBox.Show($"Имя пользователя: {loggedInUser}");

                Dispatcher.Invoke(() =>
                {
                    FromDepartComboBox.Items.Clear();
                    FromDepartComboBox.Items.Add(loggedInUser);
                    FromDepartComboBox.SelectedItem = loggedInUser;
                });

                if (_departmentService == null)
                {
                    MessageBox.Show("Сервис отделов не инициализирован.");
                    return;
                }

                var departmentNames = await _departmentService.GetDepartmentNamesAsync();

                Dispatcher.Invoke(() =>
                {
                    ToDepartComboBox.Items.Clear();
                    foreach (var dep in departmentNames)
                    {
                        ToDepartComboBox.Items.Add(dep);
                    }

                    if (ToDepartComboBox.Items.Count > 0)
                        ToDepartComboBox.SelectedIndex = 0;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Что-то пошло не так: {ex.Message}");
            }
        }



        private void TaskInfoShowButton_Click(object sender, RoutedEventArgs e)
        {
            var fadeOut = (Storyboard)FindResource("FadeOutAnimation");
            var fadeIn = (Storyboard)FindResource("FadeInAnimation");

            fadeOut.Completed += (s, args) =>
            {
                BaseInfoGrid.Visibility = Visibility.Collapsed;
                TasksInfoGrid.Visibility = Visibility.Visible;
                fadeIn.Begin(TasksInfoGrid);
            };

            fadeOut.Begin(BaseInfoGrid);
        }

        private void BaseInfoShowButton_Click(object sender, RoutedEventArgs e)
        {
            var fadeOut = (Storyboard)FindResource("FadeOutAnimation");
            var fadeIn = (Storyboard)FindResource("FadeInAnimation");

            fadeOut.Completed += (s, args) =>
            {
                TasksInfoGrid.Visibility = Visibility.Collapsed;
                BaseInfoGrid.Visibility = Visibility.Visible;
                fadeIn.Begin(BaseInfoGrid);
            };

            fadeOut.Begin(TasksInfoGrid);
        }

        public void MovingWin(object sender, EventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void PasteImageFromClipboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Clipboard.ContainsImage())
                {
                    var image = Clipboard.GetImage();
                    if (image != null)
                    {
                        ImagePath = _imageManager.SaveImageToTempFile(image);
                        ImageInfoTextBlock.Text = $"{System.IO.Path.GetFileName(ImagePath)} ({System.IO.Path.GetExtension(ImagePath)})";
                    }
                }
                else
                {
                    MessageBox.Show("Буфер обмена не содержит изображения.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при вставке изображения: {ex.Message}");
            }
        }

        private void RemoveImage_Click(object sender, RoutedEventArgs e)
        {
            if (ImagePath == null)
            {
                ImageInfoTextBlock.Text = null;
                MessageBox.Show("Изображение еще не добавлено");
            }
            else
            {
                ImagePath = null;
                ImageInfoTextBlock.Text = "Изображение удалено";
            }

        }

        private void CreateTaskButton_Click(object sender, RoutedEventArgs e) 
        {

            if (string.IsNullOrEmpty(TaskViewTextBox.Text) || string.IsNullOrEmpty(TaskDeadLineTextBox.Text))
            {
                MessageBox.Show("Просмотрите все пункты. Какие-то поля остались пустыми!");
            }


            try
            {

            }
            catch(Exception ex)
            {
                MessageBox.Show($"Словили ошибку при создании задания: {ex.Message}");
            }

        }

        private void ToPrevWindow_Click(object sender, RoutedEventArgs e)
        {
            _taskWindow.Show();
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
