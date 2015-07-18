using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Caching;
using System.Web.Http;
using System.Web.Http.Results;
using AutoComplete.Models;

namespace AutoComplete.Controllers
{
    public class CountryController : ApiController
    {
        // GET: api/Country/GetCountries?firstLetters=...
        public JsonResult<IEnumerable<Country>> GetCountries(string firstLetters)
        {
            return Json<IEnumerable<Country>>(CountriesContext.Instance.GetCountries(firstLetters));
        }
    }
}
