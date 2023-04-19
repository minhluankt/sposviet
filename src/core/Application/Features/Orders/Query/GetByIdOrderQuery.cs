using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Orders.Query
{
    public class GetByIdOrderQuery : IRequest<Result<Order>>
    {

        public int Id { get; set; }
        public bool IncludeCustomer { get; set; }
        public bool IncludePayment { get; set; }

        public class GetOrderByIdQueryHandler : IRequestHandler<GetByIdOrderQuery, Result<Order>>
        {
            private readonly IRepositoryAsync<Order> _repository;
            public GetOrderByIdQueryHandler(IRepositoryAsync<Order> repository)
            {
                _repository = repository;
            }
            public async Task<Result<Order>> Handle(GetByIdOrderQuery query, CancellationToken cancellationToken)
            {
                var Order = _repository.Entities;
                Order = Order.Where(x => x.Id == query.Id).Include(m => m.OrderDetailts);

                if (query.IncludeCustomer)
                {
                    Order = Order.Include(x => x.Customer);
                }
                if (query.IncludePayment)
                {
                    Order = Order.Include(x => x.PaymentMethod);
                }
                if (Order.Count() == 0)
                {
                    return await Result<Order>.FailAsync(HeperConstantss.ERR012);
                }
                var data = await Order.SingleOrDefaultAsync();
                return await Result<Order>.SuccessAsync(data);
            }
        }
    }
}
