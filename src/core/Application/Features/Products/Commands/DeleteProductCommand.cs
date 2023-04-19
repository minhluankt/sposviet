using Application.CacheKeys;
using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Abstractions.Repository;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

    public class DeleteProductCommand : IRequest<Result<int>>
    {
        public int[] lstid { get; set; }
        public int ComId { get; set; }
        public int Id { get; set; }
        public bool isDeleteMuti { get; set; }
        public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Result<int>>
        {
            private readonly ITableLinkRepository _tablelink;
            private readonly ILogger<DeleteProductHandler> _log;
            private readonly IRepositoryAsync<Product> _Repository;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;
            private readonly IFormFileHelperRepository _fileHelper;
            private IUnitOfWork _unitOfWork { get; set; }

            public DeleteProductHandler(IRepositoryAsync<Product> Repository,
                 IFormFileHelperRepository fileHelper, ITableLinkRepository tablelink,
                ILogger<DeleteProductHandler> log, IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache distributedCach)
            {
                _log = log;
                _Repository = Repository;
                _fileHelper = fileHelper; _tablelink = tablelink;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _distributedCache = distributedCach;
            }
            public async Task<Result<int>> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
            {
                if (command.isDeleteMuti)
                {
                   
                    var products = await _Repository.Entities.Where(m => command.lstid.Contains(m.Id) && m.ComId== command.ComId).Include(m => m.UploadImgProducts).ToListAsync();
                    if (products.Count()==0)
                    {
                        return Result<int>.Fail(HeperConstantss.ERR012);
                    }
                    foreach (var product in products)
                    {
                        await _tablelink.DeleteAsync(TypeLinkConstants.IdTypeProduct, product.Id);
                        try
                        {
                            _log.LogInformation("DeleteProductCommand delete Image start:" + product.Name);
                            if (!string.IsNullOrEmpty(product.Img))
                            {
                                _fileHelper.DeleteFile(product.Img, FolderUploadConstants.Product);
                            }
                            // deleet list img
                            if (product.UploadImgProducts.Count() > 0)
                            {
                                string[] lst = product.UploadImgProducts.Select(x => x.FileName).ToArray();
                                _fileHelper.DeleteListFile(lst, FolderUploadConstants.Product);
                            }

                            _log.LogInformation("DeleteProductCommand delete Image end:" + product.Name);
                        }
                        catch (Exception e)
                        {
                            _log.LogError("DeleteProductCommand update Image error:" + product.Name + "\n" + e.ToString());
                        }
                    }
                    await _Repository.DeleteRangeAsync(products);
                    await _distributedCache.RemoveAsync(ProductCacheKeys.ListKey);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<int>.Success();
                }
                else
                {
                    var product = await _Repository.GetByIdAsync(m => m.Id == command.Id, m => m.Include(m => m.UploadImgProducts));
                    if (product != null)
                    {
                        //var check = _Repository.GetAll(m=>m.idProduct== command.Id).Count();
                        //if (check > 0)
                        //{
                        //    return await Result<int>.FailAsync(HeperConstantss.DataExit);
                        //}
                        await _Repository.DeleteAsync(product);
                        await _tablelink.DeleteAsync(TypeLinkConstants.IdTypeProduct, product.Id);
                        await _distributedCache.RemoveAsync(ProductCacheKeys.ListKey);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        try
                        {
                            _log.LogInformation("DeleteProductCommand delete Image start:" + product.Name);
                            if (!string.IsNullOrEmpty(product.Img))
                            {
                                _fileHelper.DeleteFile(product.Img, FolderUploadConstants.Product);
                            }
                            // deleet list img
                            if (product.UploadImgProducts.Count() > 0)
                            {
                                string[] lst = product.UploadImgProducts.Select(x => x.FileName).ToArray();
                                _fileHelper.DeleteListFile(lst, FolderUploadConstants.Product);
                            }

                            _log.LogInformation("DeleteProductCommand delete Image end:" + product.Name);
                        }
                        catch (Exception e)
                        {
                            _log.LogError("DeleteProductCommand update Image error:" + product.Name + "\n" + e.ToString());
                        }
                        return Result<int>.Success(product.Id);
                    }
                    return Result<int>.Fail(HeperConstantss.ERR012);
                }
                
            }
        }
    }
}
