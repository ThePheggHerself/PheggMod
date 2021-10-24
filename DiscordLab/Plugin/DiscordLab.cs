using System.Collections.Generic;
using System.Linq;
using System.Text;
using PheggMod.API.Plugin;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using GameCore;
using System.Threading;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json.Linq;
using MEC;

namespace DiscordLab
{
	[PluginDetails(
		author = "ThePheggHerself",
		name = "DiscordLab",
		description = "Basic logging bot for SCP: Secret Laboratory",
		version = "1.0"
	)]

	public class DiscordLab : Plugin
	{
		public static Bot bot;
		public override void initializePlugin()
		{
			bot = new Bot();

			this.AddEventHandlers(new Events());

			Info("DiscordLab loaded!!");
		}
	}

	public class Bot
	{
		private static Regex _rgx = new Regex("(.gg/)|(<@)|(http)|(www)|({)|(})|(<)|(>)|(\")|(\\[)|(\\])");
		private static Regex _filterNames = new Regex("(\\*)|(_)|({)|(})|(@)|(<)|(>)|(\")|(\\[)|(\\])");

		private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		private IPAddress _ipAddress;
		private DateTime _lastSentMessage;
		internal static char[] validUnits = { 'm', 'h', 'd', 'w', 'M', 'y' };

		public enum messageType
		{
			MSG = 0,
			CMD = 1,
			PLIST = 2,
			SUPDATE = 3,
			KEEPALIVE = 4
		}

		private class msgMessage
		{
			public string Type = "msg";
			public string Message;
		}
		private class cmdMessage
		{
			public string Type = "cmdmsg";
			public string CommandMessage;
			public string ChannelID;
			public string StaffID;
		}
		private class plistMessage
		{
			public string Type = "plist";
			public string PlayerNames;
			public string ChannelID;
		}
		private class supdateMessage
		{
			public string Type = "supdate";
			public string CurrentPlayers = PlayerManager.players.Count() + "/" + ConfigFile.ServerConfig.GetInt("max_players", 20);
			internal DateTime LastUpdate = DateTime.Now;
		}
		private class keepaliveMessage
		{
			public string Type = "keepalive";
		}

		public class botmessage
		{
			//Types: plist, command

			public string Type;
			public string channel;
			public string Message = null;
			public string StaffID = null;
			public string Staff = null;
		}

		private supdateMessage _lastSupdateMessage = null;

		public Bot()
		{
			try
			{
				_ipAddress = IPAddress.Parse(ConfigFile.ServerConfig.GetString("dl_address", "127.0.0.1"));

				Timing.RunCoroutine(StatusUpdate());
				Timing.RunCoroutine(KeepAlive());
				new Thread(() =>
				{
					BotListener();
				}).Start();
			}
			catch (Exception ex) { Plugin.Error(ex.ToString()); }
		}

		private IEnumerator<float> StatusUpdate()
		{
			supdateMessage message = new supdateMessage();

			if (_lastSupdateMessage == null || _lastSupdateMessage.CurrentPlayers != message.CurrentPlayers || (DateTime.Now - _lastSupdateMessage.LastUpdate).Seconds > 30)
			{
				SendMessage(JsonConvert.SerializeObject(message));
				_lastSupdateMessage = message;
			}

			yield return Timing.WaitForSeconds(6f);

			Timing.RunCoroutine(StatusUpdate());
		}
		private IEnumerator<float> KeepAlive()
		{
			if ((DateTime.Now - _lastSentMessage).TotalMinutes > 30)
				NewMessage("RandomStringToKeepAlive", messageType.KEEPALIVE);

			yield return Timing.WaitForSeconds(1800f);

			Timing.RunCoroutine(KeepAlive());
		}

