using Abp.AutoMapper;
using MyTrainingV1231AngularDemo.Sessions.Dto;

namespace MyTrainingV1231AngularDemo.Models.Common
{
    [AutoMapFrom(typeof(ApplicationInfoDto)),
     AutoMapTo(typeof(ApplicationInfoDto))]
    public class ApplicationInfoPersistanceModel
    {
        public string Version { get; set; }

        public DateTime ReleaseDate { get; set; }
    }
}