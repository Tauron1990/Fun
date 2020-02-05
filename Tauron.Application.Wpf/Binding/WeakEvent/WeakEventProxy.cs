using System;

namespace Tauron.Application.Wpf.Binding.WeakEvent
{
    /// <summary>
    ///     An event handler wrapper used to create weak-reference event handlers, so that event subscribers
    ///     can be garbage collected without the event publisher interfering.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of event arguments used in the event handler.</typeparam>
    /// <remarks>
    ///     To understand why this class is needed, see this page:
    ///     http://www.paulstovell.net/blog/index.php/wpf-binding-bug-leads-to-possible-memory-issues/
    ///     For examples on how this is used, it is best to look at the unit test:
    ///     WeakEventProxyTests.cs
    /// </remarks>
    internal sealed class WeakEventProxy<TEventArgs> : IDisposable
        where TEventArgs : EventArgs
    {
        private readonly object _syncRoot = new object();
        private WeakReference? _callbackReference;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeakEventProxy&lt;TEventArgs&gt;" /> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public WeakEventProxy(EventHandler<TEventArgs> callback) 
            => _callbackReference = new WeakReference(callback, true);

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            lock (_syncRoot)
            {
                if (_callbackReference != null)
                    //test for null in case the reference was already cleared
                    _callbackReference.Target = null;

                _callbackReference = null;
            }
        }


        /// <summary>
        ///     Used as the event handler which should be subscribed to source collections.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Handler(object sender, TEventArgs e)
        {
            //acquire callback, if any
            EventHandler<TEventArgs>? callback;
            lock (_syncRoot) callback = _callbackReference?.Target as EventHandler<TEventArgs>;

            callback?.Invoke(sender, e);
        }
    }
}