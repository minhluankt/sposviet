namespace Web.ManagerApplication.Models
{
    public class SearchIndexModel
    {
        public string sortby { get; set; }
        public string txtcategory { get; set; }
        public string slugcate { get; set; }
        public int page { get; set; }
        public int idPrice { get; set; }//chuyên mục
        public int idcate { get; set; }//chuyên mục
        public bool isPromotion { get; set; }
        public bool loadmore { get; set; }
    }
}
