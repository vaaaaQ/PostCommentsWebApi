using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PostComments.BLL.Entities.Comment;
using PostComments.BLL.Entities.Post;
using PostComments.BLL.Interfaces;
using PostComments.BLL.Services;
using PostComments.DAL;
using PostComments.Service.Filters;
using Swashbuckle.AspNetCore.Swagger;

namespace PostComments.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAsyncRepository<Post>, PostInMemoryRepository>();
            services.AddSingleton<IAsyncRepository<Comment>, CommentsInMemoryRepositor>();
            services.AddTransient<IPostService, PostService>();
            services.AddTransient<ICommentService, CommentService>();

            services.AddMvc(options => { 
                options.Filters.Add(typeof(ExceptionResponseFilter));
                options.Filters.Add<ModelValidationFilterAttribute>();
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Post and Comments API", Version = "v1",
                    Contact = new Contact {  Name = "Valery Hilimovich",Email = "Valery_Hilimovich@epam.com"}
                });

                // Set the comments path for the Swagger JSON and UI.
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "PostComments.Service.xml");
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Post and Comments API");
            });

            app.UseMvc();
        }
    }
}
