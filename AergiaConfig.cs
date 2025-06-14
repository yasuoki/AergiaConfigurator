using System.Globalization;
using System.IO;
using Microsoft.ClearScript;
using SkiaSharp;

namespace AergiaConfigurator;

/// <summary>
/// Represents a location within a configuration or data source.
/// </summary>
public interface Location
{
	string FilePath { get; }
	string ObjectPath { get; }
	string ObjectName { get; }
}

/// <summary>
/// Represents an exception that occurs during parsing operations when invalid data is encountered or a parsing error arises.
/// </summary>
public class ParseException : AergiaException
{
	public ParseException(Location location, string message) : base(location, message)
	{
	}
}

/// <summary>
/// Provides helper methods for configuration processing, including path manipulation
/// and detection of JavaScript object types.
/// </summary>
public class ConfigHelper
{
	public string basePath;

	public ConfigHelper(string configFile)
	{
		basePath = Path.GetDirectoryName(configFile) ?? "";
	}

	/// <summary>
	/// Converts an absolute file path to a relative path based on the static file directory of the application.
	/// </summary>
	/// <param name="path">The absolute file path to be converted to a relative path.</param>
	/// <returns>A string representing the relative path, with directory separators normalized to forward slashes.</returns>
	public string toRelativePath(string path)
	{
		App app = (App)System.Windows.Application.Current;
		string basePath = app._httpOption.StaticFilePath;
		var relPath = Path.GetRelativePath(basePath, path);
		var text = relPath.Replace("\\","/");
		return text;
	}

	/// <summary>
	/// Converts a relative path, based on the provided source file path, to an absolute file path.
	/// </summary>
	/// <param name="src">The source file path used as the base directory for the relative path.</param>
	/// <param name="path">The relative path to be converted to an absolute file path.</param>
	/// <returns>A string representing the absolute file path.</returns>
	public string toAbsolutePathFromSrc(string src, string path)
	{
		var srcBase = Path.GetDirectoryName(Path.Combine(basePath, src)) ?? "";
		return Path.GetFullPath(Path.Combine(srcBase, path));
	}

	/// <summary>
	/// Determines whether a given dynamic object is a JavaScript object created within the V8 environment.
	/// </summary>
	/// <param name="obj">The dynamic object to be evaluated.</param>
	/// <returns>True if the object is a JavaScript object created within the V8 environment; otherwise, false.</returns>
	public static bool IsJsObject(dynamic obj)
	{
		if (obj == null || obj is Undefined)
			return false;
		var t = (Type)obj!.GetType();
		return t.Namespace == "Microsoft.ClearScript.V8";
	}

	/// <summary>
	/// Determines whether the specified dynamic object is a JavaScript array handled by Microsoft ClearScript.
	/// </summary>
	/// <param name="obj">The dynamic object to check for being a JavaScript array.</param>
	/// <returns>A boolean value indicating whether the object is a ClearScript V8Array type.</returns>
	public static bool IsJsArray(dynamic obj)
	{
		if (obj == null || obj is Undefined)
			return false;
		var t = (Type)obj!.GetType();
		return t.Namespace == "Microsoft.ClearScript.V8" && t.Name == "V8Array";
	}
}

/// <summary>
/// Provides a representation of a specific location within a configuration or data source,
/// allowing access to source data and associated metadata.
/// </summary>
public class Locator : Location
{
	public dynamic Source;
	public string ObjectName { get; set; }
	public string ObjectPath { get; set; }
	public string FilePath { get; set; }

	public Locator(dynamic source, string objectPath, string objectName, string filePath)
	{
		Source = source;
		ObjectName = objectName;
		if(string.IsNullOrEmpty(objectPath))
			ObjectPath = objectName;
		else
			ObjectPath = $"{objectPath}.{objectName}";
		FilePath = filePath;
	}

	/// <summary>
	/// Retrieves a child object of the current locator using the specified name.
	/// </summary>
	/// <param name="name">The name of the child object to locate.</param>
	/// <returns>A new instance of <see cref="Locator"/> representing the child object if found; otherwise, null.</returns>
	/// <exception cref="ParseException">Thrown when the source object is not a valid JavaScript object or if the requested property cannot be located.</exception>
	public Locator? Child(string name)
	{
		if (!ConfigHelper.IsJsObject(Source))
			throw new ParseException(this, string.Format(Resources.NotJavaScriptObject,ObjectName));
		var source = Source.GetProperty(name);
		if (source == null || source is Undefined)
			return null;
		string filePath;
		if (ConfigHelper.IsJsObject(source) && !(source!.location is Undefined)) {
			filePath = source!.location;
		}
		else
		{
			filePath = FilePath;
		}
		return new Locator(source, ObjectPath, name, filePath);
	}

	/// <summary>
	/// Retrieves child elements of a specified name from the current locator's source, ensuring the source object is a JavaScript array.
	/// </summary>
	/// <param name="name">The name of the child elements to locate in the source object.</param>
	/// <returns>An enumerable of Locator instances representing each child element in the specified array.</returns>
	public IEnumerable<Locator> ChildArray(string name)
	{
		if (!ConfigHelper.IsJsObject(Source))
			throw new ParseException(this, $"{ObjectName} is not JavaScript object");
		var source = Source.GetProperty(name);
		if (!ConfigHelper.IsJsArray(source))
			throw new ParseException(this,$"{name} is not JavaScript array");
		for (int i = 0; i < source.length; i++)
		{
			var arrayItem = source[i];
			string filePath;
			if (ConfigHelper.IsJsObject(arrayItem) && !(arrayItem.location is Undefined))
			{
				filePath = arrayItem.location;
			}
			else
			{
				filePath = FilePath;
			}
			yield return new Locator(arrayItem, ObjectPath, $"{name}[{i}]", filePath);
		}
	}

	/// <summary>
	/// Retrieves all child objects of the current locator's source, excluding specific properties such as "location".
	/// </summary>
	/// <returns>An enumerable collection of <see cref="Locator"/> representing the child objects with updated object paths and file paths.</returns>
	public IEnumerable<Locator> Children()
	{
		if (!ConfigHelper.IsJsObject(Source))
			throw new ParseException(this, $"{ObjectName} is not JavaScript Object");

		foreach (var childName in Source.PropertyNames)
		{
			if (childName == "location")
				continue;
			string filePath;
			var childObject = Source.GetProperty(childName);
			if (ConfigHelper.IsJsObject(childObject) && !(childObject.location is Undefined)) {
				filePath = childObject.location;
			}
			else
			{
				filePath = FilePath;
			}
			yield return new Locator(childObject, ObjectPath,childName, filePath);
		}
	}

