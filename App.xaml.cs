using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using Configurator.WebServer;

namespace AergiaConfigurator;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    public HttpServerOption _httpOption;
    public HttpServer _httpServer;
    public UsbWatcher _usbWatcher;
    public ObservableCollection<UsbDeviceInfo> Devices = new ObservableCollection<UsbDeviceInfo>();
    public App()
    {
        CultureInfo osCulture = CultureInfo.InstalledUICulture;
        Thread.CurrentThread.CurrentUICulture = osCulture;
        Thread.CurrentThread.CurrentCulture = osCulture;
        
        _httpOption = new HttpServerOption()
        {
            Port = 8800,
            StaticFilePath = "..\\httpRoot"
        };
        _httpServer = new HttpServer(_httpOption);
        _httpServer.Run();
        _usbWatcher = new UsbWatcher();
        _usbWatcher.Added += async (sender, s) =>
        {
            Console.WriteLine("usb device added " + s.Model);

            await Dispatcher.BeginInvoke(new Action(() =>
            {
                Devices.Add(s);
            }));
        };

        _usbWatcher.Removed += async (sender, s) =>
        {
            Console.WriteLine("usb device removed " + s.Model);
            await Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var dev in Devices)
                {
                    if (dev.Port == s.Port)
                    {
                        Devices.Remove(dev);
                        break;
                    }
                }
            }));
        };
        _usbWatcher.StartWatch();
    }
}