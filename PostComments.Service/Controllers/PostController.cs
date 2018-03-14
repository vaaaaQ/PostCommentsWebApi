using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        // GET api/posts
        [HttpGet]
        public async Task<IEnumerable<Post>> Get()
        {
            return await _postService.GetPostsAsync();
        }

        // GET api/posts/id
        [HttpGet("{id}")]
        public async Task<Post> Get(Guid id)
        {
            return await _postService.GetPostByIdAsync(id);
        }

        // POST api/posts
        [HttpPost]
        public async Task<Post> Post([FromBody]CreatePostDto createPostDto)
        {
            return await _postService.CreatePostAsync(createPostDto, Guid.Empty);
        }

        // PUT api/posts/id
        [HttpPut("/{id}")]
        public async Task Put(Guid id, [FromBody]UpdatePostDto updatePostDto)
        {
            await _postService.UpdatePostAsync(id, updatePostDto);
        }

        // DELETE api/posts/id
        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            await _postService.DeletePostByIdAsync(id);
        }
    }
}
