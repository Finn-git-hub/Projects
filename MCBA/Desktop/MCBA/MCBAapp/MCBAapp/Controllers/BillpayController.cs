using MCBAapp.Database;
using MCBAapp.Models;
using MCBAapp.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MCBAapp.Controllers;

public class BillpayController : Controller
{
    private readonly McbaContext _context;
    private readonly BillPay _bPayee = new();
    private readonly Payee _payee = new();
    public BillpayController(McbaContext context)
    {
        _context = context;
    }

    public IActionResult BillList()
    {
        var accountList = _context.Accounts.Where(x => x.CustomerID == HttpContext.Session.GetInt32("CustomerID")).ToList();

        var billList = new List<BillPay>();
                
        foreach (var account in accountList)
        {
            var bills = _context.BillPays.Where(x => x.AccountNumber == account.AccountNumber).ToList();
            billList.AddRange(bills);
        }
                
        return View(billList);
    }

    public async Task<IActionResult> CreateBill()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(int accountNumber, decimal amount, DateTime scheduleTimeUtc, string billType)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var newBill = new BillPay
                { 
                    PayeeID = HttpContext.Session.GetInt32("PayeeID").Value,
                    AccountNumber = accountNumber,
                    Amount = amount,
                    ScheduleTimeUtc = scheduleTimeUtc.ToUniversalTime(),
                    BillType = billType=="OneOff"?BillpayType.OneOff:BillpayType.Monthly
                };
                
                _context.BillPays.Add(newBill);
                await _context.SaveChangesAsync();
                
                return RedirectToAction("BillList","Billpay");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ModelState.AddModelError(" ","an error occured");
            }
        }
        return RedirectToAction("Index","Customer");
    }
    
    [HttpPost]
    public async Task<IActionResult> SchedulePage( string name, string address
        , string city, string state, string postcode, string phone)
    {
        
        if (ModelState.IsValid)
        {
            try
            {
                var payee = _context.Payees.FirstOrDefault(x => x.Name == name && x.Address == address
                    && x.City == city && x.State== state
                    && x.Postcode == postcode && x.Phone == phone);

                if (payee != null)
                {
                    _bPayee.Payee=payee;
                    _bPayee.PayeeID = payee.PayeeID;
                    return View(_bPayee);
                }
                
                var newPayee = new Payee
                {
                    Name = name,
                    Address = address,
                    City = city,
                    State = state,
                    Postcode = postcode,
                    Phone = phone
                };
                 _context.Payees.Add(newPayee);
                 _bPayee.Payee = newPayee;
                 await _context.SaveChangesAsync();
            
                return View(_bPayee);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(" ","an error occured");
            }
        }
        return RedirectToAction(nameof(Create));
    }

    public async Task<IActionResult> Pay(int id)
    {
        var bill = await _context.BillPays.FindAsync(id);
        return View(bill);
    }

    public async Task<IActionResult> PayBill(int id)
    {
        
        var bill = await _context.BillPays.FindAsync(id);
        var account = await _context.Accounts.FindAsync(bill.AccountNumber);
        if (account.Balance < bill.Amount)
        {
            ModelState.AddModelError("Pay","Insufficient funds");
            return View("Pay",bill);
        }

        if (account.AccountType == AccountType.Checking && account.Balance - bill.Amount < 300)
        {
            ModelState.AddModelError("Pay","Checking account must have a minimum balance of $300");
            return View("Pay",bill);
        }
        account.Balance -= bill.Amount;
        _context.Accounts.Update(account);
        var transaction = new Transaction
        {
            TransactionType = TransactionType.BillPay,
            AccountNumber = bill.AccountNumber,
            Amount = bill.Amount,
            Comment = "BillPay",
            TransactionTimeUtc = DateTime.UtcNow
        };
        _context.Transactions.Add(transaction);
        if (bill.BillType == BillpayType.OneOff)
        {
            _context.BillPays.Remove(bill);
        }
        else
        {
            bill.ScheduleTimeUtc = bill.ScheduleTimeUtc.AddMonths(1);
            _context.BillPays.Update(bill);
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("BillList","Billpay");
    }
}