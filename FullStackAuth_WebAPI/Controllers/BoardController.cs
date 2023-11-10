using FullStackAuth_WebAPI.Data;
using FullStackAuth_WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FullStackAuth_WebAPI.Controllers
{
    [Route("api/board")]
    [ApiController]
    [Authorize]
    public class BoardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BoardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/board
        [HttpGet]
        public IActionResult GetUserBoards()
        {
            try
            {
                var userId = User.FindFirstValue("id");
                var userBoards = _context.Boards
                    .Where(b => b.UserId == userId)
                    .ToList();
                return Ok(userBoards);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/board
        [HttpPost]
        public IActionResult AddBoard([FromBody] Board data)
        {
            try
            {
                var userId = User.FindFirstValue("id");
                var newBoard = new Board
                {
                    Name = data.Name,
                    UserId = userId,
                };

                _context.Boards.Add(newBoard);
                _context.SaveChanges();

                return Ok(newBoard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/board/5
        [HttpPut("{id}")]
        public IActionResult EditBoard(int id, [FromBody] Board data)
        {
            try
            {
                var userId = User.FindFirstValue("id");
                var existingBoard = _context.Boards.Find(id);

                if (existingBoard == null)
                {
                    return NotFound();
                }

                if (userId == existingBoard.UserId)
                {
                    existingBoard.Name = data.Name;
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

        // DELETE api/board/5
        [HttpDelete("{id}")]
        public IActionResult DeleteBoard(int id)
        {
            try
            {
                var userId = User.FindFirstValue("id");
                var board = _context.Boards.Find(id);
                if (board == null)
                {
                    return NotFound();
                }

                if (userId == board.UserId)
                {
                    _context.Boards.Remove(board);
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
