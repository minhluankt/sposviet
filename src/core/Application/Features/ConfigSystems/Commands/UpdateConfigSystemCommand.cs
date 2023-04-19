using Application.CacheKeys;
using Application.Constants;
using Application.Enums;
using Application.Hepers;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using Domain.XmlDataModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ConfigSystems.Commands
{

    public partial class UpdateConfigSystemCommand : ConfigSystem, IRequest<Result<int>>
    {
        public List<ConfigSaleParametersItem> lstKey { get; set; }
    }
    public class UpdateConfigSystemHandler : IRequestHandler<UpdateConfigSystemCommand, Result<int>>
    {
        private readonly ITableLinkRepository _tablelink;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private readonly IRepositoryAsync<ConfigSystem> _Repository;
        private IUnitOfWork _unitOfWork { get; set; }

        public UpdateConfigSystemHandler(IRepositoryAsync<ConfigSystem> brandRepository,
              ITableLinkRepository tablelink, IMapper mapper,
            IUnitOfWork unitOfWork, IDistributedCache distributedCach)
        {
            _tablelink = tablelink;
            _Repository = brandRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _distributedCache = distributedCach;
        }

        public async Task<Result<int>> Handle(UpdateConfigSystemCommand command, CancellationToken cancellationToken)
        {
            if (command.ComId==0)
            {
                return Result<int>.Fail(HeperConstantss.ERR012);
            }

            if (!string.IsNullOrEmpty(command.Key))
            {
                var brand = _Repository.GetAll(m => m.Key.ToLower() == command.Key.ToLower() &&m.ComId==command.ComId).SingleOrDefault();
                if (brand == null)
                {
                    var product = _mapper.Map<ConfigSystem>(command);
                    product.ComId= command.ComId;
                    brand.Value = command.Value;
                    brand.Parent = command.Parent;
                    brand.TypeValue = command.TypeValue;
                    brand.Type = command.Type;
                    await _Repository.AddAsync(product);
                    await _distributedCache.RemoveAsync(ConfigSystemCacheKeys.key);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result<int>.Success(brand.Id);
                }
                else
                {
                    if (!string.IsNullOrEmpty(command.Value))
                    {
                        brand.TypeValue = command.TypeValue;
                        brand.Type = command.Type;
                        brand.Value = command.Value;
                        brand.Parent = command.Parent;
                        await _Repository.UpdateAsync(brand);
                        await _distributedCache.RemoveAsync(ConfigSystemCacheKeys.key);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                    }
                }
                return Result<int>.Success(brand.Id);
            }

            if (command.lstKey != null && command.lstKey.Count() > 0)
            {
                int checkupdate = 0;
                foreach (var item in command.lstKey)
                {
                    var getData = _Repository.GetAll(m => m.Key.ToLower() == item.Key.ToLower() && m.ComId == command.ComId).SingleOrDefault();
                    if (getData != null)
                    {
                        checkupdate = checkupdate + 1;
                        if (!string.IsNullOrEmpty(item.Parent))
                        {
                            var getparent = _Repository.Entities.AsNoTracking().Where(m => m.Key.ToLower() == item.Parent.ToLower() && m.ComId == command.ComId).SingleOrDefault();

                            if (getparent!=null)
                            {
                                if (item.TypeValue == EnumTypeValue.BOOL.ToString())
                                {
                                    if (Convert.ToBoolean(item.Value))
                                    {
                                        if (getparent.TypeValue == EnumTypeValue.BOOL.ToString() && !string.IsNullOrEmpty(getparent.Value) && !Convert.ToBoolean(getparent.Value))
                                        {
                                            return Result<int>.Fail(HeperConstantss.ERR012);
                                        }
                                    }

                                }
                            }
                            else
                            {
                                return Result<int>.Fail("Dữ liệu không hợp lệ!");
                            }
                        }
                        

                        getData.Value = item.Value;
                        getData.Parent = item.Parent;
                        getData.TypeValue = item.TypeValue;
                        getData.Type = item.Type;
                        await _Repository.UpdateAsync(getData);
                   
                    }
                    else
                    {
                        //thêm vào nhưng có cha thì phải check cha vì có thể bọ hach
                        if (!string.IsNullOrEmpty(item.Parent))
                        {
                            var getparent = _Repository.Entities.AsNoTracking().Where(m => m.Key.ToLower() == item.Parent.ToLower() && m.ComId == command.ComId).SingleOrDefault();
                            if (getparent != null)
                            {
                                if (item.TypeValue == EnumTypeValue.BOOL.ToString())
                                {
                                    if (Convert.ToBoolean(item.Value))
                                    {
                                        if (getparent.TypeValue == EnumTypeValue.BOOL.ToString() && !Convert.ToBoolean(getparent.Value))
                                        {
                                            return Result<int>.Fail("Dữ liệu không hợp lệ!");
                                        }
                                    }
                                }

                            }
                            else
                            {
                                return Result<int>.Fail("Dữ liệu không hợp lệ!");
                            }
                               
                        }
                        checkupdate = checkupdate + 1;
                        var product = new ConfigSystem() { 
                            Key = item.Key,
                            Value = item.Value,
                            Parent = item.Parent,
                            Type = item.Type,
                            ComId = command.ComId,
                            TypeValue= item.TypeValue };
                        await _Repository.AddAsync(product);
                    }
                }
                if (checkupdate > 0)
                {
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                return Result<int>.Success();
            }
            return Result<int>.Fail(HeperConstantss.ERR012);
        }
    }
}