		public void NewMessage(string message, messageType type = messageType.MSG, JObject jObj = null)
		{
			if (string.IsNullOrEmpty(message)) return;

			message.Replace("{", string.Empty).Replace("}", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty);

			string json;

			if (type == messageType.MSG)
			{
				msgMessage msg = new msgMessage()
				{
					Message = _rgx.Replace(message, string.Empty)
				};

				json = JsonConvert.SerializeObject(msg);
			}
			else if (type == messageType.KEEPALIVE)
			{
				keepaliveMessage msg = new keepaliveMessage();

				json = JsonConvert.SerializeObject(msg);
			}
			else if (type == messageType.PLIST)
			{
				plistMessage msg = new plistMessage
				{
					ChannelID = jObj["channel"].ToString(),
					PlayerNames = message
				};

				json = JsonConvert.SerializeObject(msg);
			}
			else if (type == messageType.CMD)
			{
				cmdMessage msg = new cmdMessage
				{
					ChannelID = jObj["channel"].ToString(),
					StaffID = jObj["StaffID"].ToString(),
					CommandMessage = message
				};

				json = JsonConvert.SerializeObject(msg);
			}
			else if (type == messageType.SUPDATE)
			{
				supdateMessage msg = new supdateMessage();

				json = JsonConvert.SerializeObject(msg);
			}
			else
			{
				Plugin.Warn("Invalid messageType given!");
				return;
			}

			SendMessage(json);
		}
		private void SendMessage(string json)
		{
			if (!_socket.Connected)
				OpenConnection();

			if (!_socket.Connected) return;

			try
			{
				_socket.Send(Encoding.UTF8.GetBytes(json));
				_lastSentMessage = DateTime.Now;
			}
			catch (Exception e)
			{
				Plugin.Error(e.InnerException.Message + "\n" + e.InnerException.StackTrace);
			}
		}

		private void OpenConnection()
		{
			int port = ServerConsole.Port + 1000;

			if (port == 1000) return;
			try
			{
				_socket.Connect(_ipAddress, port);
				Plugin.Info("Bot connection established");
			}
			catch (Exception)
			{
				try
				{
					_socket.Disconnect(false);

					_socket.Connect(_ipAddress, port);
					Plugin.Info("Bot connection established");
				}
				catch (Exception e)
				{
					Plugin.Error($"{e.InnerException.Message}\n{e.InnerException.StackTrace}");
				}
			}
		}

		private void BotListener()
		{
			while (2 > 1)
			{
				if (_socket.Connected)
				{
					byte[] data = new byte[8192];
					int dataLength = _socket.Receive(data);

					string incomingData = Encoding.UTF8.GetString(data, 0, dataLength);
					List<string> messages = new List<string>(incomingData.Split('\n'));

					foreach (string message in messages)
					{
						if (!string.IsNullOrEmpty(message))
						{
							JObject jObj = JsonConvert.DeserializeObject<JObject>(message);

							if (jObj["Type"].ToString().ToLower() == "plist")
								NewMessage(Playerlist(), messageType.PLIST, jObj);
							else if (jObj["Type"].ToString().ToLower() == "cmd")
							{
								string msg = HandleCommand(jObj);
								if (!string.IsNullOrEmpty(msg))
									NewMessage(msg, messageType.CMD, jObj);
							}


						}
					}
				}
				Thread.Sleep(50);
			}
		}


		private string Playerlist()
		{
			if (PlayerManager.players.Count < 1)
			{
				if ((DateTime.Now - Events.RoundEnded).TotalSeconds < 45)
					return "*The round has recently restarted, so the playercount may not be accurate*\n**No online players**";
				else
					return "**No online players**";
			}

			List<string> players = new List<string>();
			foreach (var hub in ReferenceHub.GetAllHubs().Values)
			{
				if (string.Equals(hub.nicknameSync.MyNick, "Dedicated Server", StringComparison.OrdinalIgnoreCase))
					continue;
				else if (!hub.serverRoles.DoNotTrack)
					players.Add(_filterNames.Replace(hub.nicknameSync.MyNick, string.Empty));
				else players.Add("DNTUser");
			}

			return $"**{PlayerManager.players.Count()}/{ConfigFile.ServerConfig.GetInt("max_players", 20)}**\n```\n{string.Join(", ", players)}```";
		}


		#region Commands

		private string HandleCommand(JObject jObj)
		{
			try
			{
				string[] command = jObj["Message"].ToString().Split(' ').Skip(1).ToArray();

				switch (command[0].ToLower())
				{
					case "ban":
					case "rban":
					case "remoteban":
						return BanCommand(command, jObj);
					case "kick":
					case "rkick":
					case "remotekick":
						return KickCommand(command, jObj);
					case "unban":
					case "runban":
					case "remoteunban":
						return UnbanCommand(command, jObj);
					case "hp":
					case "drop":
					case "bc":
					case "pbc":
					case "forcestart":
					case "roundlock":
					case "lobbylock":
					case "heal":
					case "effect":
					case "overwatch":
					case "mute":
					case "imute":
					case "unmute":
					case "iunmute":
					case "intercom-timeout":
					case "give":
					case "request_data":
					case "roundtime":
						try
						{
							var dlcp = new DiscordLabCommandSender();
							dlcp.jObject = jObj;

							GameCore.Console.singleton.TypeCommand($"/{string.Join(" ", command.Skip(0))}", dlcp);

							return string.Empty;
						}
						catch (Exception e)
						{
							return e.ToString();
						}
					default:
						return "```diff\n - Unknown Command```";

				}
			}
			catch (Exception e)
			{
				return e.ToString();
			}
		}

