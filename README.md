# HeliosDiscordBot

Helios is a discord bot for notifying you of solar events.

[Add Helios to your server](https://discord.com/api/oauth2/authorize?client_id=827744716132974594&permissions=0&scope=bot), or [Join my server](https://discord.gg/t4kjy4sPUg) to interact with it.

## Using Helios

Helios works entirely through DMs, and will only respond to commands issued via DM instead of in a channel. You still need to share a server with Helios in order to send or receive DMs to/from it, this is a restriction set in place by Discord.

### !help

A brief summary of commands can be found by sending `!help` to Helios.

### !setlocation

In order to calculate sunrise and sunset, a location (latitude and longitude) are required. Use `!setlocation <latitude> <longitude>` to set your location of interest.

You can make this location as general as you'd like for privacy concerns, though of course more precision will lead to better accuracy. As an example, using `!setlocation 37.8199286 -122.4795565` will set your location to the Golden Gate Bridge. On April 7, 2021, sunrise at that location was 6:45:46 AM PDT, while sunset was at 7:38:10 PM PDT. If instead you want to use `!setlocation 38 -122` as a means of obfuscating your location, Helios will tell you sunrise is at 6:43:41 AM and sunset at 7:36:25 PM, which may not be a big deal if you're looking to be notified an hour before sunset or sunrise.

Alternatively, you may want to set your location to the local city hall or some other public landmark. Setting your location to San Francisco City Hall would return a sunrise of 6:45:32 AM and a sunset of 7:37:53 PM, a difference of approximately 15 seconds as opposed to the other method which is off by a couple of minutes.

### !listtimezones and !settimezone

By default, Helios will give you times in UTC, so you'll need to tell it what your timezone is. First you'll want to use `!listtimezones` to list all the time zones Helios knows about. This is because you'll want to exactly copy the timezone that corresponds to where you normally keep track of time. 

Using the above example, if you lived at or near the Golden Gate bridge, you'll probably want to get your time as it is in the Pacific Time Zone, so after using `!listtimezones` you'll find `(UTC-08:00) Pacific Time (US & Canada)` as an entry in the list. Use `!settimezone <time zone>` to set your timezone, in this case `!settimezone (UTC-08:00) Pacific Time (US & Canada)`. Helios will tell you your current local time as a check to make sure the time zone is what you intended. This time will automatically update for daylight savings throughout the year.

### !sunrise and !sunset

Once you set your location you can use `!sunrise` and `!sunset` to get the times for the sunrise and sunset for your location.

### !notify sunrise/sunset

Once you set your location, you can use `!notify sunrise <minutes>` and `!notify sunset <minutes>` to have Helios notify you prior to each sunrise or sunset.

As an example, I have mine set to `!notify sunset 60` to let me know an hour before sunset to go our for a walk, and `!notify sunrise 510` to let me know 8.5 hours before sunrise to go to bed. 

### !stopnotify sunrise/sunset

You can use `!stopnotify sunrise` and `!stopnotify sunset` to stop being notified of those events.

## Running Helios Locally

If you want to build Helios for yourself and run it on your local machine, you'll need to set up a Discord bot token and set it using the dotnet user-secrets utility. You can get a Discord bot token here: [https://discord.com/developers/applications](https://discord.com/developers/applications). You'll need to create a new application, then add a bot to that application. You can then copy the token from the bot page.

From the csproj folder:

`dotnet user-secrets set "DiscordSettings:Token" "<Token>"`

Alternatively you can set your local environment variables to hold the token. Set the variable `HeliosDiscordBot_DiscordSettings__Token` to the value of the token.

You can run the service directly from Visual Studio. When you run it, you should see some logging information in the console and then your bot account start up.

You can publish Helios to your local machine by using `nuke Publish` from the command line. This will by default publish the application to `C:\Workers\HeliosDiscordBot` where you can edit the `Init.ps1` script to include your Discord bot token and then run it to install the service, set the environment variable, and start the service.
