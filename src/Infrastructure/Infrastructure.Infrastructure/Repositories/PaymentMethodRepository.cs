using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private IUnitOfWork UnitOfWork;
        private readonly IRepositoryAsync<PaymentMethod> _paymentMethodRepository;
        public PaymentMethodRepository(IRepositoryAsync<PaymentMethod> paymentMethodRepository, IUnitOfWork unitOfWork)
        {
            _paymentMethodRepository = paymentMethodRepository;
            UnitOfWork = unitOfWork;
        }

        public async Task AddAsync(PaymentMethod model)
        {
            await _paymentMethodRepository.AddAsync(model);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int ComId, int Id)
        {
            var get = _paymentMethodRepository.Entities.Where(x => x.ComId == ComId && x.Id == Id).SingleOrDefault();
            if (get == null)
            {
                return false;
            }
            await _paymentMethodRepository.DeleteAsync(get);
            await UnitOfWork.SaveChangesAsync();
            return true;
        }

        public IQueryable<PaymentMethod> GetAll(int comid,bool? Active = null)
        {
            var list = _paymentMethodRepository.Entities;
            if (Active !=null)
            {
                list = list.Where(x => x.Active == Active);
            }  
            if (comid > 0)
            {
                list = list.Where(x => x.ComId == comid);
            }
            return  list;
        }

        public async Task<bool> UpdateAsync(PaymentMethod model)
        {
            var get = _paymentMethodRepository.Entities.Where(x => x.ComId == model.ComId && x.Id == model.Id).SingleOrDefault();
            if (get == null)
            {
                return false;
            }
            get.Name = model.Name;
            get.Code = model.Code;
            get.Content = model.Content;
            await _paymentMethodRepository.UpdateAsync(get);
            await UnitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
