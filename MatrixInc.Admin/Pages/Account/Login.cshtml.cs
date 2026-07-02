using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Admin.Pages.Account;

public class LoginModel : PageModel
{
    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public bool RememberMe { get; set; }

    public void OnGet()
    {
        // Check if already logged in
        var adminUser = HttpContext.Session.GetString("AdminUser");
        if (!string.IsNullOrEmpty(adminUser))
        {
            Response.Redirect("/Index");
        }
    }

    public IActionResult OnPost()
    {
        // GESIMULEERDE LOGIN - NIET FUNCTIONEEL
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ModelState.AddModelError(string.Empty, "Gebruikersnaam en wachtwoord zijn verplicht");
            return Page();
        }

        // Simuleer succesvolle login
        HttpContext.Session.SetString("AdminUser", Username);
        HttpContext.Session.SetString("AdminRole", "Administrator");

        TempData["SuccessMessage"] = $"Welkom, {Username}! (Gesimuleerd admin login)";

        return RedirectToPage("/Index");
    }
}
