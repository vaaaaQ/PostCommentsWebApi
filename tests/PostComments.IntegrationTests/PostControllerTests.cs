using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using PostComments.BLL.Entities.Post;
using PostComments.Service;
using Xunit;
using PostComments.BLL.Extensions;
using PostComments.Service.Dtos;

namespace PostComments.IntegrationTests
{
    public class PostControllerTests
    {
        private HttpClient HttpClient { get; set; }
        public PostControllerTests()
        {
            var server = new TestServer(new WebHostBuilder().UseStartup<Startup>());

            HttpClient = server.CreateClient();
        }

        [Fact]
        public async void Posts_GET_All_Returns_200()
        {
            var response = await this.HttpClient.GetAsync("/api/v1/posts");

            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async void Posts_GET_By_Id_Returns_404()
        {
            Guid id = Guid.NewGuid(); 
            var response = await this.HttpClient.GetAsync($"/api/v1/posts/{id}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async void Posts_GET_By_Id_Returns_200()
        {
            Guid id = Guid.Empty.Increment();
            var response = await this.HttpClient.GetAsync($"/api/v1/posts/{id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async void Posts_POST_Returns_201()
        {
            Guid id = Guid.Empty.Increment();
            CreatePostDto post = new CreatePostDto()
            {
                Text = "New post",
                Title = "Title of new post"
            };

            var response = await this.HttpClient.PostAsync($"/api/v1/posts/", new StringContent(JsonConvert.SerializeObject(post), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async void Posts_POST_Returns_400()
        {
            CreatePostDto post = new CreatePostDto()
            {
                Text = "New post",
                Title = string.Empty
            };

            var response = await this.HttpClient.PostAsync($"/api/v1/posts/", new StringContent(JsonConvert.SerializeObject(post), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async void Posts_PUT_Returns_400()
        {
            UpdatePostDto post = new UpdatePostDto()
            {
                Text = "New post",
                Title = string.Empty
            };

            var response = await this.HttpClient.PostAsync($"/api/v1/posts/", new StringContent(JsonConvert.SerializeObject(post), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async void Posts_PUT_Returns_404()
        {
            UpdatePostDto post = new UpdatePostDto()
            {
                Text = "Updated post",
                Title = "Updated title"
            };

            var response = await this.HttpClient.PutAsync($"/api/v1/posts/{Guid.NewGuid()}", new StringContent(JsonConvert.SerializeObject(post), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async void Posts_PUT_Returns_200()
        {

            Post createdPost = await CreatePost();

            UpdatePostDto post = new UpdatePostDto()
            {
                Text = "New post",
                Title = string.Empty
            };

            var response = await this.HttpClient.PutAsync($"/api/v1/posts/{createdPost.Id}", new StringContent(JsonConvert.SerializeObject(post), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async void Posts_DELETE_Returns_404()
        {
            var response = await this.HttpClient.DeleteAsync($"/api/v1/posts/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async void Posts_DELETE_Returns_204()
        {
            Post createdPost = await CreatePost();

            var response = await this.HttpClient.DeleteAsync($"/api/v1/posts/{createdPost.Id}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
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
