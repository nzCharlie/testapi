using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using testapi.Models;
using testapi.Services;

namespace testapi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TotalController : ControllerBase
    {   

        private readonly TextTagValidator tagValidator;

        private readonly OutputCreator outputCreator;

        public TotalController(TextTagValidator tagValidator, OutputCreator outputCreator)
        {
            this.tagValidator = tagValidator;
            this.outputCreator = outputCreator;
        }

        [HttpPost]
        public ActionResult<Output> Post([FromBody] Input value)
        {
            try
            {
                tagValidator.ValidateTags(value);

                return new ActionResult<Output>(outputCreator.create(value));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}