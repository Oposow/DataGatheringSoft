using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System.IO;
using System.Windows.Forms;
using DataGatheringSoft.Models;

namespace DataGatheringSoft.Views
{
    public class DisplayViewModel :BindableBase, IMainViewModel
    {
        public DisplayViewModel()
        {
            Files = new ObservableCollection<FileModel>();
            AllFiles = new List<FileModel>();

            DisplayFilesCommand = new DelegateCommand(DisplayFiles);
            ChooseDirectoryCommand = new DelegateCommand(ChooseDirectory);

            OptionsVM = new OptionsViewModel(this);
        }

        #region properties
        public List<FileModel> AllFiles { get; set; }
        public ObservableCollection<FileModel> Files { get; set; }

        public string _displayPath;
        public string DisplayPath
        {
            get { return _displayPath; }
            set { SetProperty(ref _displayPath,value); }
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
            if (String.IsNullOrWhiteSpace(_displayPath))
            {
                MessageBox.Show("Nie wybrano katalogu");
                return;
            }

            if (!Directory.Exists(_displayPath))
            {
                MessageBox.Show("Wybrany katalog nie istnieje");
                return;
            }
            AllFiles.Clear();
            ListFiles(_displayPath, "");
            FilterFiles();
        }
        #endregion

        public ICommand ChooseDirectoryCommand { get; private set; }
        private void ChooseDirectory()
        {
            var result = DialogsHelper.GetDirectory();
            if (result != String.Empty)
                DisplayPath = result;
        }

        private void ListFiles(string directory, string relativePath)
        {
            var directories = Directory.EnumerateDirectories(directory);
            foreach (var dir in directories)
                ListFiles(dir, Path.Combine(relativePath, Path.GetFileName(dir)));

            var fileNames = Directory.EnumerateFiles(directory);

            foreach(var fileName in fileNames)
            {
                string fullName = Path.Combine(directory, fileName);
                AllFiles.Add(new FileModel(fullName, relativePath));
            }
        }

        public void FilterFiles()
        {
            Files.Clear();
            IEnumerable<FileModel> filtered = FilteringHelper.Filter(AllFiles, OptionsVM);

            foreach (var f in filtered)
                Files.Add(f);
        }

    }
}
