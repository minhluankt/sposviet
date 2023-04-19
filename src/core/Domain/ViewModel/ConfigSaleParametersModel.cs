using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class ConfigSaleParametersModel
    {
        public List<ConfigSaleParametersItem> ConfigSaleParametersItems { get; set; }
    }
    public class ConfigSaleParametersItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string Parent { get; set; }
        public string TypeValue { get; set; }
    }
}
