using Application.Hepers;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Infrastructure.Repositories
{
    public class AreaRepository : IAreaRepository
    {
        private readonly IRepositoryAsync<Area> _repository;
        private readonly IMapper _mapper;

        public AreaRepository(
            IRepositoryAsync<Area> repository,


            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PaginatedList<AreasModel>> GetAllAsync(EntitySearchModel model)
        {
            var iquery = _repository.Entities.Where(x => x.ComId == model.Comid).AsNoTracking();
            if (!string.IsNullOrEmpty(model.Name))
            {
                iquery = iquery.Where(x => x.Name.ToLower().Contains(model.Name.ToLower()));
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

            var data = iquery.Select(x => new AreasModel()
            {
                Id = x.Id,
                Name = x.Name,
                NumberTable = x.RoomAndTables.Count(),
                TableAndRooms = x.RoomAndTables.Select(x => new TableAndRoomModel()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList()

            });
            return await PaginatedList<AreasModel>.ToPagedListAsync(data, model.PageNumber, model.PageSize, model.sortOn, model.sortDirection);
        }
    }
}
