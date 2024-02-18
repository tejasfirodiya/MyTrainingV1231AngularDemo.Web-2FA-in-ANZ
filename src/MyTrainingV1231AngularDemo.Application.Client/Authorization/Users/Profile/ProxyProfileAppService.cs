using System;
using System.Threading.Tasks;
using MyTrainingV1231AngularDemo.Authorization.Users.Dto;
using MyTrainingV1231AngularDemo.Authorization.Users.Profile.Dto;

namespace MyTrainingV1231AngularDemo.Authorization.Users.Profile
{
    public class ProxyProfileAppService : ProxyAppServiceBase, IProfileAppService
    {
        public async Task<CurrentUserProfileEditDto> GetCurrentUserProfileForEdit()
        {
            return await ApiClient.GetAsync<CurrentUserProfileEditDto>(
                GetEndpoint(nameof(GetCurrentUserProfileForEdit)));
        }

        public async Task UpdateCurrentUserProfile(CurrentUserProfileEditDto input)
        {
            await ApiClient.PutAsync(GetEndpoint(nameof(UpdateCurrentUserProfile)), input);
        }

        public async Task ChangePassword(ChangePasswordInput input)
        {
            await ApiClient.PostAsync(GetEndpoint(nameof(ChangePassword)), input);
        }

        public async Task UpdateProfilePicture(UpdateProfilePictureInput input)
        {
            await ApiClient.PutAsync(GetEndpoint(nameof(UpdateProfilePicture)), input);
        }

        public async Task<GetPasswordComplexitySettingOutput> GetPasswordComplexitySetting()
        {
            return await ApiClient.GetAsync<GetPasswordComplexitySettingOutput>(
                GetEndpoint(nameof(GetPasswordComplexitySetting)));
        }

        public async Task<GetProfilePictureOutput> GetProfilePicture()
        {
            return await ApiClient.GetAsync<GetProfilePictureOutput>(GetEndpoint(nameof(GetProfilePicture)));
        }

        public async Task<GetProfilePictureOutput> GetProfilePictureByUser(long userId)
        {
            return await ApiClient.GetAsync<GetProfilePictureOutput>(GetEndpoint(nameof(GetProfilePictureByUser)),
                new {userId = userId});
        }

        public async Task<GetProfilePictureOutput> GetProfilePictureByUserName(string username)
        {
            return await ApiClient.GetAsync<GetProfilePictureOutput>(GetEndpoint(nameof(GetProfilePictureByUserName)),
                new {username = username});
        }

        public async Task<GetProfilePictureOutput> GetFriendProfilePicture(GetFriendProfilePictureInput input)
        {
            return await ApiClient.GetAsync<GetProfilePictureOutput>(
                GetEndpoint(nameof(GetFriendProfilePicture)),
                input
            );
        }

        public async Task<GetProfilePictureOutput> GetProfilePictureById(Guid profilePictureId)
        {
            return await ApiClient.GetAsync<GetProfilePictureOutput>(GetEndpoint(nameof(GetProfilePictureById)),
                new {profilePictureId = profilePictureId});
        }

        public async Task ChangeLanguage(ChangeUserLanguageDto input)
        {
            await ApiClient.PostAsync(GetEndpoint(nameof(ChangeLanguage)), input);
        }

        public async Task<UpdateGoogleAuthenticatorKeyOutput> UpdateGoogleAuthenticatorKey()
        {
            return await ApiClient.PutAsync<UpdateGoogleAuthenticatorKeyOutput>(
                GetEndpoint(nameof(UpdateGoogleAuthenticatorKey)));
        }

        public async Task SendVerificationSms(SendVerificationSmsInputDto input)
        {
            await ApiClient.PostAsync(GetEndpoint(nameof(SendVerificationSms)), input);
        }

        public async Task VerifySmsCode(VerifySmsCodeInputDto input)
        {
            await ApiClient.PostAsync(GetEndpoint(nameof(VerifySmsCode)));
        }

        public async Task PrepareCollectedData()
        {
            await ApiClient.PostAsync(GetEndpoint(nameof(PrepareCollectedData)));
        }

        public async Task<GenerateGoogleAuthenticatorKeyOutput> GenerateGoogleAuthenticatorKey()
        {
            return await ApiClient.GetAsync<GenerateGoogleAuthenticatorKeyOutput>(GetEndpoint(nameof(GenerateGoogleAuthenticatorKey)));
        }

        public async Task<UpdateGoogleAuthenticatorKeyOutput> UpdateGoogleAuthenticatorKey(UpdateGoogleAuthenticatorKeyInput input)
        {
            return await ApiClient.PostAsync<UpdateGoogleAuthenticatorKeyOutput>(GetEndpoint(nameof(UpdateGoogleAuthenticatorKey)), input);
        }

        public async Task<bool> VerifyAuthenticatorCode(VerifyAuthenticatorCodeInput input)
        {
            return await ApiClient.PostAsync<bool>(GetEndpoint(nameof(VerifyAuthenticatorCode)), input);
        }

        public async Task DisableGoogleAuthenticator(VerifyAuthenticatorCodeInput input)
        {
            await ApiClient.PostAsync(GetEndpoint(nameof(VerifyAuthenticatorCode)), input);
        }

        public async Task<UpdateGoogleAuthenticatorKeyOutput> ViewRecoveryCodes(VerifyAuthenticatorCodeInput input)
        {
            return await ApiClient.PostAsync<UpdateGoogleAuthenticatorKeyOutput>(GetEndpoint(nameof(ViewRecoveryCodes)), input);
        }
    }
}