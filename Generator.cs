/*
 * Image format
 *  +--------------------------------+
 *  | device mode                    |
 *  +--------------------------------+
 *  | application count              |
 *  | application [ptr]              |
 *  +--------------------------------+
 *  | application[0]                 |
 *  | visual ptr                     |
 *  | page count                     |
 *  | page [ptr]                     |
 *  +--------------------------------+
 *  | application[1]                 |
 *  | visual ptr                     |
 *  | page count                     |
 *  | page [ptr]                     |
 *  +--------------------------------+
 *              :
 *  +--------------------------------+
 *  | page[0]                        |
 *  | visual ptr                     |
 *  | bind count                     |
 *  | bind [ptr]                     |
 *  +--------------------------------+
 *  | page[1]                        |
 *  | visual ptr                     |
 *  | bind count                     |
 *  | bind [ptr]                     |
 *  +--------------------------------+
 *              :
 *  +--------------------------------+
 *  | bind[0]                        |
 *  | control id                     |
 *  | visual ptr                     |
 *  | event count                    |
 *  | event [ptr]                    |
 *  +--------------------------------+
 *  | bind[1]                        |
 *  | control id                     |
 *  | visual ptr                     |
 *  | event count                    |
 *  | event [ptr]                    |
 *  +--------------------------------+
 *              :
 *  +--------------------------------+
 *  | event[0]                       |
 *  | action count                   |
 *  | action [ptr]                   |
 *  +--------------------------------+
 *  | event[1]                       |
 *  | action count                   |
 *  | action [ptr]                   |
 *  +--------------------------------+
 *              :
 *  +--------------------------------+
 *  | action[0]                      |
 *  | command count                  |
 *  | command tbl [ptr]              |
 *  +--------------------------------+
 *  | action[1]                      |
 *  | command count                  |
 *  | command tbl [ptr]              |
 *  +--------------------------------+
 *              :
 *  +--------------------------------+
 *  | command tbl[0]                 |
 *  | command ptr                    |
 *  +--------------------------------+
 *  | command tbl[1]                 |
 *  | command ptr                    |
 *  +--------------------------------+
 *              :
 *  +--------------------------------+
 *  | command[0]                     |
 *  | command id                     |
 *  | command parameters             |
 *  |     :                          |
 *  | text ptr                       |
 *  | visual ptr                     |
 *  | action ptr                     |
 *  |     :                          |
 *  +--------------------------------+
 *  | command[1]                     |
 *  | command id                     |
 *  | command parameters             |
 *  |     :                          |
 *  +--------------------------------+
 *              :
 *  +--------------------------------+
 *  | visual[0]                      |
 *  | text ptr                       | 
 *  | color                          | 
 *  | font size                      | 
 *  | icon id                        | 
 *  | background color               | 
 *  +--------------------------------+
 *  | visual[1]                      |
 *  | text ptr                       | 
 *  | color                          | 
 *  | font size                      | 
 *  | icon id                        | 
 *  | background color               | 
 *  +--------------------------------+
 *              :
 *  +--------------------------------+
 *  | string[0]                      |
 *  |     :                          |
 *  +--------------------------------+
 *  | string[1]                      | 
 *  |     :                          |
 *  +--------------------------------+
 *              :
 */ 

using System.Buffers.Binary;
using System.IO;
using System.Text;
using System.Windows.Controls;
using SkiaSharp;

namespace AergiaConfigurator;

/// <summary>
/// The IconStore class manages a collection of bitmap images and icons for use within an application.
/// It provides functionalities to store and retrieve bitmap images and to create icons based on visual formats and capabilities.
/// </summary>
internal class IconStore
{
	private Dictionary<string, SKBitmap> _bitmapImages = new Dictionary<string, SKBitmap>();
	private OrderedDictionary<string,IconImage> _iconImages = new OrderedDictionary<string, IconImage>();

	internal List<IconImage> IconList => new(_iconImages.Values);

	internal bool HasBitmapImage(string key) => _bitmapImages.ContainsKey(key);
	internal void AddBitmapImage(string fileName, SKBitmap bitmap) => _bitmapImages.Add(fileName, bitmap);
	internal UInt16 CreateIcon(VisualFormat format, IconCapabilitiesFormat capabilities)
	{
		var fileName = format.icon;
		var key = $"{fileName}/{capabilities.horzResolution}x{capabilities.vertResolutiont}-{capabilities.pixelFormat.ToString()}";
		var index = _iconImages.IndexOf(key);
		if (index == -1)
		{
			SKBitmap bitmap;
			if (_bitmapImages.ContainsKey(fileName)) {
				bitmap = _bitmapImages[fileName];
			}
			else
			{
				bitmap = format.LoadImage();
				if (bitmap == null)
					return 0;
				_bitmapImages.Add(fileName, bitmap);
			}

			var imager = new IconImager(bitmap);
			var icon = imager.CreateIcon(capabilities.horzResolution, capabilities.vertResolutiont, capabilities.pixelFormat);
			_iconImages.Add(key, icon);
			index = _iconImages.IndexOf(key);
		}
		return (UInt16)(index + 1);
	}
}

/// <summary>
/// The Context class serves as the main data repository and context manager within the application.
/// It provides access to various sections of configuration fragments, application-related definitions,
/// and other components necessary for managing and organizing the operation of the system.
/// The class encapsulates data structures such as lists of fragments, configuration objects, and format relationships
/// while offering functionality to set and retrieve the primary context values.
/// </summary>
internal class Context
{
	internal IconStore IconStore = new IconStore();
	internal ConfigFragment ConfigSection;
	internal List<ApplicationFragment> ApplicationSection = new List<ApplicationFragment>();
	internal List<PageFragment> PageSection = new List<PageFragment>();
	internal List<ControlBindFragment> ControlBindSection = new List<ControlBindFragment>();
	internal Dictionary<string, ActionFragment> ActionSection = new Dictionary<string, ActionFragment>();
	internal List<ActionCommandFragment> ActionCommandSection = new List<ActionCommandFragment>();
	internal List<CommandFragment> CommandSection = new List<CommandFragment>();
	internal List<TextFragment> TextSection = new List<TextFragment>();
	internal List<UsageListFragment> UsageSection = new List<UsageListFragment>();
	internal List<VisualFragment> VisualSection = new List<VisualFragment>();
	
	private	AergiaConfig _aergiaConfig;
	private	ApplicationFormat _application;
	private	ControlFormat _control;
	private string _actionName;
	internal AergiaConfig AergiaConfig => _aergiaConfig;
	internal ApplicationFormat Application => _application;
	internal ControlFormat Control => _control;
	internal string ActionName => _actionName;
	
	internal void SetAergiaConfig(AergiaConfig config) => _aergiaConfig = config;
	internal void SetApplication(ApplicationFormat config) => _application = config;
	internal void SetControl(ControlFormat config) => _control = config;
	internal void SetActionName(string actionName) => _actionName = actionName;
}

/// <summary>
/// The FragmentBase class serves as a foundational abstraction for different types of fragments
/// used within the configuration system. It provides core functionality such as offset management
/// and utilities for interpreting variables in various contexts.
/// </summary>
internal class FragmentBase
{
	private int _offset;
	
	internal int Offset => _offset;

	internal static AergiaTypes.VariableId getVariableId(DeviceFormat device, string v)
	{
		var seg = v.Split('.');
		if (seg.Length == 2)
		{
			var controlId = Enum.Parse<AergiaTypes.ControlId>(seg[0]);
			if (!device.controls.ContainsKey(controlId))
			{
				throw new Exception($"Control '{seg[0]}' does not defined in the '{device.model}'");
			}
			var control = device.controls[controlId];
			if (control.variables != null && control.variables.Contains(seg[1]))
			{
				return Enum.Parse<AergiaTypes.VariableId>(seg[0] + "_" + seg[1]);
			}
			throw new Exception($"Variable '{seg[1]}' does not defined in the '{device.model}.{control}'");
		}
		return 0;
	}
	
