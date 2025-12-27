using GarminFitUploader;
using Microsoft.Extensions.Configuration;

Console.WriteLine("=== Garmin FIT File Uploader (.NET 9) ===");
Console.WriteLine();

// Load configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var garminConfig = configuration.GetSection("Garmin").Get<GarminConfig>() ?? new GarminConfig();

// Get credentials
string username = garminConfig.Username;
string password = garminConfig.Password;

if (string.IsNullOrEmpty(username))
{
    Console.Write("Garmin Username: ");
    username = Console.ReadLine() ?? string.Empty;
}

if (string.IsNullOrEmpty(password))
{
    Console.Write("Garmin Password: ");
    // Check if console is interactive
    if (Console.IsInputRedirected)
    {
        password = Console.ReadLine() ?? string.Empty;
    }
    else
    {
        password = ReadPassword();
        Console.WriteLine();
    }
}

if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
{
    Console.WriteLine("Error: Username and password are required.");
    return 1;
}

// Get FIT file path
string fitFilePath;
if (args.Length > 0)
{
    fitFilePath = args[0];
}
else
{
    Console.Write("Path to FIT file: ");
    fitFilePath = Console.ReadLine() ?? string.Empty;
}

if (string.IsNullOrEmpty(fitFilePath))
{
    Console.WriteLine("Error: FIT file path is required.");
    Console.WriteLine();
    Console.WriteLine("Usage: dotnet run <fit-file-path>");
    Console.WriteLine("   or: dotnet run");
    return 1;
}

// Upload the file
using var uploader = new GarminConnectUploader();

var authenticated = await uploader.AuthenticateAsync(username, password);
if (!authenticated)
{
    Console.WriteLine("Authentication failed. Please check your credentials.");
    return 1;
}

Console.WriteLine();
var result = await uploader.UploadFitFileAsync(fitFilePath);

Console.WriteLine();
Console.WriteLine("=== Upload Result ===");
Console.WriteLine($"Success: {result.Success}");
Console.WriteLine($"Message: {result.Message}");

if (result.ActivityId.HasValue)
{
    Console.WriteLine($"Activity ID: {result.ActivityId}");
    Console.WriteLine($"Activity URL: {result.ActivityUrl}");
}

return result.Success ? 0 : 1;

// Helper method to read password without displaying it
static string ReadPassword()
{
    var password = string.Empty;
    ConsoleKey key;

    do
    {
        var keyInfo = Console.ReadKey(intercept: true);
        key = keyInfo.Key;

        if (key == ConsoleKey.Backspace && password.Length > 0)
        {
            password = password[0..^1];
            Console.Write("\b \b");
        }
        else if (!char.IsControl(keyInfo.KeyChar))
        {
            password += keyInfo.KeyChar;
            Console.Write("*");
        }
    } while (key != ConsoleKey.Enter);

    return password;
}
