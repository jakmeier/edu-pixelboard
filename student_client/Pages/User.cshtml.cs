using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PixelBoard.StudentClient;


[Authorize]
public class UserModel : PageModel
{
    public string? UserName { get; set; }
    public string? TeamName { get; set; }

    public void OnGet()
    {
        var nameClaim = User.FindFirst("name");
        var teamClaim = User.FindFirst("team");

        UserName = nameClaim?.Value ?? "Unknown";
        TeamName = teamClaim?.Value;
    }
}
