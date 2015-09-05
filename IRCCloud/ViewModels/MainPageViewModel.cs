using IRCCloudLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace IRCCloud.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        public ObservableCollection<IRCCloudLibrary.Buffer> Buffers
        {
            get { return _buffers; }
            set
            {
                if (_buffers != value)
                {
                    _buffers = value;
                    NotifyPropertyChanged("Buffers");
                }
            }
        }
        private ObservableCollection<IRCCloudLibrary.Buffer> _buffers;

        public IRCCloudLibrary.Buffer CurrentBuffer
        {
            get { return _currentBuffer; }
            set
            {
                if (_currentBuffer != value)
                {
                    _currentBuffer = value;
                    NotifyPropertyChanged("CurrentBuffer");
                }
            }
        }
        private IRCCloudLibrary.Buffer _currentBuffer;

        private IRCCloudConnection _connection;

        public MainPageViewModel()
        {
            
        }

        public MainPageViewModel(IRCCloudConnection connection)
        {
            Buffers = new ObservableCollection<IRCCloudLibrary.Buffer>();

            _connection = connection;
            _connection.OnServersUpdate += connection_serversUpdated;
            connection_serversUpdated(this, EventArgs.Empty);
        }

        void connection_serversUpdated(object sender, EventArgs args)
        {
            Buffers.Clear();
            foreach (Server server in _connection.Servers.Values)
            {
                foreach (IRCCloudLibrary.Buffer buffer in server.SortedBuffers)
                {
                    Buffers.Add(buffer);
                }
            }
        }
    }
}
