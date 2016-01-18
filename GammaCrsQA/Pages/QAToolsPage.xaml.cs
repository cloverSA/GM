using System.Windows;
using System.Windows.Controls;
using GammaCrsQA.WcfFacecade;
namespace GammaCrsQA.Pages
{
    /// <summary>
    /// Interaction logic for QAToolsPage.xaml
    /// </summary>
    public partial class QAToolsPage : Page
    {
        public QAToolsPage()
        {
            InitializeComponent();
            
        }

        private void CollectLogBtn_Click(object sender, RoutedEventArgs e)
        {
            bool collect_dump = (MessageBox.Show("Collect system dump?", "Get Log", MessageBoxButton.YesNo) == MessageBoxResult.Yes) ? true : false;
            QAToolsFacade.CollectLog(collect_dump);
        }

        private void RmLogBtn_Click(object sender, RoutedEventArgs e)
        {
            QAToolsFacade.RmLog();
        }

    }
}
