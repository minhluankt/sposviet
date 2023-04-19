
using Model;

namespace Domain.ViewModel
{
    public class NotifyUserModel : ParametersPageModel
    {
        public int Type { get; set; }//0 là user
        public int IdUser { get; set; }
        public bool IsReview { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
    }
}
