using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using CitroidForSlack.Api;
using CitroidForSlack.Extensions;
using CitroidForSlack.Plugins.CitDealer;

namespace CitroidForSlack.Plugins.NazoBrain
{
	/// <summary>
	/// 謎の学習を行い謎の会話を行う謎のBot。
	/// </summary>
    public class MessageBotNazoBrain : IMessageBot
	{
		Dictionary<string, Word> wordBrain;

		public Dictionary<string, Word> WordBrain => wordBrain;

		List<int> lengthBrain;
		NazoBrainConfig config;

		/// <summary>
		/// 記憶の時間単位の寿命。
		/// </summary>
		public static readonly int WORD_LIFE_SPAN = 4;

		public string BrainDump() => JsonConvert.SerializeObject(wordBrain, Formatting.Indented);

		public NazoBrainConfig Config => config;

		public string Name => "NazoBrain";
		public string Version => "1.0.0";
		public string Copyright => "(C)2017 Citrine";

		public string Help =>
			"発言を学習し:thinking_face:、蓄えた語彙を使ってリプライに返信します。:speech_balloon:\n" +
			"Botの設定:gear:次第でリプライ無しでも発言します:muscle:\n" +
			"\n" +
			"この bot には淫夢要素などはありません。";

		void Learn(string text) => Learn(text, DateTime.Now);

		UnicodeBlock prevBlock;

		/// <summary>
		/// 単語を分割する文字を登録します。
		/// </summary>
		public List<string> Divider { get; set; } = new List<string>
		{
			"て", "に", "を", "は", "が", "で", "と", "の", "や", "へ", "も",
			"こ", "そ", "あ", "ど", "、", "。", "，", "．", "！", "？"
		};

		/// <summary>
		/// 文を単語ごとに分割します。
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		string[] Split(string text)
		{
			if (text.Length == 0)
				return new string[0];
			var list = new List<string>();
			var buf = "";
			for (var i = 0; i < text.Length; i++)
			{
				UnicodeBlock block = text[i].GetBlock();
				if (Divider.Contains(text[i].ToString()) ||
					char.IsSeparator(text[i]) || 
					char.IsSymbol(text[i]))
				{
					if (!string.IsNullOrEmpty(buf) && !string.IsNullOrWhiteSpace(buf))
						list.Add(buf);
						list.Add(text[i].ToString());
					buf = "";
					continue;
				}

				if (block != prevBlock)
				{
					if (!string.IsNullOrEmpty(buf) && !string.IsNullOrWhiteSpace(buf))
						list.Add(buf);
					buf = "";
				}

				buf += text[i];
				/*
				now = text[i];
				next = text[i + 1];
				if (!wordBrain.ContainsKey(now))
					wordBrain[now] = new Word(now);
				wordBrain[now].Add(next, timeStamp);*/
				prevBlock = block;
			}
			if (!string.IsNullOrEmpty(buf))
				list.Add(buf);
			return list.ToArray();
		}

        /// <summary>
        /// 文を学習します。
        /// </summary>
        /// <param name="texts"></param>
        /// <param name="timeStamp"></param>
		public void Learn(string texts, DateTime timeStamp)
		{
			if (texts.Length == 0)
				return;
            texts = Regex.Replace(texts, "<[@#].+?>", "");
            // 改行を統一
            texts = texts.Replace("\r\n", "\n").Replace('\r', '\n');
            // 改行で区切って繰り返し学習
            foreach (var text in texts.Split('\n'))
            {
                string[] list = Split(text);
                string now = "", next;
                for (var i = 0; i < list.Length - 1; i++)
                {

                    now = list[i];
                    next = list[i + 1];
                    if (!wordBrain.ContainsKey(now))
                        wordBrain[now] = new Word(now);
                    wordBrain[now].Add(next, timeStamp);
                }
                lengthBrain.Add(text.Length);
            }
		}

		Random r = new Random();

		public void Clear()
		{
			wordBrain.Clear();
		}

		string Say(string text)
		{
			Word trigger = wordBrain.Values.Random();
			string[] list = Split(text);
            foreach (var s in list)
                if (wordBrain.ContainsKey(s))
                {
                    trigger = wordBrain[s];
                }
			if (trigger == null)
				return ":fu:";
			var length = lengthBrain.Count == 0 ? 140 : Math.Max(20, Math.Min(140, lengthBrain.Random()));
			var sb = new StringBuilder();

			//sb.Append(trigger.MyText) 
			Word w = trigger;
			for (var i = 0; i < length - 1;)
			{
				if (w.Candidates.Count == 0)
					break;
				var c = w.Candidates.Random().MyText;
				if (!wordBrain.ContainsKey(c))
				{
					sb.Append(c);
					break;
				}
				w = wordBrain[c];
				sb.Append(w.MyText);
				// 英単語などは空白をあける
				if (w.MyText.Last().GetBlock() == UnicodeBlock.Laten)
					sb.Append(" ");
				i += w.MyText.Length;
			}
			return sb.ToString();
		}

		public bool CanExecute(Message mes) => true;

		private bool isActive = true;
		


