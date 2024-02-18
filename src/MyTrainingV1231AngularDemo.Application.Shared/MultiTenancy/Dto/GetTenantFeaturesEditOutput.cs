using System.Collections.Generic;
using Abp.Application.Services.Dto;
using MyTrainingV1231AngularDemo.Editions.Dto;

namespace MyTrainingV1231AngularDemo.MultiTenancy.Dto
{
    public class GetTenantFeaturesEditOutput
    {
        public List<NameValueDto> FeatureValues { get; set; }

        public List<FlatFeatureDto> Features { get; set; }
    }
}