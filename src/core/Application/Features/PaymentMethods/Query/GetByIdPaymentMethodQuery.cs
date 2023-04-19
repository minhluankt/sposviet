using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.PaymentMethods.Query
{

    public class GetByIdPaymentMethodQuery : IRequest<Result<PaymentMethod>>
    {
        public int Id { get; set; }

        public class GetPaymentMethodByIdQueryHandler : IRequestHandler<GetByIdPaymentMethodQuery, Result<PaymentMethod>>
        {
            private readonly IRepositoryAsync<PaymentMethod> _repository;

            public GetPaymentMethodByIdQueryHandler(IRepositoryAsync<PaymentMethod> repository)
            {
                _repository = repository;
            }
            public async Task<Result<PaymentMethod>> Handle(GetByIdPaymentMethodQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(query.Id);
                return Result<PaymentMethod>.Success(product);
            }
        }
    }
}