	/// <summary>
	/// Returns an enumerable collection of Locator instances if the current Locator's source is a JavaScript array.
	/// Each Locator in the collection represents an individual element of the array, providing metadata about its location and name.
	/// </summary>
	/// <returns>
	/// An enumerable collection of Locator objects, where each object corresponds to an element in the JavaScript array.
	/// Throws a ParseException if the source is not a JavaScript array.
	/// </returns>
	public IEnumerable<Locator> Array()
	{
		if(!ConfigHelper.IsJsArray(Source))
			throw new ParseException(this, $"{ObjectName} is not JavaScript Array");
		for (int i = 0; i < Source.length; i++)
		{
			var arrayItem = Source[i];
			string filePath;
			if (ConfigHelper.IsJsObject(arrayItem) && !(arrayItem.location is Undefined))
			{
				filePath = arrayItem.location;
			}
			else
			{
				filePath = FilePath;
			}
			yield return new Locator(arrayItem, ObjectPath, $"{ObjectName}[{i}]", filePath);
		}
	}

}

/// <summary>
/// Represents a variable that can hold and manipulate values of different types such as boolean, integer, double, and string.
/// </summary>
public class Variable
{
	public enum Type
	{
		BoolType,
		IntType,
		DoubleType,
		StringType
	}
	public Type type;
	public bool boolValue;
	public int intValue;
	public double doubleValue;
	public string stringValue = String.Empty;

	public Variable(bool value)
	{
		type = Type.BoolType;
		boolValue = value;
	}
	public Variable(int value)
	{
		type = Type.IntType;
		intValue = value;
	}

	public Variable(double value)
	{
		type = Type.DoubleType;
		doubleValue = value;
	}

	public Variable(string value)
	{
		type = Type.StringType;
		stringValue = value;
	}

	public bool ToBool()
	{
		if (type == Type.BoolType)
			return boolValue;
		if(type == Type.IntType)
			return intValue != 0;
		if (type == Type.DoubleType)
			return doubleValue != 0.0;
		if(type == Type.StringType)
			return string.IsNullOrEmpty(stringValue) && stringValue.Equals("true", StringComparison.OrdinalIgnoreCase);
		throw new Exception("Unknown variable type");
	}

	public int ToInt()
	{
		if (type == Type.BoolType)
			return boolValue ? 1 : 0;
		if(type == Type.IntType)
			return intValue;
		if (type == Type.DoubleType)
		{
			if(doubleValue <= int.MinValue || doubleValue >= int.MaxValue)
				throw new Exception($"{doubleValue} overflows the int type");
			return (int)doubleValue;
		}
		if(type == Type.StringType)
			return int.Parse(stringValue);
		throw new Exception("Unknown variable type");
	}
	public Int16 ToInt16()
	{
		int n = ToInt();
		if(n <= Int16.MinValue || n >= Int16.MaxValue)
			throw new Exception($"{n} overflows the int16 type");
		return (Int16)n;
	}
	public UInt16 ToUInt16()
	{
		int n = ToInt();
		if(n <= UInt16.MinValue || n >= UInt16.MaxValue)
			throw new Exception($"{n} overflows the int16 type");
		return (UInt16)n;
	}
	public double ToDouble()
	{
		if (type == Type.BoolType)
			return (double)(boolValue ? 1 : 0);
		if(type == Type.IntType)
			return (double)intValue;
		if (type == Type.DoubleType)
			return doubleValue;
		if(type == Type.StringType)
			return double.Parse(stringValue);
		throw new Exception("Unknown variable type");
	}

	public override string ToString()
	{
		if (type == Type.BoolType)
			return boolValue ? "true" : "false";
		if(type == Type.IntType)
			return intValue.ToString();
		if (type == Type.DoubleType)
			return doubleValue.ToString();
		if(type == Type.StringType)
			return stringValue;
		throw new Exception("Unknown variable type");
	}

	public static Variable? Parse(Locator locator, string name)
	{
		var obj = locator.Source.GetProperty(name);
		if(obj is Undefined)
			return null;
		if(obj is int)
			return new Variable((int)obj);
		if(obj is double)
			return new Variable((double)obj);
		if(obj is string)
			return new Variable((string)obj);
		throw new ParseException(locator, $"{name} type error");
	}
	public static Variable? Parse(Locator locator)
	{
		var obj = locator.Source;
		if(obj is Undefined)
			return null;
		if(obj is int)
			return new Variable((int)obj);
		if(obj is double)
			return new Variable((double)obj);
		if(obj is string)
			return new Variable((string)obj);
		throw new ParseException(locator, $"{locator.ObjectName} type error");
	}
	public static bool? ParseBool(Locator locator, string name)
	{
		var obj = locator.Source.GetProperty(name);
		if(obj is Undefined)
			return null;
		if(obj is bool)
			return (bool)obj;
		throw new ParseException(locator, $"{name} is not boolean");
	}
	public static int? ParseInt(Locator locator, string name)
	{
		var obj = locator.Source.GetProperty(name);
		if(obj is Undefined)
			return null;
		if(obj is int)
			return (int)obj;
		throw new ParseException(locator, $"{name} is not integer");
	}
	public static double? ParseFloat(Locator locator, string name)
	{
		var obj = locator.Source.GetProperty(name);
		if(obj is Undefined)
			return null;
		if(obj is double)
			return (double)obj;
		if(obj is float)
			return (double)obj;
		throw new ParseException(locator, $"{name} is not float");
	}
	public static string? ParseString(Locator locator, string name)
	{
		var obj = locator.Source.GetProperty(name);
		if(obj is Undefined)
			return null;
		if(obj is string)
			return (string)obj;
		throw new ParseException(locator, $"{name} is not string");
	}
	public static bool ParseBool(Locator locator)
	{
		if(locator.Source is bool)
			return (bool)locator.Source;
		throw new ParseException(locator, $"{locator.ObjectName} is not boolean");
	}
	public static int ParseInt(Locator locator)
	{
		if(locator.Source is int)
			return (int)locator.Source;
		throw new ParseException(locator, $"{locator.ObjectName} is not integer");
	}
	public static double ParseFloat(Locator locator)
	{
		if(locator.Source is double)
			return (double)locator.Source;
		if(locator.Source is float)
			return (double)locator.Source;
		throw new ParseException(locator, $"{locator.ObjectName} is not float");
	}
	public static string ParseString(Locator locator)
	{
		if(locator.Source is string)
			return (string)locator.Source;
		throw new ParseException(locator, $"{locator.ObjectName} is not string");
	}
}

/// <summary>
/// Serves as a base class for defining format configurations with support for
/// location attributes such as file path, object path, and object name.
/// </summary>
public class FormatBase : Location
{
	public string ObjectName { get; set; }
	public string ObjectPath { get; set; }
	public string FilePath { get; set; }
	internal FormatBase(Locator locator)
	{
		ObjectName = locator.ObjectName;
		ObjectPath = locator.ObjectPath;
		FilePath = locator.FilePath;
	}
}

