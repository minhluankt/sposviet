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

namespace Application.Features.NotificationNewsEmails.Query
{

    public class GetByIdNotificationNewsEmailQuery : IRequest<Result<NotificationNewsEmail>>
    {
        public int Id { get; set; }

        public class GetNotificationNewsEmailByIdQueryHandler : IRequestHandler<GetByIdNotificationNewsEmailQuery, Result<NotificationNewsEmail>>
        {
            private readonly IRepositoryAsync<NotificationNewsEmail> _repository;

            public GetNotificationNewsEmailByIdQueryHandler(IRepositoryAsync<NotificationNewsEmail> repository)
            {
                _repository = repository;
            }
            public async Task<Result<NotificationNewsEmail>> Handle(GetByIdNotificationNewsEmailQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(query.Id);
                return Result<NotificationNewsEmail>.Success(product);
            }
        }
    }
}