		private string BanCommand(string[] arg, JObject jObject)
		{
			try
			{
				string command = arg[0];
				string searchvariable = string.Empty;
				TimeSpan duration;
				string durationString = string.Empty;
				string reason = string.Empty;
				ReferenceHub player = null;

				if (arg.Count() < 4 || arg[1].Length < 1)
					return $"```{arg[0]} [UserID/IP] [Duration] [Reason]```";

				//Forces the search variable to the front of the array
				arg = arg.Skip(1).ToArray();

				if (arg[0].StartsWith("'") || arg[0].StartsWith("\""))
				{
					string result = string.Join(" ", arg).Split(new string[] { "\"" }, 3, StringSplitOptions.None)[1];

					searchvariable = result;

					arg = string.Join(" ", arg).Replace("\"" + result + "\"", string.Empty).Trim(' ').Split(' ');
				}
				else
				{
					searchvariable = arg[0];

					arg = arg.Skip(1).ToArray();
				}

				durationString = arg[0];
				var chars = durationString.Where(Char.IsLetter).ToArray();
				if (chars.Length < 1 || !int.TryParse(new string(durationString.Where(Char.IsDigit).ToArray()), out int amount) || !validUnits.Contains(chars[0]) || amount < 1)
					return "```diff\n- Invalid duration provided```";

				if (!GetPlayer(searchvariable, out player))
					return $"Unable to find player `{searchvariable.Replace("`", "\\`")}`";

				duration = GetBanDuration(chars[0], amount);
				arg = arg.Skip(1).ToArray();
				reason = string.Join(" ", arg);

				if (player != null)
				{

					BanHandler.IssueBan(new BanDetails
					{
						OriginalName = player.nicknameSync.MyNick,
						Id = player.characterClassManager.UserId,
						Issuer = jObject["Staff"].ToString(),
						IssuanceTime = DateTime.UtcNow.Ticks,
						Expires = DateTime.UtcNow.Add(duration).Ticks,
						Reason = reason
					}, BanHandler.BanType.UserId);

					BanHandler.IssueBan(new BanDetails
					{
						OriginalName = player.nicknameSync.MyNick,
						Id = player.characterClassManager.connectionToClient.address,
						Issuer = jObject["Staff"].ToString(),
						IssuanceTime = DateTime.UtcNow.Ticks,
						Expires = DateTime.UtcNow.Add(duration).Ticks,
						Reason = reason
					}, BanHandler.BanType.IP);

					ServerConsole.Disconnect(player.gameObject, $"You have been banned by the server staff\nReason: " + reason);

					return $"`{player.nicknameSync.MyNick} ({player.characterClassManager.UserId})` was banned for {durationString} with reason: {reason}";
				}
				else
				{
					BanHandler.IssueBan(new BanDetails
					{
						OriginalName = "Offline player",
						Id = searchvariable,
						Issuer = jObject["Staff"].ToString(),
						IssuanceTime = DateTime.UtcNow.Ticks,
						Expires = DateTime.UtcNow.Add(duration).Ticks,
						Reason = reason
					}, searchvariable.Contains('@') ? BanHandler.BanType.UserId : BanHandler.BanType.IP);
					return $"`{searchvariable}` was banned for {durationString} with reason: {reason}!";
				}
			}
			catch (Exception e)
			{
				return e.ToString();
			}
		}
		private TimeSpan GetBanDuration(char unit, int amount)
		{
			switch (unit)
			{
				default:
					return new TimeSpan(0, 0, amount, 0);
				case 'h':
					return new TimeSpan(0, amount, 0, 0);
				case 'd':
					return new TimeSpan(amount, 0, 0, 0);
				case 'w':
					return new TimeSpan(7 * amount, 0, 0, 0);
				case 'M':
					return new TimeSpan(30 * amount, 0, 0, 0);
				case 'y':
					return new TimeSpan(365 * amount, 0, 0, 0);
			}
		}

