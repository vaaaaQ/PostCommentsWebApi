using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Moq;
using PostComments.Core.Dtos;
using PostComments.Core.Entities.Comment;
using PostComments.Core.Entities.Post;
using PostComments.Core.Exceptions;
using PostComments.Core.Interfaces;
using PostComments.Core.Services;
using PostComments.Core.Specifications;
using Xunit;

namespace UnitTests.PostComments.Core.Services
{
    public class CommentServiceTest
    {
        private const string COMMENT_TEXT_CONTENT = "Comment text content";
        private const string POST_TEXT_CONTENT = "Post text content";
        private const string POST_TITLE_CONTENT = "Post title content";
        private readonly Mock<IAsyncRepository<Comment>> _mockCommentsRepository;
        private readonly Mock<IAsyncRepository<Post>> _mockPostRepository;

        public CommentServiceTest()
        {
            _mockCommentsRepository = new Mock<IAsyncRepository<Comment>>();
            _mockPostRepository = new Mock<IAsyncRepository<Post>>();
        }

        //create post

        #region Create post
        [Fact]
        public async void Throws_Given_CommentDto_isNull()
        {
            ICommentService commentService = new CommentService(_mockCommentsRepository.Object, _mockPostRepository.Object);

            //DTO is null
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await commentService.CreateCommentAsync(null, Guid.Empty, Guid.Empty));
        }

        [Fact]
        public async void Throws_Given_From_Id_isEmpty()
        {
            ICommentService commentService = new CommentService(_mockCommentsRepository.Object, _mockPostRepository.Object);

            CreateCommentDto createCommentDto = new CreateCommentDto()
            {
                Text = COMMENT_TEXT_CONTENT
            };

            await Assert.ThrowsAsync<ArgumentException>(async () => await commentService.CreateCommentAsync(createCommentDto, Guid.Empty, Guid.NewGuid()));
        }

        [Fact]
        public async void Throws_Given_Post_Id_isEmpty()
        {
            ICommentService commentService = new CommentService(_mockCommentsRepository.Object, _mockPostRepository.Object);

            CreateCommentDto createCommentDto = new CreateCommentDto()
            {
                Text = COMMENT_TEXT_CONTENT
            };

            await Assert.ThrowsAsync<ArgumentException>(async () => await commentService.CreateCommentAsync(createCommentDto, Guid.NewGuid(), Guid.Empty));
        }

        [Fact]
        public async void Throws_Given_CommentDto_Text_isInvalid()
        {
            ICommentService commentService = new CommentService(_mockCommentsRepository.Object, _mockPostRepository.Object);

            CreateCommentDto createCommentDto = new CreateCommentDto
            {
                Text = string.Empty
            };

            await Assert.ThrowsAsync<ArgumentException>(async () => await commentService.CreateCommentAsync(createCommentDto, Guid.NewGuid(), Guid.Empty));
        }

