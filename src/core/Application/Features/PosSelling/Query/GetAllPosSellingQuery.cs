using Application.Enums;
using Application.Interfaces.Repositories;
using Application.Providers;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Model;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.PosSellings.Query
{
    public class GetAllPosSellingQuery : DatatableModel, IRequest<Result<PosModel>>
    {
        public int ComId { get; set; }
        public EnumTypeProduct TypeProduct { get; set; } = EnumTypeProduct.AMTHUC;
        public GetAllPosSellingQuery(int _comId)
        {
            ComId = _comId;
        }
    }

    public class GetAllPosSellingdQueryHandler : IRequestHandler<GetAllPosSellingQuery, Result<PosModel>>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly IOrderTableRepository _ordertable;
        private readonly IRoomAndTableRepository<RoomAndTable> _roomAndTable;
        private readonly IProductPepository<Product> _repository;
        private readonly IMapper _mapper;

        public GetAllPosSellingdQueryHandler(IRoomAndTableRepository<RoomAndTable> roomAndTable,
            IOrderTableRepository ordertable,
            IOptions<CryptoEngine.Secrets> config,
            IProductPepository<Product> repository,
            IMapper mapper)
        {
            _ordertable = ordertable;
            _config = config;
            _roomAndTable = roomAndTable;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<PosModel>> Handle(GetAllPosSellingQuery request, CancellationToken cancellationToken)
        {
            PosModel model = new PosModel();

            ProductSearch productSearch = new ProductSearch()
            {
                ComId = request.ComId,
                TypeProduct = request.TypeProduct
            };

            if (request.TypeProduct == EnumTypeProduct.AMTHUC)
            {
                model.RoomAndTables = _roomAndTable.GetAll(request.ComId).Where(x => x.Active).ToList();
                model.RoomAndTables.ForEach(x =>
                {
                    var values = "id=" + x.Id;
                    var secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                    x.IdString = secret;
                });
                model.Areas = model.RoomAndTables.GroupBy(x=>x.IdArea).Select(x => new Area()
                {
                    Name = x.First().Area?.Name,
                    IdGuid = x.First().Area.IdGuid,
                    Id = x.First().Area.Id,
                }).ToList();


            }
            else if (request.TypeProduct == EnumTypeProduct.BAN_LE || request.TypeProduct == EnumTypeProduct.TAPHOA_SIEUTHI)
            {
                model.OrderTables = _ordertable.GetOrderInvoiceRetail(request.Comid, EnumStatusOrderTable.DANG_DAT, request.TypeProduct).AsNoTracking().ToList();
            }
            return await Result<PosModel>.SuccessAsync(model);
        }
    }
}
