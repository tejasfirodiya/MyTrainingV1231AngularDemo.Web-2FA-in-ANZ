using MyTrainingV1231AngularDemo.EntityFrameworkCore;

namespace MyTrainingV1231AngularDemo.Test.Base.TestData
{
    public class TestDataBuilder
    {
        private readonly MyTrainingV1231AngularDemoDbContext _context;
        private readonly int _tenantId;

        public TestDataBuilder(MyTrainingV1231AngularDemoDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            new TestOrganizationUnitsBuilder(_context, _tenantId).Create();
            new TestSubscriptionPaymentBuilder(_context, _tenantId).Create();
            new TestEditionsBuilder(_context).Create();

            _context.SaveChanges();
        }
    }
}
