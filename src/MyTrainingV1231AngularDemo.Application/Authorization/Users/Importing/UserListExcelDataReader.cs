using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Abp.Localization;
using Abp.Localization.Sources;
using MyTrainingV1231AngularDemo.Authorization.Users.Importing.Dto;
using System.Linq;
using Abp.Collections.Extensions;
using MyTrainingV1231AngularDemo.DataExporting.Excel.MiniExcel;

namespace MyTrainingV1231AngularDemo.Authorization.Users.Importing
{
    public class UserListExcelDataReader : MiniExcelExcelImporterBase<ImportUserDto>, IUserListExcelDataReader
    {
        private readonly ILocalizationSource _localizationSource;

        public UserListExcelDataReader(ILocalizationManager localizationManager)
        {
            _localizationSource = localizationManager.GetSource(MyTrainingV1231AngularDemoConsts.LocalizationSourceName);
        }

        public List<ImportUserDto> GetUsersFromExcel(byte[] fileBytes)
        {
            return ProcessExcelFile(fileBytes, ProcessExcelRow);
        }

        private ImportUserDto ProcessExcelRow(dynamic row)
        {
            if (IsRowEmpty(row))
            {
                return null;
            }

            var exceptionMessage = new StringBuilder();
            var user = new ImportUserDto();

            try
            {
                user.UserName = GetRequiredValueFromRowOrNull(row, nameof(user.UserName), exceptionMessage);
                user.Name = GetRequiredValueFromRowOrNull(row,  nameof(user.Name), exceptionMessage);
                user.Surname = GetRequiredValueFromRowOrNull(row, nameof(user.Surname), exceptionMessage);
                user.EmailAddress = GetRequiredValueFromRowOrNull(row, nameof(user.EmailAddress), exceptionMessage);
                user.PhoneNumber = GetOptionalValueFromRowOrNull(row, nameof(user.PhoneNumber), exceptionMessage);
                user.Password = GetRequiredValueFromRowOrNull(row, nameof(user.Password), exceptionMessage);
                user.AssignedRoleNames = GetAssignedRoleNamesFromRow(row);
            }
            catch (Exception exception)
            {
                user.Exception = exception.Message;
            }

            return user;
        }

        private string GetRequiredValueFromRowOrNull(
            dynamic row,
            string columnName,
            StringBuilder exceptionMessage)
        {
            var cellValue = (row as ExpandoObject).GetOrDefault(columnName)?.ToString();
            if (cellValue != null && !string.IsNullOrWhiteSpace(cellValue))
            {
                return cellValue;
            }

            exceptionMessage.Append(GetLocalizedExceptionMessagePart(columnName));
            return null;
        }

        private string GetOptionalValueFromRowOrNull(dynamic row, string columnName, StringBuilder exceptionMessage)
        {
            var cellValue = (row as ExpandoObject).GetOrDefault(columnName)?.ToString();
            if (cellValue != null && !string.IsNullOrWhiteSpace(cellValue))
            {
                return cellValue;
            }

            exceptionMessage.Append(GetLocalizedExceptionMessagePart(columnName));
            return String.Empty;
        }

        private string[] GetAssignedRoleNamesFromRow(dynamic row)
        {
            var cellValue = (row as ExpandoObject).GetOrDefault(nameof(ImportUserDto.AssignedRoleNames))?.ToString();
            if (cellValue == null || string.IsNullOrWhiteSpace(cellValue))
            {
                return Array.Empty<string>();
            }

            var roles = cellValue.Split(',');
            return roles.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim())
                .ToArray();
        }

        private string GetLocalizedExceptionMessagePart(string parameter)
        {
            return _localizationSource.GetString("{0}IsInvalid", _localizationSource.GetString(parameter)) + "; ";
        }
        
        private bool IsRowEmpty(dynamic row)
        {
            var username = (row as ExpandoObject).GetOrDefault(nameof(User.UserName))?.ToString();
            return string.IsNullOrWhiteSpace(username);
        }
    }
}