	internal static Int16 ToReferenceOrInt16(DeviceFormat device, Variable v)
	{
		if (v.type == Variable.Type.IntType)
		{
			return (Int16)v.intValue;
		}

		if (v.type == Variable.Type.StringType && v.stringValue.StartsWith("$") && !v.stringValue.StartsWith("$$"))
		{
			var vid = getVariableId(device, v.stringValue[1..]);
			if(vid != 0) 
				return (Int16)vid;
		}
		return v.ToInt16();
	} 
	internal static UInt16 ToReferenceOrUInt16(DeviceFormat device, Variable v)
	{
		if (v.type == Variable.Type.StringType && v.stringValue.StartsWith("$") && !v.stringValue.StartsWith("$$"))
		{
			var vid = getVariableId(device, v.stringValue[1..]);
			if (vid != 0)
				return (UInt16)vid;
		}
		return v.ToUInt16();
	}
	internal static float ToReferenceOrFloat(DeviceFormat device, Variable v)
	{
		if (v.type == Variable.Type.StringType && v.stringValue.StartsWith("$") && !v.stringValue.StartsWith("$$"))
		{
			var vid = getVariableId(device, v.stringValue[1..]);
			if(vid != 0)
				return (float)vid;
		}
		return (float)v.ToDouble();
	}
	internal void SetOffset(int offset)
	{
		_offset = offset;
	}
	internal virtual int CodeSize()
	{
		 return 0;
	}
	internal virtual int WriteCode(Span<byte> buffer)
	{
		return 0;
	}
}

/// <summary>
/// Represents a fragment of text that can be used and managed within a textual structure or configuration context.
/// Provides functionalities to create, manage, and encode text fragments into a specific binary format.
/// </summary>
internal class TextFragment : FragmentBase
{
	private string _text;

	private TextFragment()
	{
		_text = "";
	}

	internal static TextFragment Create(Context store, string text)
	{
		var textFragment = store.TextSection.Find(t => t._text == text);
		if (textFragment == null)
		{
			textFragment = new TextFragment();
			textFragment._text = text;
			store.TextSection.Add(textFragment);
		}
		return textFragment;
	}
	
	public override string ToString() => _text;

	internal override int CodeSize()
	{
		return _text.Length + 1;
	}
	internal override int WriteCode(Span<byte> buffer)
	{
		Encoding.ASCII.GetBytes(_text, buffer);
		buffer[_text.Length] = 0;
		return _text.Length + 1;
	}
}

/// <summary>
/// The UsageListFragment class represents a specific portion of a usage list within the context of device configuration.
/// It is responsible for handling usage-related data, calculating code size, and writing byte code to a specified buffer.
/// </summary>
internal class UsageListFragment : FragmentBase
{
	class Usage
	{
		internal byte enModifier;
		internal byte enCode;
		internal byte lcModifier;
		internal byte lcCode;

		internal Usage(byte enModifier, byte enCode, byte lcModifier=0, byte lcCode=0) {
			this.enModifier = enModifier;
			this.enCode = enCode;
			this.lcModifier = lcModifier == 0  ? enModifier : lcModifier;
			this.lcCode = lcCode == 0 ? enCode : lcCode;
		}
	}
	private static readonly Dictionary<char, Usage> usageMap = new Dictionary<char, Usage>()
	{
		{ ' ', new Usage(0x00, 0x2c) },
		{ '!', new Usage(0xe1, 0x1e) },
		{ '"', new Usage(0xe1, 0x34) },
		{ '#', new Usage(0xe1, 0x20) },
		{ '$', new Usage(0xe1, 0x21) },
		{ '%', new Usage(0xe1, 0x22) },
		{ '&', new Usage(0xe1, 0x24) },
		{ '\'',new Usage(0x00, 0x34) },
		{ '(', new Usage(0xe1, 0x36) },
		{ ')', new Usage(0xe1, 0x27) },
		{ '*', new Usage(0xe1, 0x25, 0x1e, 0x34) },
		{ '+', new Usage(0xe1, 0x2e, 0x1e, 0x33) },
		{ ',', new Usage(0x00, 0x36) },
		{ '-', new Usage(0x00, 0x2d) },
		{ '.', new Usage(0x00, 0x37) },
		{ '/', new Usage(0x00, 0x38) },
		{ '0', new Usage(0x00, 0x27) },
		{ '1', new Usage(0x00, 0x1e) },
		{ '2', new Usage(0x00, 0x1f) },
		{ '3', new Usage(0x00, 0x20) },
		{ '4', new Usage(0x00, 0x21) },
		{ '5', new Usage(0x00, 0x22) },
		{ '6', new Usage(0x00, 0x23) },
		{ '7', new Usage(0x00, 0x24) },
		{ '8', new Usage(0x00, 0x25) },
		{ '9', new Usage(0x00, 0x26) },
		{ ':', new Usage(0xe1, 0x33, 0x00, 0x34) },
		{ ';', new Usage(0x00, 0x33) },
		{ '<', new Usage(0xe1, 0x36) },
		{ '=', new Usage(0x00, 0x2e) },
		{ '>', new Usage(0xe1, 0x37) },
		{ '?', new Usage(0xe1, 0x38) },
		{ '@', new Usage(0xe1, 0x1f, 0x00, 0x2f) },
		{ 'A', new Usage(0xe1, 0x04) },
		{ 'B', new Usage(0xe1, 0x05) },
		{ 'C', new Usage(0xe1, 0x06) },
		{ 'D', new Usage(0xe1, 0x07) },
		{ 'E', new Usage(0xe1, 0x08) },
		{ 'F', new Usage(0xe1, 0x09) },
		{ 'G', new Usage(0xe1, 0x0A) },
		{ 'H', new Usage(0xe1, 0x0B) },
		{ 'I', new Usage(0xe1, 0x0C) },
		{ 'J', new Usage(0xe1, 0x0D) },
		{ 'K', new Usage(0xe1, 0x0E) },
		{ 'L', new Usage(0xe1, 0x0F) },
		{ 'M', new Usage(0xe1, 0x10) },
		{ 'N', new Usage(0xe1, 0x11) },
		{ 'O', new Usage(0xe1, 0x12) },
		{ 'P', new Usage(0xe1, 0x13) },
		{ 'Q', new Usage(0xe1, 0x14) },
		{ 'R', new Usage(0xe1, 0x15) },
		{ 'S', new Usage(0xe1, 0x16) },
		{ 'T', new Usage(0xe1, 0x17) },
		{ 'U', new Usage(0xe1, 0x18) },
		{ 'V', new Usage(0xe1, 0x19) },
		{ 'W', new Usage(0xe1, 0x1a) },
		{ 'X', new Usage(0xe1, 0x1b) },
		{ 'Y', new Usage(0xe1, 0x1c) },
		{ 'Z', new Usage(0xe1, 0x1d) },
		{ '[', new Usage(0x00, 0x2f, 0x00, 0x30) },
		{ '\\',new Usage(0x00, 0x31, 0x00, 0x87) },
		{ ']', new Usage(0x00, 0x30, 0x00, 0x32) },
		{ '^', new Usage(0xe1, 0x23, 0x00, 0x2e) },
		{ '_', new Usage(0xe1, 0x2d, 0x1e, 0x87) },
		{ '`', new Usage(0x00, 0x35, 0x1e, 0x2f) },
		{ 'a', new Usage(0x00, 0x04) },
		{ 'b', new Usage(0x00, 0x05) },
		{ 'c', new Usage(0x00, 0x06) },
		{ 'd', new Usage(0x00, 0x07) },
		{ 'e', new Usage(0x00, 0x08) },
		{ 'f', new Usage(0x00, 0x09) },
		{ 'g', new Usage(0x00, 0x0A) },
		{ 'h', new Usage(0x00, 0x0B) },
		{ 'i', new Usage(0x00, 0x0C) },
		{ 'j', new Usage(0x00, 0x0D) },
		{ 'k', new Usage(0x00, 0x0E) },
		{ 'l', new Usage(0x00, 0x0F) },
		{ 'm', new Usage(0x00, 0x10) },
		{ 'n', new Usage(0x00, 0x11) },
		{ 'o', new Usage(0x00, 0x12) },
		{ 'p', new Usage(0x00, 0x13) },
		{ 'q', new Usage(0x00, 0x14) },
		{ 'r', new Usage(0x00, 0x15) },
		{ 's', new Usage(0x00, 0x16) },
		{ 't', new Usage(0x00, 0x17) },
		{ 'u', new Usage(0x00, 0x18) },
		{ 'v', new Usage(0x00, 0x19) },
		{ 'w', new Usage(0x00, 0x1a) },
		{ 'x', new Usage(0x00, 0x1b) },
		{ 'y', new Usage(0x00, 0x1c) },
		{ 'z', new Usage(0x00, 0x1d) },
		{ '{', new Usage(0x00, 0x2c, 0x1e, 0x30) },
		{ '|', new Usage(0x00, 0x2c, 0x1e, 0x89) },
		{ '}', new Usage(0x00, 0x2c, 0x1e, 0x32) },
		{ '~', new Usage(0xe1, 0x35, 0x1e, 0x2e) },
	};

