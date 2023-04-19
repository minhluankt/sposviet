using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Web.ManagerApplication.Abstractions
{
    public interface IViewRenderService
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model, ViewDataDictionary viewDictionary = null);
    }
}
