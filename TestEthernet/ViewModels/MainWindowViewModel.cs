using System.ComponentModel;

namespace TestEthernet.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {

        public MainWindowViewModel()
        {
            /*var networkData = NetworkInterface.GetAllNetworkInterfaces();*/
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
