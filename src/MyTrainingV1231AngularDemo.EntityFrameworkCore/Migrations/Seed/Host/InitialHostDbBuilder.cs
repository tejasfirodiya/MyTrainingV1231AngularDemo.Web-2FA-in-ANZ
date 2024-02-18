using MyTrainingV1231AngularDemo.EntityFrameworkCore;

namespace MyTrainingV1231AngularDemo.Migrations.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly MyTrainingV1231AngularDemoDbContext _context;

        public InitialHostDbBuilder(MyTrainingV1231AngularDemoDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();

            _context.SaveChanges();
        }
    }
}
