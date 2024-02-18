namespace MyTrainingV1231AngularDemo.Mobile.MAUI.Models.Settings
{
    public class ChangePasswordModel
    {
        private string _currentPassword;
        private string _newPassword;

        private string _newPasswordRepeat;

        private bool _isChangePasswordDisabled = true;

        public string CurrentPassword
        {
            get => _currentPassword;
            set
            {
                _currentPassword = value;
                SetChangePasswordButtonStatus();
            }
        }

        public string NewPassword
        {
            get => _newPassword;
            set
            {
                _newPassword = value;
                SetChangePasswordButtonStatus();
            }
        }

        public string NewPasswordRepeat
        {
            get => _newPasswordRepeat;
            set
            {
                _newPasswordRepeat = value;
                SetChangePasswordButtonStatus();
            }
        }

        public bool IsChangePasswordDisabled
        {
            get => _isChangePasswordDisabled;
            set
            {
                _isChangePasswordDisabled = value;
            }
        }

        private void SetChangePasswordButtonStatus()
        {
            IsChangePasswordDisabled = string.IsNullOrWhiteSpace(CurrentPassword)
                                      || string.IsNullOrWhiteSpace(NewPassword)
                                      || string.IsNullOrWhiteSpace(NewPasswordRepeat)
                                      || NewPassword != NewPasswordRepeat;
        }
    }
}
