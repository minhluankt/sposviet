using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;

using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.VietQRs.Commands
{

    public class DeleteVietQRCommand : IRequest<Result>
    {
        public DeleteVietQRCommand(int _comId, int _id)
        {
            ComId = _comId;
            Id = _id;
        }
        public int ComId { get; set; }
        public int Id { get; set; }
        public class DeleteVietQRHandler : IRequestHandler<DeleteVietQRCommand, Result>
        {
            private readonly IVietQRRepository<VietQR> _Repository;
            public DeleteVietQRHandler(IVietQRRepository<VietQR> brandRepository)
            {
                _Repository = brandRepository;
            }
            public async Task<Result> Handle(DeleteVietQRCommand command, CancellationToken cancellationToken)
            {
                if (command.ComId == 0 || command.Id==0)
                {
                    return await Result<int>.FailAsync(HeperConstantss.ERR012);
                }
                return await _Repository.DeleteAsync(command.ComId,command.Id);
            }
        }
    }
}
