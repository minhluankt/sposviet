using Application.CacheKeys;
using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.PostOnePagess.Querys
{
 
    public class GetAllIQueryablePostOnePagesQuery : IRequest<Result<IQueryable<PagePost>>>
    {
       
    }
    public class GetAllIQueryablePostOnePagesHandler : IRequestHandler<GetAllIQueryablePostOnePagesQuery, Result<IQueryable<PagePost>>>
    {

        private readonly IRepositoryCacheAsync<PagePost> _PostOnePagesCache;
        private readonly IRepositoryAsync<PagePost> _PostOnePages;
        private readonly IMapper _mapper;

        public GetAllIQueryablePostOnePagesHandler(IRepositoryCacheAsync<PagePost> PostOnePagesCache, IMapper mapper, IRepositoryAsync<PagePost> PostOnePages)
        {
            _PostOnePages = PostOnePages;
            _PostOnePagesCache = PostOnePagesCache;
            _mapper = mapper;
        }

        public async Task<Result<IQueryable<PagePost>>> Handle(GetAllIQueryablePostOnePagesQuery request, CancellationToken cancellationToken)
        {
            var data =  _PostOnePages.GetAllQueryable().Where(m => m.Active);
         
            return await Result<IQueryable<PagePost>>.SuccessAsync(data);
        }
    }
}
