﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;

namespace CitroidForSlack
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

		public string BrainDump() => JsonConvert.SerializeObject(wordBrain, Formatting.Indented);

		public NazoBrainConfig Config => config;


		void Learn(string text) => Learn(text, DateTime.Now);

		UnicodeBlock prevBlock;

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
				if (Config.Divider.Contains(text[i].ToString()) ||
					char.IsSeparator(text[i]) || 
					char.IsSymbol(text[i]))
				{
					if (!string.IsNullOrEmpty(buf))
						list.Add(buf);
						list.Add(text[i].ToString());
					buf = "";
					continue;
				}

				if (block != prevBlock)
				{
					if (!string.IsNullOrEmpty(buf))
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
			Word trigger = wordBrain.Values.FirstOrDefault(_ => r.Next(3) == 0);
			string[] list = Split(text);
            foreach (var s in list)
                if (wordBrain.ContainsKey(s))
                {
                    trigger = wordBrain[s];
                    if (r.Next(2) == 0) break;
                }
			if (trigger == null)
				return ":fu:";
			var length = lengthBrain.Count == 0 ? 140 : Math.Max(20, Math.Min(140, lengthBrain[r.Next(lengthBrain.Count)]));
			var sb = new StringBuilder();
			sb.Append(trigger.MyText);
			Word w = trigger;
			for (var i = 0; i < length - 1;)
			{
				if (w.Candidates.Count == 0)
					break;
				var c = w.Candidates[r.Next(w.Candidates.Count)].MyText;
				if (!wordBrain.ContainsKey(c))
				{
					sb.Append(c);
					break;
				}
				w = wordBrain[c];
				sb.Append(w.MyText);
				i += w.MyText.Length;
			}
			return sb.ToString();
		}

		public bool CanExecute(Message mes) => true;

		public async Task RunAsync(Message mes, ICitroid citroid)
		{
            
            //LOG
            var username = (mes.user == null ? mes.username : citroid.GetUser(mes.user));
			Console.WriteLine($"{username}@{mes.channel} : {mes.text}");
			if (Regex.IsMatch(mes.text, $"<@({citroid.Id}|citroid)>"))
			{
                mes.text = mes.text.Replace($"<@{citroid.Id}>", "");
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
						await citroid.PostAsync(mes.channel, $"落ちますﾉｼ！");
						await citroid.PostAsync(mes.channel, $"*Citroid が退室しました*");
						citroid.IsActive = false;
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
						citroid.IsActive = true;
					}
					return;
				}
				await Task.Delay(1000);
				await citroid.PostAsync(mes.channel, $"<@{username}> {Say(mes.text)}");
               
			}
			else if (!config.ReplyOnly && config.PostRate > 0 && r.Next((int)(1 / config.PostRate)) == 0)
			{
                mes.text = mes.text.Replace($"<@{citroid.Id}>", "");
                await Task.Delay(2000);
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