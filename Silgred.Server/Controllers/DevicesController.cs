using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Silgred.Server.Services;
using Silgred.Shared.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Silgred.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesController : ControllerBase
    {
        public DevicesController(DataService dataService, UserManager<RemotelyUser> userManager)
        {
            DataService = dataService;
            UserManager = userManager;
        }

        private DataService DataService { get; }
        private UserManager<RemotelyUser> UserManager { get; }


        [HttpGet]
        public IEnumerable<Device> Get()
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);

            if (User.Identity.IsAuthenticated) return DataService.GetDevicesForUser(User.Identity.Name);

            return DataService.GetAllDevices(orgID);
        }

        [HttpGet("{id}")]
        public Device Get(string id)
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);

            var device = DataService.GetDevice(orgID, id);

            if (User.Identity.IsAuthenticated &&
                !DataService.DoesUserHaveAccessToDevice(id, User.Identity.Name))
                return null;
            return device;
        }
    }
}