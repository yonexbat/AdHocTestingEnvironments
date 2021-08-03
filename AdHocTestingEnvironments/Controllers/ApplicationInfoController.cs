using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdHocTestingEnvironments.Data;
using AdHocTestingEnvironments.Model.Entities;

namespace AdHocTestingEnvironments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationInfoController : ControllerBase
    {
        private readonly AdHocTestingEnvironmentsContext _context;

        public ApplicationInfoController(AdHocTestingEnvironmentsContext context)
        {
            _context = context;
        }

        // GET: api/ApplicationInfoEntitiys
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationInfoEntity>>> GetApplicationInfoEntitiy()
        {
            return await _context.InfoEntities.ToListAsync();
        }

        // GET: api/ApplicationInfoEntitiys/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationInfoEntity>> GetApplicationInfoEntitiy(int id)
        {
            var applicationInfoEntitiy = await _context.InfoEntities.FindAsync(id);

            if (applicationInfoEntitiy == null)
            {
                return NotFound();
            }

            return applicationInfoEntitiy;
        }

        // PUT: api/ApplicationInfoEntitiys/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApplicationInfoEntitiy(int id, ApplicationInfoEntity applicationInfoEntitiy)
        {
            if (id != applicationInfoEntitiy.Id)
            {
                return BadRequest();
            }

            _context.Entry(applicationInfoEntitiy).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationInfoEntitiyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ApplicationInfoEntitiys
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApplicationInfoEntity>> PostApplicationInfoEntitiy(ApplicationInfoEntity applicationInfoEntitiy)
        {
            _context.InfoEntities.Add(applicationInfoEntitiy);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetApplicationInfoEntitiy", new { id = applicationInfoEntitiy.Id }, applicationInfoEntitiy);
        }

        // DELETE: api/ApplicationInfoEntitiys/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplicationInfoEntitiy(int id)
        {
            var applicationInfoEntitiy = await _context.InfoEntities.FindAsync(id);
            if (applicationInfoEntitiy == null)
            {
                return NotFound();
            }

            _context.InfoEntities.Remove(applicationInfoEntitiy);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ApplicationInfoEntitiyExists(int id)
        {
            return _context.InfoEntities.Any(e => e.Id == id);
        }
    }
}
