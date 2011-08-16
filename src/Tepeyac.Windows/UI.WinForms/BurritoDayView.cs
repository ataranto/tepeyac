using System;
using Funq;
using System.Collections.Generic;
using System.Windows.Forms;
using Tepeyac.Windows.Properties;
using Tepeyac.Core;
using System.Drawing;

namespace Tepeyac.UI.WinForms
{
    public class BurritoDayView : IBurritoDayView
    {
        private readonly NotifyIcon icon;
        private readonly ICollection<IDisposable> presenters;

        private readonly MenuItem refreshMenuItem = new MenuItem("Refresh");

        public BurritoDayView(Container container)
        {
            this.icon = new NotifyIcon();
            this.icon.ContextMenu = new ContextMenu(new MenuItem[]
            {
                this.refreshMenuItem,
            });

            this.presenters = new IDisposable[]
            {
                container.Resolve<BurritoDayPresenter, IBurritoDayView>(this),
            };
        }

        public void Dispose()
        {

        }

        #region IView

        bool IView.Visible
        {
            get { return this.icon.Visible; }
            set { this.icon.Visible = value; }
        }

        #endregion

        #region IBurritoDayView

        event EventHandler IBurritoDayView.RefreshActivated
        {
            add { this.refreshMenuItem.Click += value; }
            remove { this.refreshMenuItem.Click += value; }
        }

        event EventHandler IBurritoDayView.DismissActivated
        {
            add { }
            remove { }
        }

        void IBurritoDayView.SetState(Core.BurritoDayState state, string description)
        {
            Bitmap bitmap;
            switch (state)
            {
                case BurritoDayState.Tomorrow:
                    bitmap = Resources.tomorrow;
                    break;
                case BurritoDayState.Yes:
                    bitmap = Resources.yes;
                    break;
                case BurritoDayState.Transit:
                    bitmap = Resources.transit;
                    break;
                case BurritoDayState.Arrived:
                    bitmap = Resources.arrived;
                    break;
                default:
                    bitmap = Resources.no;
                    break;
            }
        }

        #endregion
    }
}
