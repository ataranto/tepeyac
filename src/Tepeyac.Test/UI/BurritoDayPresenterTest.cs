using System;
using Moq;
using NUnit.Framework;
using Retlang.Fibers;
using Tepeyac.Core;
using Tepeyac.UI;

namespace Tepeyac.Test.UI
{
	[TestFixture]
	public class BurritoDayPresenterTest : MoqTestFixture
	{
		private Mock<IBurritoDayModel> mockModel;
		private Mock<IBurritoDayView> mockView;
		private IFiber fiber;
		
		protected override void SetUp ()
		{
			this.mockModel = base.CreateMock<IBurritoDayModel>();
			this.mockView = base.CreateMock<IBurritoDayView>();
			this.fiber = new StubFiber() { ExecutePendingImmediately = true };
			
			new BurritoDayPresenter(this.mockModel.Object, this.mockView.Object, this.fiber);
		}
		
		[Test]
		public void TestModelChanged()
		{
			var state = BurritoDayState.Yes;
			
			this.mockModel.SetupGet(m => m.State).Returns(state);
			this.mockView.SetupSet(m => m.Visible = true);
			this.mockView.Setup(m => m.SetState(state, It.IsAny<string>()));
			this.mockModel.Raise(m => m.Changed += null, EventArgs.Empty);	
		}
		
		[Test]
		public void TestViewRefreshActivated()
		{
			this.mockModel.Setup(m => m.Refresh());
			this.mockView.Raise(m => m.RefreshActivated += null, EventArgs.Empty);	
		}
		
		[Test]
		public void TestViewDismissActivated()
		{
			this.mockView.SetupSet(m => m.Visible = false);
			this.mockView.Raise(m => m.DismissActivated += null, EventArgs.Empty);
		}
	}
}