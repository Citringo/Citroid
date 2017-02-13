using System.Threading.Tasks;

namespace CitroidForSlack
{
    public interface IBot
	{
		Task InitializeAsync(ICitroid citroid);
		void Exit(ICitroid citroid);
	}

}