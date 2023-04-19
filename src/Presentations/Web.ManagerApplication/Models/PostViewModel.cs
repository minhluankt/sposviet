using Domain.Entities;
using Model;
using X.PagedList;

namespace Web.ManagerApplication.Models
{
    public class PostViewModel
    {
        public PostModel Post { get; set; }
        public TypeCategory TypeCategory { get; set; }
        public string codeCategory { get; set; }
        public List<CategoryPost> Categorys { get; set; }
        public CategoryPost Category { get; set; }
        public IPagedList<Post> PostsIPagedList { get; set; }
        public IEnumerable<Post> PostsQuery { get; set; }
        public List<Post> ListPost { get; set; }
    }
}
