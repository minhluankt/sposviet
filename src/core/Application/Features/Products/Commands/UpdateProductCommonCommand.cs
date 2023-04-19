using Application.Constants;
using Application.Enums;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Abstractions.Repository;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Products.Commands
{
    public class UpdateProductCommonCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public int[] lstid { get; set; }
        public int ComId { get; set; }
        public ENumTypeUpdateProduct TypeUpdateProduct { get; set; }
        public class UpdateProductCommonHandler : IRequestHandler<UpdateProductCommonCommand, Result<int>>
        {
            private readonly ITableLinkRepository _tablelink;
            private readonly ILogger<UpdateProductCommonHandler> _log;
            private readonly IProductPepository<Product> _Repository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private readonly IFormFileHelperRepository _fileHelper;
            private IUnitOfWork _unitOfWork { get; set; }

            public UpdateProductCommonHandler(IProductPepository<Product> Repository,
                 IFormFileHelperRepository fileHelper, ITableLinkRepository tablelink,
                ILogger<UpdateProductCommonHandler> log, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _log = log;
                _Repository = Repository;
                _fileHelper = fileHelper; _tablelink = tablelink;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(UpdateProductCommonCommand command, CancellationToken cancellationToken)
            {
                switch (command.TypeUpdateProduct)
                {
                    case ENumTypeUpdateProduct.STOPBUSINESS:
                        return await _Repository.UpdateBusiness(command.lstid, command.ComId,true);
                     
                    case ENumTypeUpdateProduct.UNSTOPBUSINESS:
                        return await _Repository.UpdateBusiness(command.lstid, command.ComId,false);
                    default:
                        return Result<int>.Fail(HeperConstantss.ERR012);
                      
                }
             
            }
        }
    }
}
