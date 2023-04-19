namespace Web.ManagerApplication.Areas.Selling.Models
{
    public class JsonModelView
    {
        public int id { get; set; }
        public int? parentId { get; set; }
        public string name { get; set; }
        public bool isDirectory { get; set; }
        public bool expanded { get; set; }
        public bool selected { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedOn { get; set; }
        public int countProduct { get; set; }
    }
}
