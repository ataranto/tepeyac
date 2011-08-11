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
		}
		
		[Test]
		public void TestNo()
		{
			this.sequence.Enqueue("no.html");
			this.ExecuteAllScheduled(1);
			
			Assert.AreEqual(BurritoDayState.No, this.model.State);
		}
		
		[Test]
		public void TestTomorrow()
		{
			this.sequence.Enqueue("tomorrow.html");
			this.ExecuteAllScheduled(1);
			
			Assert.AreEqual(BurritoDayState.Tomorrow, this.model.State);
		}
		
		[Test]
		public void TestYes()
		{
			this.sequence.Enqueue("yes.html");
			this.ExecuteAllScheduled(2);
			
			Assert.AreEqual(BurritoDayState.Yes, this.model.State);
		}
		
		[Test]
		public void TestInTransit()
		{
			this.sequence.Enqueue("yes.html");
			this.sequence.Enqueue("harbor.html");
			this.sequence.Enqueue("harbor.xml");
			this.ExecuteAllScheduled(2);
			
			Assert.AreEqual(BurritoDayState.Transit, this.model.State);
		}
		
		[Test]
		public void TestArrived()
		{
			this.sequence.Enqueue("yes.html");
			this.sequence.Enqueue("arrived.html");
			this.sequence.Enqueue("arrived.xml");
			this.ExecuteAllScheduled(2);
			
			Assert.AreEqual(BurritoDayState.Arrived, this.model.State);
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
				GetManifestResourceStream("Tepeyac.Core.Test." + resource))
			using (var reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}
	}
}