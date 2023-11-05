using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using FullStackAuth_WebAPI.Data;
using System.Security.Claims;
using FullStackAuth_WebAPI.Models;

namespace FullStackAuth_WebAPI.Controllers
{
    [Route("api/task")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/task
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                string userId = User.FindFirstValue("id");
                var tasks = _context.Tasks.Where(t => t.UserId == userId).ToList();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/task
        [HttpPost]
        public IActionResult CreateTask([FromBody] Tasks data)
        {
            try
            {
                string userId = User.FindFirstValue("id");

                // Ensure the category is associated with the user
                var category = _context.Categories.FirstOrDefault(c => c.Id == data.CategoryId && c.UserId == userId);

                if (category == null)
                {
                    return BadRequest("Invalid category or unauthorized access.");
                }

                var newTask = new Tasks
                {
                    Title = data.Title,
                    Description = data.Description,
                    DueDate = data.DueDate,
                    Priority = data.Priority,
                    UserId = userId,
                    CategoryId = data.CategoryId,
                };

                _context.Tasks.Add(newTask);
                _context.SaveChanges();

                return Ok(newTask);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/task/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Tasks data)
        {
            try
            {
                var userId = User.FindFirstValue("id");
                var existingTask = _context.Tasks.Find(id);

                if (existingTask == null)
                {
                    return NotFound();
                }

                if (userId == existingTask.UserId)
                {
                    existingTask.Title = data.Title;
                    existingTask.Description = data.Description;
                    existingTask.DueDate = data.DueDate;
                    existingTask.Priority = data.Priority;
                    existingTask.CategoryId = data.CategoryId;

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

        // DELETE api/task/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var userId = User.FindFirstValue("id");
                var task = _context.Tasks.Find(id);

                if (task == null)
                {
                    return NotFound();
                }

                if (userId == task.UserId)
                {
                    _context.Tasks.Remove(task);
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
