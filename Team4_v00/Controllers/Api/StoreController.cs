using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ben_Project.DB;
using Ben_Project.Models.AndroidDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ben_Project.Controllers.Api
{
    [Route("api/store")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly LogicContext _dbContext;

        public StoreController(LogicContext logicContext)
        {
            _dbContext = logicContext;
        }
    }
}