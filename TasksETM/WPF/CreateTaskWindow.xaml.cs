using IssuingTasksETM.Interfaces;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TasksETM.Interfaces;
using TasksETM.Interfaces.ITasks;
using TasksETM.Models;
using TasksETM.Service;
using TasksETM.Service.Tasks;
using TasksETM.WPF;
using TasksETM.WPF.HelpingWindow;

namespace IssuingTasksETM.WPF
{
    /// <summary>
    /// Логика взаимодействия для CreateTaskWindow.xaml
    /// </summary>
    public partial class CreateTaskWindow : Window
    {
        private string ImagePath { get; set; }
        private readonly string _selectedProject;
        private readonly TaskWindow _taskWindow;
        private readonly IDatabaseConnection _dbConnection;
        private readonly ImageService _imageService;
        private readonly IDepartmentService _departmentService;
        private readonly IProjectService _projectService;
        private readonly IAuthService _authService;
        private readonly IFilterTasksService _filterTasksService;

        public static List<CheckBox> checkBoxes;

        private readonly HelpCreateTaskWindow _helpCreateTaskWindow;

        public static string loggedInUser = UserSession.Login;
        public CreateTaskWindow(string selectedProject, 
            IDatabaseConnection dbConnection, 
            IDepartmentService departmentService, 
            IProjectService projectService,
            IAuthService authService,
            IFilterTasksService filterTasksService
            )
        {
            InitializeComponent();
            _selectedProject = selectedProject;
            _filterTasksService = new FilterTasksService();
            _taskWindow = new TaskWindow(selectedProject, dbConnection, departmentService, projectService, authService, filterTasksService);
            _dbConnection = new DatabaseConnection();
            _departmentService = new DepartmentService();
            _projectService = new ProjectService();
            _authService = new AuthService(DatabaseConnection.connString);
            _imageService = new ImageService();
            FillComboBoxAsync();

            checkBoxes = new List<CheckBox> { CheckBoxAR, CheckBoxVK, CheckBoxOV, CheckBoxSS, CheckBoxES, CheckBoxGIP };
        }

        private async Task FillComboBoxAsync()
        {
            try
            {
                if (_departmentService == null)
                {
                    MessageBox.Show("Сервис отделов не инициализирован.");
                    return;
                }

                var departmentNames = await _departmentService.GetDepartmentNamesAsync();

                if (departmentNames == null || !departmentNames.Any())
                {
                    MessageBox.Show("Не удалось загрузить список отделов.");
                    return;
                }

                Dispatcher.Invoke(() =>
                {
                    FromDepartComboBox.Items.Clear();
                    foreach (var dep in departmentNames)
                    {
                        if (dep != UserSession.Login) continue;
                        else FromDepartComboBox.Items.Add(dep);
                    }
                    if (FromDepartComboBox.Items.Count > 0)
                    {
                        FromDepartComboBox.SelectedIndex = 0;
                    }
                    else
                    {
                        MessageBox.Show("FromDepartComboBox не заполнен: список отделов пуст.");
                    }
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
                        ImagePath = _imageService.SaveImageToTempFile(image);
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

        private async void CreateTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var fromDepart = FromDepartComboBox.SelectedItem?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(fromDepart))
            {
                MessageBox.Show("Пожалуйста, выберите отдел 'От кого'.");
                return;
            }

            if (string.IsNullOrEmpty(TaskDeadLineTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, укажите крайний срок выполнения.");
                return;
            }

            if (string.IsNullOrEmpty(TaskDescriptionTextBox.Text) || string.IsNullOrEmpty(TaskViewTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните Вид и Описание.");
                return;
            }

            if (!DateTime.TryParse(TaskDeadLineTextBox.Text, out DateTime deadline))
            {
                MessageBox.Show("Некорректный формат даты. Введите дату в формате ДД.ММ.ГГГГ или ГГГГ-ММ-ДД.");
                return;
            }

            if (deadline < DateTime.Now.Date)
            {
                MessageBox.Show("Дата дедлайна не может быть раньше текущей.");
                return;
            }

            var selectedToDeparts = checkBoxes
                .Where(cb => cb.IsChecked == true)
                .Select(cb => cb.Content.ToString())
                .ToList();

            if (selectedToDeparts.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите хотя бы один чекбокс 'Кому'.");
                return;
            }

            var imageBytes = _imageService.ConvertImageToBytes(ImagePath);
            var taskDate = DateTime.Now.ToString("d");

            var createTaskService = new CreateTasksService(_selectedProject);

            foreach (var section in checkBoxes.Where(cb => cb.IsChecked == true))
            {
                var taskModel = new TaskModel
                {
                    FromDepart = fromDepart,
                    ToDepart = section.Tag.ToString(),
                    TaskDescription = TaskDescriptionTextBox.Text,
                    TaskView = TaskViewTextBox.Text,
                    ScreenshotPath = imageBytes,
                    TaskDate = taskDate,
                    TaskDeadline = TaskDeadLineTextBox.Text
                };

                await createTaskService.CreateTaskAsync(taskModel, section);
            }
            

            var taskCreatedSuccessful = new TaskCreatSuccessfulWindow();
            taskCreatedSuccessful.Show();
            await taskCreatedSuccessful.UpdateProgressBarAsync();

            _taskWindow.Show();
            Close();
        }


     

        private void HelpCreateTaskWindow_Click(object sender, RoutedEventArgs e)
        {
            HelpCreateTaskWindow helpCreateTaskWindow = new HelpCreateTaskWindow(_selectedProject, _dbConnection, _departmentService, _projectService, _authService, _filterTasksService);
            helpCreateTaskWindow.Show();
            this.Close();
        }


        private void ToPrevWindow_Click(object sender, RoutedEventArgs e)
        {
            _taskWindow.Show();
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
