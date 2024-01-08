using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PixelBoard.StudentClient;

[Authorize]
public class PlayModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiUrl;

    public PlayModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _apiUrl = configuration.GetValue<string>("ApiUrl") ?? "http://localhost:5085/api";
    }

    [BindProperty]
    public int X { get; set; }
    [BindProperty]
    public int Y { get; set; }
    [BindProperty]
    public int Team { get; set; }

    public async void OnGet()
    {
        // For looking at the two JWTs and maybe putting them in jwt.io.
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        Console.WriteLine("access_token:");
        Console.WriteLine(accessToken.ToString());
        var idToken = await HttpContext.GetTokenAsync("id_token");
        Console.WriteLine("id_token:");
        Console.WriteLine(idToken.ToString());
    }
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Extract the user token to attach it to request we are about to make
        var idToken = await HttpContext.GetTokenAsync("id_token");
        var postData = new { X, Y, Team };

        using (var httpClient = _httpClientFactory.CreateClient())
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {idToken}");

            var payload = new StringContent(
                JsonSerializer.Serialize(postData),
                Encoding.UTF8,
                "application/json"
            );
            try
            {
                var response = await httpClient.PostAsync($"{_apiUrl}/color", payload);
                if (!response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(
                        string.Empty,
                        $"API request returned error: {response.StatusCode} {responseBody}"
                    );
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    $"Failed to post API request. HttpRequestException: {ex.Message}"
                );

                // For failures due to HttpIOExceptions, I find the inner exception useful, too.
                if (ex.InnerException is HttpIOException innerEx)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        $"Failed to post API request. HttpIOException: {innerEx.Message}"
                    );
                }
            }
            return Page();
        }
    }
}
