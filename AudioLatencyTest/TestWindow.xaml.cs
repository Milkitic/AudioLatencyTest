using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AudioLatencyTest.Audio;
using AudioLatencyTest.Models;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace AudioLatencyTest
{
    /// <summary>
    /// TestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestWindow : Window
    {
        private IWavePlayer _device;
        private readonly int _latency;
        private readonly bool _isExclusive;
        private readonly DeviceInfo _deviceInfo;
        private AudioPlaybackEngine _engine;
        private readonly string _soundFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sound.wav");
        private CachedSound _cacheSound;

        public TestWindow(DeviceInfo device, int latency, bool? isChecked)
        {
            _deviceInfo = device;
            _latency = latency;
            _isExclusive = isChecked ?? false;
            InitializeComponent();
        }

        private void TestWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (_deviceInfo.OutputMethod)
                {
                    case OutputMethod.WaveOut:
                        var waveOut = (WaveOutInfo)_deviceInfo;
                        _device = new WaveOutEvent { DeviceNumber = waveOut.DeviceNumber, DesiredLatency = _latency };
                        break;
                    case OutputMethod.DirectSound:
                        var dsOut = (DirectSoundOutInfo)_deviceInfo;
                        _device = new DirectSoundOut(dsOut.DeviceGuid, _latency);
                        break;
                    case OutputMethod.Wasapi:
                        var wasapi = (WasapiInfo)_deviceInfo;
                        _device = new WasapiOut(wasapi.Device, _isExclusive ? AudioClientShareMode.Exclusive : AudioClientShareMode.Shared, true, _latency);
                        break;
                    case OutputMethod.Asio:
                        var asio = (AsioOutInfo)_deviceInfo;
                        _device = new AsioOut(asio.FriendlyName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (!File.Exists(_soundFile)) throw new FileNotFoundException("找不到文件", _soundFile);
                _engine = new AudioPlaybackEngine(_device);
                _cacheSound = _engine.GetOrCreateCacheSound(_soundFile);
            }
            catch (Exception ex)
            {
                Output.WriteLine(ex.Message);
                MessageBox.Show(ex.ToString());
                Close();
            }
        }

        private void TestWindow_OnClosed(object sender, EventArgs e)
        {
            try
            {
                _device?.Dispose();
            }
            catch (Exception ex)
            {
                Output.WriteLine(ex.Message);
                MessageBox.Show(ex.ToString());
            }
        }

        private void TestWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            _engine.PlaySound(_cacheSound);
            Rec.Fill = Brushes.Red;
        }

        private void TestWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            Rec.Fill = Brushes.White;
        }
    }
}
