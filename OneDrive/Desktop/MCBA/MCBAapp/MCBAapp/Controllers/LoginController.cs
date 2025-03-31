using MCBAapp.Database;
using MCBAapp.Models;
using Microsoft.AspNetCore.Mvc;
using SimpleHashing.Net;
// Just for testing purposes - without database

namespace MCBAapp.Controllers;


public class LoginController : Controller
{
    
    private static readonly ISimpleHash s_simpleHash = new SimpleHash();
    private readonly McbaContext _context;
    
    public LoginController(McbaContext context)
    {
        _context = context;
    }

    // Basic Navigation to Login Page
    public IActionResult Login() => View();
    
    [HttpPost]
    public async Task<IActionResult> Login(string loginID, string password)
    {
        // int loginIDInt = int.Parse(loginID);
        ModelState.Clear();
        var login = await _context.Logins.FindAsync(loginID);

        if(login == null || string.IsNullOrEmpty(password) || !s_simpleHash.Verify(password, login.PasswordHash))
        {
            ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
            return View(new Login { LoginID = loginID });
        }
        
        var customer = await _context.Customers.FindAsync(login.CustomerID);
        if (customer.IsLocked)
        {
            ModelState.AddModelError("LoginFailed", "Account is locked, please contact administrator.");
            return View(new Login { LoginID = loginID });
        }
        
        // Login customer.
        HttpContext.Session.SetString("LoginID", login.LoginID);
        HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
        HttpContext.Session.SetString(nameof(Customer.Name), login.Customer.Name);
        
        return RedirectToAction("Index", "Customer");
    } 

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Login");
    }
    
    public IActionResult ResetPassword()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult>ResetPassword(string newPassword, string confirmPassword)
    {
        var login = await _context.Logins.FindAsync(HttpContext.Session.GetString("LoginID"));
        if (newPassword != confirmPassword)
        {
            ViewBag.NewPassword = newPassword;
            ViewBag.ConfirmPassword = confirmPassword;
            ModelState.AddModelError(nameof(confirmPassword), "Password does not match.");
        }
        
        if (ModelState.IsValid)
        {
            try
            {
                login.PasswordHash = s_simpleHash.Compute(newPassword);
                await _context.SaveChangesAsync();
                return RedirectToAction("Logout", "Login");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(" ","an error occured while changing password");
            }
        }
        
        return RedirectToAction("ResetPassword", "Login");
    }
}