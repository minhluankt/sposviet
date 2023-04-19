using Application.Interfaces.Repositories;
using AspNetCoreHero.Results;
using Domain.ViewModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ReportPoss.Query
{
    public class ExportImportOnhandQuery : SearchReportPosModel, IRequest<Result<ResponseReport>>
    {
        public class ExportImportOnhandQueryHandler : IRequestHandler<ExportImportOnhandQuery, Result<ResponseReport>>
        {
            private readonly IReportPosRepository _repository;
            public ExportImportOnhandQueryHandler(IReportPosRepository repository)
            {
                _repository = repository;
            }
            public async Task<Result<ResponseReport>> Handle(ExportImportOnhandQuery query, CancellationToken cancellationToken)
            {
                ResponseReport responseReport = new ResponseReport();
                var getdt = await _repository.GetExportImportOnhand(query);
                responseReport.ReportXuatNhapTonKhos = getdt;
                return await Result<ResponseReport>.SuccessAsync(responseReport);
            }
        }
    }
}
