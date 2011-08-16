using System;
using System.ComponentModel;
using System.Threading;

namespace Tepeyac.UI.WinForms
{
    public class SynchronizeInvoke : ISynchronizeInvoke
    {
        private readonly SynchronizationContext context;

        public SynchronizeInvoke(SynchronizationContext context)
        {
            this.context = context;
        }

        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            this.context.Post(x => method.DynamicInvoke(args), null);
            return null;
        }

        public object EndInvoke(IAsyncResult result)
        {
            return null;
        }

        public object Invoke(Delegate method, object[] args)
        {
            return null;
        }

        public bool InvokeRequired
        {
            get { return true; }
        }
    }
}
