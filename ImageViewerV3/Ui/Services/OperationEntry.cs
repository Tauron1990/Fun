using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using Reactive.Bindings;

namespace ImageViewerV3.Ui.Services
{
    public sealed class OperationEntry : IDisposable
    {
        private string Name { get; }

        public int Id { get; }
        public Subject<Unit> Finish { get; } = new Subject<Unit>();

        public OperationEntry(string name, int id)
        {
            Name = name;
            Id = id;
            CompositeDisposable.Add(Finish);
        }

        public override string ToString() 
            => $"{Id}: {Name}";

        public CompositeDisposable CompositeDisposable { get; } = new CompositeDisposable();

        public void Dispose()
        {
            Finish.OnNext(Unit.Default);
            CompositeDisposable.Dispose();
        }
    }
}