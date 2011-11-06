using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using Memcached.ClientLibrary;
using System.Diagnostics;

namespace DemoCache.Cache
{
    public class OutputCacheMemcachedProvider : OutputCacheProvider
    {
        public override object Add(string key, object entry, DateTime utcExpiry)
        {
            utcExpiry = TimeZoneInfo.ConvertTimeFromUtc(utcExpiry, TimeZoneInfo.Local);
            var agora = DateTime.Now;
            Debug.WriteLine("Add"+key + " __ " + utcExpiry.Subtract(agora).TotalSeconds.ToString());

            string[] servers = { "127.0.0.1:11211" };
            SockIOPool pool = SockIOPool.GetInstance();
            pool.SetServers(servers);
            pool.Initialize();

            MemcachedClient cache = new MemcachedClient();

            string chave = MD5(key);

            if (cache.KeyExists(chave))
            {
                return cache.Get(chave);
            }
            else
            {
                cache.Set(chave, entry, utcExpiry.ToUniversalTime());
                return entry;
            }

        }

        private string MD5(string key)
        {
            return key;
        }

        public override object Get(string key)
        {
            Debug.WriteLine("Get "+key);

            string[] servers = { "127.0.0.1:11211" };
            SockIOPool pool = SockIOPool.GetInstance();
            pool.SetServers(servers);
            pool.Initialize();

            MemcachedClient cache = new MemcachedClient();

            string chave = MD5(key);

            if (cache.KeyExists(chave))
            {
                return cache.Get(chave);
            }
            else
            {
                return null;
            }
        }

        public override void Remove(string key)
        {
            string[] servers = { "127.0.0.1:11211" };
            SockIOPool pool = SockIOPool.GetInstance();
            pool.SetServers(servers);
            pool.Initialize();

            MemcachedClient cache = new MemcachedClient();

            string chave = MD5(key);

            cache.Delete(chave);

            return;
        }

        public override void Set(string key, object entry, DateTime utcExpiry)
        {
            utcExpiry = TimeZoneInfo.ConvertTimeFromUtc(utcExpiry, TimeZoneInfo.Local);
            var agora = DateTime.Now;
            Debug.WriteLine("Set"+key + " __ " + utcExpiry.Subtract(agora).TotalSeconds.ToString());
            string[] servers = { "127.0.0.1:11211" };
            SockIOPool pool = SockIOPool.GetInstance();
            pool.SetServers(servers);
            pool.Initialize();

            MemcachedClient cache = new MemcachedClient();

            string chave = MD5(key);

            cache.Set(chave, entry, utcExpiry);
            return;
        }
    }
}
