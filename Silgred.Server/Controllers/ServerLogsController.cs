﻿using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Silgred.Server.Services;

namespace Silgred.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerLogsController : ControllerBase
    {
        public ServerLogsController(DataService dataService)
        {
            DataService = dataService;
        }

        public DataService DataService { get; set; }

        [HttpGet("Download")]
        public ActionResult Download()
        {
            Request.Headers.TryGetValue("OrganizationID", out var orgID);
            var logs = DataService.GetAllEventLogs(orgID);
            var fileBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(logs));
            return File(fileBytes, "application/octet-stream", "ServerLogs.json");
        }
    }
}