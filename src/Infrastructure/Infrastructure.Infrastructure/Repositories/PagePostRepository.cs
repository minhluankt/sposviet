using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Infrastructure.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class PagePostRepository : IPagePostRepository<PagePost>
    {
        private readonly ILogger<PostRepository> _log;
        private readonly IRepositoryAsync<Post> _postRepository;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public PagePostRepository(IServiceScopeFactory serviceScopeFactory, ILogger<PostRepository> log, IRepositoryAsync<Post> postRepository)
        {
            _postRepository = postRepository;
            _log = log;
            _serviceScopeFactory = serviceScopeFactory;
        }
        public void UpdateReView(int id)
        {
            _log.LogInformation("Update lượt virew bài viết");
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    var get = db.Post.Find(id);
                    if (get != null)
                    {
                        get.ViewNumber = get.ViewNumber + 1;
                        db.Post.Update(get);
                        db.SaveChanges();
                    }
                    db.Dispose();
                }
            }
            catch (Exception e)
            {
                _log.LogError(e, e.Message);
            }
        }
    }
}
