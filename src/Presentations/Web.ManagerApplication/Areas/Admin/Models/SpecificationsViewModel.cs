using Domain.Entities;

namespace Web.ManagerApplication.Areas.Admin.Models
{
    public class SpecificationsViewModel
    {
        public int Id { get; set; }
        public int idTypeSpecifications { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public double From
        {
            get
            {
                double value = !string.IsNullOrEmpty(_from) ? double.Parse(_from.Replace(",", "")) : 0;
                return value;
            }
            set
            {
                //Price = value;
            }
        }

        public string _from { get; set; }
        public string _to { get; set; }
        public double To
        {
            get
            {
                double value = !string.IsNullOrEmpty(_to) ? double.Parse(_to.Replace(",", "")) : 0;
                return value;
            }
            set
            {

            }
        }
        public int Sort { get; set; }
        public IEnumerable<TypeSpecifications> listtype { get; set; }
        public TypeSpecifications TypeSpecifications { get; set; }
    }
}
