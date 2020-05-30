using Silgred.Win.Services;

namespace Silgred.Win.ViewModels
{
    public class HostNamePromptViewModel : BaseViewModel
    {
        private string _host;

        public HostNamePromptViewModel()
        {
            Current = this;
            Host = Config.GetConfig().Host;
        }

        public static HostNamePromptViewModel Current { get; private set; }

        public string Host
        {
            get => _host;
            set
            {
                _host = value;
                OnPropertyChanged(nameof(Host));
            }
        }
    }
}