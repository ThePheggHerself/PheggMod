const Discord = require("discord.js");
const client = new Discord.Client();
const net = require('net');
const config = require("./config.json");
const tcpSocket = net.createServer();
const connection = tcpSocket.listen(config.port, config.address)

var messages = []
var mainSocket;

// #region Bot events
client.on("ready", () => {
	console.log("Bot online and listening to port " + config.port)

	setInterval(() => {
		if (messages.length > 0) {
			var DiscordMessage = messages.join('\n');
			messages.length = 0

			client.channels.get(config.channel).send(`[${new Date().toLocaleTimeString()}]\n` + DiscordMessage.replace(/.gg\//g, ""));
		}
	}, 1000)

	setInterval(() => {
		var object = { Type: "alive", channel: "RandomPacketToKeepAlive!" }

		if (mainSocket) {
			console.log("Sending ban command!")
			mainSocket.write(JSON.stringify(object))
		}
	}, 300000)
});

client.on("warn", warn => console.log(warn));
client.on("error", error => console.error(error));
client.on('disconnect', () => console.log('Connection to the Discord API has been lost. I will attempt to reconnect momentarily'));
client.on('reconnecting', () => console.log('Attempting to reconnect to the Discord API now. Please stand by...'));

tcpSocket.on("connection", (socket) => {
	messages.push(`**++ - - - - - ++ SERVER ONLINE ++ - - - - -++**`
		+ "\n```"
		+ "\nThe connection between the bot and the server has been established```");

	mainSocket = socket;

	socket.on("data", (data) => {
		var string = String.fromCharCode.apply(String, data)

		try {
			///You may be thinking why on earth this has been put here, however it's due to the way the system works. 
			///Sometimes the JSON messages get mashed together while sending it, causing the bot to error out.
			///So this solves the issue. With this, forces a split whenever the recieved data recieves "{", splitting the JSON strings and fixing the issue

			var SplitJson = string.replace(/{/gi, '[[{');
			var array = SplitJson.split('[[');
			array.shift();

			array.forEach(json => {
				var object = JSON.parse(json)

				if (object.Type === "msg") {
					if (object.Message) messages.push(object.Message);
				}
				else if (object.Type === "supdate") {
					var status
					if (object.CurrentPlayers === "0/30") status = 'idle'
					else status = 'online'
	
					client.user.setPresence({ game: { name: `${object.CurrentPlayers}`, type: "WATCHING" }, status: status })
				}
				else if (object.Type === "plist") {
	
					client.channels.get(object.ChannelID).send(`${object.PlayerNames}`);
				}
				else if (object.Type === "cmdmsg") {
					client.channels.get(object.ChannelID).send(`<@${object.StaffID}>\n` + object.CommandMessage)
				}
				else return
			});


		}
		catch (e) {
			console.log(e.message + "\n" + string);
			console.log(string)
		}
	})

	socket.on("close", () => {
		messages.push(`**++ - - - - - ++ SERVER OFFLINE ++ - - - - -++**`
			+ "\n```"
			+ "\nThe connection between the bot and the server has been terminated (The server is possibly offline or restarting)```");
		console.log("Socket closed!")
		client.user.setPresence({ game: { name: `for server startup`, type: "WATCHING" }, status: 'dnd' });
	})

	socket.on("error", error => {
		messages.push(`\n**++ - - - - - ++ SERVER OFFLINE ++ - - - - -++**`
			+ "\n```"
			+ "\nThe connection between the bot and the server has errored!"
			+ `\n${error.message}\`\`\``);
		console.log(error)
	});
	socket.on("timeout", timeout => {
		messages.push(`\n**++ - - - - - ++ SERVER OFFLINE ++ - - - - -++**`
			+ "\n```"
			+ "\nThe connection between the bot and the server has timed out!```");
		console.log("Socket closed!")
	});
});

client.on('message', async message => {
	try {
		if (message.author.bot || message.member == null || message.author == null || !message.guild) return

		if (message.mentions.users.first() != undefined && message.mentions.users.first().id === client.user.id) {
			if (message.content.split(" ").length == 1) {
				var object = { Type: "plist", channel: message.channel.id }

				if (mainSocket) {
					try {
						mainSocket.write(JSON.stringify(object))
					}
					catch (e) { message.channel.send(e.message) }
				}
			}
			else if (message.member.roles.find(r => r.id === config.staffRoleID)) {
				var object = { Type: "cmd", channel: message.channel.id, Message: message.content, StaffID: message.author.id, Staff: message.member.user.tag }

				if (mainSocket) {
					mainSocket.write(JSON.stringify(object))
				}
			}
		}
	}
	catch (e) {
		console.log(e);
	}
})

client.login(config.token);