	private List<byte> _usageList;

	/// <summary>
	/// Converts a list of variables into a usage list represented as a byte sequence.
	/// </summary>
	/// <param name="ctx">The context containing necessary configuration and metadata.</param>
	/// <param name="value">The list of variables to be converted into usage list bytes.</param>
	/// <returns>A list of bytes representing the converted usage list.</returns>
	/// <exception cref="Exception">Thrown when an invalid keycode type or value is encountered, or when a character cannot be mapped to a usage code.</exception>
	private static List<byte> toUsageList(Context ctx, List<Variable> value)
	{
		List<byte> result = new List<byte>();

		foreach (var code in value)
		{
			if (code.type == Variable.Type.IntType)
			{
				if (code.intValue < 1 || 0xff < code.intValue)
				{
					throw new Exception("invalid keycode");
				}
				result.Add((byte)code.intValue);
			}
			else if (code.type == Variable.Type.StringType)
			{
				/*
				var text = code.stringValue;
				if (code.stringValue.StartsWith('$') && !code.stringValue.StartsWith("$$"))
				{
					var variableId = ToReferenceOrUInt16(ctx.AergiaConfig.device, code);
				}
				*/
				byte shift = 0;
				foreach (var c in code.stringValue.ToCharArray())
				{
					if (!usageMap.TryGetValue(c, out Usage? u))
					{
						throw new Exception($"invalid character '{c}'");
					}
					
					byte modifier = 0;
					byte ucode = 0;
					if (ctx.AergiaConfig.device.lang == "en")
					{
						modifier = u.enModifier;
						ucode = u.enCode;
					}
					else
					{
						modifier = u.lcModifier;
						ucode = u.lcCode;
					}

					if (modifier != 0 && shift == 0)
					{
						result.Add(modifier);
						shift = modifier;
					}
					else if (modifier == 0 && shift != 0)
					{
						result.Add((byte)shift);
						shift = 0;
					}
					result.Add(ucode);
				}
				if(shift != 0)
					result.Add((byte)shift);
			}
			else
			{
				throw new Exception("invalid keycode type");
			}
		}
		return result;
	}

	private UsageListFragment(List<byte> usageList)
	{
		_usageList = usageList;
	}

	internal static UsageListFragment Create(Context ctx, List<Variable> values)
	{
		var fragment = new UsageListFragment(toUsageList(ctx, values));
		ctx.UsageSection.Add(fragment);
		return fragment;
	}
	internal override int CodeSize()
	{
		return 2 + _usageList.Count;
	}

	internal override int WriteCode(Span<byte> buffer)
	{
		int offset = 0;//base.WriteCode(buffer);
		BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(offset, 2), (UInt16)_usageList.Count);
		offset += 2;
		_usageList.CopyTo(buffer.Slice(offset, _usageList.Count));
		offset += _usageList.Count;
		return offset;
	}
}

/// <summary>
/// The VisualFragment class represents a configurable visual segment in a specific context.
/// It handles the creation and encoding of visual elements such as text, color, icons,
/// and background details based on provided format and capability specifications.
/// </summary>
internal class VisualFragment : FragmentBase
{
	private UInt32 _color;
	private UInt32 _bgColor;
	private TextFragment? _text;
	private UInt16 _iconIndex;
	private UInt16 _fontSize;

	private VisualFragment()
	{
		_text = null;
		_fontSize = 0;
		_color = 0xffffffff;
		_iconIndex = 0;
		_bgColor = 0xffffffff;
	}
	internal static VisualFragment Create(Context store, VisualCapabilitiesFormat? capabilities, VisualFormat format)
	{
		var segment = new VisualFragment();
		if (capabilities != null)
		{
			if (capabilities.textCapabilities != null)
			{
				if (format.text != null)
				{
					segment._text = TextFragment.Create(store, format.text);
				}

				if (format.color != null)
				{
					segment._color =
						ColorMapper.ColorData(capabilities.textCapabilities.pixelFormat, (int)format.color);
				}

				if (format.fontSize != null)
				{
					segment._fontSize = (UInt16)format.fontSize;
				}
			}

			if (capabilities.iconCapabilities != null)
			{
				if (format.icon != null)
				{
					segment._iconIndex = store.IconStore.CreateIcon(format, capabilities.iconCapabilities);
				}
			}

			if (capabilities.backgroundCapabilities != null)
			{
				if (format.background != null)
				{
					segment._bgColor = ColorMapper.ColorData(capabilities.backgroundCapabilities.pixelFormat,
						(int)format.background);
				}
			}
		}
		store.VisualSection.Add(segment);
		return segment;
	}
	internal override int CodeSize()
	{
		return 16;
	}

	internal override int WriteCode(Span<byte> buffer)
	{
		int offset = 0;
		BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(offset), (UInt32)(_text?.Offset ?? 0));
		offset += 4;
		BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(offset), _color);
		offset += 4;
		BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(offset), _bgColor);
		offset += 4;
		BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(offset ), _iconIndex);
		offset += 2;
		BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(offset ), _fontSize);
		offset += 2;
		return offset;
	}
}

/// <summary>
/// The ConfigFragment class represents a configuration segment built from applications and associated properties within a given context.
/// It is responsible for creating and encoding the configuration by processing applications and their respective fragments.
/// </summary>
internal class ConfigFragment : FragmentBase
{
	private OrderedDictionary<TextFragment, ApplicationFragment> _applications;

	private ConfigFragment(OrderedDictionary<TextFragment, ApplicationFragment> applications)
	{
		_applications = applications;
	}
	internal static ConfigFragment? Create(Context ctx, AergiaConfig config)
	{
		bool hasError = false;
		var applications  = new OrderedDictionary<TextFragment, ApplicationFragment>();
		ctx.SetAergiaConfig(config);
		foreach (var application in config.applications)
		{
			var nameFragment = TextFragment.Create(ctx, application.Key);
			var applicationFragment = ApplicationFragment.Create(ctx, application.Value);
			if(nameFragment == null)
				hasError = true;
			if(!hasError)
				applications.Add(nameFragment, applicationFragment);
			ctx.ActionSection = new Dictionary<string, ActionFragment>();
		}
		if(hasError)
			return null;
		var segment = new ConfigFragment(applications);
		ctx.ConfigSection = segment;
		return segment;
	}
	internal override int CodeSize()
	{
		return 4 + _applications.Count * 8;
	}
	internal override int WriteCode(Span<byte> buffer)
	{
		int offset = 0;
		BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(offset, 2), (UInt16)_applications.Count);
		offset += 4;
		foreach (var application in _applications)
		{
			BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(offset), (UInt32)application.Key.Offset);
			offset += 4;
			BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(offset), (UInt32)application.Value.Offset);
			offset += 4;
		}
		return offset;
	}
}

