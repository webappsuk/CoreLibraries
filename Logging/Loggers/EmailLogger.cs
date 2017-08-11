#region © Copyright Web Applications (UK) Ltd, 2016.  All rights reserved.
// Copyright (c) 2016, Web Applications UK Ltd
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Web Applications UK Ltd nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL WEB APPLICATIONS UK LTD BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Formatting;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Logging.Loggers
{
    /// <summary>
    /// Sents logs via email.
    /// </summary>
    /// <seealso cref="WebApplications.Utilities.Logging.Loggers.LoggerBase" />
    public class EmailLogger : LoggerBase
    {
        /// <summary>
        /// The default timeout of 5 seconds.
        /// </summary>
        private const int DefaultTimeout = 5000;

        /// <summary>
        /// The default buffer duration.
        /// </summary>
        public Duration DefaultBufferDuration = TimeHelpers.OneSecond;

        /// <summary>
        /// The format tag for the logger name.
        /// </summary>
        public const string FormatTagName = "name";

        /// <summary>
        /// The format tag for the more logs indicator.
        /// </summary>
        public const string FormatTagMore = "more";

        /// <summary>
        /// The format tag for the more logs count.
        /// </summary>
        public const string FormatTagMoreCount = "count";

        /// <summary>
        /// The default email subject.
        /// </summary>
        [NotNull]
        public static readonly FormatBuilder DefaultSubject = new FormatBuilder()
            .AppendFormat("{Application:{value}}: {Level:{value}} - {Message:{value}}{More: (+{count} more)}")
            .MakeReadOnly();

        /// <summary>
        /// The default HTML email body.
        /// </summary>
        [NotNull]
        public static readonly FormatBuilder DefaultBodyHtml = new FormatBuilder()
            .AppendLine(@"<style>
body { font-family: 'Segoe UI', sans-serif }
table { border-spacing: 0; border-collapse: collapse }
tr:first-child, td:first-child { font-weight: bold }
td { vertical-align: top; border-left: 1px solid #ccc; padding: 0 6px; }
.Debugging { color: silver }
.Information { color: gray }
.Warning { color: orange } 
.Error { color: red } 
.Critical { color: purple } 
.Emergency { color: fuchsia }
</style>")
            .AppendFormatLine("{Logs:{<items>:{<item>:{html}}}{<join>:\r\n<hr/>\r\n}}")
            .AppendLine("<hr/><p><span style=\"font-style:italic;font-size:0.6em\">This email was automatically generated.</span><p>")
            .MakeReadOnly();

        /// <summary>
        /// Regex for line break characters.
        /// </summary>
        [NotNull]
        private static readonly Regex _newLineRegex = new Regex(@"\r\n|\r|\n", RegexOptions.Compiled);

        /// <summary>
        /// Gets the address to send the email from.
        /// </summary>
        /// <value>
        /// From address.
        /// </value>
        [CanBeNull]
        public MailAddress FromAddress { get; }

        /// <summary>
        /// Gets the address of the sender of the email.
        /// </summary>
        /// <remarks>If specified, the email will appear as 
        /// "From: <c><see cref="SenderAddress">Sender</see></c> on behalf of <c><see cref="FromAddress">From</see></c>"
        /// or similar, depending on the email client.</remarks>
        /// <value>
        /// From address.
        /// </value>
        [CanBeNull]
        public MailAddress SenderAddress { get; }

        /// <summary>
        /// Gets the email addresses to send the logs to.
        /// </summary>
        /// <value>
        /// To addresses.
        /// </value>
        [CanBeNull]
        public IReadOnlyCollection<MailAddress> ToAddresses { get; }

        /// <summary>
        /// Gets the email addresses to CC the logs to.
        /// </summary>
        /// <value>
        /// CC addresses.
        /// </value>
        [CanBeNull]
        public IReadOnlyCollection<MailAddress> CCAddresses { get; }

        /// <summary>
        /// Gets the email addresses to BCC the logs to.
        /// </summary>
        /// <value>
        /// BCC addresses.
        /// </value>
        [CanBeNull]
        public IReadOnlyCollection<MailAddress> BccAddresses { get; }

        /// <summary>
        /// Gets the email subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        [NotNull]
        public FormatBuilder Subject { get; }

        /// <summary>
        /// Gets the HTML email body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        [NotNull]
        public FormatBuilder BodyHtml { get; }

        /// <summary>
        /// Gets the plain text email body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        [CanBeNull]
        public FormatBuilder BodyPlainText { get; }

        [CanBeNull]
        private SmtpClient _smtpClient;

        /// <summary>
        /// Gets the SMTP client.
        /// </summary>
        /// <value>
        /// The SMTP client.
        /// </value>
        [CanBeNull]
        public SmtpClient SmtpClient => _smtpClient;

        /// <summary>
        /// The buffered send action.
        /// </summary>
        [CanBeNull]
        private BufferedAction<Log> _bufferedSend;

        [NotNull]
        private readonly AsyncLock _sendLock = new AsyncLock();

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerBase" /> class.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="fromAddress">The from address.</param>
        /// <param name="senderAddress">The sender address.</param>
        /// <param name="toAddresses">The to addresses.</param>
        /// <param name="ccAddresses">The CC addresses.</param>
        /// <param name="bccAddresses">The BCC addresses.</param>
        /// <param name="subject">The email subject format.</param>
        /// <param name="bodyHtml">The HTML body format.</param>
        /// <param name="bodyPlainText">The plain text body format.</param>
        /// <param name="bufferCount">The maximum number of logs to send in a single email.</param>
        /// <param name="bufferDuration">The duration of time to buffer logs to send in a single email.</param>
        /// <param name="host">The SMTP host.</param>
        /// <param name="port">The SMTP port.</param>
        /// <param name="timeout">The SMTP timeout.</param>
        /// <param name="ssl">if set to <see langword="true" /> SSL will be used.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        /// <param name="deliveryFormat">The delivery format.</param>
        /// <param name="username">The network username.</param>
        /// <param name="password">The network password.</param>
        /// <param name="networkDomain">The network domain.</param>
        /// <param name="validLevels">The valid levels.</param>
        /// <remarks>The SMTP connection settings will be pulled from the <c>&lt;system.net&gt;/&lt;mailSettings&gt;/&lt;smtp&gt;</c> 
        /// configuration section if the parameters are left as their default values. 
        /// (See https://msdn.microsoft.com/en-us/library/ms164240(v=vs.110).aspx for details)</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException">At least one to/CC/BCC address must be given.
        /// or
        /// The bufferCount must be greater than zero.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is <see langword="null" /></exception>
        /// <exception cref="ArgumentOutOfRangeException"><para>The <paramref name="bufferCount" /> was less than or equal to zero.</para>
        /// <para>-or-</para>
        /// <para>At least one to/CC/BCC address was not given.</para></exception>
        public EmailLogger(
            [NotNull] string name,
            [CanBeNull] MailAddress fromAddress = null,
            [CanBeNull] MailAddress senderAddress = null,
            [CanBeNull] MailAddressCollection toAddresses = null,
            [CanBeNull] MailAddressCollection ccAddresses = null,
            [CanBeNull] MailAddressCollection bccAddresses = null,
            [CanBeNull] FormatBuilder subject = null,
            [CanBeNull] FormatBuilder bodyHtml = null,
            [CanBeNull] FormatBuilder bodyPlainText = null,
            ushort bufferCount = 10,
            Duration? bufferDuration = null,
            [CanBeNull] string host = null,
            ushort port = 0,
            [CanBeNull] Duration? timeout = null,
            [CanBeNull] bool? ssl = null,
            [CanBeNull] SmtpDeliveryMethod? deliveryMethod = null,
            [CanBeNull] SmtpDeliveryFormat? deliveryFormat = null,
            [CanBeNull] string username = null,
            [CanBeNull] SecureString password = null,
            [CanBeNull] string networkDomain = null,
            LoggingLevels validLevels = LoggingLevels.All)
            : base(name, true, validLevels)
        {
            if ((toAddresses == null || toAddresses.Count < 1) &&
                (ccAddresses == null || ccAddresses.Count < 1) &&
                (bccAddresses == null || bccAddresses.Count < 1))
                throw new ArgumentOutOfRangeException(
                    nameof(toAddresses),
                    Resources.EmailLogger_ToAddressMissing);
            if (bufferCount < 1 && bufferDuration == TimeHelpers.InfiniteDuration)
                throw new ArgumentOutOfRangeException(
                    nameof(bufferCount),
                    bufferCount,
                    Resources.EmailLogger_InvalidBufferCount);

            FromAddress = fromAddress;
            SenderAddress = senderAddress;
            ToAddresses = toAddresses;
            CCAddresses = ccAddresses;
            BccAddresses = bccAddresses;
            Subject = subject ?? DefaultSubject;
            BodyHtml = bodyHtml ?? (bodyPlainText == null ? DefaultBodyHtml : null);
            BodyPlainText = bodyPlainText;

            _smtpClient = new SmtpClient(host, port);
            _smtpClient.Timeout = (int?)timeout?.TotalMilliseconds() ?? DefaultTimeout;
            if (ssl.HasValue) _smtpClient.EnableSsl = ssl.Value;
            if (deliveryMethod.HasValue) _smtpClient.DeliveryMethod = deliveryMethod.Value;
            if (deliveryFormat.HasValue) _smtpClient.DeliveryFormat = deliveryFormat.Value;
            if (username != null && password != null)
                _smtpClient.Credentials = new NetworkCredential(username, password, networkDomain ?? string.Empty);

            _bufferedSend = new BufferedAction<Log>(
                SendLogs,
                bufferDuration ?? DefaultBufferDuration,
                bufferCount);
        }

        /// <summary>
        /// Adds the specified logs to storage in batches.
        /// </summary>
        /// <param name="logs">The logs to add to storage.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="ObjectDisposedException">The logger has been disposed.</exception>
        public override Task Add(IEnumerable<Log> logs, CancellationToken token = new CancellationToken())
        {
            BufferedAction<Log> bufferedAction = _bufferedSend;
            if (bufferedAction == null) throw new ObjectDisposedException(nameof(EmailLogger));

            foreach (Log log in logs)
                bufferedAction.Run(log);

            return TaskResult.Completed;
        }

        /// <summary>
        /// Sends the logs.
        /// </summary>
        /// <param name="logs">The logs.</param>
        private async void SendLogs([NotNull] IEnumerable<Log> logs)
        {
            try
            {
                Log[] timeOrdered = logs
                    .OrderBy(l => l.TimeStamp)
                    .ToArray();
                Log[] levelOrdered = timeOrdered
                    .OrderByDescending(l => l.Level)
                    .ToArray();

                string subject = _newLineRegex.Replace(Subject.ToString(GetSubjectResolver(levelOrdered)), m => " ");
                string bodyHtml = BodyHtml.ToString(GetBodyResolver(timeOrdered, true));
                string bodyText = BodyPlainText?.ToString(GetBodyResolver(timeOrdered, false));

                MailMessage message = new MailMessage
                {
                    Subject = subject,
                    Body = bodyHtml,
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8
                };
                if (SenderAddress != null) message.Sender = SenderAddress;
                if (FromAddress != null) message.From = FromAddress;
                if (ToAddresses != null) message.To.AddRange(ToAddresses);
                if (CCAddresses != null) message.CC.AddRange(CCAddresses);
                if (BccAddresses != null) message.Bcc.AddRange(BccAddresses);
                if (bodyText != null)
                    message.AlternateViews.Add(
                        AlternateView.CreateAlternateViewFromString(bodyText, Encoding.UTF8, "text/plain"));

                SmtpClient smtpClient = _smtpClient;
                if (smtpClient == null) return;
                using (await _sendLock.LockAsync().ConfigureAwait(false))
                {
                    smtpClient = _smtpClient;
                    if (smtpClient == null) return;

                    // ReSharper disable once PossibleNullReferenceException
                    await smtpClient.SendMailAsync(message).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Log.Add(e, LoggingLevel.Error, () => "An error occured when sending a log email.");
            }
        }

        /// <summary>
        /// Force a flush of this logger.
        /// </summary>
        public override Task Flush(CancellationToken token = new CancellationToken())
        {
            _bufferedSend.Flush();
            return TaskResult.Completed;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public override void Dispose()
        {
            Interlocked.Exchange(ref _smtpClient, null)?.Dispose();
        }

        /// <summary>
        /// Gets the subject resolver.
        /// </summary>
        /// <param name="logs">The logs.</param>
        /// <returns></returns>
        private FuncResolvable GetSubjectResolver(Log[] logs)
            => new FuncResolvable((context, chunk) => ResolveSubject(context, chunk, logs));

        /// <summary>
        /// Resolves the email subject format builder.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="chunk">The chunk.</param>
        /// <param name="logs">The logs.</param>
        /// <returns></returns>
        private object ResolveSubject(
            [NotNull] FormatWriteContext context,
            [NotNull] FormatChunk chunk,
            [NotNull] Log[] logs)
        {
            // ReSharper disable once PossibleNullReferenceException
            switch (chunk.Tag.ToLowerInvariant())
            {
                case "default":
                    return DefaultSubject;

                case FormatTagName:
                    return string.IsNullOrWhiteSpace(Name)
                        ? Resolution.Null
                        : Name;

                case FormatTagMore:
                    int count = logs.Length - 1;
                    return count < 1
                        ? Resolution.Null
                        : new FuncResolvable(
                            (ctx, ch) => String.Equals(
                                ch.Tag,
                                FormatTagMoreCount,
                                StringComparison.InvariantCultureIgnoreCase)
                                ? count
                                : Resolution.Unknown);

                default:
                    return logs[0]?.Resolve(context, chunk) ?? Resolution.Unknown;
            }
        }

        /// <summary>
        /// Gets the body resolver.
        /// </summary>
        /// <param name="logs">The logs.</param>
        /// <param name="isHtml">if set to <see langword="true" /> the body is HTML.</param>
        /// <returns></returns>
        private FuncResolvable GetBodyResolver(Log[] logs, bool isHtml)
            => new FuncResolvable((context, chunk) => ResolveBody(context, chunk, logs, isHtml));

        /// <summary>
        /// Resolves the body email body format builder.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="chunk">The chunk.</param>
        /// <param name="logs">The logs.</param>
        /// <param name="isHtml">if set to <see langword="true" /> the body is HTML.</param>
        /// <returns></returns>
        private object ResolveBody(
            [NotNull] FormatWriteContext context,
            [NotNull] FormatChunk chunk,
            [NotNull] Log[] logs,
            bool isHtml)
        {
            // ReSharper disable once PossibleNullReferenceException
            switch (chunk.Tag.ToLowerInvariant())
            {
                case "default":
                    return isHtml ? DefaultBodyHtml : Log.VerboseFormat;

                case FormatTagName:
                    return string.IsNullOrWhiteSpace(Name)
                        ? Resolution.Null
                        : Name;

                case "logs":
                    return logs;

                default:
                    return logs[0]?.Resolve(context, chunk) ?? Resolution.Unknown;
            }
        }
    }
}