/// <summary>
/// Represents a visual format configuration, including properties such as text, font size, color, icon, and background.
/// </summary>
public class VisualFormat : FormatBase
{
	public string? text;
	public int? fontSize;
	public int? color;
	public string? icon;
	public int? background;

	public bool HasIcon => icon != null;
	internal VisualFormat(Locator locator) : base(locator)
	{
	}
	internal static VisualFormat? ParseObject(Locator locator)
	{
		string? text = null;
		int? color = null;
		int? fontSize = null;
		int? background = null;
		string? icon = null;
		try {
			color = Variable.ParseInt(locator, "color");
			background = Variable.ParseInt(locator, "background");;
			fontSize = Variable.ParseInt(locator, "fontSize");
			icon = Variable.ParseString(locator, "icon");
			text = Variable.ParseString(locator, "text");;
			if(text == null && icon == null) 
				throw new ParseException(locator, $"visual text and icon is not defined");
		}
		catch (Exception ex)
		{
			Log.Error(locator, ex.Message);
			return null;
		}
		var ret = new VisualFormat(locator);
		ret.text = text;
		ret.color = color;
		ret.fontSize = fontSize;
		ret.icon = icon;
		ret.background = background;
		return ret;
	}

	internal SKBitmap? LoadImage()
	{
		SKBitmap? bitmap = null;
		try
		{
			if (!string.IsNullOrEmpty(icon))
			{
				var iconFile = AergiaConfig.helper.toAbsolutePathFromSrc(FilePath, icon);
				if (!File.Exists(iconFile))
				{
					throw new ParseException(this, $"icon file '{icon}' is not found.");
				}
				using (var stream = new FileStream(iconFile, FileMode.Open))
				{
					bitmap = SKBitmap.Decode(stream);
				}
			}
		}
		catch (ParseException ex)
		{
			Log.Error(ex.Location, ex.Message);
			return null;
		}
		catch (Exception ex)
		{
			Log.Error(this, ex.Message);
			return null;
		}
		return bitmap;
	}
}

/// <summary>
/// Represents the base implementation for all command format types
/// within the Aergia configuration system, encapsulating
/// common functionality and behaviors required for parsing
/// and handling command IDs.
/// </summary>
public abstract class CommandFormat : FormatBase
{
	public AergiaTypes.CommandId commandId;

	protected CommandFormat(Locator locator, AergiaTypes.CommandId commandId) : base(locator)
	{
		this.commandId = commandId;
	}
	internal static CommandFormat? ParseObject(Locator locator)
	{
		CommandFormat? commandFormat = null;
		var commandName = Variable.ParseString(locator,"command");
		if (commandName == null)
		{
			Log.Error(locator, "command is not defined" );
			return null;
		}

		AergiaTypes.CommandId commandId;
		try
		{
			commandId = (AergiaTypes.CommandId)Enum.Parse(typeof(AergiaTypes.CommandId), commandName, true);
		}
		catch (ArgumentException e)
		{
			throw new ParseException(locator, $"{commandName} is not a valid command");
		}

		try
		{
			switch (commandId)
			{
				case AergiaTypes.CommandId.MouseMove:
					commandFormat = new MouseMoveCommandFormat(locator, commandId);
					break;
				case AergiaTypes.CommandId.MouseTrackingStart:
					commandFormat = new SimpleCommandFormat(locator, commandId);
					break;
				case AergiaTypes.CommandId.MouseTrackingStop:
					commandFormat = new SimpleCommandFormat(locator, commandId);
					break;
				case AergiaTypes.CommandId.MouseTrackingRewind:
					commandFormat = new SimpleCommandFormat(locator, commandId);
					break;
				case AergiaTypes.CommandId.MouseWheel:
					commandFormat = new MouseWheelCommandFormat(locator, commandId);
					break;
				case AergiaTypes.CommandId.MouseClick:
					commandFormat = new ButtonCommandFormat(locator, commandId);
					break;
				case AergiaTypes.CommandId.MouseDoubleClick:
					commandFormat = new ButtonCommandFormat(locator, commandId);
					break;
				case AergiaTypes.CommandId.MouseTripleClick:
					commandFormat = new ButtonCommandFormat(locator, commandId);
					break;
				case AergiaTypes.CommandId.ButtonPress:
					commandFormat = new ButtonCommandFormat(locator, commandId);
					break;
				case AergiaTypes.CommandId.ButtonRelease:
					commandFormat = new ButtonCommandFormat(locator, commandId);
					break;
				case AergiaTypes.CommandId.KeyPress:
					commandFormat = new KeyCommandFormat(locator, commandId);
					break;
				case AergiaTypes.CommandId.KeyRelease:
					commandFormat = new KeyCommandFormat(locator, commandId);
					break;
				case AergiaTypes.CommandId.KeyInput:
					commandFormat = new KeyCommandFormat(locator, commandId);
					break;
				case AergiaTypes.CommandId.MapInput:
					commandFormat = new MapInputCommandFormat(locator, commandId);
					break;
				case AergiaTypes.CommandId.MapAction:
					commandFormat = new MapActionCommandFormat(locator, commandId);
					break;
				case AergiaTypes.CommandId.PageChange:
					commandFormat = new PageChangeCommandFormat(locator, commandId);
					break;
				case AergiaTypes.CommandId.ApplicationChange:
					commandFormat = new ApplicationChangeCommandFormat(locator, commandId);
					break;
				case AergiaTypes.CommandId.Delay:
					commandFormat = new DelayCommandFormat(locator, commandId);
					break;
				case AergiaTypes.CommandId.SetTimer:
					commandFormat = new TimerCommandFormat(locator, commandId);
					break;
				default:
					Log.Error(locator, $"'{commandName}' is not supported command");
					return null;
			}
		}
		catch (ParseException ex)
		{
			Log.Error(ex.Location, ex.Message);
			return null;
		}
		return commandFormat;
	}
}

/// <summary>
/// Represents a specific implementation of a command format in the Aergia configuration framework.
/// This class is used to define and configure commands with a simplified structure.
/// </summary>
public class SimpleCommandFormat : CommandFormat
{
	internal SimpleCommandFormat(Locator locator, AergiaTypes.CommandId commandId) : base(locator, commandId)
	{
	}
}

/// <summary>
/// Represents a command format for mouse move events, managing position (x, y), speed (s),
/// and optional scaling factors (xr, yr).
/// </summary>
public class MouseMoveCommandFormat : CommandFormat
{
	public Variable x;
	public Variable y;
	public Variable s;
	public Variable xr;
	public Variable yr;