		public async Task RunAsync(Message mes, ICitroid citroid)
		{
			foreach (KeyValuePair<string, Word> ws in WordBrain.ToList())
				if ((DateTime.UtcNow - ws.Value.TimeStamp).TotalHours > WORD_LIFE_SPAN)
					WordBrain.Remove(ws.Key);

			//LOG
			var username = (mes.user == null ? mes.username : citroid.GetUser(mes.user));
			Console.WriteLine($"{username}@{mes.channel} : {mes.text}");
			if (Regex.IsMatch(mes.text, $"<@({citroid.Id}|citroid)>"))
			{
                mes.text = mes.text.Replace($"<@{citroid.Id}> ", "").Replace($"<@{citroid.Id}>", "");
				if (mes.text.Contains("ぼんぼやーじゅ") || mes.text.Contains("ばいばい"))
				{
					if (username == "citrine")
					{
						// 反抗期卒業

						//if (r.Next(0) == 0)
						//{
						//	await citroid.PostAsync(mes.channel, $"うるせえ！", userName: "反抗期", iconEmoji: ":anger:");
						//	return;
						//}
						await citroid.PostAsync(mes.channel, $"落ちますﾉｼ");
						await citroid.PostAsync(mes.channel, $"*Citroid が退室しました*");
						isActive = false;
					}
					else
						await citroid.PostAsync(mes.channel, $"<@{username}> は親じゃないなー");
					return;
				}
				else if (mes.text.Contains("かむひーや") || mes.text.Contains("おいで"))
				{
					if (username == "citrine")
					{
						await citroid.PostAsync(mes.channel, $"*Citroid が入室しました*");
						await citroid.PostAsync(mes.channel, $"Yo");
						isActive = true;
					}
					return;
				}
				else if (mes.text.Contains("注意喚起"))
				{
					// 警告防止
					Task<Task> @void = Task.Factory.StartNew(async () =>
					{
						// ゆうさく注意喚起シリーズ
						PostedMessage post = await citroid.PostAsync(mes.channel, ":simple_smile:" + new string(' ', 20) + ":bee:");
						for (var i = 19; i >= 0; i -= 2)
						{
							// ビンにかかるだいたいの時間
							await Task.Delay(150);

							post = await post.UpdateAsync(":simple_smile:" + new string(' ', i) + ":bee:");
						}
						// ビンにかかるだいたいの時間
						await Task.Delay(330);

						post = await post.UpdateAsync(":simple_smile:");
						// チクにかかるだいたいの時間
						await Task.Delay(1000);

						post = await post.UpdateAsync(":scream:");
						// ｱｱｱｱｱｱｱｱｱｱ にかかるだいたいの時間
						await Task.Delay(2000);

						post = await post.UpdateAsync(":upside_down_face:");
						//ｱｰｲｸｯ にかかるだいたいの時間
						await Task.Delay(1000);

						post = await post.UpdateAsync(":skull:");
						//ﾁｰﾝ (ウ　ン　チ　ー　コ　ン　グ) にかかるだいたいの時間
						await Task.Delay(2000);

						//背景が黒くなって3人に増えて音が流れて注意喚起する処理
						post = await post.UpdateAsync(@"スズメバチには気をつけよう！
　　　　  :simple_smile::simple_smile::simple_smile:");
						});
					return;
				}
				await Task.Delay(1000);

				if (isActive)
					await citroid.PostAsync(mes.channel, $"<@{username}> {Say(mes.text)}");

			}
			else if (!config.ReplyOnly && config.PostRate > 0 && r.Next((int)(1 / config.PostRate)) == 0)
			{
                mes.text = mes.text.Replace($"<@{citroid.Id}>", "");
                await Task.Delay(2000);

				if (isActive)
					await citroid.PostAsync(mes.channel, Say(mes.text));
			}
            if (mes.subtype != "bot_message")
                Learn(mes.text, long.Parse(mes.ts.Split('.')[0]).ToDateTime());
		}

		public async Task InitializeAsync(ICitroid citroid)
		{
			if (File.Exists("lengthBrain.json"))
				lengthBrain = JsonConvert.DeserializeObject<List<int>>(File.ReadAllText("lengthBrain.json"));
			if (File.Exists("wordBrain.json"))
				wordBrain = JsonConvert.DeserializeObject<Dictionary<string, Word>>(File.ReadAllText("wordBrain.json"));
			if (File.Exists("NazoBrainConfig.json"))
				config = JsonConvert.DeserializeObject<NazoBrainConfig>(File.ReadAllText("NazoBrainConfig.json"));
			if (lengthBrain == null)
				lengthBrain = new List<int>();
			if (wordBrain == null)
				wordBrain = new Dictionary<string, Word>();
			if (config == null)
				config = new NazoBrainConfig();
			
		}

		public async Task LearnFromSlack(ICitroid citroid)
		{
			JObject list = await citroid.RequestAsync("channels.list");
			//Console.WriteLine(list.ToString());
			foreach (JObject ch in list["channels"].Values<JObject>())
			{
				var id = ch["id"].Value<string>();
				JObject history = await citroid.RequestAsync("channels.history", new NameValueCollection
				{
					{ "channel", id ?? "" }
				});
				foreach (JObject mes in history.GetValue("messages").Values<JObject>())
				{
					var subtype = mes["subtype"]?.Value<string>();
					if (subtype == "bot_message")
						continue;
					//if (mes["user"] != null && citroid.GetUser(mes["user"].Value<string>()) == "citrine")

					Learn(mes["text"].Value<string>(), long.Parse(mes["ts"].Value<string>().Split('.')[0]).ToDateTime());

				}
			}
		}

		public void Exit(ICitroid citroid)
		{
			File.WriteAllText("lengthBrain.json",JsonConvert.SerializeObject(lengthBrain, Formatting.Indented));
			File.WriteAllText("wordBrain.json",JsonConvert.SerializeObject(wordBrain, Formatting.Indented));
			File.WriteAllText("NazoBrainConfig.json",JsonConvert.SerializeObject(config, Formatting.Indented));

		}
	}
}