using MCBAapp.Data;
using MCBAapp.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace MCBAapp.Models.DataManager;

public class CustomerManager : IDataRepository<Customer, int>
{

    private readonly AdminApiContext _context;
    
    public CustomerManager(AdminApiContext context)
    {
        _context = context;
    }
    
    public IEnumerable<Customer> GetAll()
    {
        return _context.Customers.ToList();
    }

    public Customer Get(int id)
    {
        return _context.Customers.Find(id);
    }
    
    public int Update(int id, Customer item)
    {
        
        var customerToUpdate = _context.Customers.Find(id);
        if (customerToUpdate != null)
            _context.Entry(customerToUpdate).CurrentValues.SetValues(item);
        // _context.Update(item);
        _context.SaveChanges();
        return id;
    }
}