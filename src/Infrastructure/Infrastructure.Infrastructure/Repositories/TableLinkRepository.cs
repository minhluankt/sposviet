using Application.Interfaces.Repositories;
using Dapper;
using Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{


    public class TableLinkRepository : ITableLinkRepository
    {
        private readonly IDapperRepository _dapperdb;
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly IRepositoryAsync<TableLink> _repository;
        public TableLinkRepository(IUnitOfWork unitOfWork, IDapperRepository dapperdb, IRepositoryAsync<TableLink> repository)
        {
            _dapperdb = dapperdb;
            _unitOfWork = unitOfWork;
            _repository = repository;
        }
        public async Task AddAsync(string slug, int tableId, string type, int parentid,int comid, bool commit = true)
        {
            var model = new TableLink
            {
                slug = slug,
                Comid = comid,
                tableId = tableId,
                type = type,
                parentId = parentid
            };
            await _repository.AddAsync(model);
            if (commit)
            {
                await _unitOfWork.SaveChangesAsync(new CancellationToken());
            }
        }
        public async Task UpdateAsync(string slug, int tableId, int parentid, int comid, string type = "")
        {
            var thisLink = _repository.Entities.Where(m => m.tableId == tableId && m.parentId == parentid).SingleOrDefault();
            if (thisLink != null)
            {
                thisLink.slug = slug;
                await _repository.UpdateAsync(thisLink);
            }
            else
            {
                await this.AddAsync(slug, tableId, type, parentid, comid, false);
            }
        }

        public async Task DeleteAsync(int tableId, int parentid)
        {
            var thisLink = _repository.Entities.Where(m => m.tableId == tableId && m.parentId == parentid).SingleOrDefault();
            if (thisLink != null)
            {
                await _repository.DeleteAsync(thisLink);
            }
        }

        public async Task<TableLink> GetBySlug(string slug)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("slug", slug);
            string sql = "select * from TableLink where slug =@slug";
            var query = await _dapperdb.GetAsync<TableLink>(sql, param);
            return query;
        }
    }
}
