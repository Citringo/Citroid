using System.Threading.Tasks;

namespace CitroidForSlack
{
    public interface IBot
	{
		Task InitializeAsync(ICitroid citroid);
		void Exit(ICitroid citroid);
		string Help { get; }
		string Name { get; }
		string Copyright { get; }
		string Version { get; }
	}

}