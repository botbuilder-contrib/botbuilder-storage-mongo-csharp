using Xunit;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using BotBuilder.Storage.MongoDB;

namespace Botbuilder.Storage.Mongodb.Test
{
    public class StorageTest
    {

        private BsonDocument Render<T>(FilterDefinition<T> filter)
        {
            var serializerRegistry = BsonSerializer.SerializerRegistry;
            var documentSerializer = serializerRegistry.GetSerializer<T>();
            return filter.Render(documentSerializer, serializerRegistry);
        }
        private BsonDocument Render<T>(UpdateDefinition<T> filter)
        {
            var serializerRegistry = BsonSerializer.SerializerRegistry;
            var documentSerializer = serializerRegistry.GetSerializer<T>();
            return (BsonDocument)filter.Render(documentSerializer, serializerRegistry);
        }

        [Fact]
        public void CreateIdListFilter_Ids_CreatesFilter()
        {
            var actual = MongoBotStateStore.CreateIdListFilter(new[] { "a", "b" });

            Assert.NotNull(actual);
        }

        [Fact]
        public void CreateIdListFilter_Ids_FieldEquality()
        {
            var actual = MongoBotStateStore.CreateIdListFilter(new[] { "a", "b" });
            var json = Render(actual).ToJson();
            Assert.Equal("{ \"_id\" : { \"$in\" : [\"a\", \"b\"] } }", json);
        }

        [Fact]
        public void PackState_NoDocument_EmptyDictionary()
        {
            var actual = MongoBotStateStore.PackState("a", null);
            Assert.Equal(0, actual.Count);
        }
        [Fact]
        public void PackState_WithDocument_CreatesDictionaryEntry()
        {
            StoredState expected = new StoredState { State = "yo" };
            var actual = MongoBotStateStore.PackState("a", expected);
            Assert.Same(expected.State, actual["a"]);
        }

        [Fact]
        public void CreateUpdateDefinition_Sets_Values()
        {
            var actual = MongoBotStateStore.CreateUpdateDefinition("yo");

            Assert.Contains("{ \"$set\" : { \"State\" : \"yo\", \"Updated\" : ISODate(", Render(actual).ToJson());
        }

    }
}


