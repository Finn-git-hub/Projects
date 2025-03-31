using Microsoft.AspNetCore.Mvc;

namespace AdminPortal.Controllers;

public class LoginController : Controller
{
    private const string AdminUsername = "admin";
    private const string AdminPassword = "admin";
    
    public IActionResult Login() => View();
    
    [HttpPost]
    public IActionResult Login(string loginID, string password)
    {
        if (loginID != AdminUsername || password != AdminPassword)
        {
            ViewBag.LoginID = loginID;
            ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
            return View();
        }
        
        HttpContext.Session.SetString(nameof(AdminUsername), loginID);
        
        return RedirectToAction("Index", "Admin");
    }
    
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}