/// <summary>
/// The ApplicationFragment class processes and represents an application structure in the configuration context.
/// It is responsible for managing visual fragments, binding pages, and generating code representations for applications.
/// The class facilitates interactions between the application format and configured context, ensuring proper handling
/// of visual elements and page bindings when creating and manipulating application fragments.
/// </summary>
internal class ApplicationFragment : FragmentBase
{
	private VisualFragment? _visual;
	private OrderedDictionary<TextFragment, PageFragment> _bindPages;

	private ApplicationFragment(VisualFragment? visual, OrderedDictionary<TextFragment, PageFragment> bindPages)
	{
		_visual = visual;
		_bindPages = bindPages;
	}
	internal static ApplicationFragment? Create(Context ctx, ApplicationFormat format)
	{
		bool hasError = false;
		ControlFormat? display = null;
		VisualFragment? visual = null;
		var bindPages = new OrderedDictionary<TextFragment, PageFragment>();
		
		if (ctx.AergiaConfig.device.controls.ContainsKey(AergiaTypes.ControlId.Display))
		{
			display = ctx.AergiaConfig.device.controls[AergiaTypes.ControlId.Display];
		}
		
		if (display != null && display.visualCapabilities != null)
		{
			if (format.visual != null)
			{
				visual = VisualFragment.Create(ctx, display.visualCapabilities, format.visual);
			}
		}

		ctx.SetApplication(format);
		foreach (var page in format.pages)
		{
			var nameFragment = TextFragment.Create(ctx, page.Key);
			var pageFragment = PageFragment.Create(ctx, page.Value);
			if(pageFragment == null)
				hasError = true;
			if(!hasError)
				bindPages.Add(nameFragment, pageFragment);
		}
		
		if(hasError)
			return null;
		
		var fragment = new ApplicationFragment(visual, bindPages);
		ctx.ApplicationSection.Add(fragment);
		return fragment;
	}

	internal override int CodeSize()
	{
		return 8 + _bindPages.Count * 8;
	}

	internal override int WriteCode(Span<byte> buffer)
	{
		int offset = 0;
		BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(offset), (UInt32)(_visual?.Offset ?? 0));
		offset += 4;
		BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(offset), (UInt16)_bindPages.Count);
		offset += 4;
		foreach (var bindPage in _bindPages)
		{
			BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(offset), (UInt32)bindPage.Key.Offset);
			offset += 4;
			BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(offset), (UInt32)bindPage.Value.Offset);
			offset += 4;
		}
		return offset;
	}
}

/// <summary>
/// The PageFragment class represents a section or fragment of a page within the context of an application's configuration.
/// It manages the visual representation and related control bindings for the designated page section.
/// This class provides functionality to create instances dynamically based on the provided context and format,
/// as well as methods to calculate code size and serialize its data.
/// </summary>
internal class PageFragment : FragmentBase
{
	private VisualFragment? _visual;
	private OrderedDictionary<AergiaTypes.ControlId,ControlBindFragment> _controlBinds;

	private PageFragment(VisualFragment? visual, OrderedDictionary<AergiaTypes.ControlId,ControlBindFragment> controlBinds)
	{
		_visual = visual;
		_controlBinds = controlBinds;
	}

	internal static PageFragment? Create(Context ctx, BindPageFormat format)
	{
		bool hasError = false;
		ControlFormat? display = null;
		VisualFragment? visual = null;
		if (ctx.AergiaConfig.device.controls.ContainsKey(AergiaTypes.ControlId.Display))
		{
			display = ctx.AergiaConfig.device.controls[AergiaTypes.ControlId.Display];
		}
		if (display != null && display.visualCapabilities != null)
		{
			if (format.visual != null)
			{
				visual = VisualFragment.Create(ctx, display.visualCapabilities, format.visual);
			}
		}

		var controlBinds = new OrderedDictionary<AergiaTypes.ControlId, ControlBindFragment>();
		foreach (var bind in format.binds)
		{
			if (!ctx.AergiaConfig.device.controls.ContainsKey(bind.Key))
			{
				Log.Error(format, $"control {bind.Key.ToString()} does not defined in the '{ctx.AergiaConfig.device.model}'");
				hasError = true;
			}
			else
			{
				var control = ctx.AergiaConfig.device.controls[bind.Key];
				ctx.SetControl(control);
				var controlBind = ControlBindFragment.Create(ctx, bind.Value);
				if(controlBind == null)
					hasError = true;
				else
					controlBinds.Add(bind.Key, controlBind);
			}
		}

		if (hasError)
			return null;
		
		var fragment = new PageFragment(visual, controlBinds);
		ctx.PageSection.Add(fragment);
		return fragment;
	}

	internal override int CodeSize()
	{
		return 8 + _controlBinds.Count * 8;
	}

	internal override int WriteCode(Span<byte> buffer)
	{
		int offset = 0;
		BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(offset), (UInt32)(_visual?.Offset ?? 0));
		offset += 4;
		BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(offset), (UInt16)_controlBinds.Count);
		offset += 4;
		foreach (var control in _controlBinds)
		{
			BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(offset), (UInt16)control.Key);
			offset += 4;
			BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(offset), (UInt32)control.Value.Offset);
			offset += 4;
		}
		return offset;
	}
}

/// <summary>
/// The ControlBindFragment class represents a fragment that binds controls to specific events
/// and actions within an application configuration. It manages associations between controls,
/// associated visual fragments, and event-action mappings for runtime behavior.
/// </summary>
internal class ControlBindFragment : FragmentBase
{
	private VisualFragment? _visual;
	private OrderedDictionary<AergiaTypes.EventId, ActionCommandFragment> _eventBinds;

	private ControlBindFragment(VisualFragment? visual,
		OrderedDictionary<AergiaTypes.EventId, ActionCommandFragment> eventBinds)
	{
		_visual = visual;
		_eventBinds = eventBinds;
	}

	internal static ControlBindFragment? Create(Context ctx, ControlBindFormat format)
	{
		bool hasError = false;
		var eventBinds = new OrderedDictionary<AergiaTypes.EventId, ActionCommandFragment>();
		VisualFragment? visual = null;
		foreach (var bind in format.binds)
		{
			var bindEventId = bind.Key;
			foreach (var actionName in bind.Value.actions)
			{
				if (!ctx.Application.actions.TryGetValue(actionName, out var action))
				{
					Log.Error(format,
						$"action '{actionName}' is not defined in the '{ctx.Application.ObjectName}' application");
					hasError = true;
					continue;
				}
				ctx.SetActionName(actionName);
				var actionFragment = ActionFragment.Create(ctx, action);
				if (actionFragment == null)
				{
					hasError = true;
					continue;
				}
				if (visual == null)
				{
					visual = actionFragment.CreateVisual(ctx);
				}

				if (actionFragment == null || actionFragment.eventMapAction == null)
				{
					int a = 0;
				}

				foreach (var eventAction in actionFragment.eventMapAction)
				{
					AergiaTypes.EventId eventId = eventAction.Key;
					if (eventId == AergiaTypes.EventId.UnNamed)
					{
						if (bindEventId == AergiaTypes.EventId.UnNamed)
						{
							if (ctx.Control.events == null)
							{
								Log.Error(format, $"No event defined for control '{ctx.Control.ObjectName}'");
								hasError = true;
							}
							else
							{
								eventId = ctx.Control.events[0];
							}
						}
						else
						{
							eventId = bindEventId;
						}
					}
					else {
						if (bindEventId != AergiaTypes.EventId.UnNamed)
						{
							if (eventId != bindEventId)
							{
								Log.Error(format,
									$"Events that execute actions conflict with '{eventId}' and '{bindEventId}'");
								hasError = true;
							}
						}
					}

					if (ctx.Control.events == null || !ctx.Control.events.Contains(eventId))
					{
						Log.Error(format, $"event '{eventId.ToString()}' is not defined in the '{ctx.Control.ObjectName}' control");
						hasError = true;
					}
					if (!hasError)
					{
						ActionCommandFragment commandList;
						if (!eventBinds.TryGetValue(eventId, out commandList))
						{
							commandList = new ActionCommandFragment();
							ctx.ActionCommandSection.Add(commandList);
							eventBinds.Add(eventId, commandList);
						}

						foreach (var commandFragment in eventAction.Value._commands)
						{
							commandList.Add(commandFragment);
						}
					}
				}
			}
		}
		if (hasError)
			return null;
		var fragment = new ControlBindFragment(visual, eventBinds);
		ctx.ControlBindSection.Add(fragment);
		return fragment;
	}

