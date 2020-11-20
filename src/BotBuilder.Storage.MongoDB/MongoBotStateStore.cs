using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using MongoDB.Driver;

namespace BotBuilder.Storage.MongoDB
{
    public class MongoBotStateStore : IStorage
    {
        public const string DefaultCollectionName = "BotBuilderState";
        public const string DefaultDatabaseName = "BotBuilderState";

        public const string StateFieldName = "Values";


        public MongoBotStateStore(MongoStateStoreSettings settings)
        {
            Settings = settings;

        }

        public MongoStateStoreSettings Settings { get; }



        IMongoDatabase Db => Settings.Client.GetDatabase(Settings.DatabaseName);

        IMongoCollection<StoredState> Collection => Db.GetCollection<StoredState>(Settings.CollectionName);

        public async Task DeleteAsync(string[] keys, CancellationToken cancellationToken = default)
        {
            var filter = CreateIdListFilter(keys);

            await Collection.DeleteManyAsync(filter, cancellationToken);
        }

        public static FilterDefinition<StoredState> CreateIdListFilter(string[] keys)
        {
            return Builders<StoredState>.Filter.In(d => d.Id, keys);
        }

        public async Task<IDictionary<string, object>> ReadAsync(string[] keys, CancellationToken cancellationToken = default)
        {
            string key = keys.Single();

            var filter = CreateIdListFilter(keys);
            var document = await Collection.Find(filter).FirstOrDefaultAsync();

            return PackState(key, document);
        }

        public static IDictionary<string, object> PackState(string key, StoredState document)
        {
            var result = new Dictionary<string, Object>();
            if (document != null)
            {
                result[key] = document.State;
            }
            return result;
        }

        public async Task WriteAsync(IDictionary<string, object> changes, CancellationToken cancellationToken = default)
        {
            var (id, entry) = changes.Single();

            var filter = CreateIdListFilter(new string[] { id });

            UpdateDefinition<StoredState> update = CreateUpdateDefinition(entry);

            await Collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });

        }

        public static UpdateDefinition<StoredState> CreateUpdateDefinition(object state)
        {
            return Builders<StoredState>.Update.Set(de => de.State, state).Set(d => d.Updated, DateTime.UtcNow); ;
        }
    }
}
