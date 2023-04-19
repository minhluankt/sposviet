using Application.Enums;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class ManagerInvNoRepository : IManagerInvNoRepository
    {
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly IRepositoryAsync<ManagerInvNo> _managerInvNorepository;
        public ManagerInvNoRepository(IRepositoryAsync<ManagerInvNo> managerInvNorepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _managerInvNorepository = managerInvNorepository;
        }

        public async Task<int> UpdateInvNo(int ComId, ENumTypeManagerInv type = ENumTypeManagerInv.Invoice, bool commit = true)
        {
            var get = await _managerInvNorepository.GetAllQueryable().SingleOrDefaultAsync(x => x.ComId == ComId && x.Type == type);
            if (get == null)
            {
                var model = new ManagerInvNo { ComId = ComId, InvNo = 1, Type = type };
                await _managerInvNorepository.AddAsync(model);
                if (commit)
                {
                    await _unitOfWork.SaveChangesAsync();
                }
                return model.InvNo;
            }
            get.InvNo = get.InvNo + 1;
            get.VFkey = $"{ComId}PK{get.InvNo}T{(int)type}";
            await _managerInvNorepository.UpdateAsync(get);
            if (commit)
            {
                await _unitOfWork.SaveChangesAsync();
            }
            return get.InvNo;
        }
        public async Task<int> GetInvNoAsync(int ComId, ENumTypeManagerInv type = ENumTypeManagerInv.Invoice)
        {
            var get = await _managerInvNorepository.GetAllQueryable().SingleOrDefaultAsync(x => x.ComId == ComId && x.Type == type);
            if (get == null)
            {
                return 0;
            }
            return (int)get.InvNo;
        }

      
    }
}
