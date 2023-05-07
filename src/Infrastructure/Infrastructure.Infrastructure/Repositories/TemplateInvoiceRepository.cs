using Application.Enums;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{

    public class TemplateInvoiceRepository : ITemplateInvoiceRepository<TemplateInvoice>
    {
        private readonly IRepositoryAsync<TemplateInvoice> _repository;
        private readonly IUnitOfWork _unitOfWorkrepository;
        public IQueryable<TemplateInvoice> Entities => _repository.Entities;
        public TemplateInvoiceRepository(IRepositoryAsync<TemplateInvoice> repository, IUnitOfWork unitOfWorkrepository)
        {
            _unitOfWorkrepository = unitOfWorkrepository;
            _repository = repository;
        }
        public async Task<TemplateInvoice> GetTemPlate(int ComId, EnumTypeTemplatePrint enumTypeTemplatePrint = EnumTypeTemplatePrint.IN_BILL)
        {
            return await _repository.Entities.AsNoTracking().Where(x => x.Active && x.ComId == ComId &&x.TypeTemplatePrint== enumTypeTemplatePrint).SingleOrDefaultAsync();
        }
        public IQueryable<TemplateInvoice> GetAllAsync(int ComId)
        {
            return _repository.GetAllQueryable().Where(x => x.ComId == ComId);
        }
        public async Task Delete(int ComId, int id)
        {
            var dele = await _repository.Entities.Where(x => x.Id == id && x.ComId == ComId).SingleOrDefaultAsync();
            await _repository.DeleteAsync(dele);
            await _unitOfWorkrepository.SaveChangesAsync();
        }
        public async Task<TemplateInvoice> GetByIdAsync(int ComId, int id)
        {
            return await _repository.Entities.Where(x => x.Id == id && x.ComId == ComId).SingleOrDefaultAsync();
        }
        public async Task AddAsync(TemplateInvoice model)
        {
            if (model.Active)
            {
                var getu = await _repository.Entities.Where(x => x.Active && x.ComId == model.ComId && x.TypeTemplatePrint ==model.TypeTemplatePrint).ToListAsync();
                getu.ForEach(x => x.Active = false);
                await _repository.UpdateAsync(model);

            }
            await _repository.AddAsync(model);
            await _unitOfWorkrepository.SaveChangesAsync();
        }
        public async Task UpdateAsync(TemplateInvoice model)
        {
            await _unitOfWorkrepository.CreateTransactionAsync();
            try
            {
                var find = await _repository.Entities.Where(x => x.Id == model.Id && x.ComId == model.ComId).SingleOrDefaultAsync();
                if (model.Active)
                {
                    var getu = await _repository.Entities.Where(x => x.Active && x.ComId == model.ComId && x.Id!=find.Id && x.TypeTemplatePrint == model.TypeTemplatePrint).ToListAsync();
                    if (getu.Count()>0)
                    {
                        getu.ForEach(x => x.Active = false);
                        await _repository.UpdateAsync(model);
                    }
                }
                find.Name = model.Name;
                find.Template = model.Template;
                find.TypeTemplatePrint = model.TypeTemplatePrint;
                find.Active = model.Active;
                await _repository.UpdateAsync(model);
                await _unitOfWorkrepository.SaveChangesAsync();
                await _unitOfWorkrepository.CommitAsync();
            }
            catch (System.Exception e)
            {
                await _unitOfWorkrepository.RollbackAsync();
                throw new System.Exception(e.Message);
            }

        }
    }
}
