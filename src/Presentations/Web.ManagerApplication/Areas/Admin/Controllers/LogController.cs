using Application.Constants;
using Application.DTOs.Logs;
using Application.Features.ActivityLog.Queries.GetUserLogs;
using Application.Features.Logs.Queries;
using Application.Features.Logs.QueriesDapper;
using Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SystemVariable;
using Web.ManagerApplication.Areas.Admin.Models;
using Web.ManagerApplication.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Application.Hepers;

namespace Web.ManagerApplication.Areas.Admin.Controllers
{
    [Authorize()]
    [Area("Admin")]
    public class LogController : BaseController<LogController>
    {
        private IStringLocalizer<SharedResource> _localizer;
        //private IHostingEnvironment Environment;
        private ILogRepository _logSeri;

        public LogController(IStringLocalizer<SharedResource> localizer, ILogRepository logSeri)
        {
            _localizer = localizer;
            _logSeri = logSeri;
            //  Environment = _environment;
        }
        [Authorize(Policy = "log.file")]
        public IActionResult Serilog()
        {
            IOrderedEnumerable<FileInfo> orderedFiles = this.ListFile();
            FileModel fileModel = new FileModel();
            fileModel.FileAll = orderedFiles;
            return View(fileModel);
        }
        [Authorize(Policy = "log.database")]
        public async Task<IActionResult> Historyoperation(string userId, string FromDate, string ToDate, string textSearch, int? page, int? pageSize)
        {
            var currentUser = User.Identity.GetUserClaimLogin();

            int _pageIndex = page ?? 1;
            int _pageSize = pageSize ?? 10;
            var response = await _mediator.Send(new GetAllQuery() { 
                ComId= currentUser.ComId,
                UserId = userId, FromDate = FromDate, ToDate = ToDate, textSearch = textSearch, pageIndex = _pageIndex, pageSize = _pageSize });
            if (response.Succeeded)
            {
                return View(response.Data);
            }
            return View(response.Data);
        }
        private IOrderedEnumerable<FileInfo> ListFile()
        {
            return _logSeri.GetAllSerilog(SystemVariableHelper.folderLog);
        }
        [Authorize(Policy = "log.delete")]
        public async Task<IActionResult> DeleteSerilog(string fileName)
        {
            try
            {
                var fi1 = _logSeri.DeleteSerilog(fileName, SystemVariableHelper.folderLog);
                if (fi1)
                {
                    _notify.Success(_localizer.GetString("DeleteOk").Value);
                    IOrderedEnumerable<FileInfo> orderedFiles = this.ListFile();
                    FileModel fileModel = new FileModel();
                    fileModel.FileAll = orderedFiles;
                    var html = await _viewRenderer.RenderViewToStringAsync("_Table", fileModel);
                    return new Microsoft.AspNetCore.Mvc.JsonResult(new { isValid = true, html = html });
                }
                _notify.Error(_localizer.GetString("DeleteERR").Value);
                return new Microsoft.AspNetCore.Mvc.JsonResult(new { isValid = false, html = string.Empty });
            }
            catch (Exception e)
            {
                _notify.Error(e.ToString());
                return new Microsoft.AspNetCore.Mvc.JsonResult(new { isValid = false, html = string.Empty });
            }

            //return View();
        }
        public IActionResult SerilogView(string fileName)
        {
            FileModel html = new FileModel();
            html.Html = _logSeri.SerilogView(fileName, SystemVariableHelper.folderLog);
            return PartialView("_SerilogView", html);
            //return View();
        }
    }
}
