using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using PostComments.Core.Entities.Post;
using PostComments.Core.Interfaces;
using PostComments.Core.Services;
using Xunit;

namespace UnitTests.PostComments.Core.Services
{
    public class PostServiceTest
    {
        private const string postText = "Text";
        private const string postTitle = "Title";
        private Mock<IAsyncRepository<Post>> _mockPostRepository;

        public PostServiceTest()
        {
            _mockPostRepository = new Mock<IAsyncRepository<Post>>();
        }

        [Fact]
        public async void Throws_Given_PostDto_isNull()
        {
            var postService = new PostService(_mockPostRepository.Object);

            CreatePostDTO createPostDto = null;

            //DTO is null
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await postService.CreatePostAsync(createPostDto, Guid.Empty));
        }

        [Fact]
        public async void Throws_Given_PostDto_text_isInvalid()
        {
            var postService = new PostService(_mockPostRepository.Object);

            CreatePostDTO createPostDto = new CreatePostDTO();

            //Text is invalid
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await postService.CreatePostAsync(createPostDto, Guid.Empty));
        }

        [Fact]
        public async void Throws_Given_PostDto_title_isInvalid()
        {
            var postService = new PostService(_mockPostRepository.Object);

            CreatePostDTO createPostDto = new CreatePostDTO
            {
                Text = postText,
                Title = null
            };

            //Title is invalid
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await postService.CreatePostAsync(createPostDto, Guid.Empty));
        }

        [Fact]
        public async void Throws_Given_PostDto_from_isInvalid()
        {
            var postService = new PostService(_mockPostRepository.Object);

            CreatePostDTO createPostDto = new CreatePostDTO
            {
                Text = postText,
                Title = postTitle
            };

            //From is invalid
            await Assert.ThrowsAsync<ArgumentException>(async () => await postService.CreatePostAsync(createPostDto, Guid.Empty));
        }

        [Fact]
        public async void Create_New_Post_Success()
        {
            var postService = new PostService(_mockPostRepository.Object);


            var fromId = Guid.NewGuid();

            CreatePostDTO createPostDto = new CreatePostDTO
            {
                Text = postText,
                Title = postTitle
            };

            Post post = new Post(new Content(createPostDto.Text), new Title(createPostDto.Title), fromId);

            _mockPostRepository.Setup(repo => repo.AddAsync(It.Is<Post>(p => p.FromId == post.FromId))).ReturnsAsync(() => post);
            _mockPostRepository.Setup(repo => repo.AddAsync(It.Is<Post>(p => p.FromId != post.FromId))).ReturnsAsync(() => null);


            var returnedPost = await postService.CreatePostAsync(createPostDto, fromId);

            Assert.NotNull(returnedPost);
            Assert.Equal(post.Title.Text, createPostDto.Title);
            Assert.Equal(post.Content.Text, createPostDto.Text);
            Assert.Equal(post.FromId, fromId);
        }

        [Fact]
        public async void Get_All_Posts_Should_Returns_One_Row()
        {
            var postService = new PostService(_mockPostRepository.Object);

            var posts = new List<Post>
            {
                new Post(new Content(postText), new Title(postTitle), Guid.NewGuid())
            };

            _mockPostRepository.Setup(repo => repo.ListAllAsync()).ReturnsAsync(posts);

            var returnedPosts = await postService.GetPostsAsync();

            var returnedPost = returnedPosts.Single();
            var post = posts.Single();

            Assert.Equal(returnedPost.Content.Text, post.Content.Text);
            Assert.Equal(returnedPost.Content.Image, post.Content.Image);
            Assert.Equal(returnedPost.FromId, post.FromId);
            Assert.Equal(returnedPost.Id, post.Id);
            Assert.Equal(returnedPost.CreatedOn, post.CreatedOn);
            Assert.Equal(returnedPost.Title.Text, post.Title.Text);
        }

        [Fact]
        public async void Get_All_Posts_Should_Returns_More_Then_One_Row()
        {
            var postService = new PostService(_mockPostRepository.Object);

            var posts = new List<Post>
            {
                new Post(new Content(postText), new Title(postTitle), Guid.NewGuid()),
                new Post(new Content(postText), new Title(postTitle), Guid.NewGuid()),
                new Post(new Content(postText), new Title(postTitle), Guid.NewGuid())
            };

            _mockPostRepository.Setup(repo => repo.ListAllAsync()).ReturnsAsync(posts);

            var returnedPosts = await postService.GetPostsAsync();

            var returnedPost = returnedPosts;

            Assert.Equal(3, returnedPost.Count());
        }

        [Fact]
        public async void Get_Post_By_Id_Should_Returns_OneRow()
        {
            var postService = new PostService(_mockPostRepository.Object);

            var post = new Post(new Content(postText), new Title(postTitle), Guid.NewGuid());

            _mockPostRepository.Setup(repo => repo.GetByIdAsync(It.Is<Guid>(id => id == post.Id))).ReturnsAsync(post);

            Post returnedPost = await postService.GetPostByIdAsync(post.Id);

            Assert.NotNull(returnedPost);
            Assert.Equal(returnedPost.Content.Text, post.Content.Text);
            Assert.Equal(returnedPost.Content.Image, post.Content.Image);
            Assert.Equal(returnedPost.FromId, post.FromId);
            Assert.Equal(returnedPost.Id, post.Id);
            Assert.Equal(returnedPost.CreatedOn, post.CreatedOn);
            Assert.Equal(returnedPost.Title.Text, post.Title.Text);
        }

        [Fact]
        public async void Get_Post_By_Id_Throws_Given_Empty_id()
        {
            var postService = new PostService(_mockPostRepository.Object);

            await Assert.ThrowsAsync<ArgumentException>(async () => await postService.GetPostByIdAsync(Guid.Empty));
        }
    }
}
