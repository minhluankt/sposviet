using Application.Hepers;
using Application.Interfaces.Repositories;
using Application.Interfaces.Shared;
using Application.Providers;
using Domain.Entities;
using Domain.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class NotifiUserRepository : INotifyUserRepository<NotifiUser>
    {
        private readonly IParametersEmailRepository _parametersEmailRepository;
        private readonly IServiceProvider _serviceProvider;
        private IOptions<CryptoEngine.Secrets> _config;
        private IMailService _MailService;
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly ILogger<NotifiUserRepository> _log;
        private readonly IRepositoryAsync<NotifiUser> _repository;
        public NotifiUserRepository(IRepositoryAsync<NotifiUser> repository,
               IOptions<CryptoEngine.Secrets> config,
                 IParametersEmailRepository parametersEmailRepository,
            IServiceProvider serviceProvider, ILogger<NotifiUserRepository> log,
             IUnitOfWork unitOfWork, IMailService MailService
            )
        {
            _unitOfWork = unitOfWork;
            _parametersEmailRepository = parametersEmailRepository;
            _config = config;
            this._serviceProvider = serviceProvider;
            _MailService = MailService;
            _repository = repository;

        }
        public IQueryable<NotifiUser> GetAll(NotifyUserModel model)
        {
            IQueryable<NotifiUser> qr = _repository.Entities;
            if (model.IdUser > 0)
            {
                qr = qr.Where(x => x.IdUser == model.IdUser);
            }
            if (model.IsReview)
            {
                qr = qr.Where(x => x.IsReview);
            }
            if (model.Type >= 0)
            {
                qr = qr.Where(x => x.Type == model.Type);
            }
            return qr;
        }

        public async Task<PaginatedList<NotifiUser>> GetAllPaginatedListAsync(NotifyUserModel model)
        {
            IQueryable<NotifiUser> qr = _repository.Entities;
            if (model.IdUser > 0)
            {
                qr = qr.Where(x => x.IdUser == model.IdUser);
            }
            if (model.IsReview)
            {
                qr = qr.Where(x => x.IsReview);
            }
            if (model.Type >= 0)
            {
                qr = qr.Where(x => x.Type == model.Type);
            }
            if (string.IsNullOrEmpty(model.sortOn))
            {
                qr = qr.OrderByDescending(x => x.Id);
            }
            return await PaginatedList<NotifiUser>.ToPagedListAsync(qr, model.PageNumber, model.PageSize, model.sortOn, model.sortDirection);
        }

        public async Task<NotifiUser> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public async Task AddAsync(NotifiUser entity)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<NotifiUserRepository>>();
                try
                {

                    var _repoNotifiUser = scope.ServiceProvider.GetRequiredService<IRepositoryAsync<NotifiUser>>();
                    var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();


                    await _repoNotifiUser.AddAsync(entity);
                    await _unitOfWork.SaveChangesAsync();
                    logger.LogInformation("Lưu notify thành công: " + entity.Id + "__" + entity.Title);

                }
                catch (Exception e)
                {
                    logger.LogError(e, e.Message);

                }

            }
        }
        public async Task<bool> UpdateReviewAsync(int id)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<NotifiUserRepository>>();
                try
                {
                    var _repoNotifiUser = scope.ServiceProvider.GetRequiredService<IRepositoryAsync<NotifiUser>>();
                    var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var getid = await _repoNotifiUser.GetByIdAsync(id);
                    if (getid == null)
                    {
                        return false;
                    }
                    getid.IsReview = true;
                    await _repoNotifiUser.UpdateAsync(getid);
                    await _unitOfWork.SaveChangesAsync(new System.Threading.CancellationToken());
                    logger.LogInformation("UpdateReviewAsync thành công: " + getid.Id + "__" + getid.Title);
                    return true;

                }
                catch (Exception e)
                {
                    logger.LogError(e, e.Message);
                    return true;
                }
            }
        }

        public async Task SendNotifyAsync(NotifiUser entity)
        {
            await Task.Run(async () =>
           {
               await this.AddAsync(entity);
           });
        }

        public async Task<bool> DeleteByIdAsync(int id, int iduser)
        {
            var get = _repository.Entities.Where(x => x.Id == id && x.IdUser == iduser).SingleOrDefault();
            if (get != null)
            {
                await _repository.DeleteAsync(get);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public int CountNotifyNoReviewAsync(int iduser)
        {
            return _repository.Entities.Where(x => x.IdUser == iduser && !x.IsReview).Count();
        }
    }
}
