using Moq;
using NUnit.Framework;
using Tepeyac.Test;
using System.IO;

namespace Tepeyac.Core.Test
{
	[TestFixture]
	public class BurritoDayModelTest : MoqTestFixture
	{
		private Mock<IWebClient> mockClient;
		private IBurritoDayModel model;
		
		protected override void SetUp()
		{
			this.mockClient = base.CreateMock<IWebClient>();
			this.model = new BurritoDayModel(this.mockClient.Object);
		}
		
		[Test]
		public void TestInitialState()
		{
			Assert.AreEqual(BurritoDayState.Unknown, this.model.State);
		}
		
		[TestCase("no.html", BurritoDayState.No)]
		[TestCase("yes.html", BurritoDayState.Yes)]
		public void Test(string resource, BurritoDayState state)
		{
			using (var stream = base.GetType().Assembly.GetManifestResourceStream(resource))
			using (var reader = new StreamReader(stream))
			{
				this.mockClient.Raise(m => m.Completed += null, true, null, reader.ReadToEnd());
			}
			
			Assert.AreEqual(state, this.model.State);
		}
	}
}