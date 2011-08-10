using System.IO;
using Moq;
using NUnit.Framework;
using Tepeyac.Test;
using System;
using Retlang.Fibers;
using System.Collections.Generic;

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
				Returns(() => this.GetResource(this.sequence.Dequeue()));
		}
		
		[Test]
		public void TestNo()
		{
			this.sequence.Enqueue("Tepeyac.Core.Test.no.html");
			this.fiber.ExecuteAllScheduled();
			
			Assert.AreEqual(BurritoDayState.No, this.model.State);
		}
		
		[Test]
		public void TestTomorrow()
		{
			this.sequence.Enqueue("Tepeyac.Core.Test.tomorrow.html");
			this.fiber.ExecuteAllScheduled();
			
			Assert.AreEqual(BurritoDayState.Tomorrow, this.model.State);
		}
		
		[Test]
		public void TestYes()
		{
			this.sequence.Enqueue("Tepeyac.Core.Test.yes.html");
			this.fiber.ExecuteAllScheduled();
			
			Assert.AreEqual(BurritoDayState.Yes, this.model.State);
		}
		
		[Test]
		public void TestInTransit()
		{
			this.sequence.Enqueue("Tepeyac.Core.Test.yes.html");
			this.sequence.Enqueue("Tepeyac.Core.Test.harbor.html");
			this.sequence.Enqueue("Tepeyac.Core.Test.harbor.xml");
			this.fiber.ExecuteAllScheduled();
			
			Assert.AreEqual(BurritoDayState.Transit, this.model.State);
		}
		
		[Test]
		public void TestArrived()
		{
			this.sequence.Enqueue("Tepeyac.Core.Test.yes.html");
			this.sequence.Enqueue("Tepeyac.Core.Test.arrived.html");
			this.sequence.Enqueue("Tepeyac.Core.Test.arrived.xml");
			this.fiber.ExecuteAllScheduled();
			
			Assert.AreEqual(BurritoDayState.Arrived, this.model.State);
		}
		
		private string GetResource(string resource)
		{
			using (var stream = base.GetType().Assembly.GetManifestResourceStream(resource))
			using (var reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
				/*
				this.mockClient.Setup(m => m.DownloadString(It.IsAny<Uri>())).
					Returns(reader.ReadToEnd());
				*/
			}
		}
	}
}