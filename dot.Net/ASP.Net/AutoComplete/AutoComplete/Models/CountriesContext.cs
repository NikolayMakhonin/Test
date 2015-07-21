using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace AutoComplete.Models
{
    public class CountriesContext
    {
        private const string COUNTRIES_KEY = "Countries";
        private const string COUNTRIES_QUERY_PREFIX = "CountriesQuery:";
        private readonly TimeSpan CACHE_EXPIRATION_TIME = TimeSpan.FromDays(1);
        private readonly string _countriesFilePath;
        private readonly object _locker = new object();

        public CountriesContext(string countriesFilePath)
        {
            _countriesFilePath = countriesFilePath; 
        }

        /// <summary>
        /// Get countries contains query
        /// </summary>
        public IEnumerable<Country> GetCountries(string query, int limit)
        {
            if (String.IsNullOrWhiteSpace(query)) return new Country[0];
            query = query.Trim().ToLower();
            var queryKey = COUNTRIES_QUERY_PREFIX + query;
            var countries = HttpRuntime.Cache.Get(queryKey) as IEnumerable<Country>;
            if (countries == null)
            {
                lock (_locker)
                {
                    countries = HttpRuntime.Cache.Get(queryKey) as IEnumerable<Country>;
                    if (countries == null)
                    {
                        countries = Countries
                            .Where(country => country.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                            .Take(limit);

                        HttpRuntime.Cache.Add(queryKey, countries, new CacheDependency(_countriesFilePath),
                            DateTime.UtcNow + CACHE_EXPIRATION_TIME, Cache.NoSlidingExpiration, CacheItemPriority.Low, null);
                    }
                }
            }
            return countries;
        }

        public IEnumerable<Country> Countries
        {
            get
            {
                var countries = HttpRuntime.Cache.Get(COUNTRIES_KEY) as IEnumerable<Country>;
                if (countries == null)
                {
                    lock (_locker)
                    {
                        countries = HttpRuntime.Cache.Get(COUNTRIES_KEY) as IEnumerable<Country>;
                        if (countries == null)
                        {
                            countries = File.ReadAllLines(_countriesFilePath, Encoding.UTF8)
                                .Where(line => !String.IsNullOrWhiteSpace(line))
                                .Select(line => line.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                                .Select(fields => new Country() { Name = fields[0], Capital = fields.Length > 1 ? fields[1] : null })
                                .OrderBy(country => country.Name);

                            HttpRuntime.Cache.Add(COUNTRIES_KEY, countries, new CacheDependency(_countriesFilePath),
                                DateTime.UtcNow + CACHE_EXPIRATION_TIME, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                        }
                    }
                }
                return countries;
            }
        } 

    }
}