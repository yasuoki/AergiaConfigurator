using System.Buffers.Binary;
using SkiaSharp;

namespace AergiaConfigurator;

/// <summary>
/// Provides utility methods for converting and filling color data into buffers.
/// </summary>
internal static  class ColorMapper
{
    internal static void Rgb888Fill(Span<byte> buffer, SKColor color)
    {
        buffer[0] = color.Red;
        buffer[1] = color.Green;
        buffer[2] = color.Blue;
    }
    internal static void Rgb565Fill(Span<byte> buffer, SKColor color)
    {
        /*
        byte r = (byte)((uint)color.Red * 31.0 / 255.0);
        byte g = (byte)((uint)color.Green * 63.0 / 255.0);
        byte b = (byte)((uint)color.Blue * 31.0 / 255.0);
        buffer[0] = (byte)(((g & 0x07) << 5) | (b & 0x1f));
        buffer[1] = (byte)(((r & 0x1f) << 3) | ((g & 0x38) >> 3)); // RRRR RGGG GGGB BBBB
        */
        UInt16 rb = (UInt16)((color.Red>>3)&0x1f);
        UInt16 gb = (UInt16)((color.Green>>2)&0x3f);
        UInt16 bb = (UInt16)((color.Blue>>3)&0x1f);
        UInt16 c565 = (UInt16)((rb << 11) | (gb << 5) | bb);
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, c565);
    }
   
    internal static void BWFill(Span<byte> buffer, int bit, SKColor color)
    {
        float h, s, v;
        color.ToHsv(out h, out s, out v);
        if (v > 55)
        {
            byte d = (byte)(buffer[0] | (0x80 >> bit));
            buffer[0] = d;
        }
    }
    
    internal static UInt32 ColorData(AergiaTypes.PixelFormat pixelFormat, int color)
    {
        if (pixelFormat == AergiaTypes.PixelFormat.Rgb888)
        {
            return (UInt32)color;
        }

        if (pixelFormat == AergiaTypes.PixelFormat.Rgb565)
        {
            SKColor c = (SKColor)(UInt32)color;
            byte r = (byte)(c.Red * 31.0 / 255.0);
            byte g = (byte)(c.Green * 63.0 / 255.0);
            byte b = (byte)(c.Blue * 31.0 / 255.0);
            byte h = (byte)(((r & 0x1f) << 3) | ((g & 0x38) >> 3)); // RRRR RGGG GGGB BBBB
            byte l = (byte)(((g & 0x07) << 5) | (b & 0x1f));
            UInt32 ret = (UInt32)((h << 8)|l);
            return ret;
        }

        if (pixelFormat == AergiaTypes.PixelFormat.BW1)
        {
            SKColor c = (SKColor)(UInt32)color;
            float h, s, v;
            c.ToHsv(out h, out s, out v);
            if (v > 55)
                return 1;
            else
                return 0;
        }

        return (UInt32)color;
    }
    
}