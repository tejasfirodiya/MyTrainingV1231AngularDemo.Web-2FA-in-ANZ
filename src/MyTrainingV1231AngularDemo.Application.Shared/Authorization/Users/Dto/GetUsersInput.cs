﻿using System.Collections.Generic;
using Abp.Runtime.Validation;
using MyTrainingV1231AngularDemo.Dto;

namespace MyTrainingV1231AngularDemo.Authorization.Users.Dto
{
    public class GetUsersInput : PagedAndSortedInputDto, IShouldNormalize, IGetUsersInput
    {
        public string Filter { get; set; }

        public List<string> Permissions { get; set; }

        public int? Role { get; set; }

        public bool OnlyLockedUsers { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Name,Surname";
            }

            Filter = Filter?.Trim();
        }
    }
}
