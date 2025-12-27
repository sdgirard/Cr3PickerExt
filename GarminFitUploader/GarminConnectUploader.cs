using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace GarminFitUploader;

/// <summary>
/// Handles uploading FIT files to Garmin Connect
/// </summary>
public class GarminConnectUploader : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly HttpClientHandler _handler;
    private readonly CookieContainer _cookieContainer;
    private bool _isAuthenticated = false;

    private const string BaseUrl = "https://connect.garmin.com";
    private const string SsoUrl = "https://sso.garmin.com/sso";
    private const string ModernUrl = "https://connect.garmin.com/modern";
    private const string UploadUrl = "https://connect.garmin.com/modern/proxy/upload-service/upload";

    public GarminConnectUploader()
    {
        _cookieContainer = new CookieContainer();
        _handler = new HttpClientHandler
        {
            CookieContainer = _cookieContainer,
            UseCookies = true,
            AllowAutoRedirect = true
        };
        _httpClient = new HttpClient(_handler);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
    }

    /// <summary>
    /// Authenticates with Garmin Connect
    /// </summary>
    public async Task<bool> AuthenticateAsync(string username, string password)
    {
        try
        {
            Console.WriteLine("Starting authentication with Garmin Connect...");

            // Step 1: Get the login page to establish session
            var loginUrl = $"{SsoUrl}/signin?service={ModernUrl}";
            var response = await _httpClient.GetAsync(loginUrl);
            response.EnsureSuccessStatusCode();

            // Step 2: Prepare login data
            var loginData = new Dictionary<string, string>
            {
                { "username", username },
                { "password", password },
                { "embed", "false" }
            };

            // Step 3: Post login credentials
            var loginPostUrl = $"{SsoUrl}/signin?service={ModernUrl}";
            var content = new FormUrlEncodedContent(loginData);
            response = await _httpClient.PostAsync(loginPostUrl, content);

            // Check if login was successful
            var responseContent = await response.Content.ReadAsStringAsync();
            
            // Check for authentication failure indicators
            // Successful login typically results in a ticket and redirect
            if (!response.IsSuccessStatusCode || 
                responseContent.Contains("\"error\":") || 
                responseContent.Contains("invalid-credentials") ||
                responseContent.Contains("LOGIN_FAILED"))
            {
                Console.WriteLine("Authentication failed: Invalid credentials or login error");
                return false;
            }

            // Step 4: Complete the login flow by accessing the modern site
            response = await _httpClient.GetAsync(ModernUrl);
            
            // Verify we can access the modern site
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Authentication failed: Could not access Garmin Connect");
                return false;
            }

            _isAuthenticated = true;
            Console.WriteLine("Authentication successful!");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Authentication error: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Uploads a FIT file to Garmin Connect
    /// </summary>
    public async Task<UploadResult> UploadFitFileAsync(string filePath)
    {
        if (!_isAuthenticated)
        {
            return new UploadResult
            {
                Success = false,
                Message = "Not authenticated. Please authenticate first."
            };
        }

        if (!File.Exists(filePath))
        {
            return new UploadResult
            {
                Success = false,
                Message = $"File not found: {filePath}"
            };
        }

        try
        {
            Console.WriteLine($"Uploading file: {filePath}");

            // Read the FIT file
            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var fileName = Path.GetFileName(filePath);

            // Create multipart form data
            using var formData = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            formData.Add(fileContent, "file", fileName);

            // Send upload request
            var response = await _httpClient.PostAsync(UploadUrl, formData);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Upload successful!");
                
                // Parse response to get activity ID if available
                try
                {
                    var jsonDoc = JsonDocument.Parse(responseContent);
                    var detailedImportResult = jsonDoc.RootElement
                        .GetProperty("detailedImportResult");
                    
                    var successes = detailedImportResult.GetProperty("successes");
                    if (successes.GetArrayLength() > 0)
                    {
                        var activityId = successes[0].GetProperty("internalId").GetInt64();
                        return new UploadResult
                        {
                            Success = true,
                            Message = "File uploaded successfully",
                            ActivityId = activityId,
                            ActivityUrl = $"{BaseUrl}/modern/activity/{activityId}"
                        };
                    }
                }
                catch (JsonException ex)
                {
                    // Log the parsing error but still consider upload successful
                    Console.WriteLine($"Upload successful but couldn't parse activity ID: {ex.Message}");
                    Console.WriteLine($"Response content: {responseContent.Substring(0, Math.Min(500, responseContent.Length))}");
                }

                return new UploadResult
                {
                    Success = true,
                    Message = "File uploaded successfully"
                };
            }
            else
            {
                Console.WriteLine($"Upload failed with status: {response.StatusCode}");
                Console.WriteLine($"Response: {responseContent}");
                
                return new UploadResult
                {
                    Success = false,
                    Message = $"Upload failed: {response.StatusCode} - {responseContent}"
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Upload error: {ex.Message}");
            return new UploadResult
            {
                Success = false,
                Message = $"Upload error: {ex.Message}"
            };
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
        _handler?.Dispose();
    }
}

/// <summary>
/// Result of a FIT file upload operation
/// </summary>
public class UploadResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public long? ActivityId { get; set; }
    public string? ActivityUrl { get; set; }
}
