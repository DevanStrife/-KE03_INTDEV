using MatrixInc.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Web.Pages.Account;

public class LoginModel : PageModel
{
    [BindProperty]
    public LoginViewModel LoginData { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public IActionResult OnPost(string? returnUrl = null)
    {
        // GESIMULEERDE LOGIN - NIET FUNCTIONEEL
        // In een echte app zou je hier credentials checken

        if (string.IsNullOrWhiteSpace(LoginData.Email) || string.IsNullOrWhiteSpace(LoginData.Password))
        {
            ModelState.AddModelError(string.Empty, "Email en wachtwoord zijn verplicht");
            return Page();
        }

        // Simuleer succesvolle login
        TempData["SuccessMessage"] = $"Welkom terug! (Gesimuleerd - Email: {LoginData.Email})";

        // Store simulated user in session
        HttpContext.Session.SetString("UserEmail", LoginData.Email);
        HttpContext.Session.SetString("UserName", LoginData.Email.Split('@')[0]);

        return Redirect(returnUrl ?? "/");
    }
}
