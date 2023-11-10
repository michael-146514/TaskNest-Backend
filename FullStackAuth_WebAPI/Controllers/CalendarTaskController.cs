using FullStackAuth_WebAPI.Data;
using FullStackAuth_WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FullStackAuth_WebAPI.Controllers
{
    [Route("api/calendartasks")]
    [ApiController]
    [Authorize]
    public class CalendarTasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CalendarTasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/calendartasks
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                string userId = User.FindFirstValue("id");
                var tasks = _context.CalendarTasks.Where(t => t.UserId == userId).ToList();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{month}")]
        public IActionResult Get(int month)
        {
            try
            {
                string userId = User.FindFirstValue("id");

                // Retrieve tasks for the specified month.
                var tasks = _context.CalendarTasks
                    .Where(t => t.UserId == userId && t.DueDate.Month == month)
                    .ToList();

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/calendartasks
        [HttpPost]
        public IActionResult CreateTask([FromBody] CalendarTasks data)
        {
            try
            {
                string userId = User.FindFirstValue("id");

                var newTask = new CalendarTasks
                {
                    Title = data.Title,
                    Description = data.Description,
                    Status = data.Status,
                    DueDate = data.DueDate,
                    IsCompleted = data.IsCompleted,
                    UserId = userId,
                };

                _context.CalendarTasks.Add(newTask);
                _context.SaveChanges();

                return Ok(newTask);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/calendartasks/5
        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] CalendarTasks data)
        {
            try
            {
                var userId = User.FindFirstValue("id");
                var existingTask = _context.CalendarTasks.Find(id);

                if (existingTask == null)
                {
                    return NotFound();
                }

                if (userId == existingTask.UserId)
                {
                    existingTask.Title = data.Title;
                    existingTask.Description = data.Description;
                    existingTask.Status = data.Status;
                    existingTask.DueDate = data.DueDate;
                    existingTask.IsCompleted = data.IsCompleted;

                    _context.SaveChanges();
                    return Ok();
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/calendartasks/5
        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            try
            {
                var userId = User.FindFirstValue("id");
                var task = _context.CalendarTasks.Find(id);

                if (task == null)
                {
                    return NotFound();
                }

                if (userId == task.UserId)
                {
                    _context.CalendarTasks.Remove(task);
                    _context.SaveChanges();
                    return Ok();
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
