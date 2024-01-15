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
public class UserModel : PageModel
{
    public string? UserName { get; set; }
    public string? TeamName { get; set; }

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiUrl;

    public UserModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _apiUrl = configuration.GetValue<string>("ApiUrl") ?? "http://localhost:5085/api";
    }

    [BindProperty]
    public string? SubmitDisplayName { get; set; }

    public void OnGet()
    {
        var nameClaim = User.FindFirst("name");
        var teamClaim = User.FindFirst("team");

        UserName = nameClaim?.Value ?? "Unknown";
        TeamName = teamClaim?.Value;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Extract the user token to attach it to request we are about to make
        var idToken = await HttpContext.GetTokenAsync("id_token");
        var postData = new { Name = SubmitDisplayName };

        // TODO: dedup code with Play.cshtml.cs
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
                var response = await httpClient.PostAsync($"{_apiUrl}/player/register", payload);
                if (!response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(
                        string.Empty,
                                            $"API request returned error: {response.StatusCode} {responseBody}"
                                        );
                }
                Console.WriteLine(await response.Content.ReadAsStringAsync());
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
