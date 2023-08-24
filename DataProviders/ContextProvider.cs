using Amazon.DynamoDBv2.DataModel;

namespace YouOwlMeBot.DataProviders
{
    public abstract class ContextProvider
    {
        public IDynamoDBContext _dynamoDBContext { get; set; }

        internal ContextProvider(IDynamoDBContext dynamoDBContext)
        {
            _dynamoDBContext = dynamoDBContext;
        }
    }
}
