using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Model
{
    public class PostModel
    {
        public string seokeyword { get; set; } // 
        public string seotitle { get; set; } //
        public string seoDescription { get; set; } //
        public string keyword { get; set; }
        public string category { get; set; }
        public string createdate { get; set; }
        public string UrlParameters { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Decription { get; set; }
        public int Sort { get; set; }
        public int ViewNumber { get; set; }
        public string Img { get; set; }
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
        public int IdCategory { get; set; }
        public bool Active { get; set; } = true;
        public IFormFile ImgUpload { get; set; }
        public IEnumerable<CategoryPost> CategoryPosts { get; set; }
        public CategoryPost CategoryPost { get; set; }

    }
}
