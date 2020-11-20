using MongoDB.Driver;

namespace BotBuilder.Storage.MongoDB
{
    public readonly struct MongoStateStoreSettings
    {
        public MongoStateStoreSettings(
            IMongoClient client,
            string databaseName = "BotBuilderState",
            string collectionName = "State")
        {
            Client = client;
            DatabaseName = databaseName;
            CollectionName = collectionName;
        }

        public string DatabaseName { get; init; }
        public string CollectionName { get; init; }

        public IMongoClient Client { get; init; }

    }
}
