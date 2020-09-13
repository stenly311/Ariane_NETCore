using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Ariane.Type.Configuration;
using Ariane.Types;
using Ariane.ViewModels;
using Newtonsoft.Json;

namespace Ariane.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            var configuration  = new List<ProcessConfiguration>();
            
            if (!File.Exists(Globals.JsonFilePath))
            {
                MainWindowViewModel.CreateJsonFile();
            }

            configuration = CreateProcessConfigurations();
            VM = new MainWindowViewModel(configuration);
            //InitializeComponent();
        }

        private void btnBottomMenuShow_Click(object sender, RoutedEventArgs e)
        {
            ShowHideMenu("sbShowBottomMenu", btnBottomMenuHide, btnBottomMenuShow, pnlBottomMenu);
        }
        private void btnBottomMenuHide_Click(object sender, RoutedEventArgs e)
        {
            ShowHideMenu("sbHideBottomMenu", btnBottomMenuHide, btnBottomMenuShow, pnlBottomMenu);
        }

        private void ShowHideMenu(string sb, Button btnHide, Button btnShow, StackPanel pnl)
        {
            Storyboard storyBoard = Resources[sb] as Storyboard;
            storyBoard.Begin(pnl);

            if (sb.Contains("Show"))
            {
                btnHide.Visibility = Visibility.Visible;
                btnShow.Visibility = Visibility.Hidden;
            }
            else if (sb.Contains("Hide"))
            {
                btnHide.Visibility = Visibility.Hidden;
                btnShow.Visibility = Visibility.Visible;
            }
        }

        private List<ProcessConfiguration> CreateProcessConfigurations()
        {
            List<ProcessConfiguration> configuration = new List<ProcessConfiguration>();
            Task.Run(() =>
            {
                using (var reader = new StreamReader(Globals.JsonFilePath))
                {
                    var str = reader.ReadToEnd();
                    configuration = JsonConvert.DeserializeObject<ProcessConfiguration[]>(str)?.ToList();
                }
            }).Wait();

            return configuration;
        }

        public MainWindowViewModel VM { get; private set; }

        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    VM.CloseViewModelAsync(null);
        //    base.OnClosing(e);
        //}

        /// <summary>
        /// Use this handler for VM init process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            VM.InitializeViewModelAsync();
        }

        private void CopyCommand_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ListView lb = (ListView)(sender);
            var selected = new StringBuilder();
            foreach (var lbSelectedItem in lb.SelectedItems.Cast<OutputText>())
            {
                selected.AppendLine(lbSelectedItem.Text);
            }
            if (selected != null) Clipboard.SetText(selected.ToString());
        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var label = (Label)sender;
            var process = (ProcessViewModel)label.DataContext;
            if (!process.InProgress)
            {
                var window = new WatcherSettingsWindow(process);
                bool? shw = window.ShowDialog();
                if(window.ClosingOptionSelected == ClosingOptionEnum.SaveClose)
                {
                    VM.SaveToJsonFile();
                }
            }
        }

        private void MenuItem_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessesListBox.SelectedItem == null)
                return;

            var process = ProcessesListBox.SelectedItem as ProcessViewModel;
            if (process != null && !process.InProgress)
            {
                var window = new WatcherSettingsWindow(process);
                bool? shw = window.ShowDialog();
                if (window.ClosingOptionSelected == ClosingOptionEnum.SaveClose)
                {
                    VM.SaveToJsonFile();
                }
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NavigateToGitHub(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/stenly311/Ariane");
        }
    }
}
