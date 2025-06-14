namespace AergiaConfigurator;

public class AergiaTypes
{
	public enum PixelFormat
	{
		Rgb888,
		Rgb565,
		BW1
	}

	public enum MapComparator
	{
		Other = 0,
		Equal = 1,
		NotEqual = 2,
		Less = 3,
		LessOrEqual = 4,
		Greater = 5,
		GreaterOrEqual = 6,
	}
	
	public enum ControlId
	{
		Unknown = 0,
		KeySwitch0 = 0x8000,
		KeySwitch1 = 0x8001,
		KeySwitch2 = 0x8002,
		KeySwitch3 = 0x8003,
		KeySwitch4 = 0x8004,
		KeySwitch5 = 0x8005,
		KeySwitch6 = 0x8006,
		KeySwitch7 = 0x8007,
		KeySwitch8 = 0x8008,
		KeySwitch9 = 0x8009,
		KeySwitch10 = 0x800a,
		KeySwitch11 = 0x800b,
		KeySwitch12 = 0x800c,
		KeySwitch13 = 0x800d,
		KeySwitch14 = 0x800e,
		KeySwitch15 = 0x800f,
		Button0 = 0x8010,
		Button1 = 0x8011,
		Joystick = 0x8012,
		Wheel = 0x8013,
		Display = 0x8014,
		Ranging = 0x8015,
		Main = 0x8016
	}

	public enum VariableId
	{
		KeySwitch0_Status		= 0x8100,
		KeySwitch0_TimerData	= 0x8101,
		KeySwitch1_Status		= 0x8102,
		KeySwitch1_TimerData	= 0x8103,
		KeySwitch2_Status		= 0x8104,
		KeySwitch2_TimerData	= 0x8105,
		KeySwitch3_Status		= 0x8106,
		KeySwitch3_TimerData	= 0x8107,
		KeySwitch4_Status		= 0x8108,
		KeySwitch4_TimerData	= 0x8109,
		KeySwitch5_Status		= 0x810a,
		KeySwitch5_TimerData	= 0x810b,
		KeySwitch6_Status		= 0x810c,
		KeySwitch6_TimerData	= 0x810d,
		KeySwitch7_Status		= 0x810e,
		KeySwitch7_TimerData	= 0x810f,
		KeySwitch8_Status		= 0x8110,
		KeySwitch8_TimerData	= 0x8111,
		KeySwitch9_Status		= 0x8112,
		KeySwitch9_TimerData	= 0x8113,
		KeySwitch10_Status		= 0x8114,
		KeySwitch10_TimerData	= 0x8115,
		KeySwitch11_Status		= 0x8116,
		KeySwitch11_TimerData	= 0x8117,
		KeySwitch12_Status		= 0x8118,
		KeySwitch12_TimerData	= 0x8119,
		KeySwitch13_Status		= 0x811a,
		KeySwitch13_TimerData	= 0x811b,
		KeySwitch14_Status		= 0x811c,
		KeySwitch14_TimerData	= 0x811d,
		KeySwitch15_Status		= 0x811e,
		KeySwitch15_TimerData	= 0x811f,
		Button0_Status			= 0x8120,
		Button0_TimerData		= 0x8121,
		Button1_Status			= 0x8122,
		Button1_TimerData		= 0x8123,
		Joystick_X				= 0x8124,
		Joystick_Y				= 0x8125,
		Joystick_Z				= 0x8126,
		Joystick_MX				= 0x8127,
		Joystick_MY				= 0x8128,
		Joystick_MZ				= 0x8129,
		Joystick_Button			= 0x812a,
		Joystick_TimerData		= 0x812b,
		Wheel_Delta				= 0x812c,
		Wheel_TimerData			= 0x812d,
		Display_TimerData		= 0x812e,
		Ranging_Status			= 0x812f,
		Ranging_Distance		= 0x8130,
		Ranging_TimerData		= 0x8131,
		Main_TimerData			= 0x8132
	}

	public enum EventId
	{
		UnNamed		= 0x8200,
		KeyDown		= 0x8201,
		KeyUp		= 0x8202,
		KeyInput	= 0x8203,
		LongPress	= 0x8204,
		BeginMove	= 0x8205,
		Move		= 0x8206,
		EndMove		= 0x8207,
		BeginRotate = 0x8208,
		Rotate		= 0x8209,
		EndRotate	= 0x820a,
		BeginWheel	= 0x820b,
		Wheel		= 0x820c,
		EndWheel	= 0x820d,
		Enter		= 0x820e,
		Leave		= 0x820f,
		Load		= 0x8210,
		Connect		= 0x8211,
		Disconnect	= 0x8212,
		Timer		= 0x8213
	}

	public enum CommandId
	{
		MouseMove = 0x8401,
		MouseTrackingStart = 0x8402,
		MouseTrackingStop  = 0x8403,
		MouseTrackingRewind  = 0x8404,
		MouseWheel  = 0x8405,
		MouseClick  = 0x8406,
		MouseDoubleClick  = 0x8407,
		MouseTripleClick  = 0x8408,
		ButtonPress  = 0x8409,
		ButtonRelease  = 0x840a,
		KeyPress  = 0x840b,
		KeyRelease  = 0x840c,
		KeyInput  = 0x840d,
		MapInput  = 0x840e,
		MapAction  = 0x840f,
		PageChange  = 0x8410,
		ApplicationChange  = 0x8411,
		Delay  = 0x8412,
		SetTimer  = 0x8413
	}
}