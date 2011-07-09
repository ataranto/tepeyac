using Moq;
using NUnit.Framework;

namespace Tepeyac.Test
{
	public abstract class MoqTestFixture
	{
		private MockRepository repository;

        [SetUp]
        public void BaseSetUp()
        {
            this.repository = new MockRepository(MockBehavior.Default);
            this.SetUp();
        }

        [TearDown]
        public void BaseTearDown()
        {
            this.TearDown();
            this.repository.VerifyAll();
        }

        protected Mock<T> CreateMock<T>() where T : class
        {
            return this.repository.Create<T>();
        }

        protected virtual void SetUp()
        {

        }

        protected virtual void TearDown()
        {

        }

	}
}

