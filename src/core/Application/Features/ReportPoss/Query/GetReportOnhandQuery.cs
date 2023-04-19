using Application.Constants;
using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.ViewModel;
using HelperLibrary;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ReportPoss.Query
{

    public class GetReportOnhandQuery : SearchReportPosModel, IRequest<Result<ResponseReport>>
    {
        public class GetReportOnhandQueryHandler : IRequestHandler<GetReportOnhandQuery, Result<ResponseReport>>
        {
            private readonly IReportPosRepository _repository;
            public GetReportOnhandQueryHandler(IReportPosRepository repository)
            {
                _repository = repository;
            }
            public async Task<Result<ResponseReport>> Handle(GetReportOnhandQuery query, CancellationToken cancellationToken)
            {
                ResponseReport responseReport = new ResponseReport();
                var getdt = await _repository.GetOnhand(query);
                responseReport.Products = getdt;
                return await Result<ResponseReport>.SuccessAsync(responseReport);
            }
        }
    }
}
