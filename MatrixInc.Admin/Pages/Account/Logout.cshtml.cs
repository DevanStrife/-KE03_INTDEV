using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MatrixInc.Admin.Pages.Account;

public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        // Clear session
        HttpContext.Session.Clear();

        TempData["SuccessMessage"] = "U bent succesvol uitgelogd.";

        return RedirectToPage("/Account/Login");
    }
}
