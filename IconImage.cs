using System.Buffers.Binary;
using SkiaSharp;

namespace AergiaConfigurator;

/// <summary>
/// Represents an image specifically for icons with a particular pixel format and size.
/// </summary>
internal class IconImage
{
	private AergiaTypes.PixelFormat _format;
	private int _width;
	private int _height;
	private int _lineBytes;
	private byte[] _bitmap;
	
	internal AergiaTypes.PixelFormat Format => _format;
	internal int Width => _width;
	internal int Height => _height;

	internal byte[] Bitmap => _bitmap;
	
	internal IconImage(AergiaTypes.PixelFormat format, int width, int height)
	{
		_format = format;
		_width = width;
		_height = height;

		int lineBytes;
		int msize;
		if (width % 8 != 0)
			lineBytes = (width / 8 + 1);
		else
			lineBytes = (width / 8);
		msize = lineBytes * height;

		int size = 0;
		if (format == AergiaTypes.PixelFormat.Rgb888)
		{
			_lineBytes = width * 3;
			size += _lineBytes * height;
		}
		else if (format == AergiaTypes.PixelFormat.Rgb565)
		{
			_lineBytes = width * 2;
			size += _lineBytes * height;
		}
		else if (format == AergiaTypes.PixelFormat.BW1)
		{
			_lineBytes = lineBytes;
			size += msize;
		}
		else
		{
			throw new Exception("Unsupported format");
		}
		
		_bitmap = new byte[size + 6];
		BinaryPrimitives.WriteUInt16LittleEndian(_bitmap.AsSpan(0), (UInt16)(width));
		BinaryPrimitives.WriteUInt16LittleEndian(_bitmap.AsSpan(2), (UInt16)(height));
		BinaryPrimitives.WriteUInt16LittleEndian(_bitmap.AsSpan(4), (UInt16)(format));
	}

	internal void SetPixel(int x, int y, SKColor color)
	{
		var line = _lineBytes * y;
		int ptr;
		switch (_format)
		{
			case AergiaTypes.PixelFormat.Rgb888:
				ptr = 6 + line + x * 3;
				ColorMapper.Rgb888Fill(_bitmap.AsSpan(ptr,3), color);
				break;
			case AergiaTypes.PixelFormat.Rgb565:
				{
					ptr = 6 + line + x * 2;
					ColorMapper.Rgb565Fill(_bitmap.AsSpan(ptr, 2), color);
				}
				break;
			case AergiaTypes.PixelFormat.BW1:
				{
					ptr = 6 + line + x / 8;
					ColorMapper.BWFill(_bitmap.AsSpan(ptr,1), x % 8, color);
				}
				break;
		}
	}
}