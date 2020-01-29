using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using EcsRx.Collections;
using EcsRx.Events;
using EcsRx.Plugins.Computeds;
using EcsRx.ReactiveData;
using JetBrains.Annotations;
using Ninject;
using Syncfusion.Windows.Shared;

namespace ImageViewerV3.Core
{
    public abstract class EcsConnector : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] [CanBeNull] string? propertyName = null) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected ReactiveProperty<TData> Track<TData>(IComputed<TData> data, string name)
        {
            var prop = new ReactiveProperty<TData>(data);
            return Track(prop, name);
        }

        protected ReactiveProperty<TData> Track<TData>(ReactiveProperty<TData> data, string name)
        {
            data.Subscribe(_ => OnPropertyChanged(name));
            return data;
        }

        public ICommand BindToEvent<TEvent>(Func<IKernel, IEntityCollectionManager, TEvent> exec, Func<IKernel, IEntityCollectionManager, bool>? canExec = null)
        {
            var manager = App.Kernel.Get<IEntityCollectionManager>();
            var eventSystem = App.Kernel.Get<IEventSystem>();

            // ReSharper disable once ImplicitlyCapturedClosure
            async void Exec(object arg) =>
                await Task.Run(() =>
                {
                    var e = exec(App.Kernel, manager);

                    eventSystem.Publish(e);
                });

            // ReSharper disable once ImplicitlyCapturedClosure
            bool CanExec(object arg)
            {
                if (canExec == null)
                    return true;

                return canExec(App.Kernel, manager);
            }

            return new DelegateCommand(Exec, CanExec);
        }
    }
}