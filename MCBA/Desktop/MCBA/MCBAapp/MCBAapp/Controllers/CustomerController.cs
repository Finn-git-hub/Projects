using MCBAapp.Database;
using MCBAapp.Filters;
using MCBAapp.Models;
using MCBAapp.Models.Enums;
using MCBAapp.Utilities;
using Microsoft.AspNetCore.Mvc;
using SimpleHashing.Net;


using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace MCBAapp.Controllers;

[AuthorizeCustomer]
public class CustomerController : Controller
{
    // Testing without database
    // private Customer _customer;
    // End area for testing without database
    
    private readonly McbaContext _context;
    private static readonly ISimpleHash s_simpleHash = new SimpleHash();

    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public CustomerController(McbaContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(char? buttonType)
    {
        var customer = await _context.Customers.FindAsync(CustomerID);

        switch (buttonType)
        {
            // Go to Deposit page
            case 'D':
                HttpContext.Session.SetString(nameof(buttonType), "D");
                break;
            // Go to Withdraw page
            case 'W':
                HttpContext.Session.SetString(nameof(buttonType), "W");
                break;
            // Go to Transfer page
            case 'T':
                HttpContext.Session.SetString(nameof(buttonType), "T");
                break;
            // Check Statement
            case 'S':
                HttpContext.Session.SetString(nameof(buttonType), "S");
                break;
            case 'P' :
                HttpContext.Session.SetString(nameof(buttonType), "P");
                break;
            default:
                HttpContext.Session.SetString(nameof(buttonType), "default");
                break;
        }
        
        return View(customer);
    }
    
    public async Task<IActionResult> Deposit(int id)
    {
        var account = await _context.Accounts.FindAsync(id);
        return View(account);
    }
    
    [HttpPost]
    public async Task<IActionResult> Deposit(int id, decimal amount, string? comment)
    {
        var account = await _context.Accounts.FindAsync(id);

        if(amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        else if(amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");

        if(!ModelState.IsValid)
        {
            ViewBag.Amount = amount;
            ViewBag.Comment = comment ?? string.Empty;
            return View(account);
        }

        account.Balance += amount;
        AddTransaction(account, amount, comment, TransactionType.Deposit);
        await _context.SaveChangesAsync();
   
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Withdraw(int id)
    {
        var account = await _context.Accounts.FindAsync(id);
        return View(account);
    }

    [HttpPost]
    public async Task<IActionResult> Withdraw(int id, decimal amount, string? comment)
    {
        var account = await _context.Accounts.FindAsync(id);
        
        if(amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        else if(amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
        if (account.Balance < amount)
            ModelState.AddModelError(nameof(amount), "Insufficient balance.");
        if (account.Balance - amount < GetMinimumBalance(account.AccountType))
            ModelState.AddModelError(nameof(amount), $"Balance must be at least ${GetMinimumBalance(account.AccountType)} after transfer.");


        if(!ModelState.IsValid)
        {
            ViewBag.Amount = amount;
            ViewBag.Comment = comment ?? String.Empty;
            return View(account);
        }
        
        account.Balance -= amount;
        AddTransaction(account, amount, comment, TransactionType.Withdraw);
        var free = await CheckFreeWithdraw(account);
        if (!free)
        {
            account.Balance -= 0.05m;
            AddTransaction(account, 0.05m, "Service Charge", TransactionType.ServiceCharge);
        }
        
        await _context.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Transfer(int id)
    {
        var account = await _context.Accounts.FindAsync(id);
        return View(account);
    }
    
    [HttpPost]
    public async Task<IActionResult> Transfer(int id, decimal amount, int targetAccountNumber, string? comment)
    {
        var account = await _context.Accounts.FindAsync(id);
        var targetAccount = await _context.Accounts.FindAsync(targetAccountNumber);
        
        if(amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        else if(amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
        if (id == targetAccountNumber)
            ModelState.AddModelError(nameof(targetAccountNumber), "Cannot transfer to the same account.");
        if (targetAccount == null)
            ModelState.AddModelError(nameof(targetAccountNumber), "Target account does not exist, please verify the account number.");
        if (account.Balance < amount)
            ModelState.AddModelError(nameof(amount), "Insufficient balance.");
        if (account.Balance - amount < GetMinimumBalance(account.AccountType))
            ModelState.AddModelError(nameof(amount), $"Balance must be at least ${GetMinimumBalance(account.AccountType)} after transfer.");
        
        
        if(!ModelState.IsValid)
        {
            ViewBag.Amount = amount;
            ViewBag.TargetAccountNumber = targetAccountNumber;
            ViewBag.Comment = comment ?? String.Empty;
            return View(account);
        }
        
        account.Balance -= amount;
        targetAccount.Balance += amount;
        AddTransaction(account, amount, comment, TransactionType.Transfer, targetAccountNumber);
        AddTransaction(targetAccount, amount, comment, TransactionType.Transfer);
        var free = await CheckFreeTransfer(account);
        if (!free)
        {
            account.Balance -= 0.1m;
            AddTransaction(account, 0.05m, "Service Charge", TransactionType.ServiceCharge);
        }

        await _context.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Statement(int id, int page = 1)
    {
        var account = await _context.Accounts.FindAsync(id);
        ViewBag.Account = account;
        const int pageSize = 4;
        var pageList = await _context.Transactions.Where(x => x.AccountNumber == id)
            .OrderByDescending(x => x.TransactionID).ToPagedListAsync(page, pageSize);
        return View(pageList);
    }

    // Helper Functions
    private void AddTransaction(Account account, decimal amount, string? comment, TransactionType transactionType, int? destinationAccountNumber = null)
    {
        account.Transactions.Add(
            new Transaction
            {
                AccountNumber = account.AccountNumber,
                Account = account,
                Amount = amount,
                Comment = comment,
                DestinationAccountNumber = destinationAccountNumber,
                TransactionType = transactionType,
                TransactionTimeUtc = DateTime.UtcNow
            });
    }
    
    private async Task<bool> CheckFreeWithdraw(Account account)
    {
        var transactions = await _context.Transactions
            .Where(t => t.AccountNumber == account.AccountNumber)
            .ToListAsync();
        return transactions.Count(t => t.TransactionType == TransactionType.Withdraw) < 3;
    }


    public async Task<IActionResult> Profile(string? editState)
    {
        if (editState == "edit")
        {
            ViewBag.EditState = "edit";
        }
        
        var customer = await _context.Customers.FindAsync(CustomerID);
        return View(customer);
    }

    [HttpPost]
    public async Task <IActionResult>Save(string? name, string? tfn, string? address, 
        string? city, string? state, string? postcode, string? mobile)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(CustomerID);
                customer.Name = name;
                customer.TFN = tfn;
                customer.Address = address;
                customer.City = city;
                customer.State = state;
                customer.Postcode = postcode;
                customer.Mobile = mobile;

                await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

            }
            catch (Exception e)
            {
                ModelState.AddModelError(" ","an error occured while editting profile");
            }
        }

        return RedirectToAction(nameof(Profile));
    }

    private async Task<bool> CheckFreeTransfer(Account account)
    {
        var transactions = await _context.Transactions
            .Where(t => t.AccountNumber == account.AccountNumber)
            .ToListAsync();
        return transactions.Count(t => t.TransactionType == TransactionType.Transfer) < 3;
    }
    
    private decimal GetMinimumBalance(AccountType accountType)
    {
        return accountType switch
        {
            AccountType.Checking => 300,
            AccountType.Savings => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(accountType), accountType, null)
        };
    }
    
}