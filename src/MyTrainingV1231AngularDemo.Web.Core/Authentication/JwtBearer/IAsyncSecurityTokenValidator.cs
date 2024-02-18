using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace MyTrainingV1231AngularDemo.Web.Authentication.JwtBearer
{
    public interface IAsyncSecurityTokenValidator
    {
        /// <summary>
        /// Returns true if the token can be read, false otherwise.
        /// </summary>
        bool CanReadToken(string securityToken);

        /// <summary>
        /// Returns true if a token can be validated.
        /// </summary>
        bool CanValidateToken { get; }

        /// <summary>
        /// Gets and sets the maximum size in bytes, that a will be processed.
        /// </summary>
        Int32 MaximumTokenSizeInBytes { get; set; }

        /// <summary>
        /// Validates a token passed as a string using <see cref="TokenValidationParameters"/>
        /// </summary>
        Task<(ClaimsPrincipal, SecurityToken)> ValidateToken(string securityToken, TokenValidationParameters validationParameters);
        
        /// <summary>
        /// Validates a refresh token passed as a string using <see cref="TokenValidationParameters"/>
        /// </summary>
        Task<(ClaimsPrincipal, SecurityToken)> ValidateRefreshToken(string securityToken, TokenValidationParameters validationParameters);
    }
}
