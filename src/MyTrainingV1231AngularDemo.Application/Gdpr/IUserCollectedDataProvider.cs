using System.Collections.Generic;
using System.Threading.Tasks;
using Abp;
using MyTrainingV1231AngularDemo.Dto;

namespace MyTrainingV1231AngularDemo.Gdpr
{
    public interface IUserCollectedDataProvider
    {
        Task<List<FileDto>> GetFiles(UserIdentifier user);
    }
}
