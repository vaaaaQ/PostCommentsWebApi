using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PostComments.BLL;
using PostComments.BLL.Dtos;
using PostComments.BLL.Entities.Post;
using PostComments.Core.Interfaces;

namespace PostComments.Service.Controllers
{
    [Route("api/v1/[controller]")]
    public class PostsController : Controller
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        /// <summary>
        /// Returns posts
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/posts/
        ///
        /// </remarks>
        /// <returns>All posts</returns>
        /// <response code="200">Everything is ok</response>
        [HttpGet]
        public async Task<IEnumerable<Post>> Get()
        {
            return await _postService.GetPostsAsync();
        }

        /// <summary>
        /// Returns post data
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/posts/{id}
        ///
        /// </remarks>
        /// <param name="id">post id</param>
        /// <returns>Post data for given id</returns>
        /// <response code="200">Everything is ok</response>
        /// <response code="400">If the given id is empty</response>     
        /// <response code="404">If the post with given id doesn't exist</response>     
        [HttpGet("{id}")]
        public async Task<Post> Get(Guid id)
        {
            return await _postService.GetPostByIdAsync(id);
        }

        /// <summary>
        /// Create new post
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1/posts/
        ///     {
        ///        "title": "Great Solutions",
        ///        "text": "You can find here"
        ///     }
        ///
        /// </remarks>
        /// <param name="post">post data</param>
        /// <returns>A newly-created post</returns>
        /// <response code="201">Returns the newly-created post</response>
        /// <response code="400">If the posted item is null or required fields are empty</response>            
        [HttpPost]
        [ProducesResponseType(typeof(Post), 201)]
        [ProducesResponseType(typeof(BaseErrorResponse), 400)]
        public async Task<Post> Post([FromBody]CreatePostDto post)
        {
            Guid userId = Guid.NewGuid(); // TODO change to get from authentications claims 

            HttpContext.Response.StatusCode = (int) HttpStatusCode.Created;
            return await _postService.CreatePostAsync(post, userId);
        }

        /// <summary>
        /// Update post
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/v1/posts/{id}
        ///     {
        ///        "title": "Great Solutions",
        ///        "text": "You can find here"
        ///     }
        ///
        /// </remarks>
        /// <param name="post">post data</param>
        /// <returns>A newly-created post</returns>
        /// <response code="200">Returns the updated post</response>
        /// <response code="400">If the posted item is null or required fields are empty</response>   
        /// <response code="404">If post doesn't exist</response>   
        [HttpPut("{id}")]
        public async Task Put(Guid id, [FromBody]UpdatePostDto updatePostDto)
        {
            await _postService.UpdatePostAsync(id, updatePostDto);
        }

        /// <summary>
        /// Delete post
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     Delete /api/v1/posts/{id}
        ///
        /// </remarks>
        /// <param name="id">post id</param>
        /// <returns>A newly-created post</returns>
        /// <response code="204">Returns if deleted</response>
        /// <response code="404">If post doesn't exist</response>  
        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            HttpContext.Response.StatusCode = (int) HttpStatusCode.NoContent;
            await _postService.DeletePostByIdAsync(id);
        }
    }
}
