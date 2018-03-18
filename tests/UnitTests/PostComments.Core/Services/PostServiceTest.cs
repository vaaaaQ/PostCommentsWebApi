using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using PostComments.BLL.Entities.Post;
using PostComments.BLL.Exceptions;
using PostComments.BLL.Interfaces;
using PostComments.BLL.Services;
using PostComments.BLL.ViewModels;
using Xunit;

namespace PostComments.UnitTests.PostComments.Core.Services
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
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await postService.CreatePostAsync(null));
        }

        [Fact]
        public async void Throws_Given_PostDto_text_isInvalid()
        {
            IPostService postService = new PostService(_mockPostRepository.Object);

            CreatePostViewModel createPostViewModel = new CreatePostViewModel();

            //Text is invalid
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await postService.CreatePostAsync(createPostViewModel));
        }

        [Fact]
        public async void Throws_Given_PostDto_title_isInvalid()
        {
            IPostService postService = new PostService(_mockPostRepository.Object);

            CreatePostViewModel createPostViewModel = new CreatePostViewModel
            {
                Text = PostText,
                Title = null
            };

            //Title is invalid
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await postService.CreatePostAsync(createPostViewModel));
        }

        [Fact]
        public async void Throws_Given_PostDto_from_isInvalid()
        {
            IPostService postService = new PostService(_mockPostRepository.Object);

            CreatePostViewModel createPostViewModel = new CreatePostViewModel
            {
                Text = PostText,
                Title = PostTitle
            };

            //From is invalid
            await Assert.ThrowsAsync<ArgumentException>(async () => await postService.CreatePostAsync(createPostViewModel));
        }

        [Fact]
        public async void Create_New_Post_Success()
        {
            IPostService postService = new PostService(_mockPostRepository.Object);


            CreatePostViewModel createPostViewModel = new CreatePostViewModel
            {
                Text = PostText,
                Title = PostTitle,
                FromId = Guid.NewGuid()
            };

            Post post = new Post(new Content(createPostViewModel.Text), new Title(createPostViewModel.Title), createPostViewModel.FromId);

            _mockPostRepository.Setup(repo => repo.AddAsync(It.Is<Post>(p => p.FromId == post.FromId))).Returns(Task.CompletedTask);
            _mockPostRepository.Setup(repo => repo.AddAsync(It.Is<Post>(p => p.FromId != post.FromId)));
            _mockPostRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(post);


            var returnedPost = await postService.CreatePostAsync(createPostViewModel);

            Assert.NotNull(returnedPost);
            Assert.Equal(post.Title.Text, createPostViewModel.Title);
            Assert.Equal(post.Content.Text, createPostViewModel.Text);
            Assert.Equal(post.FromId, createPostViewModel.FromId);
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

            await Assert.ThrowsAsync<ArgumentException>(async () => await postService.UpdatePostAsync(new UpdatePostViewModel()));
        }


        [Fact]
        public async void Update_Post_By_Id_Throws_Given_Dto_Values_Are_Empty()
        {
            IPostService postService = new PostService(_mockPostRepository.Object);

            var initialPost = new Post(new Content(PostText), new Title(PostTitle), Guid.NewGuid());

            UpdatePostViewModel viewModel = new UpdatePostViewModel
            {
                Text = "",
                Title = null,
                InitialPostId = initialPost.Id
            };

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await postService.UpdatePostAsync(viewModel));
        }

        [Fact]
        public async void Update_Post_By_Id_Throws_Post_Doesnot_Exist()
        {
            IPostService postService = new PostService(_mockPostRepository.Object);

            var initialPost = new Post(new Content(PostText), new Title(PostTitle), Guid.NewGuid());

            UpdatePostViewModel viewModel = new UpdatePostViewModel
            {
                Text = "New Text",
                Title = "New Title"
            };

            Post post = new Post(new Content(PostText), new Title(PostTitle), Guid.NewGuid());
            viewModel.InitialPostId = initialPost.Id;
            _mockPostRepository.Setup(repo => repo.GetByIdAsync(It.Is<Guid>(id => id == post.Id))).ReturnsAsync(post);
            _mockPostRepository.Setup(repo => repo.UpdateAsync(post)).Returns(Task.CompletedTask);


            await Assert.ThrowsAsync<PostNotExistsException>(async () => await postService.UpdatePostAsync(viewModel));
        }

        [Fact]
        public async void Delete_Post_By_Id_Throws_Post_Doesnot_Exist()
        {
            IPostService postService = new PostService(_mockPostRepository.Object);

            await Assert.ThrowsAsync<PostNotExistsException>(async () => await postService.DeletePostByIdAsync(Guid.NewGuid()));
        }
    }
}
