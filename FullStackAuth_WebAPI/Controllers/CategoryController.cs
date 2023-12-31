﻿using FullStackAuth_WebAPI.Data;
using FullStackAuth_WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FullStackAuth_WebAPI.Controllers
{
    [Route("api/category")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/<CategoryController>
        [HttpGet]
        public IActionResult Get()
        {


            try
            {
                string userId = User.FindFirstValue("id");
                var categories = _context.Categories.Where(c => c.UserId == userId).ToList();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<CategoryController>
        [HttpPost]
        public IActionResult CreateCategory([FromBody] Category data)
        {
            try
            {
                string userId = User.FindFirstValue("id");

                var newCategory = new Category
                {
                    Name = data.Name,
                    UserId = userId,
                    BoardId = data.BoardId,
                };

                _context.Categories.Add(newCategory);
                _context.SaveChanges();

                return Ok(newCategory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); 
            }
        }

        // PUT api/<CategoryController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Category data)
        {
            try
            {
                var userId = User.FindFirstValue("id");
                var existingCategory = _context.Categories.Find(id);

                if(existingCategory == null)
                {
                    return NotFound();
                }

                if(userId == existingCategory.UserId) //Checks if the User owns this Category
                {
                    existingCategory.Name = data.Name;
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

        // DELETE api/category/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var userId = User.FindFirstValue("id");
                var category = _context.Categories.Find(id);

                if (category == null)
                {
                    return NotFound();
                }

                if (userId == category.UserId)
                {
                    var tasksToDelete = _context.Tasks.Where(t => t.CategoryId == id).ToList();
                    
                    _context.Tasks.RemoveRange(tasksToDelete);
                    _context.Categories.Remove(category);

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
