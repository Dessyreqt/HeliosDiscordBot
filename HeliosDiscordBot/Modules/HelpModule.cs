namespace HeliosDiscordBot.Modules
{
    using System.Threading.Tasks;
    using Discord.Commands;

    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        [Summary("Displays help for Helios")]
        public async Task HelpAsync()
        {
            await ReplyAsync(@"Hello! I'm Helios, a bot to help you track upcoming sunrises and sunsets!

In order for me to help you, I'll need to know a few things. First off, I'll need a latitude and logitude. These values should be decimal values, with west of the prime meridian being negative, and south of the equator being negative. For example, if you wanted to know about sunrises and sunsets at the Golden Gate Bridge, use the command `!setlocation 37.8199286 -122.4795565` to set your location. Every time you tell me a new location, I'll forget the old one.

Second, I'll need to know when you want me to notify you. I'll notify you a number of minutes before sunrise or sunset, depending on what you tell me. Use `!notify sunrise 10` to tell me to notify you 10 minutes before sunrise, and use `!notify sunset 30` to tell me to notify you 30 minutes before sunset. I'll only keep track of one notification for sunrise and one for sunset. You can tell me to stop notifying you of sunrise by using `!stopnotify sunrise` and to stop notifying you of sunset by using `!stopnotify sunset`.

Finally, if you want me to convert times to your timezone, I can do that provided you tell me what it is. Please use `!listtimezones` to list all the timezones I know about. Then use `!settimezone (UTC+08:00) Perth` to set your timezone to W. Australia Standard/Daylight Time, as an example.");

            await ReplyAsync(@"In summary:

`!setlocation <latitude> <logitude>` - (Example: `!setlocation 37.8199286 -122.4795565`) - sets your location of interest to the Golden Gate Bridge. If you're interested in getting your location try using https://www.gps-coordinates.net/my-location
`!notify sunrise <minutes>` - (Example: `!notify sunrise 10`) - will start notifying 10 minutes before sunrise
`!notify sunset <minutes>` - (Example: `!notify sunset 30`) - will start notifying 30 minutes before sunset
`!stopnotify sunrise` - will stop notifying about sunrise
`!stopnotify sunset` - will stop notifying about sunset
`!listtimezones` - will list the time zones I know about
`!settimezone <time zone>` - (Example: `!settimezone (UTC+08:00) Perth`) - will set your timezone to W. Australia Standard/Daylight Time");
            await ReplyAsync(@"If you have any questions, comments, or suggestions, please visit my github at https://github.com/Dessyreqt/HeliosDiscordBot. There you will also find more detailed usage instructions, as well as full source and some brief setup instructions. Please submit an issue there if you have a feature request, and bug, or even a question that isn't explained well by the documentation. Thanks!");
        }
    }
}
