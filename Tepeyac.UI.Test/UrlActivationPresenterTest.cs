using Moq;
using NUnit.Framework;
using Tepeyac.Test;

namespace Tepeyac.UI.Test
{
	[TestFixture]
	public class UrlActivationPresenterTest : MoqTestFixture
	{
		private Mock<ILauncher> mockModel;
		private Mock<IUrlActivationView> mockView;
		
		protected override void SetUp ()
		{
			this.mockModel = base.CreateMock<ILauncher>();
			this.mockView = base.CreateMock<IUrlActivationView>();
			
			new UrlActivationPresenter(this.mockModel.Object, this.mockView.Object);
		}
		
		[Test]
		public void TestViewActivated()
		{
			var url = "http://www.test.com";
			
			this.mockModel.Setup(m => m.Launch(url));
			this.mockView.Raise(m => m.Activated += null, this.mockView.Object, url);
		}
	}
}