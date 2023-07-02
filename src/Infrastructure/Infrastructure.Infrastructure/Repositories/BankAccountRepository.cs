using Application.Hepers;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class BankAccountRepository: IBankAccountRepository
    {
        private readonly IRepositoryAsync<BankAccount> _repository;
        private readonly IMapper _mapper;

        public BankAccountRepository(
            IRepositoryAsync<BankAccount> repository,


            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PaginatedList<BankAccount>> GetAllAsync(EntitySearchModel model)
        {
            var iquery = _repository.Entities.Where(x => x.ComId == model.Comid).AsNoTracking();
            if (!string.IsNullOrEmpty(model.Name))
            {
                iquery = iquery.Where(x => x.BankNumber.ToLower().Contains(model.Name.ToLower()) || x.AccountName.ToLower().Contains(model.Name.ToLower()));
            }

            if (string.IsNullOrEmpty(model.sortOn))
            {
                model.sortDirection = "DESC";
                model.sortOn = "Id";
            }
            else
            {
                model.sortDirection = model.sortDirection.ToString();
                model.sortOn = model.sortOn.ToString();
            }

            return await PaginatedList<BankAccount>.ToPagedListAsync(iquery, model.PageNumber, model.PageSize, model.sortOn, model.sortDirection);
        }
    }
}
