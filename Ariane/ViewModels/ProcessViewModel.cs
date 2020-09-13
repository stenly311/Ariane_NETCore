using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Ariane.Common;
using Ariane.Common.Types;
using Ariane.Helpers;
using Ariane.Type.Configuration;
using Ariane.Types;
using Catel.MVVM;
using EasyNetQ;
using EasyNetQ.Topology;

namespace Ariane.ViewModels
{
    public class ProcessViewModel : ViewModelBase
    {
        private ManagementEventWatcher startWatch;
        private ManagementEventWatcher stopWatch;
        private string _displayName;

        private CancellationTokenSource _calculationCancellationTokenSource;
        private Task _calculationWorker;
        private int _rowFilterSelected = -1;
        private readonly int LoopingDelay = 15;
        IBus bus;
        IQueue queue;
        
        public ProcessViewModel(ProcessConfiguration configuration)
        {
            _displayName = configuration.DisplayName;
            FileName = configuration.ProcessFileName;
            RootFolderPath = configuration.RootPath;
            if (Enum.TryParse<LoggingTypeSourceEnum>(configuration.LoggingSourceType, out var val))
            {
                LoggingSourceType = val;                
            }
            else
                LoggingSourceType = LoggingTypeSourceEnum.Console;

            RabbitMQTopicName = configuration.RabbitMQTopicName;
            MeasureSettings = new ObservableCollection<MeasureSettingViewModel>(configuration.MeasureSettings.Select(x=>x.ToViewModel()));
            
            Init();
        }

        private void Init()
        {
            RestartCommand = new Command(Restart, () => InProgress);
            StartStopCommand = new Command(StartStop, ()=> ExecutableExists());
            AddStartCodeCommand = new Command<object>(AddStartCode);
            AddStopCodeCommand = new Command<object>(AddStopCode);

            // populate one setting in case of empty 
            if (!MeasureSettings.Any())
            {
                MeasureSettings.Add(new MeasureSettingViewModel("1"));
            }

            if(LoggingSourceType == LoggingTypeSourceEnum.Console)
                RegisterProcessWatcher();
        }

        private void StartStop()
        {
            if (InProgress)
            {
                Stop();
            }
            else
            {
                Start();
            }
        }

        private void AddStartCode(object parameter)
        {
            var vm = parameter as OutputText;
            if (vm != null)
            {
                if (LastMeasureSetting == null || !String.IsNullOrEmpty(LastMeasureSetting.StartCode))
                {
                    MeasureSettings.Add(new MeasureSettingViewModel((MeasureSettings.Count() + 1).ToString()));
                    RaisePropertyChanged(() => LastMeasureSetting);
                }

                LastMeasureSetting.StartCode = vm.Text.Trim();
                LastMeasureSetting.StartVisibility = Visibility.Collapsed;
                LastMeasureSetting.StopVisibility = Visibility.Visible;
            }
        }

        private void AddStopCode(object parameter)
        {
            var vm = parameter as OutputText;
            if (vm != null)
            {
                LastMeasureSetting.StopCode = vm.Text.Trim();
                LastMeasureSetting.StopVisibility = Visibility.Collapsed;
                LastMeasureSetting.StartVisibility = Visibility.Visible;
            }
        }

        private void Restart()
        {
            Stop();

            Thread.SpinWait(1000);
            ClearMeasuredData();

            Start();
        }

        private void ClearMeasuredData()
        {
            // clean up measured data
            foreach (var set in MeasureSettings)
            {
                set.MeasuredUnits.Clear();
            }

            IsMeasurementOverLimit = null;
        }

        public string FileName { get; private set; }

        public LoggingTypeSourceEnum LoggingSourceType { get; set; }

        public int RowFilterSelected
        {
            get
            {
                return _rowFilterSelected;
            }
            set
            {
                if(_rowFilterSelected != value)
                {
                    _rowFilterSelected = value;

                    FilterConsoleOutput(value < 0 ? null : (int?)value);

                    RaisePropertyChanged(() => RowFilterSelected);                    
                }
            }
        }

