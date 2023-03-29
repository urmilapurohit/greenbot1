using Newtonsoft.Json;

namespace FormBot.Helper.Helper
{
    public class RedisHashSetKeyValuePair
    {
        public string RedisKey { get; set; }

        public int HashKey { get; set; }

        public string HashValuePayload { get; set; }

        public int PID { get; set; }
    }

    public class DistributedCacheAllKeysInfoForHashSetView
    {
        public DistributedCacheAllKeysInfoForHashSetView() { }
        public DistributedCacheAllKeysInfoForHashSetView(int PID, string Ids, string RedisKey)
        {
            this.PID = PID;
            this.Ids = Ids;
            this.RedisKey = RedisKey;
        }
        [JsonProperty("1", NullValueHandling = NullValueHandling.Ignore)]
        public int PID { get; set; }
        [JsonProperty("2", NullValueHandling = NullValueHandling.Ignore)]
        public string Ids { get; set; }
        [JsonIgnore]
        public string RedisKey { get; set; }

    }
}
