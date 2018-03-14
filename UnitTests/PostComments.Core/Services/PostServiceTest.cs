using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using PostComments.Core.Dtos;
using PostComments.Core.Entities.Post;
using PostComments.Core.Exceptions;
using PostComments.Core.Interfaces;
using PostComments.Core.Services;
using Xunit;

namespace UnitTests.PostComments.Core.Services
{
    public class PostServiceTest
    {
        private const string PostText = "Text";
        private const string PostTitle = "Title";
        private readonly Mock<IAsyncRepository<Post>> _mockPostRepository;

        public PostServiceTest()
        {
            _mockPostRepository = new Mock<IAsyncRepository<Post>>();
        }

        [Fact]
        public async void Throws_Given_PostDto_isNull()
        {
            IPostService postService = new PostService(_mockPostRepository.Object);

            //DTO is null
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await postService.CreatePostAsync(null, Guid.Empty));
        }

        [Fact]
        public async void Throws_Given_PostDto_text_isInvalid()
        {
            IPostService postService = new PostService(_mockPostRepository.Object);

            CreatePostDto createPostDto = new CreatePostDto();

            //Text is invalid
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await postService.CreatePostAsync(createPostDto, Guid.Empty));
        }

        [Fact]
        public async void Throws_Given_PostDto_title_isInvalid()
        {
            IPostService postService = new PostService(_mockPostRepository.Object);

            CreatePostDto createPostDto = new CreatePostDto
            {
                Text = PostText,
                Title = null
            };

            //Title is invalid
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await postService.CreatePostAsync(createPostDto, Guid.Empty));
        }

        [Fact]
        public async void Throws_Given_PostDto_from_isInvalid()
        {
            IPostService postService = new PostService(_mockPostRepository.Object);

            CreatePostDto createPostDto = new CreatePostDto
            {
                Text = PostText,
                Title = PostTitle
            };

            //From is invalid
            await Assert.ThrowsAsync<ArgumentException>(async () => await postService.CreatePostAsync(createPostDto, Guid.Empty));
        }

        [Fact]
        public async void Create_New_Post_Success()
        {
            IPostService postService = new PostService(_mockPostRepository.Object);


            var fromId = Guid.NewGuid();

            CreatePostDto createPostDto = new CreatePostDto
            {
                Text = PostText,
                Title = PostTitle
            };

            Post post = new Post(new Content(createPostDto.Text), new Title(createPostDto.Title), fromId);

            _mockPostRepository.Setup(repo => repo.AddAsync(It.Is<Post>(p => p.FromId == post.FromId))).Returns(Task.CompletedTask);
            _mockPostRepository.Setup(repo => repo.AddAsync(It.Is<Post>(p => p.FromId != post.FromId)));//.ReturnsAsync(() => null);
            _mockPostRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(post);//.ReturnsAsync(() => null);


            var returnedPost = await postService.CreatePostAsync(createPostDto, fromId);

            Assert.NotNull(returnedPost);
            Assert.Equal(post.Title.Text, createPostDto.Title);
            Assert.Equal(post.Content.Text, createPostDto.Text);
            Assert.Equal(post.FromId, fromId);
        }

        [Fact]
        public async void Get_All_Posts_Should_Returns_One_Row()
        {
            IPostService postService = new PostService(_mockPostRepository.Object);

            var posts = new List<Post>
            {
                new Post(new Content(PostText), new Title(PostTitle), Guid.NewGuid())
            };

            _mockPostRepository.Setup(repo => repo.ListAllAsync()).ReturnsAsync(posts);

            var returnedPosts = await postService.GetPostsAsync();

            var returnedPost = returnedPosts.Single();
            var post = posts.Single();

            Assert.Equal(returnedPost.Content.Text, post.Content.Text);
            Assert.Equal(returnedPost.FromId, post.FromId);
            Assert.Equal(returnedPost.Id, post.Id);
            Assert.Equal(returnedPost.CreatedOn, post.CreatedOn);
            Assert.Equal(returnedPost.Title.Text, post.Title.Text);
        }

        [Fact]
        public async void Get_All_Posts_Should_Returns_More_Then_One_Row()
        {
            IPostService postService = new PostService(_mockPostRepository.Object);

            var posts = new List<Post>
            {
                new Post(new Content(PostText), new Title(PostTitle), Guid.NewGuid()),
                new Post(new Content(PostText), new Title(PostTitle), Guid.NewGuid()),
                new Post(new Content(PostText), new Title(PostTitle), Guid.NewGuid())
            };

            _mockPostRepository.Setup(repo => repo.ListAllAsync()).ReturnsAsync(posts);

            var returnedPosts = await postService.GetPostsAsync();

            var returnedPost = returnedPosts;

            Assert.Equal(3, returnedPost.Count());
        }

        [Fact]
        public async void Get_Post_By_Id_Should_Returns_OneRow()
        {
            IPostService postService = new PostService(_mockPostRepository.Object);

            var post = new Post(new Content(PostText), new Title(PostTitle), Guid.NewGuid());

            _mockPostRepository.Setup(repo => repo.GetByIdAsync(It.Is<Guid>(id => id == post.Id))).ReturnsAsync(post);

            Post returnedPost = await postService.GetPostByIdAsync(post.Id);

            Assert.NotNull(returnedPost);
            Assert.Equal(returnedPost.Content.Text, post.Content.Text);
            Assert.Equal(returnedPost.FromId, post.FromId);
            Assert.Equal(returnedPost.Id, post.Id);
            Assert.Equal(returnedPost.CreatedOn, post.CreatedOn);
            Assert.Equal(returnedPost.Title.Text, post.Title.Text);
        }

        [Fact]
        public async void Get_Post_By_Id_Throws_Given_Empty_id()
        {
            IPostService postService = new PostService(_mockPostRepository.Object);

            await Assert.ThrowsAsync<ArgumentException>(async () => await postService.GetPostByIdAsync(Guid.Empty));
        }

        [Fact]
        public async void Get_Post_By_Id_Throws_Post_Not_Exists()
        {
            IPostService postService = new PostService(_mockPostRepository.Object);

            var existingPost = new Post(new Content(PostText), new Title(PostTitle), Guid.NewGuid());

            _mockPostRepository.Setup(repo => repo.GetByIdAsync(It.Is<Guid>(id => id == existingPost.Id))).ReturnsAsync(existingPost);
            _mockPostRepository.Setup(repo => repo.GetByIdAsync(It.Is<Guid>(id => id != existingPost.Id))).ReturnsAsync(() => null);

            await Assert.ThrowsAsync<PostNotExistsException>(async () => await postService.GetPostByIdAsync(Guid.NewGuid()));
        }

        [Fact]
        public async void Update_Post_By_Id_Throws_Given_Empty_id()
        {
            IPostService postService = new PostService(_mockPostRepository.Object);

            await Assert.ThrowsAsync<ArgumentException>(async () => await postService.UpdatePostAsync(Guid.Empty, null));
        }

        [Fact]
        public async void Update_Post_By_Id_Throws_Given_Null_dto()
        {
            IPostService postService = new PostService(_mockPostRepository.Object);

            var initialPost = new Post(new Content(PostText), new Title(PostTitle), Guid.NewGuid());

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await postService.UpdatePostAsync(initialPost.Id, null));
        }

        [Fact]
        public async void Update_Post_By_Id_Throws_Given_Dto_Values_Are_Empty()
        {
            IPostService postService = new PostService(_mockPostRepository.Object);

            var initialPost = new Post(new Content(PostText), new Title(PostTitle), Guid.NewGuid());

            UpdatePostDto dto = new UpdatePostDto
            {
                Text = "",
                Title = null
            };

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await postService.UpdatePostAsync(initialPost.Id, dto));
        }

        [Fact]
        public async void Update_Post_By_Id_Throws_Post_Doesnot_Exist()
        {
            IPostService postService = new PostService(_mockPostRepository.Object);

            var initialPost = new Post(new Content(PostText), new Title(PostTitle), Guid.NewGuid());

            UpdatePostDto dto = new UpdatePostDto
            {
                Text = "New Text",
                Title = "New Title"
            };

            Post post = new Post(new Content(PostText), new Title(PostTitle), Guid.NewGuid());

            _mockPostRepository.Setup(repo => repo.GetByIdAsync(It.Is<Guid>(id => id == post.Id))).ReturnsAsync(post);
            _mockPostRepository.Setup(repo => repo.UpdateAsync(post)).Returns(Task.CompletedTask);


            await Assert.ThrowsAsync<PostNotExistsException>(async () => await postService.UpdatePostAsync(initialPost.Id, dto));
        }

        [Fact]
        public async void Delete_Post_By_Id_Throws_Post_Doesnot_Exist()
        {
            IPostService postService = new PostService(_mockPostRepository.Object);

            await Assert.ThrowsAsync<PostNotExistsException>(async () => await postService.DeletePostByIdAsync(Guid.NewGuid()));
        }
    }
}
