using CitroidForSlack.Api;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace CitroidForSlack.Plugins.CitDealer
{
	public class MessageBotCitDealer : IMessageBot
	{
		public string Name => "しとディーラー";

		public string Copyright => "(C)2017 Citrine";

		public string Version => "2.0.0 pre-pre-alpha";

		public string Help => "できることはありません。";

		enum GameState
		{
			Inactive,
		}
		
		
		public void Exit(ICitroid citroid)
		{
			//TODO: ゲームの状態をシリアライズさせる
		}

		
		public async Task InitializeAsync(ICitroid citroid)
		{
			
		}
		

		public async Task RunAsync(Message mes, ICitroid citroid)
		{
			
		}

		#region どうでもいいやつ
		private ICitroid _citroid;

		public readonly Random Rand = new Random();

		private Timer _timer = new Timer();

		public bool CanExecute(Message mes) => string.IsNullOrEmpty(mes.subtype);
		#endregion
	}

}
