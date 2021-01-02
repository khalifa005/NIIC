using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NIIC.API.Mail.MailKit;
using Persistence;

namespace NIIC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMailer _mailer;

        public ValuesController(DataContext context, IMailer mailer)
        {
            _context = context;
            _mailer = mailer;
        }
        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Value>>> Get()
        {
            //await _mailer.SendEmailAsync("mahmoudkh@silverkeytech.com", "Weather Report", "Detailed Weather Report");
            return Ok(await _context.Values.ToListAsync());
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Value>> Get(int id)
        {
            return Ok(await _context.Values.FirstOrDefaultAsync(x=>x.Id == id));
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
