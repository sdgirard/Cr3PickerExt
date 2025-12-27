# Quick Start Guide

## Step 1: Install .NET 9

Download and install the .NET 9 SDK from: https://dotnet.microsoft.com/download/dotnet/9.0

Verify installation:
```bash
dotnet --version
```

## Step 2: Clone and Build

```bash
git clone https://github.com/sdgirard/Cr3PickerExt.git
cd Cr3PickerExt/GarminFitUploader
dotnet build
```

## Step 3: Configure Credentials

Create `appsettings.json` from the example:
```bash
cp appsettings.example.json appsettings.json
```

Edit `appsettings.json` and add your Garmin credentials:
```json
{
  "Garmin": {
    "Username": "your-email@example.com",
    "Password": "your-password"
  }
}
```

## Step 4: Run the Application

```bash
dotnet run -- /path/to/your/activity.fit
```

Or run interactively:
```bash
dotnet run
```

## Example

```bash
$ dotnet run -- ~/activities/morning-run.fit

=== Garmin FIT File Uploader (.NET 9) ===

Starting authentication with Garmin Connect...
Authentication successful!

Uploading file: /home/user/activities/morning-run.fit
Upload successful!

=== Upload Result ===
Success: True
Message: File uploaded successfully
Activity ID: 123456789
Activity URL: https://connect.garmin.com/modern/activity/123456789
```

## Alternative: Using Environment Variables

Instead of using appsettings.json, you can set environment variables:

**Linux/macOS:**
```bash
export Garmin__Username="your-email@example.com"
export Garmin__Password="your-password"
dotnet run -- activity.fit
```

**Windows (PowerShell):**
```powershell
$env:Garmin__Username="your-email@example.com"
$env:Garmin__Password="your-password"
dotnet run -- activity.fit
```

## Building a Standalone Executable

To create a standalone executable:

```bash
dotnet publish -c Release -r linux-x64 --self-contained
```

Replace `linux-x64` with your platform:
- `linux-x64` for Linux
- `osx-x64` for macOS (Intel)
- `osx-arm64` for macOS (Apple Silicon)
- `win-x64` for Windows

The executable will be in:
```
bin/Release/net9.0/[runtime]/publish/
```

## Troubleshooting

### "Authentication failed"
- Check your username and password
- Verify your Garmin Connect account is active
- Note: Two-factor authentication (2FA) is not currently supported

### "File not found"
- Ensure the path to your FIT file is correct
- Use absolute paths or paths relative to the project directory

### Build errors
- Ensure .NET 9 SDK is installed
- Run `dotnet clean` then `dotnet build`
