using System.Diagnostics;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace AergiaConfigurator;
public class DevicePortException : Exception
{
    public DevicePortException(string message) : base(message)
    {
    }
}
public class PortStatus
{
    public virtual string Status { get; }

    public bool IsConnecting { get; }
    public PortStatus( string status, bool isConnecting)
    {
        Status = status;
        IsConnecting = isConnecting;
    }
}

public class ReceiveMessage
{
    public enum MessageType
    {
        Response = 0,
        Notify = 1,
        Unknown = 2
    }
    
    public enum ResultCode
    {
        Ok = 0, 
        FileSystemError = 10, 
        UnknownCommand = 11,
        UnknownPrefix = 12, 
        BadCommandFormat = 13,
        BadCommandSequence =14,
        BadCrcFormat = 15,
        CrcUnmatch = 16,
        IntegerParseError = 17, 
        HexParseError = 18,
        DataError = 19,
        BadDeviceId = 20,
        BadOsMode = 21,
        BadDeviceMode = 22,
        BadMouseSpeed = 23,
        BadKeyIndex = 24,
        BadValue = 25,
        StorageError =30,
        TooLongCommand = 31,
        TooManyCommand = 32,
        DeviceError = 40,
        KeyboardIsDisabled = 41, 
        KeyboardNotConnected = 42,
        MouseIsDisabled = 43,
        MouseNotConnected = 44,
        BluetoothNotConnected = 45,
        PreferenceSaveError = 46,
        Error= 90
    }

    public MessageType Type;
    public ResultCode Code;
    public string Message;
    public string? Body;
    private static string _chunk = "";
    private static ReceiveMessage _chunkMessage = null;

    public ReceiveMessage(string line)
    {
        var src = line.Trim();
        if (src.StartsWith("[Result]"))
        {
            Type = MessageType.Response;
            src = src.Substring(8);
        } 
        else if (line.StartsWith("[Notify]"))
        {
            Type = MessageType.Notify;
            src = src.Substring(8);
        }
        else
        {
            Type = MessageType.Unknown;
            Message = line;
            return;
        }

        var mCode = Regex.Match(src, " +[0-9]* ");
        if (!mCode.Success)
            throw new DevicePortException("invalid result code");
        Code  = (ResultCode)Int16.Parse(mCode.Value);
        src = src.Substring(mCode.Index + mCode.Length).Trim();
        if(string.IsNullOrEmpty(src))
            throw new DevicePortException("invalid message");
        Message = src;
    }

    public static List<ReceiveMessage> ParseMessages(string data)
    {
        var rms = new List<ReceiveMessage>();
        var src = _chunk + data;
        _chunk = "";
        if (src.Length == 0)
            return rms;
        var endChar = src[src.Length - 1];
        var lines = src.Split("\r\n").ToList();
        if (!src.EndsWith('\n'))
        {
            _chunk = lines[lines.Count - 1];
            lines.RemoveAt(lines.Count - 1);
            if (lines.Count == 0)
                return rms;
        }

        var n = 0;
        while (n < lines.Count)
        {
            if (_chunkMessage == null)
            {
                var line = lines[n++];
                if (!string.IsNullOrEmpty(line))
                {
                    var m = new ReceiveMessage(line);
                    if (m.Message.EndsWith('+'))
                    {
                        _chunkMessage = m;
                    }
                    else
                    {
                        rms.Add(m);
                    }
                }
            }
            else
            {
                var line = lines[n++];
                if (line == "")
                {
                    rms.Add(_chunkMessage);
                    _chunkMessage = null;
                }
                else
                {
                    if (_chunkMessage.Body != null)
                    {
                        _chunkMessage.Body += '\n' + line;
                    }
                    else
                    {
                        _chunkMessage.Body = line;
                    }
                }
            }
        }
        return rms;
    } 
}

public class UsbDevicePort : IDisposable
{
    private string _portName;
    private string _status;
    private SerialPort? _port = null;
    public event EventHandler<ReceiveMessage> OnNotify;
    public event EventHandler<PortStatus> OnDisconnect;
    public event EventHandler<PortStatus> OnStatusChanged;
    public string PortAddress => _portName;
    public PortStatus PortStatus => new(_status, _port != null && _port.IsOpen);
    
    protected readonly AutoResetEvent _receiveEvent;
    protected ReceiveMessage? _receivedMessage;

    public UsbDevicePort(string portName)
    {
        _portName = portName;
        _status = "closed";
        _receiveEvent = new AutoResetEvent(false);
        
    }
    public void Dispose()
    {
        if (_port != null)
        {
            _port.Dispose();
            _port = null;
        }
    }

    public async Task<ReceiveMessage> SendReceiveAsync(string message)
    {
        await SendAsync(message);
        var task = Task.Run(() => _receiveEvent.WaitOne(3000));
        var rc = await task;
        if(!rc)
            throw new DevicePortException($"device was not response");
        if(_receivedMessage == null)
            throw new DevicePortException($"received message was null");
        return _receivedMessage;
    }    
    public async Task<ReceiveMessage> SendReceiveAsync(byte[] data)
    {
        await SendAsync(data);
        var task = Task.Run(() => _receiveEvent.WaitOne(30000));
        var rc = await task;
        if(!rc)
            throw new DevicePortException($"device was not response");
        if(_receivedMessage == null)
            throw new DevicePortException($"received message was null");
        return _receivedMessage;
    }
    
    protected void DataArrived(string data)
    {
        var messages = ReceiveMessage.ParseMessages(data);
        foreach (var message in messages)
        {
            if (message.Type != ReceiveMessage.MessageType.Notify)
            {
                _receivedMessage = message;
                _receiveEvent.Set();
            }
            else
            {
                OnNotify?.Invoke(this, message);
            }
        }
    }
    public async Task ConnectAsync()
    {
        Debug.WriteLine($"Serial {_portName} Try ConnectAsync");
        var port = new SerialPort();
        try
        {
            port.BaudRate = 115200;
            port.NewLine = "\r\n";
            port.Parity = Parity.None;
            port.RtsEnable = true;
            port.DtrEnable = true;
            port.DataBits = 8;
            port.StopBits = StopBits.One;
            port.Handshake = Handshake.None;
            port.PortName = _portName;
            port.Open();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Serial {_portName} Connect Error. {ex.Message}");
            port.Close();
            throw;
        }
        Debug.WriteLine($"Serial {_portName} Connect OK");
        _port = port;
        _status = "connected";
        _port.DataReceived += _onDataArrived;
        //Task.Run(_receive);
    }

    private void _onDataArrived(object sender, SerialDataReceivedEventArgs e)
    {
        SerialPort sp = (SerialPort)sender;
        string indata = sp.ReadExisting();
        if(!string.IsNullOrEmpty(indata))
            DataArrived(indata);
    }  

    public void Disconnect()
    {
        if (_port != null)
        {
            _port.Close();
            _port = null;
            _status = "closed";
        }
    }

    public async Task SendAsync(string message)
    {
        if (_port == null)
        {
            throw new DevicePortException("Device port not open");
        }
        _port.Write(message);
    }
    public async Task SendAsync(byte[] data)
    {
        if (_port == null)
        {
            throw new DevicePortException("Device port not open");
        }
        _port.Write(data, 0, data.Length);;
    }
}