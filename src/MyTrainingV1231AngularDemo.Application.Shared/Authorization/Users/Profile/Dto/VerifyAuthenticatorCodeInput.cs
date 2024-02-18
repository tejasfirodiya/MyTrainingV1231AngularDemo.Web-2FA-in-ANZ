namespace MyTrainingV1231AngularDemo.Authorization.Users.Profile.Dto
{
    public class VerifyAuthenticatorCodeInput
    {
        public string Code { get; set; }
        public string GoogleAuthenticatorKey { get; set; }
    }
}