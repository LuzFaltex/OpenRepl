using Discord;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenRepl.Infrastructure.Logging
{
    public class DiscordWebhookLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _token;
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly ulong _id;
        private static readonly HttpClient _httpCleint = new HttpClient();

        public DiscordWebhookLogger(string categoryName, ulong id, string token, Func<string, LogLevel, bool> filter)
        {
            _categoryName = categoryName;
            _token = token;
            _filter = filter;
            _id = id;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if(!_filter(_categoryName, logLevel))
            {
                return;
            }

            var webhookClient = new Discord.Webhook.DiscordWebhookClient(_id, _token);

            var message = new EmbedBuilder()
                .WithAuthor("DiscordLogger")
                .WithTitle(_categoryName)
                .WithTimestamp(DateTimeOffset.UtcNow)
                .WithColor(Color.Red)
                .AddField(new EmbedFieldBuilder()
                    .WithIsInline(false)
                    .WithName($"LogLevel: {logLevel}")
                    .WithValue(Format.Code($"{formatter(state, exception)}\n{exception?.ToString()}".TruncateTo(1010))));

            webhookClient.SendMessageAsync(string.Empty, embeds: new[] { message.Build() }, username: "OpenRepl Logger");
        }
    }
}
