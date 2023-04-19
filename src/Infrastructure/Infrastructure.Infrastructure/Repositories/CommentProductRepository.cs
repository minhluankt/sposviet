using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class CommentProductRepository : ICommentProductRepository<Comment>
    {
        private readonly ILogger<CommentProductRepository> _log;
        private readonly IRepositoryAsync<Comment> _repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private IUnitOfWork _unitOfWork { get; set; }
        public CommentProductRepository(IRepositoryAsync<Comment> repository,
              ILogger<CommentProductRepository> log,
              IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
            _log = log;
        }

        public async Task<bool> AddCommentAsync(Comment Entity)
        {
            await _repository.AddAsync(Entity);
            await _unitOfWork.SaveChangesAsync();
            return true;

        }
    }
}