        [Fact]
        public async void Throws_Post_With_Given_Id_Not_Exists()
        {
            _mockPostRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(() => null);
            ICommentService commentService = new CommentService(_mockCommentsRepository.Object, _mockPostRepository.Object);

            CreateCommentDto createCommentDto = new CreateCommentDto
            {
                Text = COMMENT_TEXT_CONTENT
            };

            await Assert.ThrowsAsync<PostNotExistsException>(async () => await commentService.CreateCommentAsync(createCommentDto, Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async void Create_Post_Returns_Right_Comment()
        {
            CreateCommentDto createCommentDto = new CreateCommentDto
            {
                Text = COMMENT_TEXT_CONTENT + Guid.NewGuid().ToString()
            };

            Post post = new Post(new Content("Post text content"), new Title("Post title"), Guid.NewGuid());

            Guid fromId = Guid.NewGuid();

            Comment newComment = new Comment(fromId, post.Id, new Content(COMMENT_TEXT_CONTENT));

            _mockPostRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(post);
            _mockCommentsRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(newComment);
            _mockCommentsRepository.Setup(repo =>
                    repo.AddAsync(It.Is<Comment>(comment => comment.Content.Text == createCommentDto.Text)))
                .Returns(Task.CompletedTask);

            ICommentService commentService = new CommentService(_mockCommentsRepository.Object, _mockPostRepository.Object);

            var createdComment = await commentService.CreateCommentAsync(createCommentDto, fromId, post.Id);

            Assert.Equal(createdComment.Id, newComment.Id);
            Assert.Equal(createdComment.FromId, newComment.FromId);
            Assert.Equal(createdComment.PostId, newComment.PostId);
            Assert.Equal(createdComment.Content.Text, newComment.Content.Text);
        } 
        #endregion

        //GetALLPostComments

        #region Get all comments
        [Fact]
        public async void Get_All_Comments_By_PostId_Throws_Given_PostId_isEmpty()
        {
            ICommentService commentService = new CommentService(_mockCommentsRepository.Object, _mockPostRepository.Object);

            await Assert.ThrowsAsync<ArgumentException>(async () => await commentService.GetCommentsByPostId(Guid.Empty));
        }

        [Fact]
        public async void Get_All_Posts_Throws_Post_Not_Exists()
        {
            _mockPostRepository.Setup(post => post.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => null);
            ICommentService commentService = new CommentService(_mockCommentsRepository.Object, _mockPostRepository.Object);

            await Assert.ThrowsAsync<PostNotExistsException>(async () => await commentService.GetCommentsByPostId(Guid.NewGuid()));
        }

        [Fact]
        public async void Get_All_Comments_Return_Right_Count()
        {
            Guid postId1 = Guid.NewGuid();

            List<Comment> comments = new List<Comment>()
            {
                new Comment(Guid.NewGuid(), postId1, new Content(COMMENT_TEXT_CONTENT)),
                new Comment(Guid.NewGuid(), postId1, new Content(COMMENT_TEXT_CONTENT)),
                new Comment(Guid.NewGuid(), postId1, new Content(COMMENT_TEXT_CONTENT)),
            };


            _mockCommentsRepository.Setup(repo => repo.ListAsync(It.IsAny<ISpecification<Comment>>()))
                .ReturnsAsync(comments);

            _mockPostRepository.Setup(post => post.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => new Post(new Content(POST_TEXT_CONTENT), new Title(POST_TITLE_CONTENT), Guid.NewGuid()));

            ICommentService commentService = new CommentService(_mockCommentsRepository.Object, _mockPostRepository.Object);

            var post1Comments = await commentService.GetCommentsByPostId(postId1);

            Assert.Equal(3, post1Comments.Count());
        } 
        #endregion

        #region Get comment by id
        

        [Fact]
        public async void Get_Comment_By_Id_Throws_Given_CommentId_isEmpty()
        {

            _mockPostRepository.Setup(post => post.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => new Post(new Content(POST_TEXT_CONTENT), new Title(POST_TITLE_CONTENT), Guid.NewGuid()));

            ICommentService commentService = new CommentService(_mockCommentsRepository.Object, _mockPostRepository.Object);

            await Assert.ThrowsAsync<ArgumentException>(async () => await commentService.GetCommentById( Guid.Empty));
        }


        [Fact]
        public async void Get_Comment_By_Id_Returns_Right_Comment()
        {
            var comment = new Comment(Guid.NewGuid(), Guid.NewGuid(), new Content(COMMENT_TEXT_CONTENT));

            _mockCommentsRepository.Setup(repo => repo.GetByIdAsync(It.Is<Guid>(id => id == comment.Id))).ReturnsAsync(comment);

            _mockPostRepository.Setup(post => post.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => new Post(new Content(POST_TEXT_CONTENT), new Title(POST_TITLE_CONTENT), Guid.NewGuid()));

            ICommentService commentService = new CommentService(_mockCommentsRepository.Object, _mockPostRepository.Object);

            var returnedComment = await commentService.GetCommentById(comment.Id);

            Assert.Equal(comment.Id, returnedComment.Id);
            Assert.Equal(comment.PostId, returnedComment.PostId);
            Assert.Equal(comment.FromId, returnedComment.FromId);
        }
        #endregion

        //UpdatePost

        #region Update comment
        [Fact]
        public async void Update_Comment_By_Id_Throws_Given_UpdateCommentDto_isNull()
        {
            ICommentService commentService = new CommentService(_mockCommentsRepository.Object, _mockPostRepository.Object);

            //DTO is null
            UpdateCommentDto updateCommentDto = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await commentService.UpdateCommentAsync(updateCommentDto, Guid.NewGuid()));
        }


        [Fact]
        public async void Update_Comment_By_Id_Throws_Given_Comment_Id_isEmpty()
        {
            ICommentService commentService = new CommentService(_mockCommentsRepository.Object, _mockPostRepository.Object);

            UpdateCommentDto updateCommentDto = new UpdateCommentDto();
            await Assert.ThrowsAsync<ArgumentException>(async () => await commentService.UpdateCommentAsync(updateCommentDto, Guid.Empty));
        }
        

        [Fact]
        public async void Update_Comment_By_Id_Create_Post_Returns_Right_Comment()
        {
            UpdateCommentDto updateCommentDto = new UpdateCommentDto()
            {
                Text = "New comment text content"
            };
            Post post = new Post(new Content("Post text content"), new Title("Post title"), Guid.NewGuid());

            Comment shouldBeComment = new Comment(Guid.NewGuid(), post.Id, new Content(updateCommentDto.Text));

            _mockPostRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(post);
            _mockCommentsRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(shouldBeComment);
            _mockCommentsRepository.Setup(repo =>
                    repo.UpdateAsync(It.IsAny<Comment>()))
                .Returns(Task.CompletedTask);

            ICommentService commentService = new CommentService(_mockCommentsRepository.Object, _mockPostRepository.Object);

            var updatedComment = await commentService.UpdateCommentAsync(updateCommentDto, shouldBeComment.Id);

            Assert.Equal(updatedComment.Content.Text, shouldBeComment.Content.Text);
            Assert.Equal(updatedComment.Id, shouldBeComment.Id);


        }
        #endregion


        #region Delete post
        [Fact]
        public async void Delete_Comment_By_Id_Throws_Given_Comment_Id_isEmpty()
        {
            ICommentService commentService = new CommentService(_mockCommentsRepository.Object, _mockPostRepository.Object);

            UpdateCommentDto updateCommentDto = new UpdateCommentDto();
            await Assert.ThrowsAsync<ArgumentException>(async () => await commentService.DeleteCommentAsync(Guid.Empty));
        }

        [Fact]
        public async void Update_Comment_By_Id_Throws_Comment_With_Given_Id_Not_Exists()
        {
            _mockCommentsRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(() => null);
            ICommentService commentService = new CommentService(_mockCommentsRepository.Object, _mockPostRepository.Object);

            await Assert.ThrowsAsync<CommentNotExistsException>(async () =>
                await commentService.DeleteCommentAsync(Guid.NewGuid()));
        } 
        #endregion
    }
}
