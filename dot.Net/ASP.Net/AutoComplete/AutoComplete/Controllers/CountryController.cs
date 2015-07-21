using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using AutoComplete.Models;

namespace AutoComplete.Controllers
{
    public class CountryController : ApiController
    {
        private static readonly CountriesContext _countriesContext = new CountriesContext(HttpContext.Current.Server.MapPath("~/App_Data/Countries.txt")); 

        // GET: api/Country/GetCountries?firstLetters=...
        public JsonResult<IEnumerable<Country>> GetCountries(string query)
        {
            return Json(_countriesContext.GetCountries(query, 10));
        }
    }
}
