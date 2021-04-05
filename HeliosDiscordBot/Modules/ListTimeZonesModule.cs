namespace HeliosDiscordBot.Modules
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Discord.Commands;

    public class ListTimeZonesModule : ModuleBase<SocketCommandContext>
    {
        [Command("listtimezones")]
        [Summary("Lists the time zones Helios knows about")]
        public async Task ListTimeZonesAsync()
        {
            await ReplyAsync("Here are the time zones I know about:");

            var sb = new StringBuilder();
            var zones = TimeZoneInfo.GetSystemTimeZones();  
            foreach (TimeZoneInfo zone in zones)  
            {
                sb.AppendLine(zone.DisplayName);
                if (sb.Length > 1500)
                {
                    await ReplyAsync($"```{sb}```");
                    sb.Clear();
                }
            }

            await ReplyAsync("Be sure to use the time zone name exactly as it appears!");
        }
    }
}
