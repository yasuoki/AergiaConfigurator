using System.Diagnostics;

namespace AergiaConfigurator;


internal class Uploader
{
	internal static async Task Uoload(AergiaDevice targetDevice, AergiaConfig configData)
	{
		var generator = new Generator();
		generator.Generate(configData);

		var byteCode = generator.Bytecode;
		var iconList = generator.Icons;
		var size = 0;
		await targetDevice.Begin();
		int i = 0;
		foreach (var icon in iconList)
		{
			size += icon.Bitmap.Length;
			Debug.WriteLine($"Icon icon{i + 1} {icon.Width}x{icon.Height} {icon.Format}");
			await targetDevice.Upload($"icon{i + 1}", icon.Bitmap);
			i++;
		}
		size += byteCode.Length;
		await targetDevice.Upload("config", byteCode);
		await targetDevice.Complete();
		
		Debug.WriteLine($"Upload {size} bytes, {size/1024} kbytes");
	}

}