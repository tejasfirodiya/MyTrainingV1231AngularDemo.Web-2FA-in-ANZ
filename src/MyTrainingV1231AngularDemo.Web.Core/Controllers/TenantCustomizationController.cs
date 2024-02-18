using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.AspNetZeroCore.Net;
using Abp.IO.Extensions;
using Abp.MimeTypes;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTrainingV1231AngularDemo.Authorization;
using MyTrainingV1231AngularDemo.Authorization.Users.Profile.Dto;
using MyTrainingV1231AngularDemo.Graphics;
using MyTrainingV1231AngularDemo.MultiTenancy;
using MyTrainingV1231AngularDemo.Storage;
using MyTrainingV1231AngularDemo.Tenants;

namespace MyTrainingV1231AngularDemo.Web.Controllers
{
    [AbpMvcAuthorize]
    public class TenantCustomizationController : MyTrainingV1231AngularDemoControllerBase
    {
        private readonly TenantManager _tenantManager;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IMimeTypeMap _mimeTypeMap;
        private readonly IImageValidator _imageValidator;

        public TenantCustomizationController(
            TenantManager tenantManager,
            IBinaryObjectManager binaryObjectManager,
            IMimeTypeMap mimeTypeMap,
            IImageValidator imageValidator)
        {
            _tenantManager = tenantManager;
            _binaryObjectManager = binaryObjectManager;
            _mimeTypeMap = mimeTypeMap;
            _imageValidator = imageValidator;
        }

