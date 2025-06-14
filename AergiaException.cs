namespace AergiaConfigurator;

public class AergiaException : Exception
{
	public Location Location;
	public AergiaException(Location location, string message) : base(message)
	{
		Location = location;
	}
}