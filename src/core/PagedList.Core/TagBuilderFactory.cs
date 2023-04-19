using PagedList.Web.Common;

namespace PagedList.Core;

internal sealed class TagBuilderFactory : ITagBuilderFactory
{
    public ITagBuilder Create(string tagName)
    {
        return new TagBuilder(tagName);
    }
}