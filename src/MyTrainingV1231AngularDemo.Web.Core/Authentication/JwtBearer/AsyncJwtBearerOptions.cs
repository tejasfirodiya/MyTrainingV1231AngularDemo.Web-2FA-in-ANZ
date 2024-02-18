using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MyTrainingV1231AngularDemo.Web.Authentication.JwtBearer
{
    public class AsyncJwtBearerOptions : JwtBearerOptions
    {
        public readonly List<IAsyncSecurityTokenValidator> AsyncSecurityTokenValidators;
        
        private readonly MyTrainingV1231AngularDemoAsyncJwtSecurityTokenHandler _defaultAsyncHandler = new MyTrainingV1231AngularDemoAsyncJwtSecurityTokenHandler();

        public AsyncJwtBearerOptions()
        {
            AsyncSecurityTokenValidators = new List<IAsyncSecurityTokenValidator>() {_defaultAsyncHandler};
        }
    }

}
