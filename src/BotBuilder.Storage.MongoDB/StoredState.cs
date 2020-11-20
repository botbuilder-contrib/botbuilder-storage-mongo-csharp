using System;

namespace BotBuilder.Storage.MongoDB
{
    public class StoredState
    {
        public string Id { get; set; }

        public DateTime Updated { get; set; }

        public Object State;
    }
}