        private void FilterConsoleOutput(int? value)
        {
            if(value == null)
            {
                ConsoleOutputStream.ToList().ForEach(x => x.Visibility = Visibility.Visible);
            }
            else
            {           
                var en = (TextTypeEnum)value;
                foreach (OutputText item in ConsoleOutputStream.ToList())
                {
                    SetOutputTextVisibility(en, item);
                }
            }
        }

        private void SetOutputTextVisibility(TextTypeEnum en, OutputText item)
        {
            if (item.TextType == en)
                item.Visibility = Visibility.Visible;
            else
                item.Visibility = Visibility.Collapsed;
        }

        public string DisplayName
        {
            get
            {
                if (!String.IsNullOrEmpty(_displayName))
                {
                    return _displayName;
                }
                return FileName;
            }
            set { _displayName = value; }
        }

        public string StartingArguments { get; private set; }
        
        public string FilePath
        {
            get
            {
                if(!String.IsNullOrEmpty(RootFolderPath) && !String.IsNullOrEmpty(FileName))
                    return Path.Combine(RootFolderPath, FileName);
                return RootFolderPath;
            }
        }

        public string RootFolderPath { get; }

        public MeasureSettingViewModel LastMeasureSetting
        {
            get { return MeasureSettings != null && MeasureSettings.Any() ? MeasureSettings.Last() : null; }
        }

        public ObservableCollection<MeasureSettingViewModel> MeasureSettings { get; private set; } 

        public ObservableCollection<MeasuredUnitViewModel> MeasuredData { get; } = new ObservableCollection<MeasuredUnitViewModel>();

        public System.Diagnostics.Process ConsoleProcess { get; private set; }

        public Command RestartCommand { get; private set; }

        public Command StartStopCommand { get; private set; }
        public string StartStopContentButton
        {
            get
            {
                return InProgress ? "Stop" :  LoggingSourceType == LoggingTypeSourceEnum.Console ? "Start" : "Connect";
            }
        }

        public Command<object> AddStartCodeCommand { get; private set; }
        public Command<object> AddStopCodeCommand { get; private set; }
        