	internal MouseMoveCommandFormat(Locator locator, AergiaTypes.CommandId commandId) : base(locator, commandId)
	{
		if (ConfigHelper.IsJsObject(locator.Source))
		{
			var lcX = Variable.Parse(locator, "x");
			var lcY = Variable.Parse(locator, "y");
			var lcS = Variable.Parse(locator, "s");
			var lcXr = Variable.Parse(locator, "xr");
			var lcYr = Variable.Parse(locator, "yr");
			if(lcX != null)
				x = lcX;
			if(lcY != null)
				y = lcY;
			if(lcS != null)
				s = lcS;
			if(lcXr != null)
				xr = lcXr;
			if(lcYr != null)
				yr = lcYr;
		}
		x ??= new Variable("$Joystick_X");
		y ??= new Variable("$Joystick_Y");
		s ??= new Variable(200);
		xr ??= new Variable(1.0);
		yr ??= new Variable(1.0);
	}
}

/// <summary>
/// Represents the format for mouse wheel commands within the configuration.
/// </summary>
public class MouseWheelCommandFormat : CommandFormat
{
	public Variable delta;
	public Variable r;
	
	internal MouseWheelCommandFormat(Locator locator, AergiaTypes.CommandId commandId) : base(locator, commandId)
	{
		if (ConfigHelper.IsJsObject(locator.Source))
		{
			var lcDelta = Variable.Parse(locator, "delta");
			var lcR = Variable.Parse(locator, "r");
			if(lcDelta != null)
				delta = lcDelta;
			if(lcR != null)
				r = lcR;
		}
		delta ??= new Variable("$Wheel_Delta");
		r ??= new Variable(1);
	}
}

/// <summary>
/// Represents a command format associated with a button interaction.
/// </summary>
public class ButtonCommandFormat : CommandFormat
{
	public int button;
	
	internal ButtonCommandFormat(Locator locator, AergiaTypes.CommandId commandId) : base(locator, commandId)
	{
		button = Variable.ParseInt(locator, "button") ?? 1;
	}
}

/// <summary>
/// Represents a specific implementation of a command format
/// designed to handle key commands in a configurable system.
/// </summary>
/// <remarks>
/// This class parses and validates information related to key commands,
/// ensuring proper definition and structure of its input data. It includes
/// functionality to process and initialize values from a given locator source.
/// </remarks>
public class KeyCommandFormat : CommandFormat
{
	public List<Variable> value;
	public Variable interval;
	internal KeyCommandFormat(Locator locator, AergiaTypes.CommandId commandId) : base(locator, commandId)
	{
		value = new List<Variable>();
		if(locator.Source.value is Undefined)
			throw new ParseException(locator, "'value' is not defined");
		if (!ConfigHelper.IsJsArray(locator.Source.value))
			throw new ParseException(locator, "'value' is not JavaScript array");
		foreach (var item in locator.ChildArray("value"))
		{
			var v = Variable.Parse(item);
			if(v != null)
				value.Add(v);
		}
		interval = Variable.Parse(locator, "interval") ?? new Variable(50);
	}
}

/// <summary>
/// Represents the mapping data constructed from a locator instance,
/// determining the mapping operation and associated key based on the locator's source.
/// This class is used to interpret and store comparator operations
/// (such as Less, LessOrEqual, Equal, etc.) and their associated keys,
/// based on the values found in the provided locator.
/// </summary>
public class MapData
{
	public AergiaTypes.MapComparator op;
	public Variable? key;

	internal MapData(Locator locator)
	{
		if (!(locator.Source.lt is Undefined))
		{
			op = AergiaTypes.MapComparator.Less;
			key = new Variable(locator.Source.lt);
		} 
		else if (!(locator.Source.le is Undefined))
		{
			op = AergiaTypes.MapComparator.LessOrEqual;
			key = new Variable(locator.Source.le);
		} 
		else if (!(locator.Source.eq is Undefined))
		{
			op = AergiaTypes.MapComparator.Equal;
			key = new Variable(locator.Source.eq);
		} 
		else if (!(locator.Source.ne is Undefined))
		{
			op = AergiaTypes.MapComparator.NotEqual;;
			key = new Variable(locator.Source.ne);
		} 
		else if (!(locator.Source.ge is Undefined))
		{
			op = AergiaTypes.MapComparator.GreaterOrEqual;
			key = new Variable(locator.Source.ge);
		}
		else if (!(locator.Source.gt is Undefined))
		{
			op = AergiaTypes.MapComparator.Greater;
			key = new Variable(locator.Source.gt);
		}
		else if (!(locator.Source.lt is Undefined))
		{
			op = AergiaTypes.MapComparator.Less;
			key = new Variable(locator.Source.lt);
		}
		else
		{
			op = AergiaTypes.MapComparator.Other;
			key = null;
		}
	}
}

/// <summary>
/// Represents the input mapping data extracted from a configuration source.
/// </summary>
/// <remarks>
/// <para>
/// This class is used to encapsulate a list of variables that are parsed and validated
/// from the configuration source. It ensures that the input data is correctly formatted
/// as a JavaScript array and contains valid variable definitions.
/// </para>
/// <para>
/// If the configuration does not adhere to the expected structure or if mandatory fields
/// are missing, exceptions are thrown during initialization to prevent further processing
/// with invalid data.
/// </para>
/// </remarks>
public class MapInputData : MapData
{
	public List<Variable> value;

	internal MapInputData(Locator locator) : base(locator)
	{
		value = new List<Variable>();
		if(locator.Source.value is Undefined)
			throw new ParseException(locator, "value is not defined");
		if (!ConfigHelper.IsJsArray(locator.Source.value))
			throw new ParseException(locator, "'value' is not JavaScript array");
		foreach (var item in locator.ChildArray("value"))
		{
			var v = Variable.Parse(item);
			if(v != null)
				value.Add(v);
		}
	}
}

/// <summary>
/// Defines the format used to map input commands for processing.
/// </summary>
public class MapInputCommandFormat : CommandFormat
{
	public Variable key;
	public List<MapInputData> map;
	
	internal MapInputCommandFormat(Locator locator, AergiaTypes.CommandId commandId) : base(locator, commandId)
	{
		map = new List<MapInputData>();
		var key = Variable.Parse(locator, "key");
		if(key == null)
			throw new ParseException(locator, "'key' is not defined");
		this.key = key;
		if(locator.Source.map is Undefined)
			throw new ParseException(locator, "'map' is not defined");
		foreach (var item in locator.ChildArray("map"))
		{
			map.Add(new MapInputData(item));
		}
	}
}

/// <summary>
/// Represents mapping data associated with an action in the configuration.
/// </summary>
public class MapActionData : MapData
{
	public ActionCommandFormat action;

	public MapActionData(Locator locator) : base(locator)
	{
		action = ActionCommandFormat.ParseObject(locator);
	}
}

/// <summary>
/// Represents a command format for mapping an action to multiple command data elements.
/// </summary>
/// <remarks>
/// This class is designed to parse and handle configurations where a key is associated with
/// a set of mapped actions. It validates the presence of required properties ('key' and 'map')
/// in the source configuration and initializes a collection of map action data elements.
/// </remarks>
public class MapActionCommandFormat : CommandFormat
{
	public Variable key;
	public List<MapActionData> map;
	
