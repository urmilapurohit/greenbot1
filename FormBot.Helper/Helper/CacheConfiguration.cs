using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using FormBot.Helper.Helper;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Configuration;
using System.Net.Sockets;
using System.Diagnostics;

namespace FormBot.Helper
{
    /// <summary>
    /// Cache Configuration
    /// </summary>
    public class CacheConfiguration
    {
        //public const string dsMenus = "dsMenus";
        //public const string dsMenuActions = "dsMenuActions";
        //public const string dsJobIndex = "dsJobIndex";
        //public const string dsSTCSubmissionIndex = "dsSTCSubmissionIndex";
        //public const string dsSTCInvoice = "dsSTCInvoice";
        //public const string dsPeakPayIndex = "dsPeakPayIndex";
        public const string dsSAASInvoiceData = "dsSAASInvoiceData";

        readonly static ObjectCache Cache;
        readonly static CacheItemPolicy policy;
        private static CacheEntryRemovedCallback removeCallback = null;

        static CacheConfiguration()
        {
            Cache = MemoryCache.Default; //Define default Region 
            policy = new CacheItemPolicy();

            //SlidingExpiration define max idle time for cache object based on uses.
            //We must set AbsoluteExpiration to DateTimeOffset.MaxValue if we use SlidingExpiration.
            policy.AbsoluteExpiration = DateTime.Now.AddMinutes(Convert.ToDouble(ProjectConfiguration.GetCacheTime));

            //RemovedCallback will call once Cached Item will be expired. 
            //We can use either CacheEntryUpdateCallback or CacheEntryRemovedCallback once. Other one should be null.
            removeCallback = new CacheEntryRemovedCallback(OnExpire);
            policy.RemovedCallback = removeCallback;
        }

        /// <summary>
        /// Key for caching of userid wise snack bar notification list.
        /// </summary>
        /// <remarks>
        /// {0} : userid
        /// </remarks>
        public const string UserWiseSnackBarNotificationList = "user.notification.list.id-{0}";
        /// <summary>
        ///  Key for caching of spv log listing data by filters.
        /// </summary>
        /// <remarks>
        /// {0} : pageSize
        /// {1} : page
        /// {2} : SortCol
        /// {3} : SortDir
        /// {4} : ServiceAdministrator
        /// {5} : ResellerId
        /// {6} : SolarCompanyId
        /// {7} : JobReferenceOrId
        /// {8} : PVDSWHcode
        /// {9} : SPVMethod
        /// {10} : VerificationStatus
        /// {11} : ResponseCode
        /// {12} : Manufacturer
        /// {13} : ModelNumber
        /// {14} : SerialNumer
        /// {15} : FromRequestDate
        /// {17} : ToRequestDate
        /// </remarks>
        public const string SpvLogList = "spv.log.list.filter-{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}-{9}-{10}-{11}-{12}-{13}-{14}-{15}-{16}";
        /// <summary>
        /// Key for caching of spv log details by id. 
        /// </summary>
        /// <remarks>
        /// {0} : SpvLogId
        /// </remarks>
        public const string SpvLogDetails = "spv.log.details.id-{0}";

        //public static ObjectCache Cache
        //{
        //    get
        //    {
        //        return MemoryCache.Default;
        //    }
        //}

