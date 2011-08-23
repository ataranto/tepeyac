using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using NUnit.Framework;
using Retlang.Fibers;
using Tepeyac.Core;

namespace Tepeyac.Test.Core
{
	[TestFixture]
	public class BurritoDayModelTest : MoqTestFixture
	{
		private Queue<string> sequence;
		private IDictionary<Uri, string> cache;
		
		private StubFiber fiber;
		private Mock<IWebClient> mockClient;
		private IBurritoDayModel model;
		
		protected override void SetUp()
		{
			this.fiber = new StubFiber();
			this.mockClient = base.CreateMock<IWebClient>();
			this.model = new BurritoDayModel(this.fiber, this.mockClient.Object);
			
			this.sequence = new Queue<string>();
			this.cache = new Dictionary<Uri, string>();
			
			this.mockClient.Setup(m => m.Download(It.IsAny<Uri>())).
				Returns((Uri uri) =>
				{
					string data;
					if (!this.cache.TryGetValue(uri, out data))
					{
						data = this.cache[uri] = this.sequence.Count > 0 ?
							this.GetResource(this.sequence.Dequeue()) :
							null;
					}
					
					return data;
				});
		}
		
		[Test]
		public void TestUnknown()
		{
			this.ExecuteAllScheduled(1);
			
			Assert.AreEqual(BurritoDayState.Unknown, this.model.State);
			Assert.AreEqual(TimeSpan.Zero, this.model.Duration);
			Assert.AreEqual(null, this.model.Location);
		}
		
		[Test]
		public void TestNo()
		{
			this.sequence.Enqueue("no.html");
			this.ExecuteAllScheduled(1);
			
			Assert.AreEqual(BurritoDayState.No, this.model.State);
			Assert.AreEqual(TimeSpan.Zero, this.model.Duration);
			Assert.AreEqual(null, this.model.Location);
		}
		
		[Test]
		public void TestTomorrow()
		{
			this.sequence.Enqueue("tomorrow.html");
			this.ExecuteAllScheduled(1);
			
			Assert.AreEqual(BurritoDayState.Tomorrow, this.model.State);
			Assert.AreEqual(TimeSpan.Zero, this.model.Duration);
			Assert.AreEqual(null, this.model.Location);
		}
		
		[Test]
		public void TestYes()
		{
			this.sequence.Enqueue("yes.html");
			this.ExecuteAllScheduled(2);
			
			Assert.AreEqual(BurritoDayState.Yes, this.model.State);
			Assert.AreEqual(TimeSpan.Zero, this.model.Duration);
			Assert.AreEqual(null, this.model.Location);
		}
		
		[Test]
		public void TestHarbor()
		{
			this.sequence.Enqueue("yes.html");
			this.sequence.Enqueue("harbor.html");
			this.sequence.Enqueue("harbor.xml");
			this.ExecuteAllScheduled(2);
			
			Assert.AreEqual(BurritoDayState.Yes, this.model.State);
			Assert.AreEqual(TimeSpan.Zero, this.model.Duration);
			Assert.AreEqual(null, this.model.Location);
		}
		
		[Test]
		public void TestInTransit()
		{
			this.sequence.Enqueue("yes.html");
			this.sequence.Enqueue("harbor.html");
			this.sequence.Enqueue("transit.xml");
			this.ExecuteAllScheduled(2);
			
			Assert.AreEqual(BurritoDayState.Transit, this.model.State);
			Assert.AreEqual(TimeSpan.FromSeconds(1200), this.model.Duration);
			Assert.AreEqual("Foo St, Bar, CA 00000", this.model.Location);
		}
		
		[Test]
		public void TestArrived()
		{			
			this.TestInTransit();
			this.cache.Clear();
			
			this.sequence.Enqueue("yes.html");
			this.sequence.Enqueue("arrived.html");
			this.sequence.Enqueue("arrived.xml");
			this.ExecuteAllScheduled(1);
			
			Assert.AreEqual(BurritoDayState.Arrived, this.model.State);
			Assert.AreEqual(TimeSpan.Zero, this.model.Duration);
			Assert.AreEqual(null, this.model.Location);
		}
		
		private void ExecuteAllScheduled(int count)
		{
			for (int x = 0; x < count; x++)
			{
				this.fiber.ExecuteAllScheduled();
			}
		}
		
		private string GetResource(string resource)
		{
			using (var stream = base.GetType().Assembly.
				GetManifestResourceStream("Tepeyac.Test.Resources." + resource))
			using (var reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}
	}
}