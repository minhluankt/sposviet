using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Web.ManagerCompany.Abstractions
{
    public interface IViewRenderService
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model, ViewDataDictionary viewDictionary = null);
    }
}
