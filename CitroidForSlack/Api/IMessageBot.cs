using System.Threading.Tasks;

namespace CitroidForSlack
{
    public interface IMessageBot : IBot
	{
		bool CanExecute(Message mes);
		Task RunAsync(Message mes, ICitroid citroid);
	}

}