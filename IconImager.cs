using SkiaSharp;

namespace AergiaConfigurator;

/// <summary>
/// Provides functionality to create resized and reformatted icon images from a bitmap.
/// </summary>
internal class IconImager
{
	private SKBitmap _bitmap;
	internal IconImager(SKBitmap bitmap)
	{
		_bitmap = bitmap;
	}

	private SKColor GetPixcel(float x, float y, float sz)
	{
		var xpn = Math.Truncate(x);
		var xs = 0.0;
		var xp = (int)xpn;
		if (xpn < x)
		{
			xs = xpn + 1 - x;
			xp++;
		}
		var xe = (x+sz) - Math.Truncate(x+sz);
		var xn = (int)Math.Truncate(sz - xs - xe);

		var ypn = Math.Truncate(y);
		var ys = 0.0;
		var yp = (int)ypn;
		if (ypn < y)
		{
			ys = ypn + 1 - y;
			yp++;
		}
		var ye = (y+sz) - Math.Truncate(y+sz);
		var yn = (int)Math.Truncate(sz - ys - ye);

		var r = 0.0;
		var g = 0.0;
		var b = 0.0;
		var s = 0.0;
		if (ys > 0)
		{
			if (xs > 0)
			{
				var c = _bitmap.GetPixel(xp - 1, yp - 1);
				var cs = xs * ys;
				r += c.Red * cs;
				g += c.Green * cs;
				b += c.Blue * cs;
				s += cs;
			}

			for (var xi = 0; xi < xn; xi++)
			{
				var c = _bitmap.GetPixel(xp + xi, yp - 1);
				r += c.Red * ys;
				g += c.Green * ys;
				b += c.Blue * ys;
				s += ys;
			}
			
			if (xe > 0)
			{
				var c = _bitmap.GetPixel(xp + xn + 1, yp - 1);
				var cs = xe * ys;
				r += c.Red * cs;
				g += c.Green * cs;
				b += c.Blue * cs;
				s += cs;
			}
		}

		for (int yi = 0; yi < yn; yi++)
		{
			if (xs > 0)
			{
				var c = _bitmap.GetPixel(xp - 1, yp + yi);
				r += c.Red * xs;
				g += c.Green * xs;
				b += c.Blue * xs;
				s += xs;
			}

			for (var xi = 0; xi < xn; xi++)
			{
				var c = _bitmap.GetPixel(xp + xi, yp + yi);
				r += c.Red;
				g += c.Green;
				b += c.Blue;
				s++;
			}

			if (xe > 0)
			{
				var c = _bitmap.GetPixel(xp + xn + 1, yp + yi);
				r += c.Red * xe;
				g += c.Green * xe;
				b += c.Blue * xe;
				s += xe;
			}
		}
		if (ye > 0)
		{
			if (xs > 0)
			{
				var c = _bitmap.GetPixel(xp - 1, yp + yn + 1);
				var cs = xs * ye;
				r += c.Red * cs;
				g += c.Green * cs;
				b += c.Blue * cs;
				s += cs;
			}

			for (var xi = 0; xi < xn; xi++)
			{
				var c = _bitmap.GetPixel(xp + xi, yp + yn + 1);
				r += c.Red * ye;
				g += c.Green * ye;
				b += c.Blue * ye; 
				s += ye;
			}
			
			if (xe > 0)
			{
				var c = _bitmap.GetPixel(xp + xn + 1, yp + yn + 1);
				var cs = xe * ye;
				r += c.Red * cs;
				g += c.Green * cs;
				b += c.Blue * cs;
				s += cs;
			}
		}
		r = r / s;
		g = g / s;
		b = b / s;
		if (r > 255) r = 255;
		if (g > 255) g = 255;
		if (b > 255) b = 255;
		return new SKColor((byte)r, (byte)g, (byte)b);
	}

	internal IconImage CreateIcon(int width, int height, string format)
	{
		return CreateIcon(width, height, (AergiaTypes.PixelFormat)Enum.Parse(typeof(AergiaTypes.PixelFormat), format, true));
	}
	internal IconImage CreateIcon(int width, int height, AergiaTypes.PixelFormat format)
	{
		IconImage bmp;
		if (width < _bitmap.Width || height < _bitmap.Height)
		{
			float hr = (float)_bitmap.Width / width;
			float vr = (float)_bitmap.Height / height;
			float r = Math.Max(hr, vr);
			var iconWidth = (int)(_bitmap.Width / r);
			var iconHeight = (int)(_bitmap.Height / r);
			bmp = new IconImage(format, iconWidth, iconHeight);
			for (int y = 0; y < iconHeight; y++)
			{
				for (int x = 0; x < iconWidth; x++)
				{
					bmp.SetPixel(x,y,GetPixcel(x * r, y * r, r));
				}
			}
		}
		else
		{
			var iconWidth = _bitmap.Width;
			var iconHeight = _bitmap.Height;
			bmp = new IconImage(format, iconWidth, iconHeight);
			for (int y = 0; y < iconHeight; y++)
			{
				for (int x = 0; x < iconWidth; x++)
				{
					bmp.SetPixel(x,y,_bitmap.GetPixel(x, y));
				}
			}
		}
		return bmp;
	}
}