        [HttpPost]
        [AbpMvcAuthorize(AppPermissions.Pages_Administration_Tenant_Settings)]
        public async Task<JsonResult> UploadLightLogo()
        {
            try
            {
                var logoObject = await UploadLogoFileInternal();

                var tenant = await _tenantManager.GetByIdAsync(AbpSession.GetTenantId());
                tenant.LightLogoId = logoObject.id;
                tenant.LightLogoFileType = logoObject.contentType;

                return Json(new AjaxResponse(new
                {
                    id = logoObject.id, 
                    TenantId = tenant.Id, 
                    fileType = tenant.LightLogoFileType
                }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
        
        [HttpPost]
        [AbpMvcAuthorize(AppPermissions.Pages_Administration_Tenant_Settings)]
        public async Task<JsonResult> UploadLightLogoMinimal()
        {
            try
            {
                var logoObject = await UploadLogoFileInternal();

                var tenant = await _tenantManager.GetByIdAsync(AbpSession.GetTenantId());
                tenant.LightLogoMinimalId = logoObject.id;
                tenant.LightLogoMinimalFileType = logoObject.contentType;

                return Json(new AjaxResponse(new
                {
                    id = logoObject.id, 
                    TenantId = tenant.Id, 
                    fileType = tenant.LightLogoMinimalFileType
                }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpPost]
        [AbpMvcAuthorize(AppPermissions.Pages_Administration_Tenant_Settings)]
        public async Task<JsonResult> UploadDarkLogo()
        {
            try
            {
                var logoObject = await UploadLogoFileInternal();

                var tenant = await _tenantManager.GetByIdAsync(AbpSession.GetTenantId());
                tenant.DarkLogoId = logoObject.id;
                tenant.DarkLogoFileType = logoObject.contentType;

                return Json(new AjaxResponse(new
                {
                    id = logoObject.id,
                    TenantId = tenant.Id,
                    fileType = tenant.DarkLogoFileType
                }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
        
        [HttpPost]
        [AbpMvcAuthorize(AppPermissions.Pages_Administration_Tenant_Settings)]
        public async Task<JsonResult> UploadDarkLogoMinimal()
        {
            try
            {
                var logoObject = await UploadLogoFileInternal();

                var tenant = await _tenantManager.GetByIdAsync(AbpSession.GetTenantId());
                tenant.DarkLogoMinimalId = logoObject.id;
                tenant.DarkLogoMinimalFileType = logoObject.contentType;

                return Json(new AjaxResponse(new
                {
                    id = logoObject.id,
                    TenantId = tenant.Id,
                    fileType = tenant.DarkLogoMinimalFileType
                }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        private async Task<(Guid id, string contentType)> UploadLogoFileInternal()
        {
            var logoFile = Request.Form.Files.First();

            //Check input
            if (logoFile == null)
            {
                throw new UserFriendlyException(L("File_Empty_Error"));
            }

            if (logoFile.Length > 102400) //100KB
            {
                throw new UserFriendlyException(L("File_SizeLimit_Error"));
            }

            byte[] fileBytes;
            await using (var stream = logoFile.OpenReadStream())
            {
                fileBytes = stream.GetAllBytes();
                _imageValidator.ValidateDimensions(fileBytes, 512, 128);
            }

            var logoObject = new BinaryObject(AbpSession.GetTenantId(), fileBytes, $"Logo {DateTime.UtcNow}");
            await _binaryObjectManager.SaveAsync(logoObject);
            return (logoObject.Id, logoFile.ContentType);
        }

        [HttpPost]
        [AbpMvcAuthorize(AppPermissions.Pages_Administration_Tenant_Settings)]
        public async Task<JsonResult> UploadCustomCss()
        {
            try
            {
                var cssFile = Request.Form.Files.First();

                //Check input
                if (cssFile == null)
                {
                    throw new UserFriendlyException(L("File_Empty_Error"));
                }

                if (cssFile.Length > 1048576) //1MB
                {
                    throw new UserFriendlyException(L("File_SizeLimit_Error"));
                }

                byte[] fileBytes;
                using (var stream = cssFile.OpenReadStream())
                {
                    fileBytes = stream.GetAllBytes();
                }

                var cssFileObject = new BinaryObject(AbpSession.GetTenantId(), fileBytes,
                    $"Custom Css {cssFile.FileName} {DateTime.UtcNow}");
                await _binaryObjectManager.SaveAsync(cssFileObject);

                var tenant = await _tenantManager.GetByIdAsync(AbpSession.GetTenantId());
                tenant.CustomCssId = cssFileObject.Id;

                return Json(new AjaxResponse(new {id = cssFileObject.Id, TenantId = tenant.Id}));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> GetLogo(int? tenantId)
        {
            if (tenantId == null)
            {
                tenantId = AbpSession.TenantId;
            }

            if (!tenantId.HasValue)
            {
                return StatusCode((int) HttpStatusCode.NotFound);
            }

            var tenant = await _tenantManager.FindByIdAsync(tenantId.Value);
            if (tenant == null || !tenant.HasLogo())
            {
                return StatusCode((int) HttpStatusCode.NotFound);
            }

            using (CurrentUnitOfWork.SetTenantId(tenantId.Value))
            {
                var logoObject = await _binaryObjectManager.GetOrNullAsync(tenant.LightLogoId.Value);
                if (logoObject == null)
                {
                    return StatusCode((int) HttpStatusCode.NotFound);
                }

                return File(logoObject.Bytes, tenant.LightLogoFileType);
            }
        }

        [AllowAnonymous]
        [Route("/TenantCustomization/GetTenantLogo/{skin}/{tenantId?}/{extension?}")]
        [HttpGet]
        public Task<ActionResult> GetTenantLogoWithCustomRoute(string skin, int? tenantId = null,
            string extension = "svg")
        {
            return GetTenantLogo(skin, tenantId, extension);
        }

        [AllowAnonymous]
        public async Task<ActionResult> GetTenantLogo(string skin, int? tenantId, string extension = "svg")
        {
            var mimeType = _mimeTypeMap.GetMimeType("." + extension);
            var defaultLogo = "/Common/Images/app-logo-on-" + skin + "." + extension;

            if (tenantId == null)
            {
                return File(defaultLogo, mimeType);
            }

            var tenant = await _tenantManager.FindByIdAsync(tenantId.Value);
            if (tenant == null || !tenant.HasLogo())
            {
                return File(defaultLogo, mimeType);
            }

            async Task<ActionResult> GetLogoInternal(Guid id, string logoFileType)
            {
                var logoObject = await _binaryObjectManager.GetOrNullAsync(id);
                if (logoObject == null)
                {
                    return File(defaultLogo, mimeType);
                }

                return File(logoObject.Bytes, logoFileType);
            }

            using (CurrentUnitOfWork.SetTenantId(tenantId.Value))
            {
                switch (skin.ToLower())
                {
                    case "dark":
                        if (tenant.HasDarkLogo())
                        {
                            return await GetLogoInternal(tenant.DarkLogoId.Value, tenant.DarkLogoFileType);
                        }
                        break;
                    case "dark-sm":
                        if (tenant.HasDarkLogoMinimal())
                        {
                            return await GetLogoInternal(tenant.DarkLogoMinimalId.Value, tenant.DarkLogoMinimalFileType);
                        }
                        break;
                    case "light":
                        if (tenant.HasLightLogo())
                        {
                            return await GetLogoInternal(tenant.LightLogoId.Value, tenant.LightLogoFileType);
                        }
                        break;
                    case "light-sm":
                        if (tenant.HasLightLogoMinimal())
                        {
                            return await GetLogoInternal(tenant.LightLogoMinimalId.Value, tenant.LightLogoMinimalFileType);
                        }
                        break;
                }
            }

            return File(defaultLogo, mimeType);
        }

        [AllowAnonymous]
        public async Task<ActionResult> GetTenantLogoOrNull(string skin, int tenantId)
        {
            var tenant = await _tenantManager.FindByIdAsync(tenantId);
            if (tenant == null || !tenant.HasLogo())
            {
                return Ok(new GetTenantLogoOutput());
            }

            async Task<ActionResult> GetLogoInternal(Guid id, string logoFileType)
            {
                var logoObject = await _binaryObjectManager.GetOrNullAsync(id);
                if (logoObject == null)
                {
                    return null;
                }

                return Ok(new GetTenantLogoOutput(Convert.ToBase64String(logoObject.Bytes), logoFileType));
            }

            using (CurrentUnitOfWork.SetTenantId(tenantId))
            {
                if (skin.ToLower() == "dark" || skin.ToLower() == "dark-sm")
                {
                    if (tenant.HasDarkLogo())
                    {
                        return await GetLogoInternal(tenant.DarkLogoId.Value, tenant.DarkLogoFileType);
                    }

                    if (tenant.HasLightLogo())
                    {
                        return await GetLogoInternal(tenant.LightLogoId.Value, tenant.LightLogoFileType);
                    }
                }
                else
                {
                    if (tenant.HasLightLogo())
                    {
                        return await GetLogoInternal(tenant.LightLogoId.Value, tenant.LightLogoFileType);
                    }

                    if (tenant.HasDarkLogo())
                    {
                        return await GetLogoInternal(tenant.DarkLogoId.Value, tenant.DarkLogoFileType);
                    }
                }
            }

            return Ok(new GetTenantLogoOutput());
        }

        [AllowAnonymous]
        public async Task<ActionResult> GetCustomCss(int? tenantId)
        {
            if (tenantId == null)
            {
                tenantId = AbpSession.TenantId;
            }

            if (!tenantId.HasValue)
            {
                return StatusCode((int) HttpStatusCode.NotFound);
            }

            var tenant = await _tenantManager.FindByIdAsync(tenantId.Value);
            if (tenant == null || !tenant.CustomCssId.HasValue)
            {
                return StatusCode((int) HttpStatusCode.NotFound);
            }

            using (CurrentUnitOfWork.SetTenantId(tenantId.Value))
            {
                var cssFileObject = await _binaryObjectManager.GetOrNullAsync(tenant.CustomCssId.Value);
                if (cssFileObject == null)
                {
                    return StatusCode((int) HttpStatusCode.NotFound);
                }

                return File(cssFileObject.Bytes, MimeTypeNames.TextCss);
            }
        }
    }
}