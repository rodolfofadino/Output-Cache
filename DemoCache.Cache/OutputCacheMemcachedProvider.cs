using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using Memcached.ClientLibrary;
using System.Diagnostics;
using System.Configuration

namespace DemoCache.Cache
{
    public class OutputCacheMemcachedProvider : OutputCacheProvider
    {
        public override object Add(string key, object entry, DateTime utcExpiry)
        {
            utcExpiry = TimeZoneInfo.ConvertTimeFromUtc(utcExpiry, TimeZoneInfo.Local);
            var agora = DateTime.Now;
            Debug.WriteLine("Add"+key + " __ " + utcExpiry.Subtract(agora).TotalSeconds.ToString());

            InicializaMemcached();

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

        private void InicializaMemcached()
        {
            string[] servers = ConfigurationManager.AppSettings["memcachedServer"].Split(';');
            SockIOPool pool = SockIOPool.GetInstance();
            pool.SetServers(servers);
            pool.Initialize();
        }

        private string MD5(string key)
        {
            return key;
        }

        public override object Get(string key)
        {
            Debug.WriteLine("Get " + key);

            InicializaMemcached();

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
            InicializaMemcached();

            MemcachedClient cache = new MemcachedClient();

            string chave = MD5(key);

            cache.Delete(chave);

            return;
        }

        public override void Set(string key, object entry, DateTime utcExpiry)
        {
            utcExpiry = TimeZoneInfo.ConvertTimeFromUtc(utcExpiry, TimeZoneInfo.Local);
            var agora = DateTime.Now;
            Debug.WriteLine("Set" + key + " __ " + utcExpiry.Subtract(agora).TotalSeconds.ToString());
            InicializaMemcached();

            MemcachedClient cache = new MemcachedClient();

            string chave = MD5(key);

            cache.Set(chave, entry, utcExpiry);
            return;
        }
    }
}
