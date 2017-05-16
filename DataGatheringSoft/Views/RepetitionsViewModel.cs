using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using DataGatheringSoft.Models;

namespace DataGatheringSoft.Views
{
    public class RepetitionsViewModel : BindableBase, IMainViewModel
    {
        const int ReadBlockSize = 512;

        public RepetitionsViewModel()
        {
            Files = new ObservableCollection<FileModel>();
            AllFiles = new List<FileModel>();

            DisplayFilesCommand = new DelegateCommand(DisplayFiles);
            GetFirstPathCommand = new DelegateCommand(GetFirstPath);
            GetSecondPathCommand = new DelegateCommand(GetSecondPath);

            OptionsVM = new OptionsViewModel(this);
        }

        #region properties
        public List<FileModel> AllFiles { get; set; }
        public ObservableCollection<FileModel> Files { get; set; }

        public string _firstPath;
        public string FirstPath
        {
            get { return _firstPath; }
            set { SetProperty(ref _firstPath, value); }
        }

        public string _secondPath;
        public string SecondPath
        {
            get { return _secondPath; }
            set { SetProperty(ref _secondPath, value); }
        }

        public string _currentFileName;
        public string CurrentFileName
        {
            get { return _currentFileName; }
            set { SetProperty(ref _currentFileName, value); }
        }

        public int _progressValue;
        public int ProgressValue
        {
            get { return _progressValue; }
            set { SetProperty(ref _progressValue, value); }
        }

        public int _filesAmount;
        public int FilesAmount
        {
            get { return _filesAmount; }
            set { SetProperty(ref _filesAmount, value); }
        }

        public bool _isComparingInProgress;
        public bool IsComparingInProgress
        {
            get { return _isComparingInProgress; }
            set { SetProperty(ref _isComparingInProgress, value); }
        }

        public bool _progressBarIndeterminate;
        public bool ProgressBarIndeterminate
        {
            get { return _progressBarIndeterminate; }
            set { SetProperty(ref _progressBarIndeterminate, value); }
        }

        public OptionsViewModel _optionsVM;
        public OptionsViewModel OptionsVM
        {
            get { return _optionsVM; }
            set { SetProperty(ref _optionsVM, value); }
        }

        #endregion

        #region commands
        public ICommand DisplayFilesCommand { get; private set; }
        private void DisplayFiles()
        {
            if (String.IsNullOrWhiteSpace(_firstPath) || String.IsNullOrWhiteSpace(_secondPath))
            {
                MessageBox.Show("Nie wybrano katalogu");
                return;
            }

            if (!Directory.Exists(_firstPath))
            {
                MessageBox.Show("Wybrany pierwszy katalog nie istnieje");
                return;
            }

            if (!Directory.Exists(_secondPath))
            {
                MessageBox.Show("Wybrany drugi katalog nie istnieje");
                return;
            }


            AllFiles.Clear();
            try
            {
                IsComparingInProgress = true;
                CurrentFileName = "";
                ProgressValue = 0;
                FilesAmount = 0;
                ProgressBarIndeterminate = true;
                Task.Run(() =>
                {
                    try
                    {
                        CurrentFileName = "Trwa analizowanie zawartości katalogu źródłowego";
                        var filesToCompare = GetFilesToCompare(FirstPath);
                        FilesAmount = filesToCompare.Count();
                        ProgressBarIndeterminate = false;
                        foreach (var file in filesToCompare)
                        {
                            FileStream firstFile = null;
                            FileStream secondFile = null;
                            try
                            {
                                ProgressValue++;
                                var firstPath = Path.ChangeExtension(
                                        Path.Combine(new string[] { FirstPath, file.Directory, file.Name }),
                                        file.Extension);
                                var secondPath = Path.ChangeExtension(
                                    Path.Combine(new string[] { SecondPath, file.Directory, file.Name }),
                                    file.Extension);
                                CurrentFileName = firstPath;

                                if (!File.Exists(secondPath))
                                    continue;

                                if (new FileInfo(secondPath).Length != file.Size)
                                    continue;

                                firstFile = File.Open(firstPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                                secondFile = File.Open(secondPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                                byte[] firstBuffer = new byte[ReadBlockSize];
                                byte[] secondBuffer = new byte[ReadBlockSize];
                                int bytesRead;
                                bool areSame = true;

                                do
                                {
                                    bytesRead = firstFile.Read(firstBuffer, 0, ReadBlockSize);
                                    secondFile.Read(secondBuffer, 0, ReadBlockSize);

                                    for (int i=0; i<bytesRead; i++)
                                        if (firstBuffer[i] != secondBuffer[i])
                                        {
                                            areSame = false;
                                            break;
                                        }
                                }
                                while (bytesRead == ReadBlockSize);

                                if (areSame)
                                    AllFiles.Add(file);
                            }
                            finally
                            {
                                if (firstFile != null)
                                    firstFile.Close();
                                if (secondFile != null)
                                    secondFile.Close();
                            }

                        }

                        FilterFiles();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Coś poszło nie tak. Upewnij się, że program ma dostęp do wybranych katalogów");
                    }
                    finally
                    {
                        IsComparingInProgress = false;
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        public ICommand GetFirstPathCommand { get; set; }
        private void GetFirstPath()
        {
            var result = DialogsHelper.GetDirectory();
            if (result != String.Empty)
                FirstPath = result;
        }

        public ICommand GetSecondPathCommand { get; set; }
        private void GetSecondPath()
        {
            var result = DialogsHelper.GetDirectory();
            if (result != String.Empty)
                SecondPath = result;
        }

        private void ListFiles(string directory, string relativePath)
        {
            var directories = Directory.EnumerateDirectories(directory);
            foreach (var dir in directories)
                ListFiles(dir, Path.Combine(relativePath, Path.GetFileName(dir)));

            var fileNames = Directory.EnumerateFiles(directory);

            foreach (var fileName in fileNames)
            {
                string fullName = Path.Combine(directory, fileName);
                AllFiles.Add(new FileModel(fullName, relativePath));
            }
        }

        public void FilterFiles()
        {
            IEnumerable<FileModel> filtered = FilteringHelper.Filter(AllFiles, OptionsVM);

            App.Current.Dispatcher.Invoke(() =>
            {
                Files.Clear();

                foreach (var f in filtered)
                    Files.Add(f);
            });
        }

        private IEnumerable<FileModel> GetFilesToCompare(string source, string relativePath = "")
        {
            var result = new List<FileModel>();
            foreach (var dir in Directory.EnumerateDirectories(source))
                result.AddRange(GetFilesToCompare(dir, Path.Combine(relativePath, Path.GetFileName(dir))));

            IEnumerable<FileModel> filesToCopy = Directory.EnumerateFiles(source).Select((x) =>
            {
                var fullname = Path.Combine(source, x);
                return new FileModel(fullname, relativePath);
            });

            result.AddRange(filesToCopy);
            return result;
        }
    }
}