        public bool InProgress { get; private set; } = false;
        public Visibility RestartButtonVisibility
        {
            get
            {
                return InProgress ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool IsCountingNotVisitedOutputLinesOn { get; set; } = true;

        public string CountOfNotVisitedOutputLines { get; set; }

        public ObservableCollection<OutputText> ConsoleOutputStream { get; set; } = new ObservableCollection<OutputText>();
        
        public bool? IsMeasurementOverLimit { get; set; }
        public string RabbitMQTopicName { get; private set; }

        public void Start()
        {
            switch (LoggingSourceType)
            {
                case LoggingTypeSourceEnum.Console:
                    if (ConsoleProcess != null)
                    {
                        if (ConsoleProcess.HasExited)
                            ConsoleProcess.Start();

                        return;
                    }
                    else
                    {
                        if (File.Exists(FilePath))
                        {
                            ConsoleOutputStream = new ObservableCollection<OutputText>();

                            var startInfo = new ProcessStartInfo(FilePath)
                            {
                                WorkingDirectory = RootFolderPath,
                                Arguments = StartingArguments,
                                RedirectStandardOutput = true,
                                UseShellExecute = false,
                                CreateNoWindow = true
                            };
                            ConsoleProcess = new System.Diagnostics.Process { StartInfo = startInfo };

                            ConsoleProcess.EnableRaisingEvents = true;

                            var setts = MeasureSettings.Where(x => x.IsReadyToUse && x.IsActive);

                            ConsoleProcess.OutputDataReceived += (sender, args) =>
                            {
                                if (args.Data != null)
                                    App.Current.Dispatcher.Invoke((System.Action)delegate
                                    {
                                        var rowData = args.Data.Trim();

                                        CreateMeasureUnit(rowData, DateTime.Now);
                                        PublishMessageToConsole(rowData);

                                    });
                            };
                            ConsoleProcess.Exited += (sender, args) =>
                            {
                                InProgress = false;
                            };

                            ConsoleProcess.Start();
                            ConsoleProcess.BeginOutputReadLine();
                        }
                        else
                        {
                            Console.WriteLine("File not found.");
                            InProgress = false;
                        }
                    }

                    _calculationCancellationTokenSource = new CancellationTokenSource();
                    _calculationWorker = RepeatHelper.Interval(TimeSpan.FromSeconds(LoopingDelay), () => EvaluateUnitData(), _calculationCancellationTokenSource.Token);

                    break;
                case LoggingTypeSourceEnum.RabbitMQ:

                    var con = ConfigurationManager.AppSettings.Get(Constants.RabbitMQConnectionName);
                    
                    // todo fix this
                    //if (con == null)
                    //    con = Properties.Settings.Default.RabbitMQConnection;

                    var processMQId = $"{FileName}_{Guid.NewGuid()}";
                    bus = RabbitHutch.CreateBus(con);
                    var exchange = bus.Advanced.ExchangeDeclare("Ariane.MQ", ExchangeType.Topic, autoDelete: true);
                    
                    // stops disposes bus and kills queue
                    //queue = bus.Advanced.QueueDeclare(processMQId, autoDelete: true, perQueueMessageTtl: 10000);
                    queue = bus.Advanced.QueueDeclare(processMQId, autoDelete: true);
                    var q = bus.Advanced.Bind(exchange, queue, RabbitMQTopicName);
              
                    bus.Advanced.Consume<LogMessageBase>(queue, (x, a) => {
                        var log = x.Body;
                     
                        string rowData;
                        var m = $"{log.TimeStamp.ToString("HH:mm:ss")} -{log.Level}- {log.FormattedMessage}";

                        // apply aditional formatting
                        if (!String.IsNullOrEmpty(log.Message))
                            rowData = String.Format(log.Message, m);
                        else
                            rowData = m;

                        var timeStamp = log.TimeStamp;

                        CreateMeasureUnit(rowData, timeStamp);
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            PublishMessageToConsole(rowData);
                        });

                        //}, x => {
                        //x.WithTopic(RabbitMQTopicName);
                    });

                    InProgress = true;

                    break;

                default:
                    throw new Exception($"Logging source '{LoggingSourceType}' has not supported.");
            }
        }

        private int _messageCounter= 0;
        private int _maxMessages = 100;

        private void PublishMessageToConsole(string rowData)
        {
            DisposeOldConsoleMessages();
            
            ConsoleOutputStream.Insert(0, CreateOutputText(rowData));

            if (IsCountingNotVisitedOutputLinesOn)
            {
                if (_messageCounter > _maxMessages)
                {
                    CountOfNotVisitedOutputLines = $"New (>{_maxMessages})";
                }
                else
                {
                    _messageCounter++;
                    CountOfNotVisitedOutputLines = $"New ({_messageCounter})";
                }
            }
        }

        private void DisposeOldConsoleMessages()
        {
            // clean up
            if (ConsoleOutputStream.Count > 1100)
            {
                for (int i = 1000; i < ConsoleOutputStream.Count; i++)
                {
                    ConsoleOutputStream.RemoveAt(i);
                }
            }
        }

        public void ResetCounter()
        {
            if (_messageCounter != 0)
            {
                _messageCounter = 0;
                CountOfNotVisitedOutputLines = "";
            }
        }

