using System.IO;
using Moq;
using NUnit.Framework;
using Tepeyac.Test;

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
		
		// XXX: nunit isn't picking these up, not sure why
		[TestCase("Tepeyac.Core.Test.no.html", BurritoDayState.No)]
		[TestCase("Tepeyac.Core.Test.yes.html", BurritoDayState.Yes)]
		public void TestParse(string resource, BurritoDayState state)
		{
			using (var stream = base.GetType().Assembly.GetManifestResourceStream(resource))
			using (var reader = new StreamReader(stream))
			{
				this.mockClient.Raise(m => m.Completed += null, true, null, reader.ReadToEnd());
			}
			
			Assert.AreEqual(state, this.model.State);
		}
		
		[Test]
		public void TestNo()
		{
			using (var stream = base.GetType().Assembly.GetManifestResourceStream("Tepeyac.Core.Test.no.html"))
			using (var reader = new StreamReader(stream))
			{
				this.mockClient.Raise(m => m.Completed += null, true, null, reader.ReadToEnd());
			}
			
			Assert.AreEqual(BurritoDayState.No, this.model.State);
		}
		
		[Test]
		public void TestYes()
		{
			using (var stream = base.GetType().Assembly.GetManifestResourceStream("Tepeyac.Core.Test.yes.html"))
			using (var reader = new StreamReader(stream))
			{
				this.mockClient.Raise(m => m.Completed += null, true, null, reader.ReadToEnd());
			}
			
			Assert.AreEqual(BurritoDayState.Yes, this.model.State);
		}
	}
}