using System.Threading.Tasks;

namespace CitroidForSlack.Api
{
    public interface IMessageBot : IBot
	{
		bool CanExecute(Message mes);
		Task RunAsync(Message mes, ICitroid citroid);
	}



}