	internal override int CodeSize()
	{
		return 8 + _eventBinds.Count * 8;
	}

	internal override int WriteCode(Span<byte> buffer)
	{
		int offset = 0;
		BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(offset), (UInt32)(_visual?.Offset ?? 0));
		offset += 4;
		BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(offset), (UInt32)_eventBinds.Count);
		offset += 4;
		foreach (var bind in _eventBinds)
		{
			BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(offset), (UInt16)bind.Key);
			offset += 4;
			BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(offset), (UInt32)bind.Value.Offset);
			offset += 4;
		}
		return offset;
	}
}

/// <summary>
/// Represents a fragment of an action within the application configuration system.
/// Provides mechanisms to associate action commands with events and to create visual representations based on the provided format.
/// </summary>
internal class ActionFragment : FragmentBase
{
	private VisualFormat? _visualFormat;
	private VisualFragment? _visual;
	internal Dictionary<AergiaTypes.EventId, ActionCommandFragment> eventMapAction;

	private ActionFragment(Dictionary<AergiaTypes.EventId, ActionCommandFragment> eventMapAction)
	{
		_visual = null;
		this.eventMapAction = eventMapAction;
	}
	
	internal VisualFragment? CreateVisual(Context ctx)
	{
		if (_visualFormat != null)
		{
			if (_visual == null)
			{
				_visual = VisualFragment.Create(ctx, ctx.Control.visualCapabilities, _visualFormat);
			}
		}
		return _visual;
	}

	internal static ActionFragment? Create(Context ctx, ActionFormat format)
	{
		if (ctx.ActionSection.ContainsKey(format.ObjectName))
		{
			return ctx.ActionSection[format.ObjectName];
		}
		bool hasError = false;
		var eventMapAction = new Dictionary<AergiaTypes.EventId, ActionCommandFragment>();
		foreach (var eventAction in format.eventMapAction)
		{
			var actionCommandFragment = ActionCommandFragment.Create(ctx, eventAction.Value);
			if(actionCommandFragment == null)
				hasError = true;
			else
				eventMapAction.Add(eventAction.Key, actionCommandFragment);
		}
		if (hasError)
			return null;
		if (eventMapAction.Count == 0)
		{
			int a = 0;
		}

		var fragment = new ActionFragment(eventMapAction);
		fragment._visualFormat = format.visual;
		ctx.ActionSection.Add(format.ObjectName, fragment);
		return fragment;
	}
}

/// <summary>
/// Represents a fragment that encapsulates a sequence of action commands.
/// This class is responsible for managing a collection of command fragments,
/// providing functionalities to add commands, calculate code size, and write binary codes.
/// </summary>
internal class ActionCommandFragment : FragmentBase
{
	internal List<CommandFragment> _commands;

	private ActionCommandFragment(List<CommandFragment> commands)
	{
		_commands = commands;
	}

	internal ActionCommandFragment()
	{
		_commands = new List<CommandFragment>();
	}
	
	internal void Add(CommandFragment fragment) => _commands.Add(fragment);
	
	internal static ActionCommandFragment? Create(Context ctx, ActionCommandFormat format)
	{
		bool hasError = false;
		var commands = new List<CommandFragment>();
		foreach (var command in format.commands)
		{
			var commandFragment = CommandFragment.Create(ctx, command);
			if(commandFragment == null)
				hasError = true;
			else
				commands.Add(commandFragment);
		}
		if(hasError)
			return null;
		var fragment = new ActionCommandFragment(commands);
		//ctx.ActionCommandSection.Add(fragment);
		return fragment;
	}

	internal override int CodeSize()
	{
		return 4 + _commands.Count * 4;
	}

	internal override int WriteCode(Span<byte> buffer)
	{
		int offset = 0;
		BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(offset), (UInt32)_commands.Count);
		offset += 4;
		foreach (var command in _commands)
		{
			BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(offset), (UInt32)command.Offset);
			offset += 4;
		}
		return offset;
	}
}

/// <summary>
/// The CommandFragment class is a base representation of a command within a system.
/// It is responsible for encapsulating command identifiers and facilitating operations
/// such as creating specific types of command fragments and writing encoded command data.
/// </summary>
internal class CommandFragment : FragmentBase
{
	protected AergiaTypes.CommandId _commandId;

	protected CommandFragment(AergiaTypes.CommandId commandId)
	{
		_commandId = commandId;
	}
	internal static CommandFragment Create(Context ctx, CommandFormat format)
	{
		CommandFragment? fragment = null;
		var type = format.GetType();
		if(type == typeof(SimpleCommandFormat))
		{
			fragment = new CommandFragment(format.commandId);
		}
		else if (type == typeof(MouseMoveCommandFormat))
		{
			fragment =  MouseMoveCommandFragment.Create(ctx, (MouseMoveCommandFormat)format);
		}
		else if (type == typeof(MouseWheelCommandFormat))
		{
			fragment =  MouseWheelCommandFragment.Create(ctx, (MouseWheelCommandFormat)format);
		}
		else if (type == typeof(ButtonCommandFormat))
		{
			fragment =  ButtonCommandFragment.Create(ctx, (ButtonCommandFormat)format);
		}
		else if (type == typeof(KeyCommandFormat))
		{
			fragment =  KeyCommandFragment.Create(ctx, (KeyCommandFormat)format);
		}
		else if (type == typeof(MapInputCommandFormat))
		{
			fragment =  MapInputCommandFragment.Create(ctx, (MapInputCommandFormat)format);
		}
		else if (type == typeof(MapActionCommandFormat))
		{
			fragment =  MapActionCommandFragment.Create(ctx, (MapActionCommandFormat)format);
		}
		else if (type == typeof(PageChangeCommandFormat))
		{
			fragment =  PageChangeCommandFragment.Create(ctx, (PageChangeCommandFormat)format);
		}
		else if (type == typeof(ApplicationChangeCommandFormat))
		{
			fragment =  ApplicationChangeCommandFragment.Create(ctx, (ApplicationChangeCommandFormat)format);
		}
		else if (type == typeof(DelayCommandFormat))
		{
			fragment =  DelayCommandFragment.Create(ctx, (DelayCommandFormat)format);
		}
		else if (type == typeof(TimerCommandFormat))
		{
			fragment =  TimerCommandFragment.Create(ctx, (TimerCommandFormat)format);
		}

		if(fragment == null)
			throw new ParseException(format, "unknown command type");
		ctx.CommandSection.Add(fragment);
		return fragment;
	}

	internal override int CodeSize()
	{
		return 2;
	}

	internal override int WriteCode(Span<byte> buffer)
	{
		BinaryPrimitives.WriteUInt16LittleEndian(buffer, (UInt16)_commandId);
		return 2;
	}
}

