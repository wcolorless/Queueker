using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Queueker.core.Client;
using Queueker.core.Protocol;
using Queueker.core.Settings;
using Queueker.core.Transport;
using QueuekerTuner.core;
using Timer = System.Timers.Timer;

namespace QueuekerTuner.ui.logic
{
    public class PrimeWindowVM : INotifyPropertyChanged
    {
        private Thread _primeWindowThread;
        private Timer _updateTimer;
        private Window _primeWindow;
        public ObservableCollection<QueueItemInfo> _AllResources = new ObservableCollection<QueueItemInfo>();
        public QueueItemInfo SelectedResource { get; set; }

        private double _sendingRate;
        private double _readingRate;

        public double SendingRate
        {
            get
            {
                return _sendingRate;
            }
            set
            {
                _sendingRate = value;
                NotifyPropertyChanged("SendingRate");
            }

        }
        public double ReadingRate
        {
            get
            {
                return _readingRate;
            }
            set
            {
                _readingRate = value;
                NotifyPropertyChanged("ReadingRate");
            }
        }

        private Command _PurgeQueueCommand;
        private Command _RemoveQueueCommand;
        private Command _AddNewQueueWindowOpenCommand;
        private Command _SendingRateTestCommand;
        private Command _ReadingRateTestCommand;


        private QueuekerClient _queuekerClient;

        public ObservableCollection<QueueItemInfo> AllResources => _AllResources;

        public Command PurgeQueueCommand
        {
            get
            {
                return _PurgeQueueCommand ??= new Command(async o =>
                {
                    if (SelectedResource != null)
                    {
                        var settings = new QueuekerSettings
                        {
                            Host = Credentials.Host,
                            Login = Credentials.Login,
                            Password = Credentials.Password
                        };
                        _queuekerClient = QueuekerClient.Create(TransportTypes.Http, settings);
                        await _queuekerClient.PurgeQueue(SelectedResource.Name);
                    }
                });
            }
        }

        public Command RemoveQueueCommand
        {
            get
            {
                return _RemoveQueueCommand ??= new Command(async o =>
                {
                    if (SelectedResource != null)
                    {
                        var settings = new QueuekerSettings
                        {
                            Host = Credentials.Host,
                            Login = Credentials.Login,
                            Password = Credentials.Password
                        };
                        _queuekerClient = QueuekerClient.Create(TransportTypes.Http, settings);
                        await _queuekerClient.RemoveQueue(SelectedResource.Name);
                    }
                });
            }
        }

        public Command AddNewQueueWindowOpenCommand
        {
            get
            {
                return _AddNewQueueWindowOpenCommand ??= new Command(async o =>
                {
                    var addNewQueueWindow = new AddNewQueueWindow() {Owner = _primeWindow };
                    addNewQueueWindow.ShowDialog();
                    if (addNewQueueWindow.DataContext is QueueItemInfo newQueue)
                    {
                        var settings = new QueuekerSettings
                        {
                            Host = Credentials.Host,
                            Login = Credentials.Login,
                            Password = Credentials.Password
                        };
                        _queuekerClient = QueuekerClient.Create(TransportTypes.Http, settings);
                        await _queuekerClient.AddNewQueue(newQueue.Name, newQueue.Limit);
                    }
                });
            }
        }

        public Command SendingRateTestCommand
        {
            get
            {
                return _SendingRateTestCommand ??= new Command(async o =>
                {

                    if (Credentials.Check())
                    {
                        var testLen = 500D;
                        var workTime = new Stopwatch();
                        var settings = new QueuekerSettings
                        {
                            Host = Credentials.Host,
                            Login = Credentials.Login,
                            Password = Credentials.Password
                        };
                        _queuekerClient = QueuekerClient.Create(TransportTypes.Http, settings);
                        workTime.Start();
                        for (var i = 0; i < testLen; i++)
                        {
                            await _queuekerClient.PublishObjectAsJson(
                                new TestItem() {Name = "123", Time = DateTime.MaxValue}, "default");
                        }
                        workTime.Stop();
                        var spendTime = workTime.ElapsedMilliseconds;
                        var spendTimeInSec = spendTime / 1000D;
                        var rate = SendingRate = testLen / spendTimeInSec;
                        SendingRate = Math.Round(rate, 2);
                    }
                });
            }
        }

        public Command ReadingRateTestCommand
        {
            get
            {
                return _ReadingRateTestCommand ??= new Command(async o =>
                {
                    if (Credentials.Check())
                    {
                        var testLen = 500D;
                        var workTime = new Stopwatch();
                        var settings = new QueuekerSettings
                        {
                            Host = Credentials.Host,
                            Login = Credentials.Login,
                            Password = Credentials.Password
                        };
                        _queuekerClient = QueuekerClient.Create(TransportTypes.Http, settings);
                        for (var i = 0; i < testLen; i++)
                        {
                            await _queuekerClient.PublishObjectAsJson(new TestItem() { Name = "123", Time = DateTime.MaxValue }, "default");
                        }

                        var readingObjectsList = new List<TestItem>();
                        workTime.Start();
                        for (var i = 0; i < testLen; i++)
                        {
                           var response = await _queuekerClient.GetObjectFromJson<TestItem>("default");
                           if(response.Complete) readingObjectsList.Add(response.Result);
                        }
                        workTime.Stop();
                        var spendTime = workTime.ElapsedMilliseconds;
                        var spendTimeInSec = spendTime / 1000D;
                        var rate = ReadingRate = testLen / spendTimeInSec;
                        ReadingRate = Math.Round(rate, 2);
                    }
                });
            }
        }


        public async Task ChangeLoginAndPasswordCommand(string newLogin, string newPass)
        {
            try
            {
                if (Credentials.Check())
                {
                    var settings = new QueuekerSettings
                    {
                        Host = Credentials.Host,
                        Login = Credentials.Login,
                        Password = Credentials.Password
                    };
                    _queuekerClient = QueuekerClient.Create(TransportTypes.Http, settings);
                    await _queuekerClient.ChangeLoginAndPassword(newLogin, newPass);
                    Credentials.Login = Credentials.NewLogin;
                    Credentials.Password = Credentials.Password;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"PrimeWindowVM ChangeLoginAndPasswordCommand Error: {e.Message}");
                MessageBox.Show("Change Login And Password Error!");
            }
        }

        private void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (Credentials.Check())
                {
                    _updateTimer.Stop();
                    var settings = new QueuekerSettings
                    {
                        Host = Credentials.Host,
                        Login = Credentials.Login,
                        Password = Credentials.Password
                    };
                    _queuekerClient = QueuekerClient.Create(TransportTypes.Http, settings);
                    var info = _queuekerClient.GetInfo().Result;

                    Dispatcher.FromThread(_primeWindowThread)?.Invoke(() =>
                    {
                        _AllResources.Clear();
                        foreach (var queue in info)
                        {
                            _AllResources.Add(queue);
                        }
                    });

                    _updateTimer.Start();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"PrimeWindowVM _updateTimer_Elapsed Error: {exception.Message}");
            }
        }

        public PrimeWindowVM(Window primeWindow, Thread primeWindowThread)
        {
            _updateTimer = new Timer(3000);
            _updateTimer.Elapsed += _updateTimer_Elapsed;
            _primeWindow = primeWindow;
            _primeWindowThread = primeWindowThread;
            _updateTimer.Start();
        }

        public static PrimeWindowVM Create(Window primeWindow, Thread primeWindowThread)
        {
            return new PrimeWindowVM(primeWindow, primeWindowThread);
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
