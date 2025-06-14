using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AergiaConfigurator;

internal class AergiaDeviceException : AergiaException
{
    internal AergiaDeviceException(Location location,string message) : base(location, message)
    {
    }
}

internal class AergiaDeviceLocator : Location
{
    public string FilePath { get; set; }
    public string ObjectPath { get; set;  }
    public string ObjectName { get; set; }

    internal AergiaDeviceLocator(AergiaDevice device, string command="")
    {
        FilePath = device.Info.Port;
        ObjectPath = device.Info.Model;
        ObjectName = command;
    }
}

/// <summary>
/// Represents an Aergia device with its associated USB information and port functionality.
/// </summary>
internal class AergiaDevice
{
    internal UsbDeviceInfo Info { init; get; }
    internal UsbDevicePort Port { init; get; }

    private UInt16 crc16(string buff)
    {
        UInt16 result = 0;
        foreach (var c in buff)
        {
            result ^= c;
            for (var j = 0; j < 8; ++j)
            {
                if ((result & 0x01) != 0)
                    result = (ushort)((result >> 1) ^ 0xA001);
                else
                    result >>= 1;
            }
        }

        return result;
    }

    protected AergiaDevice(UsbDeviceInfo portInfo)
    {
        Info = portInfo;
        Port = new UsbDevicePort(portInfo.Port);
    }
    
    internal static async Task<UsbDeviceInfo?> DetectDevice(UsbDevicePort port)
    {
        port.OnNotify += (sender, args) =>
        {
            Console.WriteLine(args.Message);
        };
        var model = "";
        var version = "";
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            await port.ConnectAsync();
            await Task.Delay(TimeSpan.FromSeconds(1));
            Debug.WriteLine("Send Enter");
            var ret = await port.SendReceiveAsync("\r\n");
            Debug.WriteLine($"receive response '{ret.Message}'");
            var segs = ret.Message.Split('/');
            if (segs.Length == 3)
            {
                var _maker = segs[0].Trim();
                var _model = segs[1].Trim();
                var _ver = segs[2].Trim();
                if (_maker == "Yonabe Factory")
                {
                    model = _model;
                    if (_ver.StartsWith("VERSION"))
                    {
                        version = _ver.Substring(8).Trim();
                    }
                }
            }
            if (string.IsNullOrEmpty(version))
                return null;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            return null;
        }
        finally
        {
            port.Disconnect();
        }
        return new UsbDeviceInfo()
        {
            Port = port.PortAddress,
            Model = model,
            Version = version
        };
    }

    internal static async Task<AergiaDevice>  Connect(UsbDeviceInfo portInfo)
    {
        var device = new AergiaDevice(portInfo);
        
        await device.Port.ConnectAsync();
        await Task.Delay(TimeSpan.FromMilliseconds(500));
        Debug.WriteLine("connect");
        var ret = await device.Port.SendReceiveAsync("connect\r\n");
        Debug.WriteLine($"receive response '{ret.Type.ToString()} {ret.Code} {ret.Message}'");
        if (ret.Code != ReceiveMessage.ResultCode.Ok)
        {
            device.Port.Disconnect();
            throw new AergiaDeviceException(new AergiaDeviceLocator(device, "connect"), ret.Message);
        }
        return device;
    }

    internal void Disconnect()
    {
        Port.Disconnect();
    }

    internal async Task Begin()
    {
        Debug.WriteLine("begin");
        var ret = await Port.SendReceiveAsync("begin\r\n");
        Debug.WriteLine($"receive response '{ret.Type.ToString()} {ret.Code} {ret.Message}'");
        if (ret.Code != ReceiveMessage.ResultCode.Ok)
        {
            Port.Disconnect();
            throw new AergiaDeviceException(new AergiaDeviceLocator(this, "begin"), ret.Message);
        }
    }

    internal async Task Complete()
    {
        Debug.WriteLine("complete");
        var ret = await Port.SendReceiveAsync("complete\r\n");
        Debug.WriteLine($"receive response '{ret.Type.ToString()} {ret.Code} {ret.Message}'");
        if (ret.Code != ReceiveMessage.ResultCode.Ok)
        {
            Port.Disconnect();
            throw new AergiaDeviceException(new AergiaDeviceLocator(this, "complete"), ret.Message);
        }
    }
    internal async Task Upload(string name, byte[] data)
    {
        Debug.WriteLine($"upload {name}/{data.Length}");
        var ret = await Port.SendReceiveAsync($"upload {name}/{data.Length}\r\n");
        Debug.WriteLine($"receive response '{ret.Type.ToString()} {ret.Code} {ret.Message}'");
        if (ret.Code != ReceiveMessage.ResultCode.Ok)
        {
            Port.Disconnect();
            throw new AergiaDeviceException(new AergiaDeviceLocator(this, "upload"), ret.Message);
        }
        Debug.WriteLine($"send data name={name} size={data.Length}");
        ret = await Port.SendReceiveAsync(data);
        Debug.WriteLine($"receive response '{ret.Type.ToString()} {ret.Code} {ret.Message}'");
        if (ret.Code != ReceiveMessage.ResultCode.Ok)
        {
            Port.Disconnect();
            throw new AergiaDeviceException(new AergiaDeviceLocator(this, "upload"), ret.Message);
        }
        
    }
}