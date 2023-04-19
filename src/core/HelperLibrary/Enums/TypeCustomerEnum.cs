using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Enums
{
    // phân loại khách hàng là nhà cung cấp hay khách hàng mua hàng hay đơn vị bảo dưỡng
    public enum TypeCustomerEnum
    {
        None =0,
        Companypartner =1,
        Customer =2,
        UnitMaintenance = 3,
        Admin = 4,
    }
}
