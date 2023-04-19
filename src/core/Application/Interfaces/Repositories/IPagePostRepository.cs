namespace Application.Interfaces.Repositories
{
    public interface IPagePostRepository<T> where T : class
    {
        void UpdateReView(int id);

    }
}
