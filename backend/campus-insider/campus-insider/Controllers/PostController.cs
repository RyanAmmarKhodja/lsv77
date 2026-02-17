using campus_insider.DTOs;
using campus_insider.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace campus_insider.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostService _postService;
       
        public PostController(IPostService postService) {
             _postService = postService;
        }

        private long GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(userIdString, out long userId) ? userId : 0;
        }


        // GET: api/post/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(long id)
        {
            var post = await _postService.GetPostByIdAsync(id);

            if (post == null)
                return NotFound(new { message = "Post not found" });

            return Ok(post);
        }

        //// GET: api/post/user/{userId}
        //[HttpGet("user/{userId}")]
        //public async Task<IActionResult> GetUserPosts(Guid userId)
        //{
        //    var posts = await _postService.GetUserPostsAsync(userId);
        //    return Ok(posts);
        //}

        // POST: api/post/coride
        [Authorize]
        [HttpPost("coride")]
        public async Task<IActionResult> CreateCoride([FromBody] CreateCorideDto dto)
        {
            var authorId = GetCurrentUserId();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var coride = await _postService.CreateCorideAsync(authorId, dto);
            return CreatedAtAction(nameof(GetPost), new { id = coride.Id }, coride);
        }

        // POST: api/post/equipment
        [Authorize]
        [HttpPost("equipment")]
        public async Task<IActionResult> CreateEquipment([FromBody] CreateEquipmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorId = GetCurrentUserId(); 

            var equipment = await _postService.CreateEquipmentAsync(authorId, dto);
            return CreatedAtAction(nameof(GetPost), new { id = equipment.Id }, equipment);
        }


        // PATCH: api/post/{id}/deactivate
        [Authorize]
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> DeactivatePost(long id)
        {
            var authorId = GetCurrentUserId();

            var result = await _postService.DeactivatePostAsync(id, authorId);

            if (!result)
                return NotFound(new { message = "Post not found or you don't have permission to deactivate it" });

            return Ok(new { message = "Post deactivated successfully" });
        }

        // DELETE: api/post/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(long id)
        {
            var authorId = GetCurrentUserId();
            var result = await _postService.DeletePostAsync(id, authorId);

            if (!result)
                return NotFound(new { message = "Post not found or you don't have permission to delete it" });

            return Ok(new { message = "Post deleted successfully" });
        }
    }
}
