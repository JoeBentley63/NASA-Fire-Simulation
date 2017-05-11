using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace Kinesense.Interfaces.Useful
{
    /// <summary>
    /// forces you to name the thread - helps in debugging
    /// </summary>
    public class NamedBackgroundWorker : BackgroundWorker
    {
        public NamedBackgroundWorker(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null) // Can only set it once
                Thread.CurrentThread.Name = Name;

            base.OnDoWork(e);
        }
    }
}
