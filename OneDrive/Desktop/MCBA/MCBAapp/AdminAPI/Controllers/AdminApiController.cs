using MCBAapp.Models;
using MCBAapp.Models.DataManager;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/Customers")]
public class AdminApiController : ControllerBase
{
    private CustomerManager _repo;
    
    public AdminApiController(CustomerManager repo)
    {
        _repo = repo;
    }
    
    // GET: api/Customers
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_repo.GetAll());
    }
    
    // GET api/Customers/2100
    [HttpGet("{customerID}")]
    public IActionResult Get(int customerID)
    {
        var customer = _repo.Get(customerID);
        if (customer == null)
            return NotFound();
        return Ok(customer);
    }
    
    // PUT api/Customers
    [HttpPut]
    public IActionResult Put([FromBody] Customer customer)
    {
        var customerToUpdate = _repo.Get(customer.CustomerID);
        if (customerToUpdate == null)
            return NotFound();
        _repo.Update(customer.CustomerID, customer);
        return Ok();
    }
    
}