	internal MapActionCommandFormat(Locator locator, AergiaTypes.CommandId commandId) : base(locator, commandId)
	{
		map = new List<MapActionData>();
		var key = Variable.Parse(locator, "key");
		if(key == null)
			throw new ParseException(locator, "'key' is not defined");
		this.key = key;
		if(locator.Source.map is Undefined)
			throw new ParseException(locator, "'map' is not defined");
		foreach (var item in locator.ChildArray("map"))
		{
			map.Add(new MapActionData(item));
		}
	}
}

/// <summary>
/// Represents a command format for handling page changes within the configuration system.
/// </summary>
public class PageChangeCommandFormat : CommandFormat
{
	public string? page;
	
	internal PageChangeCommandFormat(Locator locator, AergiaTypes.CommandId commandId) : base(locator, commandId)
	{
		page = Variable.ParseString(locator, "page");
	}
}

/// <summary>
/// Represents a command format for changing the application context or application-specific settings.
/// </summary>
public class ApplicationChangeCommandFormat : CommandFormat
{
	public string? application;
	public string? page;
	
	internal ApplicationChangeCommandFormat(Locator locator, AergiaTypes.CommandId commandId) : base(locator, commandId)
	{
		application = Variable.ParseString(locator, "application");
		page = Variable.ParseString(locator, "page");
	}
}

/// <summary>
/// Represents a command format that introduces a delay within a sequence of commands, using a specified timeout duration.
/// </summary>
public class DelayCommandFormat : CommandFormat
{
	public uint timeout; 
	internal DelayCommandFormat(Locator locator, AergiaTypes.CommandId commandId) : base(locator, commandId)
	{
		var timeout = Variable.ParseInt(locator, "wait");
		if (timeout == null)
			throw new ParseException(locator, "'timeout' is not defined");
		if (timeout < 0)
			timeout = 0;
		if(timeout > UInt16.MaxValue)
			throw new ParseException(locator, "'timeout' is overflow");
		this.timeout = (uint)timeout;
	}
}

/// <summary>
/// Represents a specialized command format that includes timer-related functionality,
/// such as defining a target, timeout duration, and optional associated data.
/// </summary>
public class TimerCommandFormat : CommandFormat
{
	public int target;
	public uint timeout;
	public Variable? data;
	
	internal TimerCommandFormat(Locator locator, AergiaTypes.CommandId commandId) : base(locator, commandId)
	{
		int? target = Variable.ParseInt(locator, "target");
		if (target == null)
			throw new ParseException(locator, "'target' is not defined");
		this.target = (int)target;

		var timeout = Variable.ParseInt(locator, "wait");
		if (timeout == null)
			throw new ParseException(locator, "'timeout' is not defined");
		if(timeout < 0)
			timeout = 0;
		if(timeout > UInt16.MaxValue)
			throw new ParseException(locator, "'timeout' is overflow");
		this.timeout = (uint)timeout;
		data = Variable.Parse(locator, "data");
	}
}

/// <summary>
/// Represents a format for defining a set of commands that constitute an actionable interaction.
/// </summary>
public class ActionCommandFormat : FormatBase
{
	public List<CommandFormat>? commands;

	internal ActionCommandFormat(Locator locator) : base(locator)
	{
	}

	internal static ActionCommandFormat? ParseObject(Locator locator)
	{
		var hasError = false;
		var commands = new List<CommandFormat>();
		if (!(locator.Source.command is Undefined)) 
		{
			if( !(locator.Source.commands is Undefined))
				throw new ParseException(locator, "'command' and 'commands' are defined");
			var command = CommandFormat.ParseObject(locator);
			if (command == null)
				hasError = true;
			else
				commands.Add(command);
		}
		else
		{
			foreach (var actionLocator in locator.ChildArray("commands"))
			{
				var command = CommandFormat.ParseObject(actionLocator);	
				if (command == null)
					hasError = true;
				if(!hasError)
					commands.Add(command!);
			}
		}
		if (hasError)
			return null;

		var actionCommand = new ActionCommandFormat(locator);
		actionCommand.commands = commands;
		return actionCommand;
	}
}

/// <summary>
/// Defines the structure and metadata for an action within the configuration system.
/// Encapsulates visual properties and mappings of events to command formats.
/// </summary>
public class ActionFormat : FormatBase
{
	public VisualFormat? visual;
	public OrderedDictionary<AergiaTypes.EventId, ActionCommandFormat> eventMapAction;

	internal ActionFormat(Locator locator, VisualFormat?  visual, OrderedDictionary<AergiaTypes.EventId, ActionCommandFormat> eventMapAction) : base(locator)
	{
		this.visual = visual;
		this.eventMapAction = eventMapAction;
	}
	internal static ActionFormat? ParseObject(Locator locator)
	{
		var hasError = false;
		VisualFormat? visual = null;
		ActionCommandFormat? actionCommand = null;
		var eventMaps = new OrderedDictionary<AergiaTypes.EventId, ActionCommandFormat>();
		try
		{
			var visualLocator = locator.Child("visual");
			if (visualLocator != null)
			{
				visual = VisualFormat.ParseObject(visualLocator);
			}

			if (!(locator.Source.command is Undefined) || !(locator.Source.commands is Undefined))
			{
				actionCommand = ActionCommandFormat.ParseObject(locator);
			}

			foreach (var propertyLocator in locator.Children())
			{
				if (propertyLocator.ObjectName == "location")
					continue;
				if (propertyLocator.ObjectName.StartsWith("event_"))
				{
					if (actionCommand != null)
					{
						throw new ParseException(locator, "'actions' and event map actions are defined");
					}
					var eventName = ((string)propertyLocator.ObjectName)[6..];
					AergiaTypes.EventId eventId;
					try
					{
						eventId = (AergiaTypes.EventId)Enum.Parse(typeof(AergiaTypes.EventId), eventName, true);
					}
					catch (ArgumentException e)
					{
						throw new ParseException(propertyLocator, $"event '{eventName}' is unknown");
					}
					var bindAction = ActionCommandFormat.ParseObject(propertyLocator);
					if(bindAction == null)
						hasError = true;
					if(!hasError)
						eventMaps.Add(eventId, bindAction!);
				}
			}

			if (eventMaps.Count == 0)
			{
				if (actionCommand == null)
					throw new ParseException(locator, "'actions' is not defined");
			}
			else
			{
				if(actionCommand != null)
					throw new ParseException(locator, "'actions' and event map actions are defined");
			}
		}
		catch (ParseException e)
		{
			Log.Error(e.Location, e.Message);
			return null;
		}

		if (hasError)
			return null;
		if(actionCommand != null)
			eventMaps.Add(AergiaTypes.EventId.UnNamed, actionCommand);
		return new ActionFormat(locator, visual, eventMaps);
	}
}

