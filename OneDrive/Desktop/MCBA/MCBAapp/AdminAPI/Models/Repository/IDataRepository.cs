namespace MCBAapp.Models.Repository;

public interface IDataRepository<TEntity, TKey> where TEntity : class
{
    IEnumerable<Customer> GetAll();
    Customer Get(TKey id);
    int Update(TKey id, TEntity item);
}