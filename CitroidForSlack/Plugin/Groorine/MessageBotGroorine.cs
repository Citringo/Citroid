using GroorineCore;
using GroorineCore.DataModel;
using GroorineCore.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		enum Pitch
		{
			C, CS, D, DS, E, F, FS, G, GS, A, AS, B
		}

		static Pitch[] PitchTable = { Pitch.A, Pitch.B, Pitch.C, Pitch.D, Pitch.E, Pitch.F, Pitch.G };
		static Pitch[] Pitches = (Pitch[])Enum.GetValues(typeof(Pitch));

		static Pitch ToPitch(char m) => PitchTable[m - 'A'];
		public static MidiFile ParseMml(string mml, string title = null, short resolution = 480)
		{
			bool IsNote(char m) => 'A' <= m && m <= 'G';
			ParseState st = ParseState.Identifier;
			
			// is B#?
			bool isBs = false;
			var ct = new ConductorTrack(new ObservableCollection<MetaEvent>{
				new BeatEvent(4, 4),
				new TempoEvent{ Tempo = 120 }
			}, resolution);
			var tracks = new ObservableCollection<Track>();
			// Default Octave
			var defoct = 4;
			// Octave
			var oct = defoct;
			// Default MML Length
			var deflen = 4;
			// MML Numeric
			var num = -1;
			// MML Identifier
			var identifier = '\0';
			// Program Change
			var pc = 0;
			// Pitch
			var pitch = default(Pitch);
			// MIDI Tick
			var tick = 0;
			Track curTrack = null;
			// MIDI Channel
			byte? channel = null;
			double dlen = 0;
			var ties = new List<(Pitch, double)>();
			
			void AddChannel()
			{
				tracks.Add(curTrack = new Track(new ObservableCollection<MidiEvent>()));
				tick = 0;
				channel++;
			}

			int MmlLengthToMidiTick(int l)
			{
				if (l == 0)
					return 0;
				// ８分音符, 分解能480 => 4 / 8 * 480 => 240
				return (int)(4d / l * resolution);
			}
			// データをMIDI信号の範囲(0～127)に収めます。
			byte SnapToMidiValue(int value) => (byte)(value < 0 ? 0 : value > 127 ? 127 : value);

			void Add(Pitch p, int o, double dl)
			{
				curTrack.Events.Add(new NoteEvent { Channel = channel ?? 0, Velocity = 100, Tick = tick, Note = SnapToMidiValue(+ 12 * oct + (int)pitch), Gate = (int)dl });
				tick += (int)dlen;
			}

			AddChannel();

			var bs = false;
			mml = mml.ToUpper();
			for (var i = 0; i <= mml.Length; i++)
			{
				var m = i >= mml.Length ? '\0' : mml[i];
				if (st == ParseState.Identifier)
				{
					if (IsNote(m))
					{
						identifier = m;
						pitch = ToPitch(m);
						st = ParseState.NumAfterNote;
						continue;
					}
					if (m == 'R')
					{
						identifier = m;
						st = ParseState.NumAfterNote;
						continue;
					}
					if (m == 'V')
					{
						identifier = m;
						st = ParseState.Num;
						continue;
					}
					if (m == 'L')
					{
						identifier = m;
						st = ParseState.Num;
						continue;
					}
					if (m == 'O')
					{
						identifier = m;
						st = ParseState.Num;
						continue;
					}
					if (m == 'T')
					{
						identifier = m;
						st = ParseState.Num;
						continue;
					}
					if (m == '@')
					{
						identifier = m;
						st = ParseState.Num;
						continue;
					}
					if (m == ';' || m  == ':')
					{
						AddChannel();
						continue;
					}
					if (m == '<')
					{
						oct++;
						defoct++;
						continue;
					}
					if (m == '>')
					{
						oct--;
						defoct--;
						continue;
					}
				}
				else if (st == ParseState.Num)
				{
					if (char.IsDigit(m))
					{
						if (num == -1)
							num = 0;
						num *= 10;
						num += m - '0';
						continue;
					}
					else
					{
						if (num == -1)
							num = 0;
						switch (identifier)
						{
							case 'T':
								ct.Events.Add(new TempoEvent { Tempo = num > 0 ? num : 120, Tick = tick });
								break;
							case 'V':
								curTrack.Events.Add(new ControlEvent { Channel = channel ?? 0, Tick = tick, ControlNo = 007, Data = (byte)SnapToMidiValue(num) });
								break;
							case 'L':
								deflen = num;
								break;
							case 'R':
								if (num == 0)
									num = deflen;

								break;
							case 'O':
								oct = defoct = num;
								break;
							case '@':
								pc = num;
								curTrack.Events.Add(new ProgramEvent { Channel = channel ?? 0, Tick = tick, ProgramNo = (byte)pc });
								break;
						}
						num = -1;
						st = ParseState.Identifier;
						i--;
						continue;
					}
				}
				else if (st == ParseState.NumAfterNote)
				{
					if (identifier != 'R')
					{
						if (m == '+' || m == '#')
						{
							if (pitch == Pitch.B)
							{
								pitch = Pitch.C;
								oct++;
							}
							else
							{
								pitch = Pitches[(int)pitch + 1];
							}
							i++;
						}
						else if (m == '-')
						{
							if (pitch == Pitch.C)
							{
								pitch = Pitch.B;
								oct--;
							}
							else
							{
								pitch = Pitches[(int)pitch - 1];
							}
							i++;
						}
					}
					st = ParseState.NumAfterHalfNote;
					i--;
					continue;
				}
				else if (st == ParseState.NumAfterHalfNote)
				{
					if (char.IsDigit(m))
					{
						if (num == -1)
							num = 0;
						num *= 10;
						num += m - '0';
						continue;
					}
					else
					{
						if (num == -1 || num == 0)
							num = deflen;
						dlen = MmlLengthToMidiTick(num);
						num = -1;
						if (m == '.')
							dlen *= 1.5;
						if (identifier != 'R')
						{
							if (m == '&')
							{
								ties.Add((pitch, dlen));
								dlen = -1;
								oct = defoct;
								st = ParseState.Identifier;
								continue;
							}
							if (ties.Count != 0)
							{
								ties.Add((pitch, dlen));
								pitch = ties[0].Item1;
								dlen = 0;
								for (var j = 0; j <= ties.Count; j++)
								{
									if (ties.Count <= j)
									{
										Add(pitch, oct, dlen);
										break;
									}
									(Pitch p, double l) = ties[j];
									dlen += l;
									if (pitch != p)
									{
										Add(pitch, oct, dlen);
										pitch = p;
									}
								}
								dlen = -1;
								oct = defoct;
								st = ParseState.Identifier;
								i--;
								ties.Clear();
								continue;
							}
							Add(pitch, oct, dlen);
						}
						else
							tick += (int)dlen;
						dlen = -1;
						oct = defoct;
						st = ParseState.Identifier;
						i--;
						continue;
					}
				}
			}
			return new MidiFile(ct, tracks, resolution, title ?? "MML Music on Groorine", "", null);
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
			if (File.Exists(path))
			{
				var p = new P();
				try
				{
					MidiFile grobj;
						if (new[] { ".mid", ".midi", ".smf" }.Contains(Path.GetExtension(path)))
							grobj = SmfParser.Parse(new FileStream(path, FileMode.Open, FileAccess.Read));
						else if (Path.GetExtension(path) == ".txt")
						{
							var lines = File.ReadAllText(path).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
							if (lines.Length == 0)
								return;
							if (lines[0].ToUpper() != "#!MML")
								return;
							grobj = ParseMml(string.Join("", lines.Skip(1)));
						}
						else
							return;
						// Groorine でmid => wav
						/*if (!File.Exists(wavpath))
							*/
						await p.SaveAsync(wavpath, 2, grobj);

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
				if (cmd.Contains("reload"))
				{
					
				}
			}
		}

	}
}