/// <summary>
/// Represents a format for binding events to specific actions within the configuration system.
/// </summary>
public class EventBinderFormat : FormatBase
{
	public List<string> actions;

	internal EventBinderFormat(Locator locator, List<string> actions) : base(locator)
	{
		this.actions = actions;
	}

	public static EventBinderFormat? parseObject(Locator locator)
	{
		List<string> actionList = new List<string>();
		if (!ConfigHelper.IsJsObject(locator.Source))
		{
			if (!(locator.Source is string))
			{
				Log.Error(locator, $"{locator.ObjectName} is not string");
				return null;
			}
			var actionDef = locator.Source;
			var actionSeg = ((string)actionDef).Split(",");
			foreach (var action in actionSeg)
			{
				actionList.Add(action.Trim());
			}
		}
		else
		{
			foreach (var eventLocator in locator.Children())
			{
				if (!(eventLocator.Source is string))
				{
					Log.Error(eventLocator, $"{eventLocator.ObjectName} is not string");
					return null;
				}
				var actionDef = eventLocator.Source;
				var actionSeg = ((string)actionDef).Split(",");
				foreach (var action in actionSeg)
				{
					actionList.Add(action.Trim());
				}
			}
		}
		return new EventBinderFormat(locator,actionList);
	}
}

/// <summary>
/// Represents a format for binding control events to specific actions.
/// </summary>
public class ControlBindFormat : FormatBase
{
	public OrderedDictionary<AergiaTypes.EventId, EventBinderFormat> binds;

	internal ControlBindFormat(Locator locator) : base(locator)
	{
	}
	internal static ControlBindFormat? ParseObject(Locator locator)
	{
		var hasError = false;
		OrderedDictionary<AergiaTypes.EventId,EventBinderFormat> eventMap = new OrderedDictionary<AergiaTypes.EventId,EventBinderFormat>();
		try
		{
			if (!ConfigHelper.IsJsObject(locator.Source))
			{
				var eventBind = EventBinderFormat.parseObject(locator);
				if (eventBind == null)
					hasError = true;
				else
					eventMap.Add(AergiaTypes.EventId.UnNamed, eventBind);
			}
			else if (ConfigHelper.IsJsArray(locator.Source))
			{
				EventBinderFormat? eventBindRef = null; 
				foreach (var eventLocator in locator.Array())
				{
					var eventBind = EventBinderFormat.parseObject(eventLocator);
					if(eventBind == null)
						hasError = true;
					if(!hasError)
					{
						if (eventBindRef == null)
							eventBindRef = eventBind;
						else
							eventBindRef.actions.AddRange(eventBind.actions);
					}
				}
				if (eventBindRef != null && !hasError) 
					eventMap.Add(AergiaTypes.EventId.UnNamed, eventBindRef);
			}
			else {
				foreach (var eventLocator in locator.Children())
				{
					AergiaTypes.EventId eventId;
					try
					{
						eventId = (AergiaTypes.EventId)Enum.Parse(typeof(AergiaTypes.EventId), (string)eventLocator.ObjectName, true);
					}
					catch (ArgumentException e)
					{
						throw new ParseException(eventLocator, $"event '{(string)eventLocator.ObjectName}' is unknown");
					}

					var eventBind = EventBinderFormat.parseObject(eventLocator);
					if (eventBind == null)
						hasError = true;
					else
						eventMap.Add(eventId, eventBind);
				}
			}
		}
		catch (ParseException e)
		{
			Log.Error(e.Location, e.Message);
			hasError = true;
		}
		catch (Exception e)
		{
			Log.Error(locator, e.Message);
			hasError = true;
		}
		if(hasError)
			return null;
		var ret = new ControlBindFormat(locator);
		ret.binds = eventMap;
		return ret;
	}
}

/// <summary>
/// Represents a format for defining page bindings, providing the structure and controls associated with a specific page.
/// </summary>
/// <remarks>
/// This class is responsible for managing visual settings and control bindings for a page.
/// The format associates unique identifiers for controls with their corresponding binding definitions.
/// </remarks>
public class BindPageFormat : FormatBase
{
	public VisualFormat? visual;
	public OrderedDictionary<AergiaTypes.ControlId, ControlBindFormat> binds { get; set; }

	internal BindPageFormat(Locator locator) : base(locator)
	{
	}
	public static BindPageFormat? ParseObject(Locator locator)
	{
		bool hasError = false;
		VisualFormat? visual = null;
		OrderedDictionary<AergiaTypes.ControlId, ControlBindFormat> binds = new OrderedDictionary<AergiaTypes.ControlId, ControlBindFormat>();
		try
		{
			foreach (var bindLocator in locator.Children())
			{
				if (bindLocator.ObjectName == "visual")
				{
					visual = VisualFormat.ParseObject(bindLocator);
					if (visual == null)
						hasError = true;
				}
				else
				{
					AergiaTypes.ControlId controlId;
					try
					{
						controlId = (AergiaTypes.ControlId)Enum.Parse(typeof(AergiaTypes.ControlId),
							bindLocator.ObjectName,true);
					}
					catch (ArgumentException e)
					{
						throw new ParseException(bindLocator, $"control '{(string)bindLocator.ObjectName}' is unknown");
					}
					var bind = ControlBindFormat.ParseObject(bindLocator);
					if (bind == null)
						hasError = true;
					if (!hasError)
						binds.Add(controlId, bind);
				}
			}
		}
		catch (ParseException e)
		{
			Log.Error(e.Location, e.Message);
			hasError = true;
		}
		if(hasError)
			return null;
		var ret = new BindPageFormat(locator);
		ret.visual = visual;
		ret.binds = binds;
		return ret;
	}
}

/// <summary>
/// Represents the format for an application configuration.
/// </summary>
/// <remarks>
/// This class handles the definition of application-specific configurations,
/// including device mode, associated visual configurations, action mappings,
/// and bind pages.
/// </remarks>
public class ApplicationFormat : FormatBase
{
	public VisualFormat? visual;
	public OrderedDictionary<string, ActionFormat> actions { get; set; }
	public OrderedDictionary<string, BindPageFormat> pages { get; set; }

	internal ApplicationFormat(Locator locator) : base(locator)
	{
	}

