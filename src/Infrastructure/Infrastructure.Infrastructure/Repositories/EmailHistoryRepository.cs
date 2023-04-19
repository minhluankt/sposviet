using Application.Interfaces.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class EmailHistoryRepository : IEmailHistoryRepository<Mailhistory> 
    {
        private readonly IRepositoryAsync<Mailhistory> _repositoryAsync;
        private readonly IUnitOfWork _UnitOfWork;
        public EmailHistoryRepository(IRepositoryAsync<Mailhistory> repositoryAsync, IUnitOfWork UnitOfWork)
        {
            _repositoryAsync = repositoryAsync;
            _UnitOfWork = UnitOfWork;
        }
        public async Task<Mailhistory> AddAsync(Mailhistory model)
        {
            await  _repositoryAsync.AddAsync(model);
            await _UnitOfWork.SaveChangesAsync(new CancellationToken());
            return model;
        }   
        public Mailhistory Add(Mailhistory model)
        {
              _repositoryAsync.Add(model);
             _UnitOfWork.SaveChanges();
            return model;
        }   

        public async Task<bool> UpdateStatusAsync(int id, bool sendAgain = false)
        {
            var model =  await _repositoryAsync.GetByIdAsync(id);
            model.Status = true;
            if (sendAgain)
            {
                model.CountSendAgain = model.CountSendAgain++;
            }
            await _repositoryAsync.UpdateAsync(model);
            await _UnitOfWork.SaveChangesAsync(new CancellationToken());
            return true;
        } 
        public bool UpdateStatus(int id, bool sendAgain = false)
        {
            var model =   _repositoryAsync.GetById(id);
            model.Status = true;
            if (sendAgain)
            {
                model.CountSendAgain = model.CountSendAgain++;
            }
            _repositoryAsync.Update(model);
             _UnitOfWork.SaveChanges();
            return true;
        }

        public async Task<Mailhistory> GetByIdAsync(int id)
        {
            return await _repositoryAsync.GetByIdAsync(id);
        }
    }
}