/// <summary>
/// Represents a command fragment that encodes mouse movement actions.
/// This class allows for the creation of a mouse movement command based on positional and optional scaling parameters.
/// It provides mechanisms to transform the input data into a binary code representation for execution by devices.
/// </summary>
internal class MouseMoveCommandFragment : CommandFragment
{
	private Int16 _x;
	private Int16 _y;
	private Int16 _s;
	private float _xr;
	private float _yr;
	private MouseMoveCommandFragment(AergiaTypes.CommandId commandId) : base(commandId)
	{
		_x = 0;
		_y = 0;
		_s = 0;
		_xr = 0;
		_yr = 0;
	}

	internal static MouseMoveCommandFragment Create(Context ctx, MouseMoveCommandFormat format)
	{
		bool hasError = false;
		Int16 x = 0;
		Int16 y = 0;
		Int16 s = 0;
		float xr = 0;
		float yr = 0;

		try
		{
			x = ToReferenceOrInt16(ctx.AergiaConfig.device, format.x);
		}
		catch (Exception e)
		{
			Log.Error(format, $"x {e.Message}");
			hasError = true;
		}
		try
		{
			y = ToReferenceOrInt16(ctx.AergiaConfig.device, format.y);
		}
		catch (Exception e)
		{
			Log.Error(format, $"y {e.Message}");
			hasError = true;
		}
		try
		{
			s = ToReferenceOrInt16(ctx.AergiaConfig.device, format.s);
		}
		catch (Exception e)
		{
			Log.Error(format, $"s {e.Message}");
			hasError = true;
		}
		try
		{
			xr = ToReferenceOrInt16(ctx.AergiaConfig.device, format.s);
		}
		catch (Exception e)
		{
			Log.Error(format, $"s {e.Message}");
			hasError = true;
		}

		if (hasError)
			return null;
		
		var fragment = new MouseMoveCommandFragment(format.commandId);
		fragment._x = x;
		fragment._y = y;
		fragment._s = s;
		fragment._xr = xr;
		fragment._yr = yr;
		return fragment;
	}

	internal override int CodeSize()
	{
		return base.CodeSize() + 14;
	}

	internal override int WriteCode(Span<byte> buffer)
	{
		int offset = base.WriteCode(buffer);
		BinaryPrimitives.WriteInt16LittleEndian(buffer.Slice(offset, 2), _x);
		offset += 2;
		BinaryPrimitives.WriteInt16LittleEndian(buffer.Slice(offset, 2), _y);
		offset += 2;
		BinaryPrimitives.WriteInt16LittleEndian(buffer.Slice(offset, 2), _s);
		offset += 2;
		BinaryPrimitives.WriteSingleLittleEndian(buffer.Slice(offset, 4), _xr);
		offset += 4;
		BinaryPrimitives.WriteSingleLittleEndian(buffer.Slice(offset, 4), _yr);
		offset += 4;
		return offset;
	}
}

/// <summary>
/// The MouseWheelCommandFragment class represents a specific type of command fragment for handling mouse wheel operations.
/// It extends the functionality of the CommandFragment class and provides support for encoding and processing mouse wheel rotation
/// parameters such as delta values and scaling factors.
/// </summary>
internal class MouseWheelCommandFragment : CommandFragment
{
	private Int16 _delta;
	private float _r;
	protected MouseWheelCommandFragment(AergiaTypes.CommandId commandId) : base(commandId)
	{
		_delta = 0;
		_r = 0;
	}

	internal static MouseWheelCommandFragment? Create(Context ctx, MouseWheelCommandFormat format)
	{
		bool hasError = false;
		Int16 delta = 0;
		float r = 1;
		try
		{
			delta = ToReferenceOrInt16(ctx.AergiaConfig.device, format.delta);
		}
		catch (Exception e)
		{
			Log.Error(format, $"delta {e.Message}");
			hasError = true;
		}
		try
		{
			r = ToReferenceOrFloat(ctx.AergiaConfig.device, format.r);
		}
		catch (Exception e)
		{
			Log.Error(format, $"r {e.Message}");
			hasError = true;
		}
		if(hasError)
			return null;

		var fragment = new MouseWheelCommandFragment(format.commandId);
		fragment._delta = delta;
		fragment._r = r;
		return fragment;
	}
	internal override int CodeSize()
	{
		return base.CodeSize() + 6;
	}

	internal override int WriteCode(Span<byte> buffer)
	{
		int offset = base.WriteCode(buffer);
		BinaryPrimitives.WriteInt16LittleEndian(buffer.Slice(offset,2), _delta);
		offset += 2;
		BinaryPrimitives.WriteSingleLittleEndian(buffer.Slice(offset, 4), _r);
		offset += 4;
		return offset;
	}
}

/// <summary>
/// The ButtonCommandFragment class represents a specialized command fragment designed to encapsulate a command identifier
/// and an associated button value. It facilitates the processing, encoding, and writing of command fragments involving buttons.
/// </summary>
internal class ButtonCommandFragment : CommandFragment
{
	private UInt16 _button;

	private ButtonCommandFragment(AergiaTypes.CommandId commandId, UInt16 button) : base(commandId)
	{
		_button = button;
	}

	internal static ButtonCommandFragment Create(Context ctx, ButtonCommandFormat format)
	{
		var fragment = new ButtonCommandFragment(format.commandId, (UInt16)format.button);
		return fragment;
	}

	internal override int CodeSize()
	{
		return base.CodeSize() + 2;
	}

	internal override int WriteCode(Span<byte> buffer)
	{
		var offset = base.WriteCode(buffer);
		BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(offset, 2), _button);
		offset += 2;
		return offset;
	}
}

/// <summary>
/// The KeyCommandFragment class is a specialized type of CommandFragment responsible for managing and representing
/// key command data. It is designed to process specific configuration details such as intervals and associated usage values
/// based on the KeyCommandFormat input.
/// This class handles the creation of key command fragments by validating and transforming the provided format data
/// into the required internal structure for subsequent use in applications.
/// </summary>
internal class KeyCommandFragment : CommandFragment
{
	private UsageListFragment _usagelist;
	private UInt16 _interval;

	private KeyCommandFragment(AergiaTypes.CommandId commandId, UInt16 intervaal, UsageListFragment usagelist) : base(commandId)
	{
		_usagelist = usagelist;
		_interval = intervaal;
	}

	internal static KeyCommandFragment Create(Context ctx, KeyCommandFormat format)
	{
		bool hasError = false;
		UInt16 interval = 0;
		UsageListFragment? values = null;
		try
		{
			interval = ToReferenceOrUInt16(ctx.AergiaConfig.device, format.interval);
		}
		catch (Exception e)
		{
			Log.Error(format, $"interval {e.Message}");
			hasError = true;
		}

		try
		{
			values = UsageListFragment.Create(ctx, format.value);
		}
		catch (Exception e)
		{
			Log.Error(format, $"value {e.Message}");
			hasError = true;
		}
		
		if(hasError)
			return null;
		
		var fragment = new KeyCommandFragment(format.commandId, interval, values!);
		return fragment;
	}

	internal override int CodeSize()
	{
		return base.CodeSize() + 6;
	}

	internal override int WriteCode(Span<byte> buffer)
	{
		int offset = base.WriteCode(buffer);
		BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(offset), _interval);
		offset += 2;
		BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(offset), (UInt32)_usagelist.Offset);
		offset += 4;
		return offset;
	}
}

/// <summary>
/// Represents a fragment that encapsulates mapping input data within the context of a configurable system.
/// This class is used to define a comparator operation, associated data, and usage list specific to the mapping input process.
/// </summary>
internal class MapInputDataFragment : CommandFragment
{
	internal float compData;
	internal AergiaTypes.MapComparator op;
	internal UsageListFragment usagelist;

	private MapInputDataFragment(AergiaTypes.MapComparator op, float compData, UsageListFragment usagelist) : base(AergiaTypes.CommandId.MapInput)
	{
		this.op = op;
		this.compData = compData;
		this.usagelist = usagelist;
	}