	internal static ApplicationFormat? ParseObject(Locator locator)
	{
		var hasError = false;
		VisualFormat? visual = null;
		var actions = new OrderedDictionary<string,ActionFormat>(); 
		var pages = new OrderedDictionary<string, BindPageFormat>();
		
		var appricationLocator = locator.Child("application");
		try
		{
			if (appricationLocator == null)
				throw new ParseException(locator, $"'application' is not defined");

			var visualLocator = appricationLocator.Child("visual");
			if (visualLocator != null)
			{
				visual = VisualFormat.ParseObject(visualLocator);
				if(visual == null)
					hasError = true;
			}

			var actionsLocator = appricationLocator.Child("actions");
			if (actionsLocator != null)
			{
				foreach (var actionLocator in actionsLocator.Children())
				{
					var action = ActionFormat.ParseObject(actionLocator);
					if(action == null)
						hasError = true;
					if(!hasError)
						actions.Add(actionLocator.ObjectName, action);
				}
			}

			var bindsLocator = locator.Child("binds");
			if (bindsLocator != null)
			{
				foreach (var bindLocator in bindsLocator.Children())
				{
					var bind = BindPageFormat.ParseObject(bindLocator);
					if(bind == null)
						hasError = true;
					if(!hasError)
						pages.Add(bindLocator.ObjectName, bind);
				}
			}
		}
		catch (ParseException e)
		{
			Log.Error(e.Location, e.Message);
			return null;
		}
		if(hasError)
			return null;

		var ret = new ApplicationFormat(appricationLocator);
		ret.visual = visual;
		ret.actions = actions;
		ret.pages = pages;
		return ret;
	}
}

/// <summary>
/// Represents a specialized format for configuring text capabilities, including specific pixel formats.
/// </summary>
/// <remarks>
/// This class extends the base configuration format and is used for parsing and defining text-related
/// display capabilities such as the pixel format. It integrates with the locator system to handle configuration data.
/// </remarks>
public class TextCapabilitiesFormat : FormatBase
{
	public AergiaTypes.PixelFormat pixelFormat;

	internal TextCapabilitiesFormat(Locator locator, AergiaTypes.PixelFormat pixelFormat) : base(locator)
	{
		this.pixelFormat = pixelFormat;
	}

	public static TextCapabilitiesFormat? ParseObject(Locator locator)
	{
		var hasError = false;
		AergiaTypes.PixelFormat pixelFormat = AergiaTypes.PixelFormat.Rgb888;
		try
		{
			string? pixelFormatName = Variable.ParseString(locator, "pixelFormat");
			if (pixelFormatName == null)
			{
				Log.Error(locator, "'pixelFormat' is not defined");
				hasError = true;
			}
			else
			{
				pixelFormat = (AergiaTypes.PixelFormat)Enum.Parse(typeof(AergiaTypes.PixelFormat), pixelFormatName, true);
			}
		}
		catch (ParseException e)
		{
			Log.Error(e.Location, e.Message);
			hasError = true;
		}
		catch (Exception e)
		{
			Log.Error(locator, e.Message);
			hasError = true;
		}
		if(hasError)
			return null;
		
		var ret = new TextCapabilitiesFormat(locator, pixelFormat);
		return ret;
	}
}

/// <summary>
/// Represents the format for defining the capabilities of an icon within the configuration system.
/// This includes attributes such as pixel format and resolution.
/// </summary>
public class IconCapabilitiesFormat : FormatBase
{
	public AergiaTypes.PixelFormat pixelFormat;
	public int horzResolution;
	public int vertResolutiont;

	IconCapabilitiesFormat(Locator locator, AergiaTypes.PixelFormat pixelFormat, int horzResolution, int vertResolutiont) : base(locator)
	{
		this.pixelFormat = pixelFormat;
		this.horzResolution = horzResolution;
		this.vertResolutiont = vertResolutiont;
	}
	public static IconCapabilitiesFormat? ParseObject(Locator locator)
	{
		bool hasError = false;
		AergiaTypes.PixelFormat pixelFormat = AergiaTypes.PixelFormat.Rgb888;
		int? h = null;
		int? v = null;
		try
		{
			string? pixelFormatName = Variable.ParseString(locator, "pixelFormat");
			if (pixelFormatName == null)
				throw new ParseException(locator, "'pixelFormat' is not defined");
			pixelFormat = (AergiaTypes.PixelFormat)Enum.Parse(typeof(AergiaTypes.PixelFormat), pixelFormatName, true);
			var resolutionLocator = locator.Child("resolution");
			if (resolutionLocator == null)
				throw new ParseException(locator, "'resolution' is not defined");
			h = Variable.ParseInt(resolutionLocator, "h");
			if (h == null)
				throw new ParseException(resolutionLocator, "'h' is not defined");
			v = Variable.ParseInt(resolutionLocator, "v");
			if (v == null)
				throw new ParseException(resolutionLocator, "'v' is not defined");
		}
		catch (ParseException e)
		{
			Log.Error(e.Location, e.Message);
			hasError = true;
		}
		catch (Exception e)
		{
			Log.Error(locator, e.Message);
			hasError = true;
		}
		if(hasError)
			return null;
		return new IconCapabilitiesFormat(locator, pixelFormat, (int)h, (int)v);
	}
}

/// <summary>
/// Represents the format and capabilities configuration for background rendering,
/// including the specified pixel format.
/// </
public class BackgroundCapabilitiesFormat : FormatBase
{
	public AergiaTypes.PixelFormat pixelFormat;

	internal BackgroundCapabilitiesFormat(Locator locator, AergiaTypes.PixelFormat pixelFormat) : base(locator)
	{
		this.pixelFormat = pixelFormat;
	}

	public static BackgroundCapabilitiesFormat? ParseObject(Locator locator)
	{
		bool hasError = false;
		AergiaTypes.PixelFormat pixelFormat = AergiaTypes.PixelFormat.Rgb888;
		try
		{
			string? pixelFormatName = Variable.ParseString(locator, "pixelFormat");
			if (pixelFormatName == null)
				throw new ParseException(locator, "'pixelFormat' is not defined");
			pixelFormat = (AergiaTypes.PixelFormat)Enum.Parse(typeof(AergiaTypes.PixelFormat), pixelFormatName, true);
		}
		catch (ParseException e)
		{
			Log.Error(e.Location, e.Message);
			hasError = true;
		}
		catch (Exception e)
		{
			Log.Error(locator, e.Message);
			hasError = true;
		}
		if(hasError)
			return null;
		var ret = new BackgroundCapabilitiesFormat(locator, pixelFormat);
		return ret;
	}
}

/// <summary>
/// Represents the visual capabilities format, which contains configuration and data related to text,
/// icon, and background visual capabilities in a configurable system.
/// </summary>
public class VisualCapabilitiesFormat : FormatBase
{
	public TextCapabilitiesFormat? textCapabilities;
	public IconCapabilitiesFormat? iconCapabilities;
	public BackgroundCapabilitiesFormat? backgroundCapabilities;

	internal VisualCapabilitiesFormat(Locator locator) : base(locator)
	{
	}

