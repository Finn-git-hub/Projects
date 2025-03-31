using System.Net.Mime;
using System.Text;
using MCBAapp.Filters;
using MCBAapp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AdminPortal.Controllers;

[AuthorizeAdmin]
public class AdminController : Controller
{
    private readonly HttpClient _client;
    
    public AdminController(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient("api");
    }
    
    public async Task<IActionResult> Index()
    {
        using var response = await _client.GetAsync("api/Customers");
        
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadAsStringAsync();
        
        var customers = JsonConvert.DeserializeObject<List<Customer>>(result);
        
        return View(customers);
    }

    public async Task<IActionResult> Edit(int? customerID)
    {
        if(customerID == null)
            return NotFound();

        using var response = await _client.GetAsync($"api/Customers/{customerID}");

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var customer = JsonConvert.DeserializeObject<Customer>(result);

        return View(customer);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int customerID, Customer customer)
    {
        if(customerID != customer.CustomerID)
            return NotFound();

        if(ModelState.IsValid)
        {
            var content =
                new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, MediaTypeNames.Application.Json);

            using var response = _client.PutAsync("api/Customers", content).Result;

            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index");
        }

        return View(customer);
    }
    
    public async Task<IActionResult> Lock(int? customerID)
    {
        if(customerID == null)
            return NotFound();

        using var response = await _client.GetAsync($"api/Customers/{customerID}");
        
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadAsStringAsync();
        var customer = JsonConvert.DeserializeObject<Customer>(result);
        if (customer == null)
            return NotFound();

        customer.IsLocked = customer.IsLocked == false;
        
        var content =
            new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, MediaTypeNames.Application.Json);
        
        using var response2 = _client.PutAsync("api/Customers", content).Result;
        
        response2.EnsureSuccessStatusCode();
        
        return RedirectToAction("Index");
    }
}