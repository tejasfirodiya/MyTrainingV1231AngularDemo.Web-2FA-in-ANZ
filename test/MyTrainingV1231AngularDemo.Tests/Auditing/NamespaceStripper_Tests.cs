using MyTrainingV1231AngularDemo.Auditing;
using MyTrainingV1231AngularDemo.Test.Base;
using Shouldly;
using Xunit;

namespace MyTrainingV1231AngularDemo.Tests.Auditing
{
    // ReSharper disable once InconsistentNaming
    public class NamespaceStripper_Tests: AppTestBase
    {
        private readonly INamespaceStripper _namespaceStripper;

        public NamespaceStripper_Tests()
        {
            _namespaceStripper = Resolve<INamespaceStripper>();
        }

        [Fact]
        public void Should_Stripe_Namespace()
        {
            var controllerName = _namespaceStripper.StripNameSpace("MyTrainingV1231AngularDemo.Web.Controllers.HomeController");
            controllerName.ShouldBe("HomeController");
        }

        [Theory]
        [InlineData("MyTrainingV1231AngularDemo.Auditing.GenericEntityService`1[[MyTrainingV1231AngularDemo.Storage.BinaryObject, MyTrainingV1231AngularDemo.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null]]", "GenericEntityService<BinaryObject>")]
        [InlineData("CompanyName.ProductName.Services.Base.EntityService`6[[CompanyName.ProductName.Entity.Book, CompanyName.ProductName.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null],[CompanyName.ProductName.Services.Dto.Book.CreateInput, N...", "EntityService<Book, CreateInput>")]
        [InlineData("MyTrainingV1231AngularDemo.Auditing.XEntityService`1[MyTrainingV1231AngularDemo.Auditing.AService`5[[MyTrainingV1231AngularDemo.Storage.BinaryObject, MyTrainingV1231AngularDemo.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null],[MyTrainingV1231AngularDemo.Storage.TestObject, MyTrainingV1231AngularDemo.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null],]]", "XEntityService<AService<BinaryObject, TestObject>>")]
        public void Should_Stripe_Generic_Namespace(string serviceName, string result)
        {
            var genericServiceName = _namespaceStripper.StripNameSpace(serviceName);
            genericServiceName.ShouldBe(result);
        }
    }
}
