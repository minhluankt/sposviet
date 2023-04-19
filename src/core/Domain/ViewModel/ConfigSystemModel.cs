using Domain.Entities;

namespace Domain.ViewModel
{
    public class ConfigSystemModel : ConfigSystem
    {
        public int pageSizeProductInCategory { get; set; }
        public int pageSizeTable { get; set; }
        public int layoutHeader { get; set; }
        public SellModelSetting SellModelSetting { get; set; }

    }
    public class SellModelSetting
    {
        public string UrlSell { get; set; }
        public string Color { get; set; }
        public string ImgSell { get; set; }
        public string TitleSell { get; set; }
        public bool ShowTitle { get; set; }
        public string ColorTitle { get; set; }
    }
}
