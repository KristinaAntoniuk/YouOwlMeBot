using Amazon.DynamoDBv2.DataModel;
using YouOwlMeBot.Models;

namespace YouOwlMeBot.DataProviders;

public interface IDataProvider<T> where T : IModel
{
    IDynamoDBContext _dynamoDBContext { get; set; }
    Task<IEnumerable<T>> GetAll();
    Task<T> GetById(Guid? guid);
    void Delete(T data);
    Task Save(T data);
}
