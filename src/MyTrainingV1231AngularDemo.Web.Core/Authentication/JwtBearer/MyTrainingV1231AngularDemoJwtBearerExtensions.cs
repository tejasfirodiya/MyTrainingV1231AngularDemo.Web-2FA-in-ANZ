/*
 *  Copied from https://github.com/dotnet/aspnetcore/blob/main/src/Security/Authentication/JwtBearer/src/JwtBearerExtensions.cs
 *  Updated to implement async token validation 
 */

using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MyTrainingV1231AngularDemo.Web.Authentication.JwtBearer;

/// <summary>
/// Extension methods to configure JWT bearer authentication.
/// </summary>
public static class MyTrainingV1231AngularDemoJwtBearerExtensions
{
    /// <summary>
    /// Enables JWT-bearer authentication using the default scheme <see cref="JwtBearerDefaults.AuthenticationScheme"/>.
    /// <para>
    /// JWT bearer authentication performs authentication by extracting and validating a JWT token from the <c>Authorization</c> request header.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddAbpAsyncJwtBearer(this AuthenticationBuilder builder)
        => builder.AddAbpAsyncJwtBearer(JwtBearerDefaults.AuthenticationScheme, _ => { });

    /// <summary>
    /// Enables JWT-bearer authentication using the default scheme <see cref="JwtBearerDefaults.AuthenticationScheme"/>.
    /// <para>
    /// JWT bearer authentication performs authentication by extracting and validating a JWT token from the <c>Authorization</c> request header.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="configureOptions">A delegate that allows configuring <see cref="JwtBearerOptions"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddAbpAsyncJwtBearer(this AuthenticationBuilder builder, Action<AsyncJwtBearerOptions> configureOptions)
        => builder.AddAbpAsyncJwtBearer(JwtBearerDefaults.AuthenticationScheme, configureOptions);

    /// <summary>
    /// Enables JWT-bearer authentication using the specified scheme.
    /// <para>
    /// JWT bearer authentication performs authentication by extracting and validating a JWT token from the <c>Authorization</c> request header.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="configureOptions">A delegate that allows configuring <see cref="JwtBearerOptions"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddAbpAsyncJwtBearer(this AuthenticationBuilder builder, string authenticationScheme, Action<AsyncJwtBearerOptions> configureOptions)
        => builder.AddAbpAsyncJwtBearer(authenticationScheme, displayName: null, configureOptions: configureOptions);

    /// <summary>
    /// Enables JWT-bearer authentication using the specified scheme.
    /// <para>
    /// JWT bearer authentication performs authentication by extracting and validating a JWT token from the <c>Authorization</c> request header.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="displayName">The display name for the authentication handler.</param>
    /// <param name="configureOptions">A delegate that allows configuring <see cref="JwtBearerOptions"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddAbpAsyncJwtBearer(this AuthenticationBuilder builder, string authenticationScheme, string? displayName, Action<AsyncJwtBearerOptions> configureOptions)
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<AsyncJwtBearerOptions>, JwtBearerPostConfigureOptions>());
        return builder.AddScheme<AsyncJwtBearerOptions, MyTrainingV1231AngularDemoAsyncJwtBearerHandler>(authenticationScheme, displayName, configureOptions);
    }
}
