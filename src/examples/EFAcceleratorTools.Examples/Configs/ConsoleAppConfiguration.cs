
namespace EFAcceleratorTools.Examples.Configs;
public class ConsoleAppConfiguration
{
    public ConnectionConfiguration ConnectionConfiguration { get; set; }

    public ConsoleAppConfiguration()
    {
        ConnectionConfiguration = new ConnectionConfiguration();
    }
}

public class ConnectionConfiguration
{
    public int MaxTimeoutInSeconds { get; set; }

    public ConnectionStrings ConnectionStrings { get; set; }

    public ConnectionConfiguration()
    {
        ConnectionStrings = new ConnectionStrings();
    }
}

public class ConnectionStrings
{
    public string DefaultConnection { get; set; }

    public ConnectionStrings()
    {
        DefaultConnection = string.Empty;
    }
}
