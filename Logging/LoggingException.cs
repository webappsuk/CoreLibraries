#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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
using System.Globalization;
using System.Linq.Expressions;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Formatting;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    ///   Implements a specialised Exception handler that is automatically logged and supports late translation of resources.
    /// </summary>
    [PublicAPI]
    [Serializable]
    [DebuggerDisplay("[{ExceptionTypeFullName}] {Message} @ {TimeStamp}")]
    public class LoggingException : ApplicationException, IFormattable
    {
        /// <summary>
        /// Set's the underlying exception format, not strictly necessary but easily doable.
        /// </summary>
        [NotNull]
        private static readonly Action<Exception, string> _setMessage =
            // ReSharper disable once AssignNullToNotNullAttribute
            typeof(Exception).GetSetter<Exception, string>("_message");

        /// <summary>
        /// The associated log item.
        /// </summary>
        [NotNull]
        public readonly Log Log;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException"/> class.
        /// </summary>
        /// <param name="log">The log.</param>
        public LoggingException([NotNull] Log log)
            : base(log.Message)
        {
            Log = log.Add();

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="format">The exception format.</param>
        /// <param name="parameters">The parameters.</param>
        [StringFormatMethod("format")]
        public LoggingException(
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(this, LoggingLevel.Error, format, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="format">The exception format.</param>
        /// <param name="parameters">The parameters.</param>
        [StringFormatMethod("format")]
        public LoggingException(
            [CanBeNull] LogContext context,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(context, this, LoggingLevel.Error, format, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="format">The exception format.</param>
        /// <param name="parameters">The parameters.</param>
        [StringFormatMethod("format")]
        public LoggingException(
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(this, level, format, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="format">The exception format.</param>
        /// <param name="parameters">The parameters.</param>
        [StringFormatMethod("format")]
        public LoggingException(
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(context, this, level, format, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="level">The log level.</param>
        public LoggingException([CanBeNull] Exception exception, LoggingLevel level = LoggingLevel.Error)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(this, level).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="exception">The inner exception.</param>
        /// <param name="format">The exception format.</param>
        /// <param name="parameters">The parameters.</param>
        [StringFormatMethod("format")]
        public LoggingException(
            [CanBeNull] Exception exception,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(this, LoggingLevel.Error, format, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="exception">The inner exception.</param>
        /// <param name="level">The log level.</param>
        /// <param name="format">The exception format.</param>
        /// <param name="parameters">The parameters.</param>
        [StringFormatMethod("format")]
        public LoggingException(
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(this, level, format, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="level">The log level.</param>
        /// <param name="format">The exception format.</param>
        /// <param name="parameters">The parameters.</param>
        [StringFormatMethod("format")]
        public LoggingException(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(context, this, level, format, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="resource">The resource expression, e.g. ()=> Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException([CanBeNull] Expression<Func<string>> resource, [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(this, LoggingLevel.Error, resource, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="resource">The resource expression, e.g. ()=> Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] LogContext context,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(context, this, LoggingLevel.Error, resource, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="resource">The resource expression, e.g. ()=> Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(this, level, resource, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resource">The resource expression, e.g. ()=> Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(context, this, level, resource, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="exception">The inner exception.</param>
        /// <param name="resource">The resource expression, e.g. ()=> Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] Exception exception,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(this, LoggingLevel.Error, resource, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="exception">The inner exception.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resource">The resource expression, e.g. ()=> Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(this, level, resource, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resource">The resource expression, e.g. ()=> Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(context, this, level, resource, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="format">The exception format.</param>
        /// <param name="parameters">The parameters.</param>
        [StringFormatMethod("format")]
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(culture, this, LoggingLevel.Error, format, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="format">The exception format.</param>
        /// <param name="parameters">The parameters.</param>
        [StringFormatMethod("format")]
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(culture, context, this, LoggingLevel.Error, format, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="level">The log level.</param>
        /// <param name="format">The exception format.</param>
        /// <param name="parameters">The parameters.</param>
        [StringFormatMethod("format")]
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(culture, this, level, format, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="format">The exception format.</param>
        /// <param name="parameters">The parameters.</param>
        [StringFormatMethod("format")]
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(culture, context, this, level, format, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="level">The log level.</param>
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Exception exception,
            LoggingLevel level = LoggingLevel.Error)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(culture, this, level).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="exception">The inner exception.</param>
        /// <param name="format">The exception format.</param>
        /// <param name="parameters">The parameters.</param>
        [StringFormatMethod("format")]
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Exception exception,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(culture, this, LoggingLevel.Error, format, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="exception">The inner exception.</param>
        /// <param name="level">The log level.</param>
        /// <param name="format">The exception format.</param>
        /// <param name="parameters">The parameters.</param>
        [StringFormatMethod("format")]
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(culture, this, level, format, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="level">The log level.</param>
        /// <param name="format">The exception format.</param>
        /// <param name="parameters">The parameters.</param>
        [StringFormatMethod("format")]
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(culture, context, this, level, format, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="resource">The resource expression, e.g. ()=&gt; Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(culture, this, LoggingLevel.Error, resource, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="resource">The resource expression, e.g. ()=&gt; Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(culture, context, this, LoggingLevel.Error, resource, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resource">The resource expression, e.g. ()=&gt; Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(culture, this, level, resource, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resource">The resource expression, e.g. ()=&gt; Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(culture, context, this, level, resource, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="exception">The inner exception.</param>
        /// <param name="resource">The resource expression, e.g. ()=&gt; Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Exception exception,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(culture, this, LoggingLevel.Error, resource, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="exception">The inner exception.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resource">The resource expression, e.g. ()=&gt; Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(culture, this, level, resource, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resource">The resource expression, e.g. ()=&gt; Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(culture, context, this, level, resource, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType" />.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(this, LoggingLevel.Error, resourceType, resourceProperty, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType" />.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] LogContext context,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(context, this, LoggingLevel.Error, resourceType, resourceProperty, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType" />.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(this, level, resourceType, resourceProperty, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType" />.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(context, this, level, resourceType, resourceProperty, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="exception">The inner exception.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType" />.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] Exception exception,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(this, LoggingLevel.Error, resourceType, resourceProperty, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="exception">The inner exception.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType" />.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(this, level, resourceType, resourceProperty, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType" />.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(context, this, level, resourceType, resourceProperty, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType" />.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(culture, this, LoggingLevel.Error, resourceType, resourceProperty, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType" />.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(culture, context, this, LoggingLevel.Error, resourceType, resourceProperty, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType" />.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(culture, this, level, resourceType, resourceProperty, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType" />.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : base(string.Empty)
        {
            // Log the exception
            Log = new Log(culture, context, this, level, resourceType, resourceProperty, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="exception">The inner exception.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType" />.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Exception exception,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(culture, this, LoggingLevel.Error, resourceType, resourceProperty, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="exception">The inner exception.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType" />.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(culture, this, level, resourceType, resourceProperty, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType" />.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(culture, context, this, level, resourceType, resourceProperty, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="resource">The resource expression, e.g. ()=> Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(context, this, LoggingLevel.Error, resource, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="format">The exception format.</param>
        /// <param name="parameters">The parameters.</param>
        [StringFormatMethod("format")]
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(culture, context, this, LoggingLevel.Error, format, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="resource">The resource expression, e.g. ()=&gt; Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(culture, context, this, LoggingLevel.Error, resource, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType" />.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(context, this, LoggingLevel.Error, resourceType, resourceProperty, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType" />.</param>
        /// <param name="parameters">The parameters.</param>
        public LoggingException(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : base(exception?.Message ?? string.Empty, exception)
        {
            // Log the exception
            Log = new Log(culture, context, this, LoggingLevel.Error, resourceType, resourceProperty, parameters).Add();
            _setMessage(this, Log.Message);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }
        #endregion

        /// <summary>
        /// Gets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        public CombGuid Guid => Log.Guid;

        /// <summary>
        /// Gets the log group.
        /// </summary>
        /// <value>The log group.</value>
        public LoggingLevel Level => Log.Level;

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        [NotNull]
        public IEnumerable<string> Parameters => Log.Parameters;

        /// <summary>
        /// Gets the time stamp.
        /// </summary>
        /// <value>The time stamp.</value>
        public DateTime TimeStamp => Log.TimeStamp;

        /// <summary>
        /// Gets the format format.
        /// </summary>
        /// <value>The format format.</value>
        [CanBeNull]
        public string MessageFormat => Log.MessageFormat;

        /// <summary>
        /// Gets the thread ID.
        /// </summary>
        /// <value>The thread ID.</value>
        public int ThreadID => Log.ThreadID;

        /// <summary>
        /// Gets the name of the thread.
        /// </summary>
        /// <value>The name of the thread.</value>
        [CanBeNull]
        public string ThreadName => Log.ThreadName;

        /// <summary>
        /// Gets the full name of the type of the exception.
        /// </summary>
        /// <value>The full name of the type of the exception.</value>
        [NotNull]
            // ReSharper disable once AssignNullToNotNullAttribute
        public string ExceptionTypeFullName => Log.ExceptionType ?? GetType().FullName;

        /// <summary>
        /// Gets the stored procedure name (if a SQL exception - otherwise null).
        /// </summary>
        /// <value>The stored procedure.</value>
        [CanBeNull]
        public string StoredProcedure => Log.StoredProcedure;

        /// <summary>
        /// Gets the stored procedure line number (if a SQL exception - otherwise -1).
        /// </summary>
        /// <value>The stored procedure line.</value>
        public int StoredProcedureLine => Log.StoredProcedureLine;

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value associated with the specified key. If the specified key is not found, throws a <see cref="T:System.Collections.Generic.KeyNotFoundException" />.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key" /> does not exist in the collection.</exception>
        [CanBeNull]
        public string this[[NotNull] string key] => Log[key];

        /// <summary>
        /// Gets a format that describes the current exception.
        /// </summary>
        /// <value>The format.</value>
        /// <returns>The error format that explains the reason for the exception, or an empty string("").</returns>
        public override string Message => Log.Message ?? string.Empty;

        /// <summary>
        ///   Gets the stack trace as a <see cref="string"/>.
        ///   Occasionally the stack trace becomes unavailable, so we capture to string on construction.
        /// </summary>
        /// <value>The safe stack trace.</value>
        [NotNull]
        public new string StackTrace => Log.StackTrace ?? base.StackTrace ?? string.Empty;

        /// <summary>
        /// Gets all the properties of the logging exception.
        /// </summary>
        [NotNull]
        public IEnumerable<KeyValuePair<string, string>> AllProperties => Log.AllProperties;

        /// <summary>
        /// Gets the value of the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>string.</returns>
        [CanBeNull]
        public string Get([NotNull] string key) => Log.Get(key);

        /// <summary>
        /// Gets all keys and their values that match the prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns>IEnumerable{KeyValuePair{stringstring}}.</returns>
        [NotNull]
        public IEnumerable<KeyValuePair<string, string>> GetPrefixed([NotNull] string prefix) => Log.GetPrefixed(prefix);

        #region ToString overloads
        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> representation this instance.</returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*"/>
        /// </PermissionSet>
        public override string ToString() => ToString(Log.VerboseFormat);

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        [NotNull]
        public string ToString([CanBeNull] IFormatProvider formatProvider) => ToString(Log.VerboseFormat, formatProvider);

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format to use.-or- A null reference (Nothing in Visual Basic) to use the default format defined for the type of the <see cref="T:System.IFormattable" /> implementation.</param>
        /// <param name="formatProvider">The provider to use to format the value.-or- A null reference (Nothing in Visual Basic) to obtain the numeric format information from the current locale setting of the operating system.</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public string ToString([CanBeNull] string format, [CanBeNull] IFormatProvider formatProvider = null) => ToString((FormatBuilder)format, formatProvider);

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format to use.-or- A null reference (Nothing in Visual Basic) to use the default format defined for the type of the <see cref="T:System.IFormattable" /> implementation.</param>
        /// <param name="formatProvider">The provider to use to format the value.-or- A null reference (Nothing in Visual Basic) to obtain the numeric format information from the current locale setting of the operating system.</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        [NotNull]
        public string ToString([CanBeNull] FormatBuilder format, [CanBeNull] IFormatProvider formatProvider = null)
        {
            if (format == null)
                format = Log.VerboseFormat;

            ICustomFormatter formatter = formatProvider?.GetFormat(GetType()) as ICustomFormatter;

            if (formatter != null)
                return formatter.Format(format.ToString(formatProvider), this, formatProvider) ?? string.Empty;

            return Log.ToString(format, formatProvider);
        }
        #endregion
    }
}