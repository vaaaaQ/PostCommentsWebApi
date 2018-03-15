using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using PostComments.BLL.Dtos;
using PostComments.BLL.Entities.Comment;
using PostComments.BLL.Entities.Post;
using PostComments.Service;
using Xunit;
using PostComments.BLL.Extensions;

namespace PostComments.IntegrationTests
{
    public class CommentsControllerTests
    {
        private HttpClient HttpClient { get; set; }
        public CommentsControllerTests()
        {
            var server = new TestServer(new WebHostBuilder().UseStartup<Startup>());

            HttpClient = server.CreateClient();
        }

        [Fact]
        public async void Comments_GET_All_By_Post_Returns_200()
        {
            #region Create new post
            Post createdPost = await CreatePost();
            #endregion


            var response = await this.HttpClient.GetAsync($"/api/v1/posts/{createdPost.Id}/comments");

            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async void Comments_GET_By_Id_Returns_404()
        {
            Guid id = Guid.NewGuid();
            var response = await this.HttpClient.GetAsync($"/api/v1/comments/{id}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async void Comments_GET_By_Id_Returns_400()
        {
            var response = await this.HttpClient.GetAsync($"/api/v1/comments/{Guid.Empty}");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }


        [Fact]
        public async void Comments_GET_By_Id_Returns_200()
        {
            Post post = await CreatePost();
            Comment createComment = await CreateComment(post);

            var response = await this.HttpClient.GetAsync($"/api/v1/comments/{createComment.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Fact]
        public async void Comments_POST_Returns_201()
        {
            Post post = await CreatePost();
            CreateCommentDto commentDto = new CreateCommentDto()
            {
                Text = "Text"
            };

            var response = await this.HttpClient.PostAsync($"/api/v1/posts/{post.Id}/comments", new StringContent(JsonConvert.SerializeObject(commentDto), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async void Comments_POST_Returns_400()
        {
            Post post = await CreatePost();
            CreateCommentDto commentDto = new CreateCommentDto()
            {
                Text = String.Empty
            };

            var response = await this.HttpClient.PostAsync($"/api/v1/posts/{post.Id}/comments", new StringContent(JsonConvert.SerializeObject(commentDto), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async void Comments_POST_Returns_404()
        {
            CreateCommentDto commentDto = new CreateCommentDto()
            {
                Text = "Text"
            };

            var response = await this.HttpClient.PostAsync($"/api/v1/posts/{Guid.NewGuid()}/comments", new StringContent(JsonConvert.SerializeObject(commentDto), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async void Comments_PUT_Returns_400()
        {
            UpdateCommentDto updatedComment = new UpdateCommentDto()
            {
                Text = string.Empty
            };

            Post post = await CreatePost();
            Comment createComment = await CreateComment(post);

            var response = await this.HttpClient.PutAsync($"/api/v1/comments/{createComment.Id}", new StringContent(JsonConvert.SerializeObject(updatedComment), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async void Comments_PUT_Returns_404()
        {
            UpdateCommentDto updatedComment = new UpdateCommentDto()
            {
                Text = string.Empty
            };

            var response = await this.HttpClient.PutAsync($"/api/v1/comments/{Guid.NewGuid()}", new StringContent(JsonConvert.SerializeObject(updatedComment), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async void Comments_PUT_Returns_200()
        {
            UpdateCommentDto updatedComment = new UpdateCommentDto()
            {
                Text = string.Empty
            };

            Post post = await CreatePost();
            Comment createComment = await CreateComment(post);

            var response = await this.HttpClient.PutAsync($"/api/v1/comments/{createComment.Id}", new StringContent(JsonConvert.SerializeObject(updatedComment), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }



        [Fact]
        public async void Comments_DELETE_Returns_404()
        {
            var response = await this.HttpClient.DeleteAsync($"/api/v1/comments/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async void Posts_DELETE_Returns_204()
        {
            Post post = await CreatePost();
            Comment createComment = await CreateComment(post);

            var response = await this.HttpClient.DeleteAsync($"/api/v1/comments/{createComment.Id}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }




        private async System.Threading.Tasks.Task<Comment> CreateComment(Post post)
        {
            CreateCommentDto commentDto = new CreateCommentDto()
            {
                Text = "Text"
            };

            var create_comment_response = await this.HttpClient.PostAsync($"/api/v1/posts/{post.Id}/comments", new StringContent(JsonConvert.SerializeObject(commentDto), Encoding.UTF8, "application/json"));
            var createdPostJson = await create_comment_response.Content.ReadAsStringAsync();
            create_comment_response.EnsureSuccessStatusCode();
            Comment createComment = JsonConvert.DeserializeObject<Comment>(createdPostJson);
            return createComment;
        }
        private async System.Threading.Tasks.Task<Post> CreatePost()
        {
            CreatePostDto newpost = new CreatePostDto()
            {
                Text = "New post",
                Title = "Title of new post"
            };

            var create_post_response = await this.HttpClient.PostAsync($"/api/v1/posts/", new StringContent(JsonConvert.SerializeObject(newpost), Encoding.UTF8, "application/json"));
            var createdPostJson = await create_post_response.Content.ReadAsStringAsync();
            create_post_response.EnsureSuccessStatusCode();
            Post createdPost = JsonConvert.DeserializeObject<Post>(createdPostJson);
            return createdPost;
        }
    }
}
