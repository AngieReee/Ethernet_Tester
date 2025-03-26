using System.ComponentModel;

namespace TestEthernet.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        /*UserControl uc = new CurrentNetworkView();*/

        public MainWindowViewModel()
        {
            /*var networkData = NetworkInterface.GetAllNetworkInterfaces();*/
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
