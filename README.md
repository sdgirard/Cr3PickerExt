# Garmin FIT File Uploader

A .NET 9 console application for uploading FIT (Flexible and Interoperable Data Transfer) files to Garmin Connect.

## Features

- ✅ Built with .NET 9
- ✅ Upload FIT files to Garmin Connect
- ✅ Secure credential handling
- ✅ Support for configuration file or interactive input
- ✅ Activity URL returned after successful upload
- ✅ Cross-platform support (Windows, macOS, Linux)

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- Garmin Connect account

## Installation

1. Clone the repository:
```bash
git clone https://github.com/sdgirard/Cr3PickerExt.git
cd Cr3PickerExt/GarminFitUploader
```

2. Restore dependencies:
```bash
dotnet restore
```

## Configuration

### Option 1: Configuration File (Recommended)

1. Copy the example configuration file:
```bash
cp appsettings.example.json appsettings.json
```

2. Edit `appsettings.json` with your Garmin Connect credentials:
```json
{
  "Garmin": {
    "Username": "your-email@example.com",
    "Password": "your-password"
  }
}
```

**Note:** The `appsettings.json` file is excluded from git to protect your credentials.

### Option 2: Environment Variables

Set the following environment variables:

**Windows (PowerShell):**
```powershell
$env:Garmin__Username="your-email@example.com"
$env:Garmin__Password="your-password"
```

**Linux/macOS:**
```bash
export Garmin__Username="your-email@example.com"
export Garmin__Password="your-password"
```

### Option 3: Interactive Input

If no configuration is found, the application will prompt you for credentials at runtime.

## Usage

### Run with File Path Argument

```bash
dotnet run -- /path/to/your/activity.fit
```

### Run Interactively

```bash
dotnet run
```

The application will prompt you for the FIT file path if not provided as an argument.

### Build and Run Executable

Build the application:
```bash
dotnet build -c Release
```

Run the executable:
```bash
./bin/Release/net9.0/GarminFitUploader /path/to/your/activity.fit
```

## Example Output

```
=== Garmin FIT File Uploader (.NET 9) ===

Starting authentication with Garmin Connect...
Authentication successful!

Uploading file: /path/to/activity.fit
Upload successful!

=== Upload Result ===
Success: True
Message: File uploaded successfully
Activity ID: 123456789
Activity URL: https://connect.garmin.com/modern/activity/123456789
```

## Project Structure

```
GarminFitUploader/
├── GarminFitUploader.csproj    # Project file
├── Program.cs                   # Main entry point
├── GarminConnectUploader.cs    # Upload service implementation
├── GarminConfig.cs             # Configuration model
├── appsettings.json            # Configuration (gitignored)
└── appsettings.example.json    # Configuration template
```

## How It Works

1. **Authentication**: The application authenticates with Garmin Connect using your credentials
2. **Session Management**: Maintains cookies and session information
3. **File Upload**: Uploads the FIT file as multipart form data to Garmin's upload endpoint
4. **Result Processing**: Parses the response to extract activity information

## Security Considerations

- Never commit `appsettings.json` with real credentials to version control
- The `.gitignore` file is configured to exclude `appsettings.json`
- Use environment variables in production environments
- Consider using a secrets manager for production deployments

## Troubleshooting

### Authentication Fails
- Verify your Garmin Connect credentials are correct
- Check if your account requires 2FA (two-factor authentication) - this application currently doesn't support 2FA
- Ensure you have an active internet connection

### Upload Fails
- Verify the file is a valid FIT file
- Check that the file exists and is readable
- Ensure the file is not corrupted

### Build Errors
- Ensure you have .NET 9 SDK installed: `dotnet --version`
- Try cleaning and rebuilding: `dotnet clean && dotnet build`

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is open source. Please check the repository for license details.

## Disclaimer

This is an unofficial tool and is not affiliated with, endorsed by, or associated with Garmin Ltd. or any of its subsidiaries. Use at your own risk.

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/sdgirard/Cr3PickerExt).
