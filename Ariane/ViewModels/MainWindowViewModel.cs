using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ariane.Helpers;
using Ariane.Type.Configuration;
using Catel.Collections;
using Catel.Logging;
using Catel.MVVM;
using Newtonsoft.Json;

namespace Ariane.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public MainWindowViewModel(IList<ProcessConfiguration> conf)
        {
            RegisterProcesses(conf);
            Init();
        }

        private void SubscribeToEvents()
        {
            PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(SelectedProcess):

                        Processes.Where(x => x != SelectedProcess).ForEach(x => x.IsCountingNotVisitedOutputLinesOn = true);

                        SelectedProcess.IsCountingNotVisitedOutputLinesOn = false;
                        SelectedProcess.ResetCounter();
                        break;
                }                
            };
        }

        public void SaveToJsonFile()
        {
            using (var writer = new StreamWriter(Globals.JsonFilePath))
            {
                var conf = Processes.Select(x => x.ToProcessConfiguration()).ToArray();
                var str = JsonConvert.SerializeObject(conf, Formatting.Indented);
                writer.Write(str);
                writer.Flush();
            }
        }

        public static void CreateJsonFile()
        {
            Task.Factory.StartNew(() =>
            {
                using (var writer = new StreamWriter(Globals.JsonFilePath))
                {
                    var str = JsonConvert.SerializeObject(new ProcessConfiguration[]
                    {
                        new ProcessConfiguration()
                        {
                            DisplayName = "", 
                            Arguments = "",
                            ProcessFileName = "Ariane.TestProcess.exe",
                            RootPath = Environment.CurrentDirectory
                        }
                    });
                    writer.Write(str);
                    writer.Flush();
                }
            }).Wait();
        }

        private void Init()
        {
            SelectedProcess = Processes.FirstOrDefault();
            SelectedProcess.IsCountingNotVisitedOutputLinesOn = false;
            StartProcessesCommand = new Command(StartProcesses, () => Processes.Any(x => !x.InProgress));
            StopProcessesCommand = new Command(StopProcesses, () => Processes.Any(x => x.InProgress));            
        }        

        private void RegisterProcesses(IList<ProcessConfiguration> conf)
        {
            foreach (ProcessConfiguration processConfiguration in conf)
            {
                Processes.Add(new ProcessViewModel(processConfiguration));
            }

            SubscribeToProcessesEvent();
        }

        private void SubscribeToProcessesEvent()
        {
            Processes.ForEach(x => x.PropertyChanged += (sender, args) =>
            {
                var processViewModel = (ProcessViewModel)sender;
                if (args.PropertyName == "InProgress")
                {
                    //SelectedProcess = processViewModel;
                    RaisePropertyChanged(() => StopProcessesCommand);
                }
                if (IsSelectedProcessTogglingOn && args.PropertyName == "CountOfNotVisitedOutputLines" && SelectedProcess != processViewModel)
                {
                    SelectedProcess = processViewModel;
                }
            });
        }

        public static bool InProgress { get; private set; }

        public Command StartProcessesCommand { get; set; }
        public Command StopProcessesCommand { get; set; }

        private void StartProcesses()
        {
            Processes.ForEach(x => x.Start());
        }

        private void StopProcesses()
        {
            Processes.ForEach(x => x.Stop());
        }

        public ProcessViewModel SelectedProcess { get; set; }

        public ObservableCollection<ProcessViewModel> Processes { get; set; } = new ObservableCollection<ProcessViewModel>();

        public bool IsSelectedProcessTogglingOn { get; set; }

        protected override async Task InitializeAsync()
        {
            try
            {
                InProgress = true;

                // Subscribe to events here
                Processes.ForEach(x=>x.InitializeViewModelAsync());
                SubscribeToEvents();

                await base.InitializeAsync();
            }
            catch(Exception e)
            {
                Log.Error("Error while Main MVVM model getting initialized.",e);
            }
            finally
            {
                InProgress = false;
            }
        }

        protected override async Task CloseAsync()
        {
            try
            {
                InProgress = true;
                // Unsubscribe from events here
                
                Processes.ForEach(x=>x.CloseViewModelAsync(null));

                SaveToJsonFile();

                await base.CloseAsync();
            }
            finally
            {
                InProgress = false;
            }
        }
        
    }
}
