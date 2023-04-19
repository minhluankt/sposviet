namespace Application.Interfaces.Shared
{
    public interface IAuthenticatedUserService
    {
        string _comId { get; }
        int? ComId { get; }
        string UserId { get; }
        public string Username { get; }
    }
}