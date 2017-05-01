using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System.Windows.Forms;
using System.Windows.Threading;
using System.IO;

namespace DataGatheringSoft.Views
{
    public class OptionsViewModel : BindableBase
    {
        public OptionsViewModel(IMainViewModel iParentViewModel)
        {
            _parentViewModel = iParentViewModel;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.5);
            _timer.Tick += TimerTickHandler;
            ResetTimerCommand = new DelegateCommand(ResetTimer);
        }

        private void TimerTickHandler(object sender, EventArgs e)
        {
            _timer.Stop();
            if (_parentViewModel != null)
                _parentViewModel.FilterFiles();
        }

        #region Properties
        private IMainViewModel _parentViewModel;
        private DispatcherTimer _timer;

        private string _extensions;
        public string Extensions
        {
            get { return _extensions; }
            set { SetProperty(ref _extensions, value); _timer.Start(); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); _timer.Start(); }
        }

        private string _author;
        public string Author
        {
            get { return _author; }
            set { SetProperty(ref _author, value); _timer.Start(); }
        }

        private DateTime? _creationDateFrom;
        public DateTime? CreationDateFrom
        {
            get { return _creationDateFrom; }
            set { SetProperty(ref _creationDateFrom, value); TimerTickHandler(null, null); }
        }

        private DateTime? _creationDateTo;
        public DateTime? CreationDateTo
        {
            get { return _creationDateTo; }
            set { SetProperty(ref _creationDateTo, value); TimerTickHandler(null, null); }
        }

        private DateTime? _modificationDateFrom;
        public DateTime? ModificationDateFrom
        {
            get { return _modificationDateFrom; }
            set { SetProperty(ref _modificationDateFrom, value); TimerTickHandler(null, null); }
        }

        private DateTime? _modificationDateTo;
        public DateTime? ModificationDateTo
        {
            get { return _modificationDateTo; }
            set { SetProperty(ref _modificationDateTo, value); TimerTickHandler(null, null); }
        }

        private DateTime? _accessDateFrom;
        public DateTime? AccessDateFrom
        {
            get { return _accessDateFrom; }
            set { SetProperty(ref _accessDateFrom, value); TimerTickHandler(null, null); }
        }

        private DateTime? _accessDateTo;
        public DateTime? AccessDateTo
        {
            get { return _accessDateTo; }
            set { SetProperty(ref _accessDateTo, value); TimerTickHandler(null, null); }
        }

        private long? _sizeFrom;
        public string SizeFrom
        {
            get { return _sizeFrom.HasValue ? _sizeFrom.ToString() : ""; }
            set
            {
                long? size = String.IsNullOrEmpty(value) ? null : (long?)Int64.Parse(value);
                SetProperty(ref _sizeFrom, size);
                _timer.Start();
            }
        }

        private long? _sizeTo;
        public string SizeTo
        {
            get { return _sizeTo.HasValue ? _sizeTo.ToString() : ""; }
            set
            {
                long? size = String.IsNullOrEmpty(value) ? null : (long?)Int64.Parse(value);
                SetProperty(ref _sizeTo, size);
                _timer.Start();
            }
        }
        #endregion

        public ICommand ResetTimerCommand { get; set; }
        private void ResetTimer()
        {
            _timer.Start();
        }
    }
}
