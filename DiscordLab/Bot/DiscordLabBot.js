// Require the necessary discord.js classes

console.log(process.version)

const { Client, Intents, MessageEmbed } = require('discord.js');
const config = require('./config.json');

const myIntents = new Intents();
myIntents.add(Intents.FLAGS.GUILD_MESSAGES, Intents.FLAGS.GUILDS);

// Create a new client instance
const botClient = new Client({ intents: myIntents });
const net = require('net');
const tcpSocket = net.createServer();
const connection = tcpSocket.listen(config.port, config.address)

var messages = []
var mainSocket;

console.log("Bot starting up");


// #region Bot events
botClient.on("ready", () => {
    console.log("Bot online and listening to port " + config.port)

    setInterval(() => {
        if (messages.length > 0) {
            var DiscordMessage = messages.join('\n');
            messages.length = 0

            if (DiscordMessage.length > 1950)
                DiscordMessage = DiscordMessage.substring(0, 1950);

            botClient.channels.cache.get(config.channel).send(`[${new Date().toLocaleTimeString()}]\n` + DiscordMessage.replace(/.gg\//g, ""));
        }
    }, 2000)

    setInterval(() => {
        var object = { Type: "alive", channel: "RandomPacketToKeepAlive!" }

        if (mainSocket) {
            mainSocket.write(JSON.stringify(object))
        }
    }, 300000)
});

botClient.on("warn", warn => console.warn(warn));
botClient.on("error", error => console.error(error.message));
botClient.on("unhandledRejection", error => console.error(error));
botClient.on('disconnect', () => console.log('Connection to the Discord API has been lost. I will attempt to reconnect momentarily'));
botClient.on('reconnecting', () => console.log('Attempting to reconnect to the Discord API now. Please stand by...'));

tcpSocket.on("connection", (socket) => {
    socket.setEncoding('utf8');

    messages.push(`**++ - - - - - ++ SERVER ONLINE ++ - - - - -++**`
        + "\n```"
        + "\nThe connection between the bot and the server has been established```");

    mainSocket = socket;

    socket.on("data", (data) => {
        //console.log(data);

        var string = data.toString();

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

                    if (botClient.user != null) {
                        if (object.CurrentPlayers === "0/30")
                            botClient.user.setStatus('idle');
                        else
                            botClient.user.setStatus('online');

                        botClient.user.setActivity(`${object.CurrentPlayers}`, { type: 'WATCHING' })
                    }
                }
                else if (object.Type === "plist") {

                    if (object.PlayerNames === "**No online players**") {
                        botClient.channels.cache.get(object.ChannelID).send(`${object.PlayerNames}`);
                        return;
                    }
                    else {
                        var args = object.PlayerNames.split("```");
                        var players = args[1].split(", ");
                        var playerLists = [];
                        var playerListTemp = "";

                        players.forEach(player => {
                            if (playerListTemp.length + player.length > 1023) {
                                playerLists.push(playerListTemp)

                                playerListTemp = player;
                            }
                            else if (playerListTemp.length > 0) {
                                playerListTemp = playerListTemp.concat(`, ${player}`);
                            }
                            else {
                                playerListTemp = player;
                            }
                        })

                        if (playerListTemp.length > 0)
                            playerLists.push(playerListTemp)

                        //steam:/run/700330/args/-connect-51.68.204.237:7790
                        var embed = new MessageEmbed()
                            .setColor('#e63120')
                            .setAuthor('DragonSCP Player list')
                            .setTimestamp()
                            .addFields(
                                { name: args[0].replace("\n", " ") + "Players currently online", value: `Join in at 51.68.204.237:${config.port - 1000}` }
                            );

                        var index = 0;
                        playerLists.forEach(list => {
                            if (index === 0)
                                embed.addField("Player list", list)
                            else {
                                embed.addField("Player list Cont.", list)
                                index++;
                            }
                        })

                        botClient.channels.cache.get(object.ChannelID).send({ embeds: [embed] });
                    }
                }
                else if (object.Type === "cmdmsg") {
                    botClient.channels.cache.get(object.ChannelID).send(`<@${object.StaffID}>\n` + object.CommandMessage)
                }
                else return
            });


        }
        catch (e) {
            console.log(e);
        }
    })

    socket.on("close", () => {
        messages.push(`**++ - - - - - ++ SERVER OFFLINE ++ - - - - -++**`
            + "\n```"
            + "\nThe connection between the bot and the server has been terminated (The server is possibly offline or restarting)```");
        console.log("Socket closed!")
        botClient.user.setPresence({ game: { name: `for server startup`, type: "WATCHING" }, status: 'dnd' });
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

botClient.on('messageCreate', async message => {
    try {
        if (message.author.bot || message.member == null || message.author == null || !message.guild) return
        if (message.mentions.has(botClient.user)) {

            if (message.content.split(" ").length === 1) {
                var object = { Type: "plist", channel: message.channel.id }
                if (mainSocket) {
                    try {

                        console.log("Requesting player list")
                        mainSocket.write(JSON.stringify(object))
                    }
                    catch (e) { message.channel.send(e.message) }
                }
                else {
                    message.channel.send("Bot is not connected to the server");
                }
            }
            else if (message.member.roles.cache.has(config.staffRoleID) || message.member.hasPermission(["MANAGE_ROLES"])) {
                if (!message.channel.permissionsFor(message.guild.roles.everyone).has('VIEW_CHANNEL')) {
                    var object = { Type: "cmd", channel: message.channel.id, Message: message.content, StaffID: message.author.id, Staff: message.member.user.tag }

                    if (mainSocket) {
                        console.log("Sending ban command")
                        mainSocket.write(JSON.stringify(object))
                    }
                }
            }
        }
    }
    catch (e) {
        console.log(e);
    }
})

botClient.login(config.token);