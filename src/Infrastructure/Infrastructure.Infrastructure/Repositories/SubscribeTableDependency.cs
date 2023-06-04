using Application.Enums;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Identity;
using Infrastructure.Infrastructure.HubS;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;

namespace Infrastructure.Infrastructure.Repositories
{
    public class SubscribeProductTableDependency : ISubscribeTableDependency
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ILogger<SubscribeProductTableDependency> _logger;
        SqlTableDependency<Kitchen> tableDependency;
        SignalRHub dashboardHub;

        public SubscribeProductTableDependency(SignalRHub dashboardHub, ILogger<SubscribeProductTableDependency> logger)
        {
            // _userManager = userManager;
            this.dashboardHub = dashboardHub;
            _logger = logger;
        }

        public void SubscribeTableDependency(string connectionString)
        {
            tableDependency = new SqlTableDependency<Kitchen>(connectionString);
            tableDependency.OnChanged += TableDependency_OnChanged;
            tableDependency.OnError += TableDependency_OnError;
            tableDependency.Start();
        }

        private async void TableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<Kitchen> e)
        {
            switch (e.ChangeType)
            {
                case ChangeType.None:
                    _logger.LogError("OnChanged realtime ChangeType None");
                    break;
                case ChangeType.Delete:
                    //await dashboardHub.sendNotifyPos(EnumTypeSignalRHub.CHITKEN, EnumTypeSignalRHub.DELETECHITKEN);
                    break;
                case ChangeType.Insert:
                    //await dashboardHub.sendNotifyPos(EnumTypeSignalRHub.CHITKEN, EnumTypeSignalRHub.CHITKEN);
                    break;
                case ChangeType.Update:
                   // await dashboardHub.sendNotifyPos(EnumTypeSignalRHub.CHITKEN, EnumTypeSignalRHub.UPDATECHITKEN);
                    break;
                default:
                    break;
            }
        }

        private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {

            _logger.LogError("Lỗi realtime");
            _logger.LogError($"{nameof(Kitchen)} SqlTableDependency error: {e.Error.Message}");
            _logger.LogError(e.Error.ToString());
            Console.WriteLine($"{nameof(Kitchen)} SqlTableDependency error: {e.Error.Message}");
        }
    }
}
