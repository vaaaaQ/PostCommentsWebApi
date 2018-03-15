using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PostComments.BLL.Entities;
using PostComments.BLL.Entities.Comment;
using PostComments.BLL.Entities.Post;
using PostComments.BLL.Extensions;
using PostComments.BLL.Interfaces;

namespace PostComments.DAL
{
    public class InMemoryRepository<T> : IAsyncRepository<T> where T : BaseEntity
    {
        protected readonly List<T> _items = new List<T>();

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await Task.FromResult(_items.FirstOrDefault(item => item.Id == id));
        }

        public async Task<List<T>> ListAllAsync()
        {
            return await Task.FromResult(_items);
        }

        public async Task<List<T>> ListAsync(ISpecification<T> spec)
        {
            return await Task.FromResult(_items.Where(spec.Criteria.Compile()).ToList());
        }

        public async Task AddAsync(T entity)
        {
            _items.Add(entity);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(T entity)
        {
            var index = _items.FindIndex(item => item.Id == entity.Id);
            _items[index] = entity;
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(T entity)
        {
            var index = _items.Single(item => item.Id == entity.Id);
            _items.Remove(index);
            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// Test repository for comments with initial values
    /// </summary>
    public class CommentsInMemoryRepositor : InMemoryRepository<Comment>
    {
        public CommentsInMemoryRepositor()
        {
            SeedComments();
        }

        //init test comments
        private void SeedComments()
        {
            Guid fromId = Guid.Empty.Increment();
            var post1Id = Guid.Empty.Increment();
            var comment11 = new Comment(Guid.NewGuid(), post1Id, new Content("Wrong. Expensive blockbuster movies are made for foreign audiences where there\'s far more money to be made than in the US. It\'s not for \"fans\". It\'s for the broadest possible appeal. It\'s all about the money."));
            var comment12 = new Comment(Guid.NewGuid(), post1Id, new Content("Depends on the movie...but roughly 30% of all movie profits come from the US...spurn them, and global drops. It happens everytime"));
            var comment13 = new Comment(Guid.NewGuid(), post1Id, new Content("Keep in mind though that studios generally keep a far smaller percentage of the box office for foreign theaters than they do for showings in U.S. theaters. Studios keep approximately 60% of tickets sales in U.S. theaters but only 20 to 40% of tickets sales in foreign theaters, depending upon the country."));
            var comment14 = new Comment(Guid.NewGuid(), post1Id, new Content("I\'ve read something similar to that. Apparently, the reason why movies are suffering from increasingly shittier dialogue is because they\'re just going to be subtitled for foreign audiences anyway. So there\'s little incentive for executives to ensure that writers are doing a decent job."));
            var post1Comments = new List<Comment>()
            {
                comment11,
                comment12,
                comment13,
                comment14
            };

            var post2Id = Guid.Empty.Increment().Increment();
            var comment21 = new Comment(Guid.NewGuid(), post1Id, new Content("Most cross shopped phone against an iPhoneX is a S8 or S9, Pixel 2 is much lower on the list of potential alternatives despite of Google lineage. this is a thinly veiled advertisement for Pixel."));
            var comment22 = new Comment(Guid.NewGuid(), post1Id, new Content("How can I change since my digital life is surrounded by Apple now and I like it."));
            var comment23 = new Comment(Guid.NewGuid(), post1Id, new Content("Android P is coming out and it is only for the Google Pixel 2 and the Google Pixel 2 XL, obviously... But it\'s another reason to buy those phones, right&"));
            var comment24 = new Comment(Guid.NewGuid(), post1Id, new Content("1. Once you get involved in the Apple operating system (All data shared between iMac, iPad, iPhone, and Apple Watch), a system that works very well, it’s hard to consider a system that resides outside the Apple realm.\r\n\r\n2. Apple products always have great resale value (likely a key reason why we upgrade so frequently). Although - research does show that Samsung is starting to catch up in this regard .. but .. other Android devices are pretty much being left in the dust resale value wise."));
            var post2Comments = new List<Comment>()
            {
                comment21,
                comment22,
                comment23,
                comment24
            };

            var post3Id = Guid.Empty.Increment().Increment().Increment();
            var comment31 = new Comment(Guid.NewGuid(), post1Id, new Content("Whatever it is, it most certainly is not a 2D material. Because there is no such thing as a 2D material. At least not one that is perceivable to humans. No matter how incredibly thin it might be, it still exists in 3 dimensions."));
            var comment32 = new Comment(Guid.NewGuid(), post1Id, new Content("Sounds awesome. Hope it goes consumer level soon. The energy applications are probably the most important, but the camera tech sounds equally great. Could be a big deal for AV's, night vision, etc."));
            var comment33 = new Comment(Guid.NewGuid(), post1Id, new Content("I doubt the 7-sec. phone battery recharge will come to fruition."));
            var comment34 = new Comment(Guid.NewGuid(), post1Id, new Content("I\'m so overwhelmed, NOT. Another vaporware product. \r\nRead about this years ago - talk to me when I can hold it in my hand. \r\nAnyone remember an article about gel batteries? They actually talked about incorporating them into the framework of toys etc. Anyone ever seen an incorporated battery?"));
            var post3Comments = new List<Comment>()
            {
                comment31,
                comment32,
                comment33,
                comment34
            };

            _items.AddRange(post1Comments);
            _items.AddRange(post2Comments);
            _items.AddRange(post3Comments);
        }
    }

    /// <summary>
    /// Test repository for post with initial values
    /// </summary>
    public class PostInMemoryRepository : InMemoryRepository<Post>
    {
        public PostInMemoryRepository()
        {
            SeedPosts();    
        }

        //init test posts
        private void SeedPosts()
        {
            Guid id = Guid.Empty;
            Guid fromId = Guid.Empty.Increment();
            var post1 = new Post(
                new Content(
                    "\r\nTomb Raider fans are having none of it. \r\n\r\nA YouTube personality with a sizable audience suggested over the weekend that Alicia Vikander will not be considered a good Lara Croft when her film opens Friday because her lean, athletic body does not match the sexist, over-the-top cartoon version of the initial video game character, introduced in the 1990s.\r\n\r\n\"Do I have to be the asshole who says her tits are too small for me to see her as Lara Croft? Do I have to be that guy? Do I have to be the one who fucking says it? I guess I do. Sorry,\" tweeted ​TJ Kirk aka The Amazing Atheist. He has more than a million YouTube suscribers. \r\n\r\nBacklash to his comment was instant."),
                new Title("\'Tomb Raider\': Fans Slam Criticism of Alicia Vikander\'s Body"), fromId);
            post1.Id = id.Increment();

            var post2 = new Post(
                new Content(
                    "Sometimes, you just need a friend to convince you that you\'re right. Over the past weeks, I\'ve had a few friends who are \"lifelong\" iPhone users message me and ask about switching away from Apple. They want to break their biannual iPhone upgrade cycle and eschew the new iPhone X for the Google Pixel 2. One of my friends wrote me this:\r\n\r\n\"I\'m really over Apple iPhones. I\'m kind of into the Pixel 2, but that\'s only on Verizon, right? I don\'t like their unlimited plans. I like the iPhone X, but don\'t want it.\""),
                new Title("The American Idol Contestant Katy Perry Kissed Shared His Side Of The Story On Instagram"), fromId);
            post2.Id = post1.Id.Increment();


            var post3 = new Post(
                new Content(
                    "Graphene is 200 times stronger than steel and lighter than paper and often referred to as a wonder material. But what can this substance actually do?\r\n\r\nFortunately, graphene had a chance to shine at last month\'s Mobile World Congress trade show in Barcelona. Buried away at the furthest point from the entrance to the convention center was the Graphene Pavilion, where around 25 different graphene-based research projects, including robotics and wearables, were being shown off."),

                new Title("Want to charge your smartphone in 7 seconds? Look to graphene"), fromId);
            post3.Id = post2.Id.Increment();

            var testposts = new List<Post>()
            {
                post1,
                post2,
                post3
            };

            this._items.AddRange(testposts);
        }
    }
}