        private void CreateMeasureUnit(string rowData, DateTime timeStamp)
        {
            // only if any settings are available
            var setts = MeasureSettings.Where(x => x.IsReadyToUse && x.IsActive);
            if (rowData != null && setts.Any())
            {
                MeasureSettingViewModel set = setts.FirstOrDefault(x => (x.UseContainsEvaluation ? rowData.Contains(x.StartCode) : x.StartCode == rowData) && x.MeasuredUnits.All(y => !y.IsRunning));
                if (set != null)
                {
                    string matchKey = null;
                    if (set.UseKeyLockedMatch)
                    {
                        matchKey = Regex.Match(rowData, String.Format(MeasureSettingViewModel.KeyLockedPatternFormat, MeasureSettingViewModel.KeyLockedPattern)).Value;
                    }
                    set.MeasuredUnits.Add(new MeasuredUnitViewModel(set, matchKey)
                    {
                        StartTime = timeStamp,
                        MeasureSetting = set
                    });
                }

                var setting = setts.FirstOrDefault(x =>
                   (x.UseContainsEvaluation ? rowData.Contains(x.StopCode) : x.StopCode == rowData) && x.MeasuredUnits.Any(y => y.IsRunning));
                if (setting != null)
                {
                    var unit = setting.MeasuredUnits.FirstOrDefault(x => x.IsRunning
                                                                        && (x.MeasureSetting.UseKeyLockedMatch ? Regex.IsMatch(rowData, String.Format(MeasureSettingViewModel.KeyLockedPatternFormat, $"({x.MatchKey})"), RegexOptions.IgnoreCase) : true));
                    if (unit != null)
                        unit.StopTime = timeStamp;
                }
            }
        }

        private OutputText CreateOutputText(string rowData)
        {
            var d = new OutputText(rowData);
            if(RowFilterSelected > 0)
                SetOutputTextVisibility((TextTypeEnum)RowFilterSelected, d);

            return d;
        }

        private void EvaluateUnitData()
        {
            var setts = MeasureSettings.Where(x => x.IsActive && x.IsReadyToUse).ToList();
            if (setts.All(x => x.IsMeasureOverLimit() == null))
                IsMeasurementOverLimit = null;
            else
            {
                IsMeasurementOverLimit = setts.Any(x => x.IsMeasureOverLimit() != null && x.IsMeasureOverLimit().Value);
            }
        }

        private void RegisterProcessWatcher()
        {
            startWatch = new ManagementEventWatcher(
                new WqlEventQuery($"SELECT * FROM Win32_ProcessStartTrace where ProcessName = '{FileName}'"));
            startWatch.EventArrived += new EventArrivedEventHandler(startProcessWatch_EventArrived);
            
            stopWatch = new ManagementEventWatcher(
                new WqlEventQuery($"SELECT * FROM Win32_ProcessStopTrace where ProcessName = '{FileName}'"));
            stopWatch.EventArrived += new EventArrivedEventHandler(stopProcessWatch_EventArrived);
        }

        private void stopProcessWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            InProgress = false;
        }

        private void startProcessWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            InProgress = true;
        }

        public void Stop()
        {
            switch (LoggingSourceType)
            {
                case LoggingTypeSourceEnum.Console:                  

                    _calculationCancellationTokenSource?.Cancel();
                    if (ConsoleProcess != null && !ConsoleProcess.HasExited)
                    {
                        ConsoleProcess.Kill();
                        ConsoleProcess = null;
                    }                    

                    break;
                case LoggingTypeSourceEnum.RabbitMQ:
                    
                    bus?.Dispose();

                    break;
                //default:
                    //throw new Exception("Not implemented.");
            }

            // delete not finished measurements
            MeasuredData.Where(x => x.IsRunning).ToList().ForEach(x => MeasuredData.Remove(x));

            InProgress = false;
        }

        private bool ExecutableExists()
        {
            return File.Exists(FilePath);
        }

        protected override Task InitializeAsync()
        {
            try
            {
                startWatch.Start();
                stopWatch.Start();                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return base.InitializeAsync();
        }

        protected override Task CloseAsync()
        {
            Stop();

            startWatch?.Stop();
            stopWatch?.Stop();

            return base.CloseAsync();
        }
    }    
}
