using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CustomerModel
    {
        public int Id
        {
            get;
            set;
        }

        public string CreatedBy
        {
            get;
            set;
        }

        public DateTime CreatedOn
        {
            get;
            set;
        }

        public string LastModifiedBy
        {
            get;
            set;
        }

        public DateTime? LastModifiedOn
        {
            get;
            set;
        }
        public DateTime? LoginLast
        {
            get;
            set;
        }
        public DateTime? BirthDate
        {
            get;
            set;
        }
        public Guid IdCodeGuid { get; set; }
        public string Salt { get; set; }
        public string UserName { get; set; }
        public string LoginProvider { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public string Image { get; set; } // ảnh đại diện
        public IFormFile LogoUpload { get; set; } // ảnh đại diện
        public IFormFile ImageUpload { get; set; } // ảnh đại diện

        public string Address { get; set; }
        public int? IdDistrict { get; set; } // quận
        public int? IdCity { get; set; }
        public int? IdWard { get; set; }
        public string Email { get; set; }
        public string createdate { get; set; }
        public string PhoneNumber { get; set; }
        public string UrlParameters { get; set; }
        public string TextPhoneOrEmail { get; set; }
        public int Status { get; set; } // tọa độ
    }
    public class CustomerModelView : LoginCustomerViewModel
    {
        public string EmailConfirm { get; set; }
        public bool isEmailConfirm { get; set; }
        public string ConfirmPassword { get; set; }
        public string PasswordOld { get; set; }
        public Guid IdCodeGuid { get; set; }
        public string Salt { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public string Image { get; set; }
        public IFormFile ImageUpload { get; set; }
        public IFormFile LogoUpload { get; set; }

        public string Address { get; set; }
        public int Id { get; set; } // quận
        public int? IdDistrict { get; set; } // quận
        public int? IdCity { get; set; }
        public int? IdWard { get; set; }
        public DateTime? LoginLast { get; set; } // đăng nhập lần cuối
        public string LoginProvider { get; set; } // login bằng gg hay face

        public string ProviderKey { get; set; } // key login gg hay face
        public string PhoneNumber { get; set; }
        public string secretId { get; set; }
        public bool Active { get; set; } // tọa độ
        public ICollection<Product> Products { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public IEnumerable<City> Citys { get; set; }
        public City City { get; set; }
        public District District { get; set; }
        public Ward Ward { get; set; }
    }
}
