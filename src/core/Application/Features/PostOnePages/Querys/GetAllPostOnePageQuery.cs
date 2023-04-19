using Application.CacheKeys;
using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Repositories;
using Application.Providers;
using AspNetCoreHero.Results;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Application.Features.PostOnePages.Querys
{

    //public class GetAllPostCacheQuery : IRequest<Result<List<Domain.Entities.Post>>>
    //{

    public class GetAllPostQuery : IRequest<Result<List<PostModel>>>
    {
        public string text { get; set; }
        public int? idCategory { get; set; }
        public string sortColumn { get; set; }
        public int recordsTotal { get; set; }
        public int pageSize { get; set; }
        public int skip { get; set; }
        public string sortColumnDirection { get; set; }
        public GetAllPostQuery(out int _count)
        {
            _count = recordsTotal;
        }
    }

    public class PostCachedQueryHandler : IRequestHandler<GetAllPostQuery, Result<List<PostModel>>>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly IRepositoryAsync<PagePost> _repository;
        private readonly IMapper _mapper;
        private readonly IRepositoryCacheAsync<PagePost> _repositoryCache;

        public PostCachedQueryHandler(IRepositoryAsync<PagePost> repository,
            IRepositoryCacheAsync<PagePost> repositoryCache, IOptions<CryptoEngine.Secrets> config,
                IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
            _repository = repository;
            _repositoryCache = repositoryCache;
        }

        public async Task<Result<List<PostModel>>> Handle(GetAllPostQuery request, CancellationToken cancellationToken)
        {
            int coute = 0;
            List<PostModel> productList = new List<PostModel>();

            var datalist = _repository.GetAllQueryable();
            if (!(string.IsNullOrEmpty(request.sortColumn) && string.IsNullOrEmpty(request.sortColumnDirection)))
            {
                datalist = datalist.OrderBy(request.sortColumn + " " + request.sortColumnDirection);
            }
            if (!string.IsNullOrEmpty(request.text))
            {
                datalist = datalist.Where(m => m.Name.ToLower().Contains(request.text.ToLower()));
            }
          
            coute = datalist.Count();
            request.recordsTotal = coute;
            datalist = datalist.Skip(request.skip).Take(request.pageSize);
            if (coute > 0)
            {
                productList = datalist.Select(x => new
                     PostModel
                {
                    Name = x.Name,
                    Title = x.Title,
                    Active = x.Active,
                    ViewNumber = x.ViewNumber,
                    Img = x.Img,
                    Decription = x.Decription,
                    Id = x.Id,
                    createdate = x.CreatedOn.ToString("dd/MM/yyyy HH:mm:ss"),
                }).ToList();
                foreach (var item in productList)
                {
                    //var user = _userManagerRepository.GetDataUser(item.CreatedBy);
                    //if (user != null)
                    //{
                    //    item.PostedbyAdmin = user.FullName;
                    // }
                    var values = "id=" + item.Id;
                    var secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                    item.UrlParameters = secret;
                }
            }

            return await Result<List<PostModel>>.SuccessAsync(productList, coute.ToString());
        }
    }




}
