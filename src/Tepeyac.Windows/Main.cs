using System.Windows.Forms;
using Funq;
using Retlang.Core;
using Retlang.Fibers;
using Tepeyac.Core;
using Tepeyac.UI.WinForms;
using Tepeyac.UI;

namespace Tepeyac.Windows
{
    class MainClass
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var container = new Container();
            Tepeyac.Funq.Registry.Register(container);

            container.Register<IFiber>("GuiFiber", c => c.Resolve<IFiber>());
            /*
            container.Register<IFiber>("GuiFiber", c =>
            {
                var executor =
                    c.Resolve<IExecutor>() ??
                    new Executor();
                var invoke = new SynchronizeInvoke(new WindowsFormsSynchronizationContext());
                var fiber = new FormFiber(invoke, executor);
                fiber.Start();

                return fiber;
            });
             */

            container.Register<IBurritoDayView>(c =>
                new BurritoDayView(c));

            container.Resolve<IBurritoDayView>();
            Application.Run();
        }
    }
}
