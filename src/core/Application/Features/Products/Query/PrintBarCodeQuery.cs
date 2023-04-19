using Application.Constants;
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

namespace Application.Features.Products.Query
{
   
    public class PrintBarCodeQuery : IRequest<Result<List<Product>>>
    {

        public int Comid { get; set; }
        public int[] lstidPro { get; set; }

        public class PrintBarCodeQueryHandler : IRequestHandler<PrintBarCodeQuery, Result<List<Product>>>
        {
            private readonly IRepositoryAsync<Product> _repository;
            private readonly IProductPepository<Product> _productrepository;
            public PrintBarCodeQueryHandler(IRepositoryAsync<Product> repository, IProductPepository<Product> productrepository)
            {
                _productrepository = productrepository;
                _repository = repository;
            }
            public async Task<Result<List<Product>>> Handle(PrintBarCodeQuery query, CancellationToken cancellationToken)
            {
                var product = await _productrepository.GetProductbyListId(query.lstidPro, query.Comid);
                // int Comid,EnumTypePrintBarCode enumTypePrintBarCode, int columtem =2
                _productrepository.MapBarCode(product);
                return await Result<List<Product>>.SuccessAsync(product);
            }
        }
    }
}
