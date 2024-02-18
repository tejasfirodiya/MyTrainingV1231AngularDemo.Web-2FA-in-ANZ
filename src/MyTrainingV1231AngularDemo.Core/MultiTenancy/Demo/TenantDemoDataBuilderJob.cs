using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;

namespace MyTrainingV1231AngularDemo.MultiTenancy.Demo
{
    public class TenantDemoDataBuilderJob : AsyncBackgroundJob<int>, ITransientDependency
    {
        private readonly TenantDemoDataBuilder _tenantDemoDataBuilder;
        private readonly TenantManager _tenantManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public TenantDemoDataBuilderJob(
            TenantDemoDataBuilder tenantDemoDataBuilder, 
            TenantManager tenantManager, 
            IUnitOfWorkManager unitOfWorkManager)
        {
            _tenantDemoDataBuilder = tenantDemoDataBuilder;
            _tenantManager = tenantManager;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public override async Task ExecuteAsync(int args)
        {
            var tenantId = args;
            var tenant = await _tenantManager.GetByIdAsync(tenantId);
            using (var uow = _unitOfWorkManager.Begin())
            {
                await _tenantDemoDataBuilder.BuildForAsync(tenant);
                await uow.CompleteAsync();
            }
        }
    }
}
