
using Application.Constants;
using Application.Enums;
using Application.Features.Products.Query;
using Application.Features.PromotionRuns.Query;
using Application.Hepers;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ViewModel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.ManagerApplication.Models;

namespace Web.ManagerApplication.Pages.Shared.Components.ListProductDiscountRun_Home
{
    public class ListProductDiscountRun_HomeViewComponent : ViewComponent
    {
        private IMediator _mediator;
        private readonly ILogger<ListProductDiscountRun_HomeViewComponent> _logger;
        private readonly IRepositoryAsync<ConfigSystem> _configRepository;
        private readonly IPromotionRunRepository _promotionRunRepository;
        public ListProductDiscountRun_HomeViewComponent(IMediator mediator,
            IRepositoryAsync<ConfigSystem> configRepository, ILogger<ListProductDiscountRun_HomeViewComponent> logger,
            IPromotionRunRepository promotionRunRepository)
        {
            _logger = logger;
            _promotionRunRepository = promotionRunRepository;
            _mediator = mediator;
            _configRepository = configRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            HomeViewModel model = new HomeViewModel();
            try
            {

                var getevent = await _mediator.Send(new GetActivePromotionRunQuery() { IsProcessing = true });
                if (getevent.Succeeded)
                {
                    if (getevent.Data != null)
                    {
                        model.PromotionRun = getevent.Data.SingleOrDefault();
                        model.Products = new List<Product>();
                        if (model.PromotionRun != null)
                        {
                            if (model.PromotionRun.EndDate <= DateTime.Now || model.PromotionRun.IsCancelEvent || model.PromotionRun.Status > (int)StatusPromotionRun.Processing)
                            {
                                if (model.PromotionRun.EndDate <= DateTime.Now && !model.PromotionRun.IsCancelEvent && model.PromotionRun.Status < (int)StatusPromotionRun.Done)
                                {
                                    // tức là thời hạn đã hết mà chưa update gì trạng thái thfi up sang done
                                    _promotionRunRepository.CheckUpdateStatus(model.PromotionRun.Id, (int)StatusPromotionRun.Done);

                                }
                                model.Products = new List<Product>();
                                //return View(model);
                            }
                            else
                            {
                                var getid = await _mediator.Send(new SearchProductQuery() { isRunPromotion = true, IdPromotionRun = model.PromotionRun.Id });
                                if (getid.Succeeded)
                                {
                                    model.Products = getid.Data;
                                }
                            }

                        }
                    }
                    else
                    {
                        model.Products = new List<Product>();
                    }



                    var getidProductSell = await _mediator.Send(new SearchProductQuery() { isPromotion = true, CheckExpirationDateDiscount = true });
                    if (getidProductSell.Succeeded)
                    {
                        model.ProductSell = getidProductSell.Data;
                    }
                    else
                    {
                        model.ProductSell = new List<Product>();
                    }


                    var getconfigsell = _configRepository.GetAll(m => m.Key.ToLower() == ParametersConfigSystem.SellSettingInHome.ToLower()).SingleOrDefault();
                    if (getconfigsell != null)
                    {
                        var modelsell = Common.ConverJsonToModel<SellModelSetting>(getconfigsell.Value);
                        if (modelsell != null)
                        {
                            model.SellModelSetting = modelsell;
                        }

                    }
                    else
                    {
                        model.SellModelSetting = new SellModelSetting();
                    }
                    return View(model);
                }

                return View(model);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return View(model);
            }

        }
    }
}
