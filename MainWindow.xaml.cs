using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;
using Microsoft.Win32;
using Path = System.IO.Path;

namespace AergiaConfigurator;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>

public enum LogLevel
{
    Info,
    Warning,
    Error
}

public class LogEntry
{
    public string Message { get; set; }
    public LogLevel Level { get; set; }
    public string Prefix
    {
        get
        {
            return Level switch
            {
                LogLevel.Error   => "ERROR: ",
                LogLevel.Warning => "WARNING: ",
                LogLevel.Info    => "INFO: "
            };
        }
        set
        {
            Prefix = value;
        }
    }
}

public partial class MainWindow : Window
{
    private AergiaDevice? _targetDevice;
    public ObservableCollection<LogEntry> Logs { get; set; } = new ObservableCollection<LogEntry>();

    public MainWindow()
    {
        InitializeComponent();
        CBDeviceList.ItemsSource = ((App)System.Windows.Application.Current).Devices;
        TBLog.ItemsSource = Logs;
        Log.Error += Error;
        Log.Warn += Warn;
        Log.Info += Info;
    }

    public void Error(Location location, string msg)
    {
        Logs.Add(new LogEntry { Message = $"{location.FilePath}: {location.ObjectPath} {msg}", Level = LogLevel.Error });
        Debug.Print($"ERRPR: {location.FilePath}: {location.ObjectPath} {msg}");
    }
    public void Warn(Location location, string msg)
    {
        Logs.Add(new LogEntry { Message = $"{location.FilePath}: {location.ObjectPath} {msg}", Level = LogLevel.Warning });
        Debug.Print($"WARNING: {location.FilePath}: {location.ObjectPath} {msg}");
    }
    public void Info(Location location, string msg)
    {
        Logs.Add(new LogEntry { Message = $"{location.FilePath}: {location.ObjectPath} {msg}", Level = LogLevel.Info });
        Debug.Print($"INFO: {location.FilePath}: {location.ObjectPath} {msg}");
    }
    private void OnFileSelectClick(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dlg = new OpenFileDialog();
        dlg.FileName = "Configuration File";
        dlg.DefaultExt = ".js";
        dlg.Filter = "JavaScript File (.js)|*.js";
        var result = dlg.ShowDialog();
        if (result == true)
        {
            string filename = dlg.FileName;
            TBFileName.Text = filename;
            /*
            var ic = new IconImager(filename);
            ic.CreateIcon(64,32, PixelFormat.Rgb565);
            */
        }
    }

    private void OnDeviceDetected(object sender, EventArgs e)
    {
        if (CBDeviceList.SelectionBoxItem == null && CBDeviceList.HasItems)
        {
            CBDeviceList.SelectedIndex = 0;
        }
    }

    private async void OnDeviceChanged(object sender, SelectionChangedEventArgs args)
    {
        try
        {
            if (_targetDevice != null)
            {
                _targetDevice.Disconnect();
                _targetDevice = null;
            }

            UsbDeviceInfo? targetDeviceInfo = (UsbDeviceInfo?)CBDeviceList.SelectedItem;
            if (targetDeviceInfo != null)
            {
                _targetDevice = await AergiaDevice.Connect(targetDeviceInfo);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
    
    private async void OnUpdateClick(object sender, RoutedEventArgs e)
    {
        var srcFile = TBFileName.Text;
        var helper = new ConfigHelper(srcFile);
        Logs.Clear();
        
        DocumentLoader.Default.DiscardCachedDocuments();
        var engine = new V8ScriptEngine(V8ScriptEngineFlags.EnableTaskPromiseConversion
                                        | V8ScriptEngineFlags.EnableDynamicModuleImports);
        engine.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableFileLoading;
        if (!string.IsNullOrEmpty(helper.basePath))
        {
            engine.DocumentSettings.SearchPath = helper.basePath;
        }
        engine.AddHostObject("helper", helper);
        App app = (App)System.Windows.Application.Current;

        try
        {
            Logs.Add(new LogEntry { Message = $"Loading {srcFile}", Level = LogLevel.Info });

            app._httpOption.StaticFilePath = helper.basePath;
            app._httpOption.IndexFile = Path.GetFileName(srcFile);
            Logs.Add(new LogEntry { Message = $"Execute JavaScript {srcFile}", Level = LogLevel.Info });

            engine.Script.configure = "";
            var uri = new Uri(srcFile);
            var code = 
                $"import {{ConfigData}} from \"./{Path.GetFileName(srcFile)}\";\n" +
                "configure = ConfigData;";
            engine.Execute(
                new DocumentInfo() { Category = ModuleCategory.Standard },
                code);
        }
        catch (ScriptEngineException ex)
        {
            var result = MessageBox.Show(ex.ErrorDetails, "DEBUG with Chrome Browser?",
                MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var startInfo = new ProcessStartInfo("chrome")
                {
                    UseShellExecute = true
                };
                startInfo.ArgumentList.Add("http://localhost:8800/index.html");
                startInfo.ArgumentList.Add("--auto-open-devtools-for-tabs");
                try
                {
                    Process.Start(startInfo);
                }
                catch (Win32Exception ex2)
                {
                    MessageBox.Show(ex2.Message);
                }
            }
            Logs.Add(new LogEntry { Message = ex.Message, Level = LogLevel.Error });
            return;
        }
        catch (Exception ex)
        {
            Logs.Add(new LogEntry { Message = ex.Message, Level = LogLevel.Error });
            MessageBox.Show(ex.Message);
            return;
        }

        AergiaConfig.helper = helper;
        try
        {
            var configData = AergiaConfig.ParseObject(new Locator(engine.Script.configure, "","ConfigData",
                helper.toRelativePath(srcFile)));
            if (configData != null)
            {
                        
                if (_targetDevice == null)
                {
                    MessageBox.Show("Select target device");
                    return;
                }
                
                if (_targetDevice.Info.Model != configData.device.model)
                {
                    MessageBox.Show($"The model of the selected target device '{_targetDevice.Info.Model}' does not match the model in the configuration file '{configData.device.model}'.");
                    return;
                }
                Logs.Add(new LogEntry { Message = $"Upload to '{_targetDevice.Info.Model}' device", Level = LogLevel.Info });
                await Uploader.Uoload(_targetDevice, configData);
                _targetDevice.Disconnect();
                Logs.Add(new LogEntry { Message = $"Complete", Level = LogLevel.Info });
            }
        }
        catch (Exception ex)
        {
            Logs.Add(new LogEntry { Message = ex.Message, Level = LogLevel.Error });
            Debug.Print(ex.Message);
            Debug.Print(ex.StackTrace);
        }
    }
}

