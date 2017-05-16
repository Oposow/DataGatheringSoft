using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System.Windows.Forms;
using System.Windows.Threading;
using System.IO;
using DataGatheringSoft.Models;

namespace DataGatheringSoft.Views
{
    public class CopyViewModel : BindableBase
    {
        public CopyViewModel()
        {
            OptionsVM = new OptionsViewModel(null);
            GetSourcePathCommand = new DelegateCommand(GetSourcePath);
            GetDestinationPathCommand = new DelegateCommand(GetDestinationPath);
            CopyCommand = new DelegateCommand(Copy);
        }

        #region commands
        public ICommand GetSourcePathCommand { get; set; }
        private void GetSourcePath()
        {
            var result = DialogsHelper.GetDirectory();
            if (result != String.Empty)
                SourcePath = result;
        }

        public ICommand GetDestinationPathCommand { get; set; }
        private void GetDestinationPath()
        {
            var result = DialogsHelper.GetDirectory();
            if (result != String.Empty)
                DestinationPath = result;
        }

        public ICommand CopyCommand { get; set; }
        private void Copy()
        {
            try
            {
                if (String.IsNullOrWhiteSpace(SourcePath))
                {
                    MessageBox.Show("Nie wybrano katalogu źródłowego");
                    return;
                }
                if (!Directory.Exists(SourcePath))
                {
                    MessageBox.Show("Wybrany katalog źródłowy nie istnieje");
                    return;
                }

                if (String.IsNullOrWhiteSpace(DestinationPath))
                {
                    MessageBox.Show("Nie wybrano katalogu docelowego");
                    return;
                }

                if(!Directory.Exists(DestinationPath))
                {
                    Directory.CreateDirectory(DestinationPath);
                }


                IsCopyingInProgress = true;
                CurrentFileName = "";
                ProgressValue = 0;
                FilesAmount = 0;
                ProgressBarIndeterminate = true;
                Task.Run(() =>
                {
                    try
                    {
                        if (ClearDestinationDirectory)
                        {
                            CurrentFileName = "Trwa czyszczenie katalogu docelowego";
                            foreach (var dir in Directory.EnumerateDirectories(DestinationPath))
                                Directory.Delete(dir);
                            foreach (var file in Directory.EnumerateFiles(DestinationPath))
                                File.Delete(file);
                        }

                        CurrentFileName = "Trwa analizowanie zawartości katalogu źródłowego";
                        var filesToCopy = GetFilesToCopy(SourcePath);
                        FilesAmount = filesToCopy.Count();
                        ProgressBarIndeterminate = false;
                        foreach (var file in filesToCopy)
                        {
                            ProgressValue++;
                            var sourceName = Path.ChangeExtension(
                                    Path.Combine(
                                        new string[] { SourcePath, file.Directory, file.Name }),
                                    file.Extension);
                            CurrentFileName = sourceName;
                            File.Copy(sourceName,
                                Path.ChangeExtension(
                                    Path.Combine(
                                        new string[] { DestinationPath, file.Directory, file.Name }),
                                    file.Extension),true);
                        }
                        MessageBox.Show("Kopiowanie zakończone pomyślnie!");
                    }
                    catch(Exception)
                    {
                        MessageBox.Show("Coś poszło nie tak. Upewnij się, że program ma dostęp do wybranych katalogów");
                    }
                    finally
                    {
                        IsCopyingInProgress = false;
                    }
                });
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region properties
        public string _sourcePath;
        public string SourcePath
        {
            get { return _sourcePath; }
            set { SetProperty(ref _sourcePath, value); }
        }

        private bool _clearDestinationDirectory;
        public bool ClearDestinationDirectory
        {
            get { return _clearDestinationDirectory; }
            set { SetProperty(ref _clearDestinationDirectory, value); }
        }

        public string _destinationPath;
        public string DestinationPath
        {
            get { return _destinationPath; }
            set { SetProperty(ref _destinationPath, value); }
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

        public bool _isCopyigInProgress;
        public bool IsCopyingInProgress
        {
            get { return _isCopyigInProgress; }
            set { SetProperty(ref _isCopyigInProgress, value); }
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

        private IEnumerable<FileModel> GetFilesToCopy(string source, string relativePath = "")
        {
            var result = new List<FileModel>();
            foreach (var dir in Directory.EnumerateDirectories(source))
                result.AddRange(GetFilesToCopy(dir, Path.Combine(relativePath, Path.GetFileName(dir))));

            IEnumerable<FileModel> filesToCopy = Directory.EnumerateFiles(source).Select((x) =>
            {
                var fullname = Path.Combine(source, x);
                return new FileModel(fullname, relativePath);
            });

            filesToCopy = FilteringHelper.Filter(filesToCopy, OptionsVM);

            if (filesToCopy.Any())
                Directory.CreateDirectory(Path.Combine(DestinationPath, relativePath));

            result.AddRange(filesToCopy);
            return result;
        }
    }
}
