using System.Windows.Controls;

namespace DataGatheringSoft.Views
{
    /// <summary>
    /// Interaction logic for RepetitionsView.xaml
    /// </summary>
    public partial class RepetitionsView : UserControl
    {
        public RepetitionsView()
        {
            InitializeComponent();
            this.DataContext = new RepetitionsViewModel();
        }
    }
}
