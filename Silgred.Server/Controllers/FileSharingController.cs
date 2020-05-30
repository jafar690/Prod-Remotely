﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Silgred.Server.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Silgred.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileSharingController : ControllerBase
    {
        public FileSharingController(DataService dataService)
        {
            DataService = dataService;
        }

        public DataService DataService { get; set; }

        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            var sharedFile = DataService.GetSharedFiled(id);
            // Shared files expire after a minute and become locked.
            if (sharedFile != null && sharedFile.Timestamp.AddMinutes(1) > DateTimeOffset.Now)
                return File(sharedFile.FileContents, sharedFile.ContentType, sharedFile.FileName);
            return NotFound();
        }

        [HttpPost]
        [RequestSizeLimit(500_000_000)]
        public List<string> Post()
        {
            var fileIDs = new List<string>();
            foreach (var file in Request.Form.Files)
            {
                var orgID = User.Identity.IsAuthenticated
                    ? DataService.GetUserByName(User.Identity.Name).OrganizationID
                    : null;
                var id = DataService.AddSharedFile(file, orgID);
                fileIDs.Add(id);
            }

            return fileIDs;
        }
    }
}