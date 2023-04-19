using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.PostOnePages.Querys
{

    public class GetByIdPagePostQuery : IRequest<Result<PagePost>>
    {
        public bool IsView { get; set; }
        public int Id { get; set; }

        public class GetPagePostByIdQueryHandler : IRequestHandler<GetByIdPagePostQuery, Result<PagePost>>
        {
            private readonly IRepositoryAsync<PagePost> _repository;
            private readonly IPagePostRepository<PagePost> _postrepository;
            public GetPagePostByIdQueryHandler(IRepositoryAsync<PagePost> repository, IPagePostRepository<PagePost> postrepository)
            {
                _postrepository = postrepository;
                _repository = repository;
            }
            public async Task<Result<PagePost>> Handle(GetByIdPagePostQuery query, CancellationToken cancellationToken)
            {
                var product = await _repository.GetByIdAsync(query.Id);
                if (product == null)
                {
                    return await Result<PagePost>.FailAsync(HeperConstantss.ERR012);
                }
                if (query.IsView)
                {
                    var task = Task.Run(() =>
                    {
                        _postrepository.UpdateReView(product.Id);
                    });
                }
                return await Result<PagePost>.SuccessAsync(product);
            }
        }
    }
}
