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
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Posts.Querys
{

    //public class GetAllPostCacheQuery : IRequest<Result<List<Domain.Entities.Post>>>
    //{

    public class GetAllPostQuery : IRequest<Result<MediatRResponseModel<List<PostModel>>>>
    {
        public string text { get; set; }
        public int? idCategory { get; set; }
        public string sortColumn { get; set; }
        public int recordsTotal { get; set; }
        public int pageSize { get; set; }
        public int skip { get; set; }
        public string sortColumnDirection { get; set; }
    }

    public class PostCachedQueryHandler : IRequestHandler<GetAllPostQuery, Result<MediatRResponseModel<List<PostModel>>>>
    {
        private IOptions<CryptoEngine.Secrets> _config;
        private readonly IRepositoryAsync<Post> _repository;
        private readonly ICategoryPostRepository<CategoryPost> _categoryPostRepository;
        private readonly IMapper _mapper;
        private readonly IRepositoryCacheAsync<Post> _repositoryCache;

        public PostCachedQueryHandler(IRepositoryAsync<Post> repository,
            ICategoryPostRepository<CategoryPost> categoryPostRepository,
            IRepositoryCacheAsync<Post> repositoryCache, IOptions<CryptoEngine.Secrets> config,
                IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
            _repository = repository;
            _repositoryCache = repositoryCache;
            _categoryPostRepository = categoryPostRepository;
        }

        public async Task<Result<MediatRResponseModel<List<PostModel>>>> Handle(GetAllPostQuery request, CancellationToken cancellationToken)
        {
            int coute = 0;
            List<PostModel> productList = new List<PostModel>();

            var datalist = _repository.GetAllQueryable().AsNoTracking();
            if (!(string.IsNullOrEmpty(request.sortColumn) && string.IsNullOrEmpty(request.sortColumnDirection)))
            {
                datalist = datalist.OrderBy(request.sortColumn + " " + request.sortColumnDirection);
            }
            if (!string.IsNullOrEmpty(request.text))
            {
                datalist = datalist.Where(m => m.Name.ToLower().Contains(request.text.ToLower()));
            }
            if (request.idCategory != null && request.idCategory > 0)
            {
                datalist = datalist.Where(m => m.IdCategory == request.idCategory);
            }
            coute = datalist.Count();
            request.recordsTotal = coute;
            datalist = datalist.Skip(request.skip).Take(request.pageSize);
            MediatRResponseModel<List<PostModel>> response = new MediatRResponseModel<List<PostModel>>();
            response.Count = coute;
            response.Data = new List<PostModel>();
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
                    // category = x.CategoryPost != null ? x.CategoryPost.Name : "",
                    Id = x.Id,
                    IdCategory = x.IdCategory,
                    createdate = x.CreatedOn.ToString("dd/MM/yyyy HH:mm:ss"),
                }).ToList();
                foreach (var item in productList)
                {
                    var getcategory = await _categoryPostRepository.GetByIdAsync(item.IdCategory);
                    if (getcategory != null)
                    {
                        item.category += getcategory.Name + "/";
                    }
                    int? idPat = getcategory != null ? getcategory.IdPattern : 0;
                    while (idPat > 0)
                    {
                        getcategory = await _categoryPostRepository.GetByIdAsync(idPat.Value);
                        if (getcategory != null)
                        {
                            item.category += getcategory.Name + "/";
                            idPat = getcategory != null ? getcategory.IdPattern : 0;
                        }
                        else
                        {
                            idPat = null;
                        }

                    };
                    if (!string.IsNullOrEmpty(item.category))
                    {
                        string[] arrycate = item.category.Split('/');
                        arrycate = arrycate.SkipLast(1).ToArray();
                        Array.Reverse(arrycate);
                        string name = String.Join("/", arrycate);
                        item.category = name;
                    }

                    var values = "id=" + item.Id;
                    var secret = CryptoEngine.Encrypt(values, _config.Value.Key);
                    item.UrlParameters = secret;
                }
                response.Data = productList;

            }

            return await Result<MediatRResponseModel<List<PostModel>>>.SuccessAsync(response, coute.ToString());
        }
    }




}