	public static VisualCapabilitiesFormat? ParseObject(Locator locator)
	{
		var hasError = false;
		BackgroundCapabilitiesFormat? backgroundCapabilities = null;
		IconCapabilitiesFormat? iconCapabilities = null;
		TextCapabilitiesFormat? textCapabilities = null;
		try
		{
			var backgroundLocation = locator.Child("background");
			if (backgroundLocation != null)
			{
				backgroundCapabilities = BackgroundCapabilitiesFormat.ParseObject(backgroundLocation);
				if (backgroundCapabilities == null)
					hasError = true;
			}

			var iconLocation = locator.Child("icon");
			if (iconLocation != null)
			{
				iconCapabilities = IconCapabilitiesFormat.ParseObject(iconLocation);
				if(iconCapabilities == null)
					hasError = true;
			}

			var textLocation = locator.Child("text");
			if (textLocation != null)
			{
				textCapabilities = TextCapabilitiesFormat.ParseObject(textLocation);
				if(textCapabilities == null)
					hasError = true;
			}
		}
		catch (ParseException e)
		{
			Log.Error(e.Location, e.Message);
			return null;
		}
		if(hasError) 
			return null;
		var ret = new VisualCapabilitiesFormat(locator);
		ret.textCapabilities = textCapabilities;
		ret.iconCapabilities = iconCapabilities;
		ret.backgroundCapabilities = backgroundCapabilities;
		return ret;
	}
}

/// <summary>
/// Represents the format for defining a control, including visual capabilities and events.
/// </summary>
public class ControlFormat : FormatBase
{
	public VisualCapabilitiesFormat? visualCapabilities;
	public List<AergiaTypes.EventId>? events;
	public List<string> variables;

	internal ControlFormat(Locator locator) : base(locator)
	{
	}

	public static ControlFormat? ParseObject(Locator locator)
	{
		var hasError = false;
		VisualCapabilitiesFormat? visualCapabilities = null;
		var events =  new List<AergiaTypes.EventId>();
		var variables =  new List<string>();
		try
		{
			var visualLocator = locator.Child("visualCapabilities");
			if (visualLocator != null)
			{
				visualCapabilities = VisualCapabilitiesFormat.ParseObject(visualLocator);
				if (visualCapabilities == null)
					hasError = true;
			}

			if (!(locator.Source.events is Undefined))
			{
				foreach (var eventLocator in locator.ChildArray("events"))
				{
					if (!(eventLocator.Source is string))
						throw new ParseException(eventLocator, "'event' type is not string");
					var eventName = ((string)eventLocator.Source).Trim();
					AergiaTypes.EventId eventId;
					try
					{
						eventId = (AergiaTypes.EventId)Enum.Parse(typeof(AergiaTypes.EventId),eventName, true);
					}
					catch (ArgumentException e)
					{
						throw new ParseException(eventLocator, $"event '{eventName}' is unknown");
					}
					events.Add(eventId);
				}
			}
			if (!(locator.Source.variables is Undefined))
			{
				foreach (var varLocator in locator.ChildArray("variables"))
				{
					if (!(varLocator.Source is string))
						throw new ParseException(varLocator, "'variables' type is not string");
					var varName = ((string)varLocator.Source).Trim();
					variables.Add(varName);
				}
			}
		}
		catch (ParseException e)
		{
			Log.Error(e.Location, e.Message);
			return null;
		}
		if(hasError)
			return null;
		var ret = new ControlFormat(locator);
		ret.visualCapabilities = visualCapabilities;
		ret.events = events;
		ret.variables = variables;
		return ret;
	}
}

/// <summary>
/// Defines the format or structure of a device used in a system or application.
/// </summary>
public class DeviceFormat : FormatBase
{
	public string model;
	public string lang;
	public OrderedDictionary<AergiaTypes.ControlId, ControlFormat> controls;

	internal DeviceFormat(Locator locator) : base(locator)
	{
	}

	internal static DeviceFormat? ParseObject(Locator locator)
	{
		var hasError = false;
		string? model = null;
		string? lang = null;
		var controls = new OrderedDictionary<AergiaTypes.ControlId, ControlFormat>();
		try
		{
			model = Variable.ParseString(locator, "model");
			if (model == null)
			{
				Log.Error(locator, "'model' is not defined");
				hasError = true;
			}
			lang = Variable.ParseString(locator, "lang");
			if (lang == null)
				lang = "us";

			var controlsLocator = locator.Child("controls");
			if (controlsLocator != null)
			{
				foreach (var controlLocator in controlsLocator.Children())
				{
					var control = ControlFormat.ParseObject(controlLocator);
					if(control == null)
						hasError = true;
					AergiaTypes.ControlId id = AergiaTypes.ControlId.Unknown;
					try
					{
						id = (AergiaTypes.ControlId)Enum.Parse(typeof(AergiaTypes.ControlId), controlLocator.ObjectName,
							true);
					}
					catch (ArgumentException e)
					{
						Log.Error(controlLocator, $"control '{controlLocator.ObjectName}' is unknown");
						hasError = true;
					}
					if(!hasError)
						controls.Add(id, control!);
				}
			}
		}
		catch (ParseException e)
		{
			Log.Error(e.Location, e.Message);
			hasError = true;
		}
		if (hasError)
			return null;

		var ret = new DeviceFormat(locator);
		ret.controls = controls;
		ret.model = model;
		ret.lang = lang;
		return ret;
	}
}

/// <summary>
/// Represents the primary configuration for the Aergia system, handling devices and applications.
/// This class provides methods for parsing and managing configuration data from a specified locator.
/// </summary>
public class AergiaConfig : FormatBase
{
	public static ConfigHelper helper;
	public DeviceFormat device;
	public OrderedDictionary<string, ApplicationFormat> applications;

	internal AergiaConfig(Locator locator) : base(locator)
	{
	}

	public static AergiaConfig? ParseObject(Locator locator)
	{
		var hasError = false;
		DeviceFormat? device = null;
		var applications = new OrderedDictionary<string, ApplicationFormat>();
		try
		{
			var deviceLocator = locator.Child("device");
			if (deviceLocator == null)
			{
				throw new ParseException(locator, "'device' is not defined");
			}
			device = DeviceFormat.ParseObject(deviceLocator);
			if(device == null)
				hasError = true;
			var applicationsLocator = locator.Child("applications");
			if (applicationsLocator == null)
			{
				throw new ParseException(locator, "'applications' is not defined");
			}
			foreach (var appLocatior in applicationsLocator.Children())
			{
				var application = ApplicationFormat.ParseObject(appLocatior);
				if(application == null)
					hasError = true;
				if(!hasError)
					applications.Add(appLocatior.ObjectName, application);
			}
		}
		catch (ParseException e)
		{
			Log.Error(e.Location, e.Message);
			hasError = true;
		}
		if (hasError)
			return null;

		var ret = new AergiaConfig(locator);
		ret.device = device;
		ret.applications = applications;
		return ret;
	}
}
