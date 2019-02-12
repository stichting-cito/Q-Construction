using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Citolab.QConstruction.Logic.HelperClasses.Entities;
using Citolab.QConstruction.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Citolab.QConstruction.Backend.Controllers
{
    [Route("api/[controller]")]
    public class SettingsController : Controller
    {
        [HttpGet]
        [SwaggerOperation("Get auth0 settings for client")]
        public ActionResult<Auth0Settings> Get() => new Auth0Settings();
    }

    public class Auth0Settings
    {
        public string Auth0ClientId => Constants.FontendClientId;
        public string Auth0TenantUrl => Constants.Auth0Domain;
    }
}
