/*
 *  Coppied from https://github.com/dotnet/aspnetcore/blob/main/src/Security/Authentication/JwtBearer/src/LoggingExtensions.cs
 *  Used in MyTrainingV1231AngularDemoAsyncJwtBearerHandler. Since it is internal in "Microsoft.Extensions.Logging", we need to copy it here.
 */

using System;
using Microsoft.Extensions.Logging;

namespace MyTrainingV1231AngularDemo.Web.Authentication.JwtBearer
{
    internal static class LoggingExtensions
    {
        private static readonly Action<ILogger, Exception> _tokenValidationFailed = LoggerMessage.Define(
            eventId: new EventId(1, "TokenValidationFailed"),
            logLevel: LogLevel.Information,
            formatString: "Failed to validate the token.");

        private static readonly Action<ILogger, Exception?> _tokenValidationSucceeded = LoggerMessage.Define(
            eventId: new EventId(2, "TokenValidationSucceeded"),
            logLevel: LogLevel.Debug,
            formatString: "Successfully validated the token.");

        private static readonly Action<ILogger, Exception> _errorProcessingMessage = LoggerMessage.Define(
            eventId: new EventId(3, "ProcessingMessageFailed"),
            logLevel: LogLevel.Error,
            formatString: "Exception occurred while processing message.");

        public static void TokenValidationFailed(this ILogger logger, Exception ex)
            => _tokenValidationFailed(logger, ex);

        public static void TokenValidationSucceeded(this ILogger logger)
            => _tokenValidationSucceeded(logger, null);

        public static void ErrorProcessingMessage(this ILogger logger, Exception ex)
            => _errorProcessingMessage(logger, ex);
    }
}
