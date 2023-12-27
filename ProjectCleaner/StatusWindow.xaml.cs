namespace SolutionCleaner
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    /// <summary>
    /// Interaction logic for StatusWindow.xaml
    /// </summary>
    public partial class StatusWindow : Window, IDisposable, INotifyPropertyChanged
    {
        private System.Threading.Timer? _timer;
        private bool _isFinished;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Creates a new <see cref="StatusWindow"/>.
        /// </summary>
        public StatusWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public bool IsFinished { get => _isFinished; private set => SetProperty(ref _isFinished, value); }

        public ObservableCollection<SolutionCleanStatus> SolutionStatuses { get; } = [];

        internal void SetSolutions(IEnumerable<Solution> solutions)
        {
            foreach (Solution s in solutions)
            {
                SolutionStatuses.Add(new SolutionCleanStatus(s.FilePath, s.SolutionName));
            }

            _timer = new Timer(StartCleaning, null, TimeSpan.FromMilliseconds(100), Timeout.InfiniteTimeSpan);
        }

        private void StartCleaning(object? state)
        {
            System.Diagnostics.Debug.WriteLine("Start cleaning");
            foreach (var solution in SolutionStatuses)
            {
                var cleaner = new Cleaner();
                cleaner.CleanSolution(solution.FilePath);
                solution.NumberOfDeletedFiles = cleaner.DeletedFiles;
                solution.NumberOfDeletedFolders = cleaner.DeletedFolders;
                System.Diagnostics.Debug.WriteLine($"Cleaned {solution.SolutionName} of {cleaner.DeletedFiles} files in {cleaner.DeletedFolders} folders");

                if (cleaner.ErrorMessages.Count > 0)
                {
                    MessageBox.Show($"Deleted {cleaner.DeletedFiles} files in {cleaner.DeletedFolders} folders.\nHowever: {string.Join("\n", cleaner.ErrorMessages)}", solution.SolutionName);
                }
            }
    
            IsFinished = true;
            _timer!.Dispose();
            _timer = null;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? property = null)
            where T: IEquatable<T>
        {
            if (field.Equals(value)) return false;

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

            return true;
        }

        void IDisposable.Dispose()
        {
            _timer?.Dispose();
            _timer = null;
            GC.SuppressFinalize(this);
        }
    }
}
