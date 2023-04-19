using Application.Enums;

namespace Domain.ViewModel
{
    public class DatatableModel
    {
        public string sortColumn { get; set; }
        public int recordsTotal { get; set; }
        public int pageSize { get; set; } = 10;
        public int skip { get; set; }
        public int currentPage { get; set; }
        public int Comid { get; set; }
        public EnumTypeProduct TypeProduct { get; set; } = EnumTypeProduct.THOITRANG;
        public string sortColumnDirection { get; set; }
    }
}
