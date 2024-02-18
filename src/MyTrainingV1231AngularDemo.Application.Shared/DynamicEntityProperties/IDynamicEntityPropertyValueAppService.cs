using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using MyTrainingV1231AngularDemo.DynamicEntityProperties.Dto;
using MyTrainingV1231AngularDemo.DynamicEntityPropertyValues.Dto;

namespace MyTrainingV1231AngularDemo.DynamicEntityProperties
{
    public interface IDynamicEntityPropertyValueAppService
    {
        Task<DynamicEntityPropertyValueDto> Get(int id);

        Task<ListResultDto<DynamicEntityPropertyValueDto>> GetAll(GetAllInput input);

        Task Add(DynamicEntityPropertyValueDto input);

        Task Update(DynamicEntityPropertyValueDto input);

        Task Delete(int id);

        Task<GetAllDynamicEntityPropertyValuesOutput> GetAllDynamicEntityPropertyValues(GetAllDynamicEntityPropertyValuesInput input);
    }
}
