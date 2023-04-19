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

namespace Application.Features.Mail.Querys
{
    public class GetByIdMailSettingQuery : IRequest<Result<MailSettings>>
    {
        public int Id { get; set; }

        public class GetMailSettingByIdQueryHandler : IRequestHandler<GetByIdMailSettingQuery, Result<MailSettings>>
        {
            private readonly IRepositoryAsync<MailSettings> _repository;

            public GetMailSettingByIdQueryHandler(IRepositoryAsync<MailSettings> repository)
            {
                _repository = repository;
            }
            public async Task<Result<MailSettings>> Handle(GetByIdMailSettingQuery query, CancellationToken cancellationToken)
            {
                MailSettings product = new MailSettings();
                if (query.Id == 0)
                {
                    product = await _repository.GetFirstAsync();
                }
                else
                {
                    product = await _repository.GetByIdAsync(query.Id);
                }
                return Result<MailSettings>.Success(product);
            }
        }
    }
}
