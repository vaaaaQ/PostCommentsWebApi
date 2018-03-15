using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PostComments.BLL;
using PostComments.BLL.Dtos;
using PostComments.BLL.Entities.Comment;
using PostComments.BLL.Entities.Post;
using PostComments.BLL.Interfaces;
using PostComments.Core.Interfaces;

namespace PostComments.Service.Controllers
{
    [Route("api/v1/[controller]")]
    public class CommentsController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        /// <summary>
        /// Returns comments by post
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/posts/{postId}/comments
        ///
        /// </remarks>
        /// <param name="postId">post id</param>
        /// <returns>All comments by post</returns>
        /// <response code="200">Everything is ok</response>
        /// <response code="404">If the post with given postId doesn't exist</response>     
        [HttpGet]
        [Route("~/api/v1/posts/{postId}/[controller]")]
        public async Task<IEnumerable<Comment>> GetByPost(Guid postId)
        {
            return await _commentService.GetCommentsByPostId(postId);
        }

        /// <summary>
        /// Returns comment data
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/comments/{id}
        ///
        /// </remarks>
        /// <param name="id">comment id</param>
        /// <returns>Post data for given id</returns>
        /// <response code="200">Everything is ok</response>
        /// <response code="400">If the given id is empty</response>     
        /// <response code="404">If the comment with given id doesn't exist</response>     
        [HttpGet("{id}")]
        public async Task<Comment> Get(Guid id)
        {
            return await _commentService.GetCommentById(id);
        }

        /// <summary>
        /// Create new comment
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1/posts/{postId}/comments
        ///     {
        ///        "text": "You can find here"
        ///     }
        ///
        /// </remarks>
        /// <param name="postId">post id</param>
        /// <param name="comment">new comment data</param>
        /// <returns>A newly-created post</returns>
        /// <response code="201">Returns the newly-created post</response>
        /// <response code="400">If the posted item is null or required fields are empty</response>            
        /// <response code="404">If the post with given id doesn't exist</response>     
        [HttpPost]
        [Route("~/api/v1/posts/{postId}/[controller]")]
        public async Task<Comment> Post([FromBody]CreateCommentDto comment, Guid postId)
        {
            Guid userId = Guid.NewGuid(); // TODO change to get user_id from authentication claims

            HttpContext.Response.StatusCode = (int) HttpStatusCode.Created;
            return await _commentService.CreateCommentAsync(comment, userId, postId);
        }

        /// <summary>
        /// Update comment
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/v1/comments/{id}
        ///     {
        ///        "text": "You can find here"
        ///     }
        ///
        /// </remarks>
        /// <param name="comment">comment data</param>
        /// <param name="id">comment id</param>
        /// <returns>An updated comment</returns>
        /// <response code="200">Returns the updated comment</response>
        /// <response code="400">If the posted item is null or required fields are empty</response>   
        /// <response code="404">If the comment doesn't exist</response>   
        [HttpPut("{id}")]
        [Route("api/v1/[controller]/{id}")]
        public async Task<Comment> Put(Guid id, [FromBody]UpdateCommentDto comment)
        {
            return await _commentService.UpdateCommentAsync(comment, id);
        }

        /// <summary>
        /// Delete comment
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     Delete /api/v1/comments/{id}
        ///
        /// </remarks>
        /// <param name="id">comment id</param>
        /// <response code="204">Returns if deleted</response>
        /// <response code="404">If post doesn't exist</response>  
        [HttpDelete("{id}")]
        [Route("api/v1/[controller]/{id}")]
        public async Task Delete(Guid id)
        {
            HttpContext.Response.StatusCode = (int) HttpStatusCode.NoContent;
            await _commentService.DeleteCommentAsync(id);
        }
    }
}
