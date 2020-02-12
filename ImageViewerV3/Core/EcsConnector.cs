using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using JetBrains.Annotations;
using Reactive.Bindings;
using Syncfusion.Windows.Shared;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Core
{
    [PublicAPI]
    public abstract class EcsConnector : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private readonly IListManager _listManager;
        private readonly IEventSystem _eventSystem;

        protected EcsConnector(IListManager listManager, IEventSystem eventSystem)
        {
            _listManager = listManager;
            _eventSystem = eventSystem;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] [CanBeNull] string? propertyName = null) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected ReactiveProperty<TData> Track<TData>(IObservable<TData> data, string name) 
            => Track(data.ToReactiveProperty(), name);

        protected ReactiveProperty<TData> Track<TData>(ReactiveProperty<TData> data, string name)
        {
            _compositeDisposable.Add(data.Subscribe(_ => OnPropertyChanged(name)));
            return data;
        }

        protected ICommand BindToEvent<TEvent>(Func<IListManager, TEvent?> exec, Func<IListManager, bool>? canExec = null, Action? postExec = null)
            where TEvent : class
        {
            var manager = _listManager;

            // ReSharper disable once ImplicitlyCapturedClosure
            async void Exec(object arg) =>
                await Task.Run(() =>
                {
                    var e = exec(manager);
                    if(e == null)
                        return;

                    _eventSystem.Publish(e);

                    postExec?.Invoke();
                });

            // ReSharper disable once ImplicitlyCapturedClosure
            bool CanExec(object arg) => canExec == null || canExec(manager);

            return new DelegateCommand(Exec, CanExec);
        }

        protected async void SendEvent<TEvent>(TEvent @event)
            => await Task.Run(() => _eventSystem.Publish(@event));

        protected void ReactOn<TEvent>(Action<TEvent> handler) 
            => _compositeDisposable.Add(_eventSystem.Receive<TEvent>().Subscribe(handler));

        protected TType DisposeThis<TType>(TType toDispose)
            where TType : IDisposable
        {
            _compositeDisposable.Add(toDispose);
            return toDispose;
        }
        public void Dispose() 
            => _compositeDisposable.Dispose();
    }
}