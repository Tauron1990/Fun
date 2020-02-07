using System;
using System.Reactive.Disposables;
using Reactive.Bindings;

namespace ImageViewerV3.Ui.Services
{
    public sealed class OperationEntry : IDisposable
    {
        private string Name { get; }

        public int Id { get; }
        public ReactiveProperty<bool> IsRunning { get; }

        public ReactiveProperty<bool> IsSheduled { get; }

        public OperationEntry(string name, int id, ReactiveProperty<bool> isRunning, ReactiveProperty<bool> isSheduled)
        {
            Name = name;
            Id = id;
            IsRunning = isRunning;
            IsSheduled = isSheduled;
        }

        public override string ToString() 
            => $"{Id}: {Name}";

        public CompositeDisposable CompositeDisposable { get; } = new CompositeDisposable();

        public void Dispose() 
            => CompositeDisposable.Dispose();
    }
}