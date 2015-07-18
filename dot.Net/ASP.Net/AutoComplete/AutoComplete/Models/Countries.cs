using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace AutoComplete.Models
{
    public class Country : IComparable
    {
        public string Name { get; set; }
        public string Capital { get; set; }

        public int CompareTo(object obj)
        {
            var typedObj = obj as Country;
            if (typedObj == null) return -1;
            return String.Compare(Name, typedObj.Name, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class CountriesContext
    {
        const string COUNTRIES_KEY = "Countries";
        private readonly string _countriesFilePath;

        private CountriesContext(string countriesVirtualPath)
        {
            _countriesFilePath = HttpContext.Current.Server.MapPath(countriesVirtualPath);
        }

        private static CountriesContext _context;
        public static CountriesContext Instance
        {
            get { return _context ?? (_context = new CountriesContext("~/App_Data/Countries.txt")); }
        }

        private bool equalsFirstLetters(Country country, string firstLetters)
        {
            return String.Compare(country.Name, 0, firstLetters, 0, firstLetters.Length, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public IList<Country> GetCountries(string firstLetters)
        {
            if (firstLetters != null) firstLetters = firstLetters.Trim();
            if (String.IsNullOrEmpty(firstLetters)) return new Country[0];
            var countries = Countries;
            var index = Array.BinarySearch(countries, new Country { Name = firstLetters });
            var searched = new List<Country>();
            if (index < 0)
            {
                index = ~index;
            }
            var len = Countries.Length;
            for (int i = index; i < len; i++)
            {
                var country = countries[i];
                if (!equalsFirstLetters(country, firstLetters)) break;
                searched.Add(country);
            }
            for (int i = index - 1; i >= 0; i--)
            {
                var country = countries[i];
                if (!equalsFirstLetters(country, firstLetters)) break;
                searched.Add(country);
            }
            return searched;
        }

        public Country[] Countries
        {
            get
            {
                var countries = HttpRuntime.Cache.Get(COUNTRIES_KEY) as Country[];
                if (countries == null)
                {
                    var text = File.ReadAllText(_countriesFilePath, Encoding.UTF8);
                    var lines = text.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    var len = lines.Length;
                    countries = new Country[len];
                    for (int i = 0; i < len; i++)
                    {
                        var fields = lines[i].Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        countries[i] = new Country() { Name = fields[0], Capital = fields[1] };   
                    }
                    Array.Sort(countries);
                    HttpRuntime.Cache.Add(COUNTRIES_KEY, countries, null, DateTime.UtcNow.AddDays(1), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                }
                return countries;
            }
        } 

    }
}