	internal static MapInputDataFragment? Create(Context ctx, MapInputData data)
	{
		var usagelist = UsageListFragment.Create(ctx, data.value);
		if (usagelist == null)
			return null;

		float key = 0;
		if(data.key != null)
			key = ToReferenceOrFloat(ctx.AergiaConfig.device, data.key!);

		var fragment = new MapInputDataFragment(data.op, key, usagelist);
		return fragment;
	}

	internal override int CodeSize()
	{
		throw new Exception("bad call");
	}

	internal override int WriteCode(Span<byte> buffer)
	{
		throw new Exception("bad call");
	}
}

/// <summary>
/// The MapInputCommandFragment class represents a specific type of command fragment used to map input commands.
/// It encapsulates the processing of command data, including key references and mappings, and provides functionality for
/// creating the fragment, calculating its code size, and writing its binary representation into a buffer.
/// </summary>
internal class MapInputCommandFragment : CommandFragment
{
	private float _key;
	private List<MapInputDataFragment> _map;
	private MapInputCommandFragment(AergiaTypes.CommandId commandId, float key,  List<MapInputDataFragment> map) : base(commandId)
	{
		_key = key;
		_map = map;
	}

	internal static MapInputCommandFragment Create(Context ctx, MapInputCommandFormat format)
	{
		bool hasError = false;
		float key = 0;
		try
		{
			key = ToReferenceOrFloat(ctx.AergiaConfig.device, format.key);
		}
		catch (Exception e)
		{
			Log.Error(format, $"key {e.Message}");
			hasError = true;
		}

		var map = new List<MapInputDataFragment>();
		foreach (var m in format.map)
		{
			var seg = MapInputDataFragment.Create(ctx, m);
			if(seg == null)
				hasError = true;
			else 
				map.Add(seg);
		}
		if(hasError)
			return null;

		var fragment = new MapInputCommandFragment(format.commandId, key, map);
		return fragment;
	}

	internal override int CodeSize()
	{
		return base.CodeSize() + 6 + _map.Count * 12;
	}

	internal override int WriteCode(Span<byte> buffer)
	{
		int offset = base.WriteCode(buffer);
		BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(offset,2), (UInt16)_map.Count);
		offset += 2;
		BinaryPrimitives.WriteSingleLittleEndian(buffer.Slice(offset,4), _key);
		offset += 4;
		foreach (var m in _map)
		{
			BinaryPrimitives.WriteSingleLittleEndian(buffer.Slice(offset), m.compData);
			offset += 4;
			BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(offset), (UInt32)m.usagelist.Offset);
			offset += 4;
			BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(offset), (UInt16)m.op);
			offset += (2+2); // padding 2
		}
		return offset;
	}
}

/// <summary>
/// The MapActionFragment class represents a specialized type of command fragment
/// that defines a mapping action used in the process of creating and managing
/// command structures in the Aergia configuration framework.
/// It encapsulates comparison logic, mapping keys, and associated action components.
/// </summary>
internal class MapActionFragment : CommandFragment
{
	internal float compData;
	internal AergiaTypes.MapComparator op;
	internal ActionCommandFragment action;

	private MapActionFragment(AergiaTypes.MapComparator op, float compData, ActionCommandFragment action) : base(AergiaTypes.CommandId.MapAction)
	{
		this.op = op;
		this.compData = compData;
		this.action = action;
	}

	internal static MapActionFragment? Create(Context ctx, MapActionData data)
	{
		
		var actionCommandFragment = ActionCommandFragment.Create(ctx, data.action);
		if (actionCommandFragment == null)
			return null;

		float key = 0;
		if(data.key != null)
			key = ToReferenceOrFloat(ctx.AergiaConfig.device, data.key!);

		var fragment = new MapActionFragment(data.op, key, actionCommandFragment);
		ctx.CommandSection.Add(fragment);
		return fragment;
	}

	internal override int CodeSize()
	{
		throw new Exception("bad call");
	}

	internal override int WriteCode(Span<byte> buffer)
	{
		throw new Exception("bad call");
	}
}

/// <summary>
/// Represents a specialized command fragment that processes mapping actions based on a specific command format.
/// It extends the CommandFragment class to include functionalities for handling action mappings with additional data encoding and writing capabilities.
/// </summary>
internal class MapActionCommandFragment : CommandFragment
{
	private UInt16 _key;
	private List<MapActionFragment> _map;
	private MapActionCommandFragment(AergiaTypes.CommandId commandId, UInt16 key,  List<MapActionFragment> map) : base(commandId)
	{
		_key = key;
		_map = map;
	}

	internal static MapActionCommandFragment? Create(Context ctx, MapActionCommandFormat format)
	{
		bool hasError = false;
		UInt16 key = 0;
		try
		{
			key = ToReferenceOrUInt16(ctx.AergiaConfig.device, format.key);
		}
		catch (Exception e)
		{
			Log.Error(format, $"key {e.Message}");
			hasError = true;
		}
		
		var map = new List<MapActionFragment>();
		foreach (var m in format.map)
		{
			var seg = MapActionFragment.Create(ctx, m);
			if(seg == null)
				hasError = true;
			else
				map.Add(seg);
		}
		if(hasError)
			return null;
		var fragment = new MapActionCommandFragment(format.commandId, key, map);
		return fragment;
	}

	internal override int CodeSize()
	{
		return base.CodeSize() + 6 + _map.Count * 12;
	}

	internal override int WriteCode(Span<byte> buffer)
	{
		int offset = base.WriteCode(buffer);
		BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(offset), (UInt16)_map.Count);
		offset += 2;
		BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(offset), _key);
		offset += 4;
		foreach (var m in _map)
		{
			BinaryPrimitives.WriteSingleLittleEndian(buffer.Slice(offset), m.compData);
			offset += 4;
			BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(offset), (UInt32)m.action.Offset);
			offset += 4;
			BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(offset), (UInt16)m.op);
			offset += (2+2); // padding 2
		}
		return offset;
	}
}

/// <summary>
/// The PageChangeCommandFragment class represents a specific type of command fragment that handles page change commands.
/// It encapsulates the logic required to define, create, and manage the page change command, including associated metadata
/// such as the command ID and the target page.
/// </summary>
internal class PageChangeCommandFragment : CommandFragment
{
	internal TextFragment _page;
	internal PageChangeCommandFragment(AergiaTypes.CommandId commandId, TextFragment page) : base(commandId)
	{
		_page = page;
	}

	internal static PageChangeCommandFragment? Create(Context ctx, PageChangeCommandFormat format)
	{
		var page = TextFragment.Create(ctx, format.page);
		if (page == null)
			return null;
		var fragment = new PageChangeCommandFragment(format.commandId, page);
		return fragment;
	}
	internal override int CodeSize()
	{
		return base.CodeSize() + 6;
	}

	internal override int WriteCode(Span<byte> buffer)
	{
		int offset = base.WriteCode(buffer) + 2; // padding 2
		BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(offset), (UInt32)_page.Offset);
		offset += 4;
		return offset;
	}
}

/// <summary>
/// The ApplicationChangeCommandFragment class represents a specialized type of CommandFragment
/// used to encapsulate changes related to an application's state or properties.
/// It provides methods to create a fragment instance based on a specific application change command format and
/// includes logic for calculating code size and writing relevant data into a binary buffer.
/// </summary>
internal class ApplicationChangeCommandFragment : CommandFragment
{
	private TextFragment _application;
	private TextFragment? _page;
	private ApplicationChangeCommandFragment(AergiaTypes.CommandId commandId, TextFragment application, TextFragment? page) : base(commandId)
	{
		_application = application;
		_page = page;
	}

	internal static ApplicationChangeCommandFragment? Create(Context ctx, ApplicationChangeCommandFormat format)
	{
		var application = TextFragment.Create(ctx, format.application);
		if (application == null)
			return null;
		TextFragment? page = null;
		if(format.page != null) 
			page = TextFragment.Create(ctx, format.page);
		var fragment = new ApplicationChangeCommandFragment(format.commandId, application, page);
		return fragment;
	}
	