        public static void OnExpire(CacheEntryRemovedArguments cacheEntryRemovedArguments)
        {
            Log.WriteLog(DateTime.Now + " : " + $"Key [{cacheEntryRemovedArguments.CacheItem.Key}] Expired. Reason: {cacheEntryRemovedArguments.RemovedReason.ToString()}");
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        public static bool IsContainsKey(string key)
        {
            return (Cache.Contains(key));
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value t o get.</param>
        /// <returns>The value associated with the specified key.</returns>
        public static T Get<T>(string key, Func<T> acquire = null)
        {
            //if(acquire != null)
            //    if (!IsContainsKey(key))
            //        Set(key, acquire());

            return (T)Cache[key];
        }
        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        public static void Set(string key, object data)
        {
            if (data == null)
                return;

            //var policy = new CacheItemPolicy();
            //policy.AbsoluteExpiration = DateTime.Now.AddMinutes(Convert.ToDouble(ProjectConfiguration.GetCacheTime));
            //policy.Priority = CacheItemPriority.NotRemovable;
            //policy.RemovedCallback = new CacheEntryRemovedCallback(CacheRemovedCallback);
            //Cache.Set(new CacheItem(key, data), policy);

            Cache.Set(new CacheItem(key, data), policy);
        }
        public static void OnUpdate(string key)
        {
            Log.WriteLog(DateTime.Now + " : " + $"Key [{key}] Updated.");
        }

        public static void CacheRemovedCallback(CacheEntryRemovedArguments arguments)
        {
            try
            {
                Log.WriteLog(DateTime.Now.ToString() + " ENter in Cache remove callback: key: " + arguments.CacheItem.Key);
                var policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = DateTime.Now.AddMinutes(Convert.ToDouble(ProjectConfiguration.GetCacheTime));
                policy.Priority = CacheItemPriority.NotRemovable;
                //Cache.Set(new CacheItem(arguments.CacheItem.Key, arguments.CacheItem.Value), policy);
            }
            catch (Exception ex)
            {
                Log.WriteLog(DateTime.Now + " cache removed Exception: " + " CacheItem: " + arguments.CacheItem.Key + " exception:" + ex.InnerException);
            }
            //WriteToLogFile(DateTime.Now + " cache removed end: " + " CacheItem: " + arguments.CacheItem.Key + "value:"+arguments.CacheItem.Value+"ExistOrNot:" + CacheConfiguration.IsContainsKey(arguments.CacheItem.Key).ToString() + " CacheCount" + Cache.GetCount().ToString());
        }
        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">/key</param>
        public static void Remove(string key)
        {
            Cache.Remove(key);
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        public static void Clear()
        {
            foreach (var item in Cache)
                Remove(item.Key);
        }

        ///// <summary>
        ///// Remove cache by pattern
        ///// </summary>
        ///// <param name="prefix"></param>
        //public static void RemoveByPattern(string prefix)
        //{
        //    prefix = prefix.Contains("-") ? prefix.Split('-')[0] : prefix;
        //    List<string> enumerator = Cache.Select(x => x.Key.ToString().ToLower()).ToList().Where(x=>x.StartsWith(prefix)).ToList();

        //    foreach (string itemToRemove in enumerator)
        //    {
        //        var data =  Cache.Remove(itemToRemove);
        //    }
        //}
        ///// <summary>
        ///// Search cache key list
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public static List<string> SearchKey(string key)
        //{
        //    List<string> enumerator = !string.IsNullOrEmpty(key) ?  Cache.Select(x => x.Key.ToString().ToLower()).ToList().Where(x => x.Contains(key.ToLower())).ToList() : Cache.Select(x=>x.Key).ToList();
        //    return enumerator;
        //}
        //private static void WriteToLogFile(string content)
        //{
        //    FileStream fs = null;
        //    try
        //    {
        //        //set up a filestream
        //        //fs = new FileStream(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6) + "\\FormBotLog.txt", FileMode.OpenOrCreate, FileAccess.Write);
        //        fs = new FileStream(FormBot.Helper.ProjectSession.LogFilePath, FileMode.OpenOrCreate, FileAccess.Write);

        //        //set up a streamwriter for adding text
        //        using (StreamWriter sw = new StreamWriter(fs))
        //        {
        //            sw.BaseStream.Seek(0, SeekOrigin.End);
        //            //add the text
        //            sw.WriteLine(content);
        //            //add the text to the underlying filestream
        //            sw.Flush();
        //            //close the writer
        //            sw.Close();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        fs.Dispose();
        //        //throw;
        //    }
        //    //StreamWriter sw = new StreamWriter(fs);
        //    //find the end of the underlying filestream            
        //}
    }

    public class RedisCacheConfiguration
    {
        #region Keys Variables
        public const string dsSTCAllKeysInfoHashKey = "STCSCPerYear-{0}";
        public const string dsSTCCERApprovedNotInvoicedAllKeysInfoHashKey = "STCCERNotApproved";
        public const string dsSTCHashKey = "STC-{0}";
        public const string dsJobAllKeysInfoHashKey = "JobSCPerYear-{0}";
        public const string dsJobHashKey = "Job-{0}";
        public const string dsPeakPayAllKeysInfoHashKey = "PPAllKeysInfo";
        public const string dsPeakPayHashKey = "PP-{0}";
        public const string dsInstallerDesignerHashKey = "InstDesg";
        public const string dsSolarCompanyHashKey = "SolarComp";
        public const string dsMenus = "dsMenus";
        public const string dsMenuActions = "dsMenuActions";
        public const string dsJobIndex = "dsJobIndex";
        public const string dsJobKey = "dsJob";
        public const string dsJobGroupKey = dsJobKey + "-{0}-{1}";
        public const string dsJobEvenGroupKey = "{dsJobE}" + "-{0}-{1}";
        public const string dsJobMainKey = "dsJobMain";
        public const string dsSTCSubmissionIndex = "dsSTCSubmissionIndex";
        public const string dsSTCInvoice = "dsSTCInvoice";
        public const string dsSTCSubmissionKey = "dsSTCSub";
        public const string dsSTCSubmissionGroupKey = dsSTCSubmissionKey + "-{0}-{1}";
        public const string dsSTCSubmissionMainKey = "dsSTCSubMain";
        public const string dsPeakPayIndex = "dsPeakPayIndex";
        public const string dsPeakPayKey = "dsPeakPay";
        public const string dsPeakPayGroupKey = dsPeakPayKey + "-{0}-{1}";
        public const string dsPeakPayMainKey = "dsPeakPayMain";
        public static string GetJobKey(int solarCompanyId = 0)
        {
            return (solarCompanyId % 2 == 0 ? dsJobEvenGroupKey : dsJobGroupKey);
        }
        #endregion

        #region Connection With Redis Cache
        private static int KeyExpireTimeInMinutes = ProjectConfiguration.DistributedCacheTimeOutInMinutes;
        private static long _lastReconnectTicks = DateTimeOffset.MinValue.UtcTicks;
        private static DateTimeOffset _firstErrorTime = DateTimeOffset.MinValue;
        private static DateTimeOffset _previousErrorTime = DateTimeOffset.MinValue;
        private static SemaphoreSlim _reconnectSemaphore = new SemaphoreSlim(initialCount: 1, maxCount: 1);
        private static SemaphoreSlim _initSemaphore = new SemaphoreSlim(initialCount: 1, maxCount: 1);
        private static ConnectionMultiplexer _connection;
        private static bool _didInitialize = false;
        // In general, let StackExchange.Redis handle most reconnects,
        // so limit the frequency of how often ForceReconnect() will
        // actually reconnect.
        public static TimeSpan ReconnectMinInterval => TimeSpan.FromSeconds(60);
        // If errors continue for longer than the below threshold, then the
        // multiplexer seems to not be reconnecting, so ForceReconnect() will
        // re-create the multiplexer.
        public static TimeSpan ReconnectErrorThreshold => TimeSpan.FromSeconds(30);
        public static TimeSpan RestartConnectionTimeout => TimeSpan.FromSeconds(15);
        public static int RetryMaxAttempts => 5;
        public static ConnectionMultiplexer Connection { get { return _connection; } }
        public static async Task InitializeAsync()
        {
            if (_didInitialize)
            {
                throw new InvalidOperationException("Cannot initialize more than once.");
            }
            _connection = await CreateConnectionAsync();
            _didInitialize = true;
        }
        // This method may return null if it fails to acquire the semaphore in time.
        // Use the return value to update the "connection" field
        private static async Task<ConnectionMultiplexer> CreateConnectionAsync()
        {
            if (_connection != null)
            {
                // If we already have a good connection, let's re-use it
                return _connection;
            }
            try
            {
                await _initSemaphore.WaitAsync(RestartConnectionTimeout);
            }
            catch
            {
                // We failed to enter the semaphore in the given amount of time. Connection will either be null, or have a value that was created by another thread.
                return _connection;
            }
            // We entered the semaphore successfully.
            try
            {
                if (_connection != null)
                {
                    // Another thread must have finished creating a new connection while we were waiting to enter the semaphore. Let's use it
                    return _connection;
                }
                // Otherwise, we really need to create a new connection.
                string cacheConnection = ConfigurationManager.AppSettings["CacheConnection"].ToString();
                return await ConnectionMultiplexer.ConnectAsync(cacheConnection);
            }
            finally
            {
                _initSemaphore.Release();
            }
        }
        private static async Task CloseConnectionAsync(ConnectionMultiplexer oldConnection)
        {
            if (oldConnection == null)
            {
                return;
            }
            try
            {
                await oldConnection.CloseAsync();
            }
            catch (Exception)
            {
                // Ignore any errors from the oldConnection
            }
        }
        /// <summary>
        /// Force a new ConnectionMultiplexer to be created.
        /// NOTES:
        ///     1. Users of the ConnectionMultiplexer MUST handle ObjectDisposedExceptions, which can now happen as a result of calling ForceReconnectAsync().
        ///     2. Call ForceReconnectAsync() for RedisConnectionExceptions and RedisSocketExceptions. You can also call it for RedisTimeoutExceptions,
        ///         but only if you're using generous ReconnectMinInterval and ReconnectErrorThreshold. Otherwise, establishing new connections can cause
        ///         a cascade failure on a server that's timing out because it's already overloaded.
        ///     3. The code will:
        ///         a. wait to reconnect for at least the "ReconnectErrorThreshold" time of repeated errors before actually reconnecting
        ///         b. not reconnect more frequently than configured in "ReconnectMinInterval"
        /// </summary>
        public static async Task ForceReconnectAsync()
        {
            var utcNow = DateTimeOffset.UtcNow;
            long previousTicks = Interlocked.Read(ref _lastReconnectTicks);
            var previousReconnectTime = new DateTimeOffset(previousTicks, TimeSpan.Zero);
            TimeSpan elapsedSinceLastReconnect = utcNow - previousReconnectTime;
            // If multiple threads call ForceReconnectAsync at the same time, we only want to honor one of them.
            if (elapsedSinceLastReconnect < ReconnectMinInterval)
            {
                return;
            }
            try
            {
                await _reconnectSemaphore.WaitAsync(RestartConnectionTimeout);
            }
            catch
            {
                // If we fail to enter the semaphore, then it is possible that another thread has already done so.
                // ForceReconnectAsync() can be retried while connectivity problems persist.
                return;
            }
            try
            {
                utcNow = DateTimeOffset.UtcNow;
                elapsedSinceLastReconnect = utcNow - previousReconnectTime;
                if (_firstErrorTime == DateTimeOffset.MinValue)
                {
                    // We haven't seen an error since last reconnect, so set initial values.
                    _firstErrorTime = utcNow;
                    _previousErrorTime = utcNow;
                    return;
                }
                if (elapsedSinceLastReconnect < ReconnectMinInterval)
                {
                    return; // Some other thread made it through the check and the lock, so nothing to do.
                }
                TimeSpan elapsedSinceFirstError = utcNow - _firstErrorTime;
                TimeSpan elapsedSinceMostRecentError = utcNow - _previousErrorTime;
                bool shouldReconnect =
                    elapsedSinceFirstError >= ReconnectErrorThreshold // Make sure we gave the multiplexer enough time to reconnect on its own if it could.
                    && elapsedSinceMostRecentError <= ReconnectErrorThreshold; // Make sure we aren't working on stale data (e.g. if there was a gap in errors, don't reconnect yet).
                                                                               // Update the previousErrorTime timestamp to be now (e.g. this reconnect request).
                _previousErrorTime = utcNow;
                if (!shouldReconnect)
                {
                    return;
                }
                _firstErrorTime = DateTimeOffset.MinValue;
                _previousErrorTime = DateTimeOffset.MinValue;
                ConnectionMultiplexer oldConnection = _connection;
                await CloseConnectionAsync(oldConnection);
                _connection = null;
                _connection = await CreateConnectionAsync();
                Interlocked.Exchange(ref _lastReconnectTicks, utcNow.UtcTicks);
            }
            finally
            {
                _reconnectSemaphore.Release();
            }
        }
        // In real applications, consider using a framework such as
        // Polly to make it easier to customize the retry approach.
        private static async Task<T> BasicRetryAsync<T>(Func<T> func)
        {
            int reconnectRetry = 0;
            int disposedRetry = 0;
            while (true)
            {
                try
                {
                    return func();
                }
                catch (Exception ex) when (ex is RedisConnectionException || ex is SocketException)
                {
                    reconnectRetry++;
                    if (reconnectRetry > RetryMaxAttempts)
                        throw;
                    await ForceReconnectAsync();
                }
                catch (ObjectDisposedException)
                {
                    disposedRetry++;
                    if (disposedRetry > RetryMaxAttempts)
                        throw;
                }
            }
        }
        public static Task<IDatabase> GetDatabaseAsync()
        {
            return BasicRetryAsync(() => Connection.GetDatabase());
        }
        public static Task<System.Net.EndPoint[]> GetEndPointsAsync()
        {
            return BasicRetryAsync(() => Connection.GetEndPoints());
        }
        public static Task<IServer> GetServerAsync(string host, int port)
        {
            return BasicRetryAsync(() => Connection.GetServer(host, port));
        }
        #endregion

        #region New Code for Hash Testing
        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        public static void Set(IDatabase cache, string key, object data)
        {
            try
            {
                if (data == null)
                    return;
                Log.WriteLog("Method Set - 1");
                //set cache time
                var expiresIn = TimeSpan.FromMinutes(KeyExpireTimeInMinutes);
                //serialize item
                Log.WriteLog("Method Set - 2");
                var serializedItem = JsonConvert.SerializeObject(data);
                // GZip serialized item
                Log.WriteLog("Method Set - 3");
                var compressedJsonData = GZipHelper.Zip(serializedItem);//In bytes
                //and set it to cache
                Log.WriteLog("Method Set - 4");
                cache.StringSet(key, compressedJsonData, expiresIn);
            }
            catch (Exception ex)
            {
                //Log.WriteLog(DateTime.Now + " Redis Cache Set Exception: " + " CacheItem: " + key + " exception:" + ex.InnerException);
                Log.WriteLog(DateTime.Now + " Redis Cache Set Exception: " + " CacheItem: " + key + " exception:" + ex.ToString());
                Log.WriteLog(DateTime.Now + " Redis Cache Set Exception: " + " CacheItem: " + key + " exception:" + JsonConvert.SerializeObject(ex));
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Key of cached item</param>
        /// <returns>The cached value associated with the specified key</returns>
        public static T Get<T>(IDatabase cache, string key)
        {
            try
            {
                var serializedItem = cache.StringGet(key);
                if (!serializedItem.HasValue)
                    return default(T);
                var decompressedJsonData = GZipHelper.DecompressString(serializedItem);

                var item = JsonConvert.DeserializeObject<T>(decompressedJsonData);
                if (item == null)
                    return default(T);

                return item;
            }
            catch (Exception ex)
            {
                Log.WriteLog(DateTime.Now + " Redis Cache Get Exception: " + " CacheItem: " + key + " exception:" + ex.Message);
                return default(T);
            }
        }

        public static List<T> GetAllBatchUsingHash<T>(IDatabase cache, string redisKey)
        {
            List<T> list = new List<T>();
            try
            {
                //Stopwatch timer = new Stopwatch();
                //timer.Start();
                var batch = cache.CreateBatch();
                var value = batch.HashGetAllAsync(redisKey);
                batch.Execute();

                var result = value.Result;
                result.Select(k => new { val = k.Value }).ToList().ForEach(k =>
                {
                    if (!string.IsNullOrEmpty(k.val))
                    {
                        list.Add(Deserialize<T>(k.val));
                    }
                });

                //timer.Stop();
                //Log.WriteLog("Cache Configuration : GetAllBatchUsingHash -  & Time taken " + timer.ElapsedMilliseconds + " & Records Returned: " + list.Count);
            }
            catch (Exception ex)
            {
                Log.WriteLog("Exception in GetAllBatchUsingHash Time : " + DateTime.Now + " AND Exception : " + ex.Message + " Inner Exception: " + ex.InnerException);
            }
            return list;
        }

        public static List<T> GetBatchUsingHash<T>(IDatabase cache, Dictionary<int, string> keys)
        {
            List<T> list = new List<T>();
            try
            {
                //Stopwatch timer = new Stopwatch();
                //timer.Start();
                List<Task<RedisValue>> tasks = new List<Task<RedisValue>>(keys.Count);
                var batch = cache.CreateBatch();
                foreach (var key in keys)
                {
                    var value = batch.HashGetAsync(key.Value, key.Key);
                    tasks.Add(value);
                }
                batch.Execute();
                foreach (var item in tasks)
                {
                    var result = item.Result;
                    if (result.HasValue)
                    {
                        list.Add(Deserialize<T>(result));
                    }
                }

                //timer.Stop();
                //Log.WriteLog("Cache Configuration : GetBatchUsingHash - Total Keys : " + keys.Count + " & Time taken " + timer.ElapsedMilliseconds + " & Records Returned: " + list.Count);
            }
            catch (Exception ex)
            {
                Log.WriteLog("Exception in GetBatchUsingHash Time : " + DateTime.Now + " AND Exception : " + ex.Message + " Inner Exception: " + ex.InnerException);
            }
            return list;
        }

        public static bool SetHash(IDatabase cache, string redisKey, int hashKey, string payLoad)
        {
            try
            {
                if (string.IsNullOrEmpty(redisKey) || hashKey <= 0 || string.IsNullOrEmpty(payLoad))
                    return false;
                var expiresIn = TimeSpan.FromMinutes(KeyExpireTimeInMinutes);
                bool setCache = cache.HashSet(redisKey, hashKey, payLoad);
                var expireCache = cache.KeyExpireAsync(redisKey, expiresIn, flags: CommandFlags.FireAndForget);
                Task.WhenAll(expireCache);
                return setCache;
            }
            catch (Exception ex)
            {
                Log.WriteLog(DateTime.Now + " Redis Cache HashSet Exception:" + ex.Message);
            }
            return false;
        }

        public static void SetHashAsync(IDatabase cache, string redisKey, int hashKey, string payLoad)
        {
            try
            {
                if (string.IsNullOrEmpty(redisKey) || hashKey <= 0 || string.IsNullOrEmpty(payLoad))
                    return;
                List<Task<bool>> listTasksInBatch = new List<Task<bool>>();

                var expiresIn = TimeSpan.FromMinutes(KeyExpireTimeInMinutes);
                listTasksInBatch.Add(cache.HashSetAsync(redisKey, hashKey, payLoad, flags: CommandFlags.FireAndForget));
                listTasksInBatch.Add(cache.KeyExpireAsync(redisKey, expiresIn, flags: CommandFlags.FireAndForget));
                Task.WhenAll(listTasksInBatch.ToArray());
            }
            catch (Exception ex)
            {
                Log.WriteLog(DateTime.Now + " Redis Cache HashSet Exception:" + ex.Message);
            }
        }

        public static List<T> GetHash<T>(IDatabase cache, string redisKey, int hashKey)
        {
            List<T> list = new List<T>();
            try
            {
                if (string.IsNullOrEmpty(redisKey) || hashKey <= 0)
                    return list;
                List<Task<RedisValue>> tasks = new List<Task<RedisValue>>(1);
                var value = cache.HashGetAsync(redisKey, hashKey);
                tasks.Add(value);
                foreach (var item in tasks)
                {
                    var result = item.Result;
                    if (result.HasValue)
                    {
                        list.Add(Deserialize<T>(result));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(DateTime.Now + " Redis Cache HashSet Exception:" + ex.Message);
            }
            return list;
        }

        public static void SetBatchUsingHash(IDatabase cache, List<RedisHashSetKeyValuePair> redisHashSetKeyValuePairs)
        {
            try
            {
                var timer = new Stopwatch();
                timer.Start();
                if (redisHashSetKeyValuePairs == null || redisHashSetKeyValuePairs.Count == 0)
                    return;

                //set cache time
                var expiresIn = TimeSpan.FromMinutes(KeyExpireTimeInMinutes);

                IBatch batch = cache.CreateBatch();
                List<Task<bool>> listTasksInBatch = new List<Task<bool>>();

                foreach (var item in redisHashSetKeyValuePairs)
                {
                    //serialize item
                    var task = batch.HashSetAsync(item.RedisKey, item.HashKey, item.HashValuePayload, flags: CommandFlags.FireAndForget);
                    listTasksInBatch.Add(task);
                }
                batch.Execute();
                Task.WhenAll(listTasksInBatch.ToArray());
                timer.Stop();
                Log.WriteLog("SetBatchUsingHash method total keys- " + redisHashSetKeyValuePairs.Count + " & total time taken(MS) - " + timer.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                Log.WriteLog(DateTime.Now + " Redis Cache Batch Group HashSet Exception:" + ex.Message);
            }
        }

        public static void SetExpireForHash(IDatabase cache, List<RedisHashSetKeyValuePair> redisHashSetKeyValuePairs)
        {
            try
            {
                if (redisHashSetKeyValuePairs == null || redisHashSetKeyValuePairs.Count == 0)
                    return;

                //set cache time
                var expiresIn = TimeSpan.FromMinutes(KeyExpireTimeInMinutes);

                IBatch batch = cache.CreateBatch();
                List<Task<bool>> listTasksInBatch = new List<Task<bool>>();
                var items = redisHashSetKeyValuePairs.GroupBy(d => d.RedisKey).Select(k => k.Key).ToList();
                foreach (var item in items)
                {
                    var task = batch.KeyExpireAsync(item, expiresIn, flags: CommandFlags.FireAndForget);
                    listTasksInBatch.Add(task);
                }
                batch.Execute();
                Task.WhenAll(listTasksInBatch.ToArray());
            }
            catch (Exception ex)
            {
                Log.WriteLog(DateTime.Now + " Redis Cache Expire Set HashSet Exception:" + ex.Message);
            }
        }

        public static void DeleteHashParentAsync(IDatabase cache, string redisKey)
        {
            cache.KeyDeleteAsync(redisKey, flags: CommandFlags.FireAndForget);
        }

        public static void DeleteHashAsync(IDatabase cache, Dictionary<int, string> keys)
        {
            try
            {
                if (keys == null || keys.Count == 0)
                    return;

                IBatch batch = cache.CreateBatch();
                List<Task<bool>> listTasksInBatch = new List<Task<bool>>();

                foreach (var key in keys)
                {
                    var task = batch.HashDeleteAsync(key.Value, key.Key, flags: CommandFlags.FireAndForget);
                    listTasksInBatch.Add(task);
                }
                batch.Execute();
                Task.WhenAll(listTasksInBatch.ToArray());
            }
            catch (Exception ex)
            {
                Log.WriteLog(DateTime.Now + " Redis Cache HashSet Exception:" + ex.Message);
            }
        }
        public static T Deserialize<T>(string serialized)
        {
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        #endregion
    }
    public static class GZipHelper
    {
        private static byte[] Compress(byte[] data)
        {
            // `compressed` will contain result of compression
            using (var compressed = new MemoryStream())
            {
                // source is our original uncompressed data
                using (var source = new MemoryStream(data))
                {
                    using (var gzip = new GZipStream(compressed, CompressionMode.Compress))
                    {
                        // just write whole source into gzip stream with CopyTo
                        source.CopyTo(gzip);
                    }
                }
                return compressed.ToArray();
            }
        }

        public static byte[] Zip(string s)
        {
            return Compress(Encoding.UTF8.GetBytes(s));
        }

        static string CompressStringToBase64(string s, Encoding encoding)
        {
            return Convert.ToBase64String(Zip(s));
        }

        private static byte[] UnZip(Stream source)
        {
            using (var gzip = new GZipStream(source, CompressionMode.Decompress))
            {
                using (var decompressed = new MemoryStream())
                {
                    gzip.CopyTo(decompressed);
                    return decompressed.ToArray();
                }
            }
        }

        static byte[] Decompress(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                return UnZip(ms);
            }
        }

        public static string DecompressString(byte[] bytes)
        {
            using (var inputStream = new MemoryStream(bytes))
            {
                return Encoding.UTF8.GetString(UnZip(inputStream));
            }
        }
    }
}
