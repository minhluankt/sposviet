using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Features.Customers.Query
{

    public class GetByIdCustomerQuery : IRequest<Result<Customer>>
    {
        public int Id { get; set; }
        public int? ComId { get; set; }
        public string IdCode { get; set; }

        public class GetCustomerByIdQueryHandler : IRequestHandler<GetByIdCustomerQuery, Result<Customer>>
        {
            private readonly IRepositoryAsync<Customer> _repository;

            public GetCustomerByIdQueryHandler(IRepositoryAsync<Customer> repository)
            {
                _repository = repository;
            }
            public async Task<Result<Customer>> Handle(GetByIdCustomerQuery query, CancellationToken cancellationToken)
            {
                if (!string.IsNullOrEmpty(query.IdCode))
                {
                    var data = _repository.Entities.Where(m => m.IdCodeGuid.ToString() == query.IdCode);
                    if (query.ComId != null)
                    {
                        data = data.Where(x => x.Comid == query.ComId);
                    }
                    return Result<Customer>.Success(await data.Include(m => m.City).Include(x => x.District).Include(x => x.Ward).SingleOrDefaultAsync());
                }
                var product = _repository.Entities.Where(x => x.Id == query.Id);
                if (query.ComId != null)
                {
                    product = product.Where(x => x.Comid == query.ComId);
                }
                return Result<Customer>.Success(await product.Include(m => m.City).Include(x => x.District).Include(x => x.Ward).SingleOrDefaultAsync());
            }
        }
    }
}
