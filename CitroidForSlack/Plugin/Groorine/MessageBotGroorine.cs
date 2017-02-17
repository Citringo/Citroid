using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitroidForSlack
{
	public class MessageBotGroorine : IMessageBot
	{
		public string Help => "アップロードされたスタンダード MIDI ファイルを MP3 ファイルに変換します。";

		public string Name => "GroorineBot";

		public string Copyright => "(C)2016-2017 Citrine";

		public string Version => "1.0.0pre-alpha";

		public bool CanExecute(Message mes) => true;

		public void Exit(ICitroid citroid) { }

		public Task InitializeAsync(ICitroid citroid) => Task.Delay(0);

		public async Task RunAsync(Message mes, ICitroid citroid)
		{

		}




	}
}
