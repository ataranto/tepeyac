using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using NUnit.Framework;
using Retlang.Fibers;
using Tepeyac.Test;

namespace Tepeyac.Core.Test
{
	[TestFixture]
	public class BurritoDayModelTest : MoqTestFixture
	{
		private Queue<string> sequence;
		
		private StubFiber fiber;
		private Mock<IWebClient> mockClient;
		private IBurritoDayModel model;
		
		protected override void SetUp()
		{
			this.fiber = new StubFiber();
			this.mockClient = base.CreateMock<IWebClient>();
			this.model = new BurritoDayModel(this.fiber, this.mockClient.Object);
			
			this.sequence = new Queue<string>();
			this.mockClient.Setup(m => m.DownloadString(It.IsAny<Uri>())).
			Returns(() => this.sequence.Count > 0 ?
				this.GetResource(this.sequence.Dequeue()) :
				null);
		}
		
		[Test]
		public void TestUnknown()
		{
			this.fiber.ExecuteAllScheduled();
			
			Assert.AreEqual(BurritoDayState.Unknown, this.model.State);
		}
		
		[Test]
		public void TestNo()
		{
			this.sequence.Enqueue("no.html");
			this.fiber.ExecuteAllScheduled();
			
			Assert.AreEqual(BurritoDayState.No, this.model.State);
		}
		
		[Test]
		public void TestTomorrow()
		{
			this.sequence.Enqueue("tomorrow.html");
			this.fiber.ExecuteAllScheduled();
			
			Assert.AreEqual(BurritoDayState.Tomorrow, this.model.State);
		}
		
		[Test]
		public void TestYes()
		{
			this.sequence.Enqueue("yes.html");
			this.fiber.ExecuteAllScheduled();
			
			Assert.AreEqual(BurritoDayState.Yes, this.model.State);
		}
		
		[Test]
		public void TestInTransit()
		{
			this.sequence.Enqueue("yes.html");
			this.sequence.Enqueue("harbor.html");
			this.sequence.Enqueue("harbor.xml");
			this.fiber.ExecuteAllScheduled();
			
			Assert.AreEqual(BurritoDayState.Transit, this.model.State);
		}
		
		[Test]
		public void TestArrived()
		{
			this.sequence.Enqueue("yes.html");
			this.sequence.Enqueue("arrived.html");
			this.sequence.Enqueue("arrived.xml");
			this.fiber.ExecuteAllScheduled();
			
			Assert.AreEqual(BurritoDayState.Arrived, this.model.State);
		}
		
		private string GetResource(string resource)
		{
			using (var stream = base.GetType().Assembly.
				GetManifestResourceStream("Tepeyac.Core.Test." + resource))
			using (var reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}
	}
}