		private string KickCommand(string[] arg, JObject jObject)
		{
			if (arg.Count() < 3 || arg[1].Length < 1)
				return $"```{arg[0]} [UserID/IP] [Reason]```";

			string searchvariable = string.Empty;

			//Forces the search variable to the front of the array
			arg = arg.Skip(1).ToArray();

			if (arg[0].StartsWith("'") || arg[0].StartsWith("\""))
			{
				string result = string.Join(" ", arg).Split(new string[] { "\"" }, 3, StringSplitOptions.None)[1];

				searchvariable = result;

				arg = string.Join(" ", arg).Replace("\"" + result + "\"", string.Empty).Trim(' ').Split(' ');
			}
			else
			{
				searchvariable = arg[0];

				arg = arg.Skip(1).ToArray();
			}

			if (!GetPlayer(searchvariable, out ReferenceHub player))
				return $"Unable to find player `{searchvariable.Replace("`", "\\`")}`";

			string reason = string.Join(" ", arg);

			ServerConsole.Disconnect(player.gameObject, $"You have been kicked by the server staff\nReason: " + reason);

			return $"`{player.nicknameSync.MyNick} ({player.characterClassManager.UserId})` was kicked with reason: {reason}";
		}

		private string UnbanCommand(string[] arg, JObject jObject)
		{
			if (arg.Count() < 2) return $"```{arg[0]} [UserID/Ip]```";

			bool validUID = arg[2].Contains('@');
			bool validIP = IPAddress.TryParse(arg[1], out IPAddress ip);

			BanDetails details;

			if (!validIP && !validUID)
				return $"```diff\n- Invalid UserID or IP given```";

			if (validUID)
				details = BanHandler.QueryBan(arg[1], null).Key;
			else
				details = BanHandler.QueryBan(null, arg[1]).Value;

			if (details == null)
				return $"No ban found for `{arg[1]}`.\nMake sure you have typed it correctly, and that it has the @domain prefix if it's a UserID";

			BanHandler.RemoveBan(arg[1], (validUID ? BanHandler.BanType.UserId : BanHandler.BanType.IP));

			return $"`{arg[1]}` has been unbanned.";
		}

		private bool GetPlayer(string SearchParameter, out ReferenceHub Player)
		{
			Player = null;

			KeyValuePair<GameObject, ReferenceHub>[] playerList;

			if (SearchParameter.Contains('@'))
			{
				playerList = ReferenceHub.GetAllHubs().Where(p => p.Value.characterClassManager.UserId == SearchParameter).ToArray();

				if (playerList.Any())
				{
					Player = playerList[0].Value;
					return true;
				}

				else return true;

			}
			else if (IPAddress.TryParse(SearchParameter, out IPAddress IP))
			{
				playerList = ReferenceHub.GetAllHubs().Where(p => p.Value.characterClassManager.connectionToClient.address == SearchParameter).ToArray();

				if (playerList.Any())
				{
					Player = playerList[0].Value;
					return true;
				}

				else return true;
			}

			playerList = ReferenceHub.GetAllHubs().Where(p => p.Value.nicknameSync.MyNick.ToLowerInvariant() == SearchParameter.ToLowerInvariant()).ToArray();

			if (playerList.Any())
			{
				Player = playerList[0].Value;
				return true;
			}


			return false;

		}

		internal class DiscordLabCommandSender : CommandSender
		{
			public JObject jObject;
			public override string SenderId => "Discord";

			public override string Nickname => "Discord";

			public override ulong Permissions => ServerStatic.GetPermissionsHandler().FullPerm;

			public override byte KickPower => 255;

			public override bool FullPermissions => true;

			public override void Print(string text)
			{
				DiscordLab.bot.NewMessage($"```{text}```", messageType.CMD, jObject);
			}

			public override void RaReply(string text, bool success, bool logToConsole, string overrideDisplay)
			{
				if (!text.Contains('#'))
					return;

				string[] commandText = text.Split('#');

				if (!success)
					DiscordLab.bot.NewMessage($"```diff\n- {commandText[0]} # {commandText[1]}```", messageType.CMD, jObject);

				else
					DiscordLab.bot.NewMessage($"```{commandText[0]}{(!string.IsNullOrEmpty(commandText[1]) ? $"\n{commandText[1]}" : string.Empty)}```", messageType.CMD, jObject);
			}

		}

		#endregion
	}
}