	internal override int CodeSize()
	{
		return base.CodeSize() + 10;
	}

	internal override int WriteCode(Span<byte> buffer)
	{
		int offset = base.WriteCode(buffer) + 2; // padding 2;
		BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(offset), (UInt32)_application.Offset);
		offset += 4;
		BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(offset), (UInt32)(_page?.Offset??0));
		offset += 4;
		return offset;
	}
}

/// <summary>
/// The DelayCommandFragment class represents a specialized command fragment
/// that applies a delay operation within a command sequence. It extends the
/// CommandFragment class to define delay-specific behavior and data handling.
/// </summary>
internal class DelayCommandFragment : CommandFragment
{
	private UInt16 _timeout;
	private DelayCommandFragment(AergiaTypes.CommandId commandId, UInt16 timeout) : base(commandId)
	{
		_timeout = timeout;
	}

	internal static DelayCommandFragment? Create(Context ctx, DelayCommandFormat format)
	{
		var fragment = new DelayCommandFragment(format.commandId, (UInt16)format.timeout);
		return fragment;
	}
	
	internal override int CodeSize()
	{
		return base.CodeSize() + 2;
	}

	internal override int WriteCode(Span<byte> buffer)
	{
		int offset = base.WriteCode(buffer);
		BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(offset), _timeout);
		offset += 2;
		return offset;
	}
}

/// The TimerCommandFragment class encapsulates a specific type of command fragment that represents a timer-based command.
/// It is responsible for creating valid command fragments by processing and validating a TimerCommandFormat object.
/// This class also manages the serialization of its internal state into a byte buffer for transmission or storage.
/// </summary>
internal class TimerCommandFragment : CommandFragment
{
	private UInt16 _timeout;
	private UInt16 _target;
	private Int16 _data;
	private TimerCommandFragment(AergiaTypes.CommandId commandId, UInt16 timeout, UInt16 taregt, Int16 data) : base(commandId)
	{
		_timeout = timeout;
		_target = taregt;
		_data = data;
	}

	internal static TimerCommandFragment? Create(Context ctx, TimerCommandFormat format)
	{
		bool hasError = false;
		if (!Enum.IsDefined(typeof(AergiaTypes.ControlId), format.target))
		{
			Log.Error(format, $"target {format.target} is not defined in the {ctx.AergiaConfig.device.model}");
			hasError = true;
		}
		
		Int16 data = 0;
		try
		{
			if(format.data != null)
				data = ToReferenceOrInt16(ctx.AergiaConfig.device, format.data);
		}
		catch (Exception e)
		{
			Log.Error(format, $"data {e.Message}");
			hasError = true;
		}
		if(hasError)
			return null;

		var fragment = new TimerCommandFragment(format.commandId, (UInt16)format.timeout, (UInt16)format.target, data);;
		return fragment;
	}
	
	internal override int CodeSize()
	{
		return base.CodeSize() + 6;
	}

	internal override int WriteCode(Span<byte> buffer)
	{
		int offset = base.WriteCode(buffer);
		BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(offset), _timeout);
		offset += 2;
		BinaryPrimitives.WriteUInt16LittleEndian(buffer.Slice(offset), _target);
		offset += 2;
		BinaryPrimitives.WriteInt16LittleEndian(buffer.Slice(offset), _data);
		offset += 2;
		return offset;
	}
}

/// <summary>
/// The Generator class is responsible for generating configuration binary data from a given AergiaConfig object.
/// It computes memory offsets for different configuration fragments and organizes their data into a single binary file.
/// This class ensures that the memory alignment of the fragments is met and creates the resulting configuration file.
/// </summary>
internal class Generator
{
	private AergiaConfig _config;
	private DeviceFormat _device;
	private Context _context = new Context();
	private byte[]? _bytecode;
	private OrderedDictionary<AergiaTypes.ControlId, ControlFormat> _controls;

	internal byte[] Bytecode => _bytecode!;

	internal List<IconImage> Icons => _context.IconStore.IconList;

	private int nextPosition(int offset, int add)
	{
		int next = offset + add;
		if (next % 4 != 0)
			next = (next / 4 + 1) * 4;
		return next;
	}
	internal void Generate(AergiaConfig config)
	{
		_config = config;
		_device = config.device;
		_controls = config.device.controls;
		_bytecode = null;
		var configFragment = ConfigFragment.Create(_context, config);
		if (configFragment == null)
			return;

		int size = 0;
		{
			configFragment.SetOffset((UInt16)size);
			size = nextPosition(size,configFragment.CodeSize());
		}
		foreach (var fragment in _context.ApplicationSection)
		{
			fragment.SetOffset((UInt16)size);
			size = nextPosition(size,fragment.CodeSize());
		}
		foreach (var fragment in _context.PageSection)
		{
			fragment.SetOffset((UInt16)size);
			size = nextPosition(size,fragment.CodeSize());
		}
		foreach (var fragment in _context.ControlBindSection)
		{
			fragment.SetOffset((UInt16)size);
			size = nextPosition(size,fragment.CodeSize());
		}
		foreach (var fragment in _context.ActionCommandSection)
		{
			fragment.SetOffset((UInt16)size);
			size = nextPosition(size,fragment.CodeSize());
		}
		foreach (var fragment in _context.CommandSection)
		{
			fragment.SetOffset((UInt16)size);
			size = nextPosition(size,fragment.CodeSize());
		}
		foreach (var fragment in _context.VisualSection)
		{
			fragment.SetOffset((UInt16)size);
			size = nextPosition(size,fragment.CodeSize());
		}
		foreach (var fragment in _context.UsageSection)
		{
			fragment.SetOffset((UInt16)size);
			size = nextPosition(size,fragment.CodeSize());
		}
		foreach (var fragment in _context.TextSection)
		{
			fragment.SetOffset((UInt16)size);
			//size = nextPosition(size,fragment.CodeSize());
			size += fragment.CodeSize();
		}
		
		_bytecode = new byte[size];
		int offset = 0;
		configFragment.WriteCode(_bytecode.AsSpan(offset,configFragment.CodeSize()));
		foreach (var fragment in _context.ApplicationSection)
		{
			offset = fragment.Offset;
			fragment.WriteCode(_bytecode.AsSpan(offset, fragment.CodeSize()));
		}
		foreach (var fragment in _context.PageSection)
		{
			offset = fragment.Offset;
			fragment.WriteCode(_bytecode.AsSpan(offset, fragment.CodeSize()));
		}
		foreach (var fragment in _context.ControlBindSection)
		{
			offset = fragment.Offset;
			fragment.WriteCode(_bytecode.AsSpan(offset, fragment.CodeSize()));
		}
		foreach (var fragment in _context.ActionCommandSection)
		{
			offset = fragment.Offset;
			fragment.WriteCode(_bytecode.AsSpan(offset, fragment.CodeSize()));
		}
		foreach (var fragment in _context.CommandSection)
		{
			offset = fragment.Offset;
			fragment.WriteCode(_bytecode.AsSpan(offset, fragment.CodeSize()));
		}
		foreach (var fragment in _context.VisualSection)
		{
			offset = fragment.Offset;
			fragment.WriteCode(_bytecode.AsSpan(offset, fragment.CodeSize()));
		}
		foreach (var fragment in _context.UsageSection)
		{
			offset = fragment.Offset;
			fragment.WriteCode(_bytecode.AsSpan(offset, fragment.CodeSize()));
		}
		foreach (var fragment in _context.TextSection)
		{
			offset = fragment.Offset;
			fragment.WriteCode(_bytecode.AsSpan(offset, fragment.CodeSize()));
		}
/*
		using (var stream = File.Open("config.bin", FileMode.Create))
		{
			using (var writer = new BinaryWriter(stream))
			{
				writer.Write(_bytecode);
			}
		}
*/
	}
	
}