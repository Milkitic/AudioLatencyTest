using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AudioLatencyTest.Models;
using AudioLatencyTest.Properties;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using Newtonsoft.Json;

namespace AudioLatencyTest
{
    public class MainWindowVm : INotifyPropertyChanged
    {
        private ObservableCollection<DeviceInfo> _deviceInfos;


        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<DeviceInfo> DeviceInfos
        {
            get => _deviceInfos;
            set
            {
                if (Equals(value, _deviceInfos)) return;
                _deviceInfos = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowVm _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = (MainWindowVm)DataContext;
            Output.OnLogging += (str) => TbLogging.AppendText(str + Environment.NewLine);
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var sb = new List<DeviceInfo>();
            for (int n = -1; n < WaveOut.DeviceCount; n++)
            {
                try
                {
                    var caps = WaveOut.GetCapabilities(n);
                    Output.WriteLine(JsonConvert.SerializeObject(new
                    {
                        Index = n,
                        caps.ProductName
                    }));

                    sb.Add(new WaveOutInfo(n, caps.ProductName));
                }
                catch (Exception ex)
                {
                    Output.WriteLine(ex.Message);
                }
            }

            Output.WriteLine();
            foreach (var dev in DirectSoundOut.Devices)
            {
                try
                {
                    Output.WriteLine(JsonConvert.SerializeObject(new
                    {
                        dev.Guid,
                        dev.ModuleName,
                        dev.Description
                    }));

                    sb.Add(new DirectSoundOutInfo(dev.Guid, dev.Description));
                }
                catch (Exception ex)
                {
                    Output.WriteLine(ex.Message);
                }
            }

            Output.WriteLine();
            var enumerator = new MMDeviceEnumerator();
            foreach (var wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.All))
            {
                try
                {
                    if (wasapi.DataFlow != DataFlow.Render || wasapi.State != DeviceState.Active) continue;
                    Output.WriteLine(JsonConvert.SerializeObject(new
                    {
                        wasapi.DataFlow,
                        wasapi.FriendlyName,
                        wasapi.DeviceFriendlyName,
                        wasapi.State
                    }));

                    sb.Add(new WasapiInfo(wasapi, wasapi.FriendlyName));
                }
                catch (Exception ex)
                {
                    Output.WriteLine(ex.Message);
                }
            }

            Output.WriteLine();
            foreach (var asio in AsioOut.GetDriverNames())
            {
                try
                {
                    Output.WriteLine(JsonConvert.SerializeObject(new
                    {
                        Name = asio
                    }));

                    sb.Add(new AsioOutInfo(asio));
                }
                catch (Exception ex)
                {
                    Output.WriteLine(ex.Message);
                }
            }

            _viewModel.DeviceInfos = new ObservableCollection<DeviceInfo>(sb);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var t = new TestWindow((DeviceInfo)DeviceInfoCombo.SelectedItem, (int)LatencySlider.Value,
                CheckExclusive.IsChecked);
            t.ShowDialog();
        }

        private void DeviceInfoCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newVal = (DeviceInfo)e.AddedItems[0];
            LatencySlider.IsEnabled = newVal.OutputMethod != OutputMethod.Asio;
            CheckExclusive.Visibility =
                newVal.OutputMethod == OutputMethod.Wasapi ? Visibility.Visible : Visibility.Hidden;
        }

        private void BtnGayhub_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(@"https://github.com/Milkitic/AudioLatencyTest");
        }
    }
}