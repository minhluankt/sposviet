using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Constants
{
    public class CustomClaimTypes
    {
        public const string Permission = "Permission";
    }
    public class ClaimUser
    {
        public const string FULLNAME = "FULLNAME";
        public const string COMID = "COMID";
        public const string IDDICHVU = "IDDICHVU";
        public const string IDGUID = "IDGUID";
        public const string IsAdmin = "IsAdmin";
        public const string IsPhucVu = "IsPhucVu";
        public const string IsPhucVuPayment = "IsPhucVuPayment";
        public const string IsBep = "IsBep";
        public const string IsThuNgan = "IsThuNgan";
        public const string IsKeToan = "IsKeToan";
    }
    public class PermissionUser
    {
        public const string admin = "superadmin";
        public const string phucvuthanhtoan = "posStaff.payment";//thu ngân phục vụ
        public const string nhanvienphucvu = "orderStaff.index";
        public const string quanlyketoan = "selling.Dashboard";
        public const string thunganpos = "pos.order";
        public const string thunganSaleRetail = "saleRetail.order";
        public const string beppos = "posKitchen.order";//màn hình bếp
    }
}
