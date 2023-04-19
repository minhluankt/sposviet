namespace PagedList.Web.Common;

public interface ITagBuilderFactory
{
    ITagBuilder Create(string tagName);
}