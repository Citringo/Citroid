using GroorineCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using P = GroorineCore.DotNet45.Player;

namespace CitroidForSlack
{

	public class MessageBotGroorine : IMessageBot
	{
		public string Help => "アップロードされたスタンダード MIDI ファイルを、Groorineを使用して音楽ファイルに変換し、投稿します。\n" +
			"この機能を有効化するには、 \"enable groorine\"と話します。\n" +
			"無効化するには、\"disable groorine\"と話します。\n" +
			"イイ曲を生み出せ！";

		public string Name => "Groorine Bot";

		public string Copyright => "(C)2016-2017 Citrine";

		public string Version => "1.0.0alpha";

		public bool CanExecute(Message mes) => true;

		public void Exit(ICitroid citroid) { }

		/// <summary>
		/// MML パーサーの状態を表します。
		/// </summary>
		enum ParseState
		{
			/// <summary>
			/// 音符や設定などの MML 識別子。
			/// </summary>
			Identifier,
			/// <summary>
			/// 音符以外の識別子に続く数値。
			/// </summary>
			Num,
			/// <summary>
			/// 音符に続く数値。
			/// </summary>
			NumAfterNote,
			/// <summary>
			/// 半音符に続く数値。
			/// </summary>
			NumAfterHalfNote,
		}

		/// <summary>
		/// ファイルをダウンロードする場所です。
		/// </summary>
		public static readonly string DIRECTORY_TO_DOWNLOAD = "downloads";

		/// <summary>
		/// Groorine で使用する一時フォルダです。
		/// </summary>
		public static readonly string GROORINE_TEMP = "grtemp";

		public async Task InitializeAsync(ICitroid citroid)
		{
			citroid.FileShared += (s, e) => Task.Factory.StartNew(async () =>
			{
				if (!_isActive)
					return;
				string path;
				await e.DownloadAsync(path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DIRECTORY_TO_DOWNLOAD));
				path = Path.Combine(path, e.name);
				var wavpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GROORINE_TEMP, Path.ChangeExtension(e.name, "wav"));
				var mp3path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GROORINE_TEMP, Path.ChangeExtension(e.name, "mp3"));
				Directory.CreateDirectory(Path.GetDirectoryName(wavpath));
				var lamepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lame.exe");
				if (File.Exists(path) && new[] { ".mid", ".midi", ".smf" }.Contains(Path.GetExtension(path)))
				{
					var p = new P();
					try
					{
						// Groorine でmid => wav
						/*if (!File.Exists(wavpath))
							*/
						await p.SaveAsync(wavpath, 2, SmfParser.Parse(new FileStream(path, FileMode.Open, FileAccess.Read)));

						var uploadFile = wavpath;
						if (File.Exists(lamepath) /*&& !File.Exists(mp3path)*/)
						{
							// lameが存在するならmp3に変換する
							Process.Start(new ProcessStartInfo(lamepath, $@"""{wavpath}""")
							{
								UseShellExecute = false,
								RedirectStandardOutput = true
							}).WaitForExit(114514); // くさい
							uploadFile = mp3path;
						}
						else
							Console.WriteLine("I couldn't find lame.exe. Music file won't be converted to mp3...");
						await citroid.UploadFileAsync(uploadFile, p.CorePlayer.CurrentFile.Title ?? Path.GetFileName(uploadFile) ?? "MIDI Music by Groorine", e.channels);
					}
					catch (ArgumentException ae)
					{
						await citroid.PostAsync(e.channels?.LastOrDefault() ?? "", "Groorineがエラーを吐いたよ: " + ae.Message);
						Debug.WriteLine(ae.ToString());
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex.ToString());
					}

				}

				Console.WriteLine(JsonConvert.SerializeObject(e, Formatting.Indented));
			});
		}

		private bool _isActive = true;
		


		public async Task RunAsync(Message mes, ICitroid citroid)
		{
			var cmd = mes.text.ToLower();
			if (cmd.Contains("groorine"))
			{
				if (cmd.Contains("enable"))
				{
					_isActive = true;
					await citroid.PostAsync(mes.channel, ":white_check_mark: Groorine が有効になりました。");
				}
				if (cmd.Contains("disable"))
				{
					_isActive = false;
					await citroid.PostAsync(mes.channel, ":x: Groorine が無効になりました。");
				}
				if (cmd.Contains("hage"))
				{
					await citroid.UploadFileAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hage.mp4"), "アタマフリー", mes.channel);
				}
			}
			if (cmd.Contains("hello"))
				await citroid.PostAsync(mes.channel, "https://otyakai.slack.com/files/otya/F47D0SQ9K/default.mp4");
			
		}

	}
}
