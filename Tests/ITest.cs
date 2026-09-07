




using System.Threading.Tasks;



namespace InputConnect.Tests
{
    public interface ITest
    {
        string Name { get; }

        Task<int> Initialize();
    }
}