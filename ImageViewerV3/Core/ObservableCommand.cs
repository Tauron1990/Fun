using System;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using EcsRx.MicroRx.Subjects;

namespace ImageViewerV3.Core
{
    public sealed class ObservableCommand : ICommand, IObservable<Unit>, IDisposable
    {
        private readonly Subject<Unit> _subject = new Subject<Unit>();
        
        public IDisposable Subscribe(IObserver<Unit> observer) 
            => _subject.Subscribe(observer);

        public void Dispose() 
            => _subject.Dispose();

        public bool CanExecute(object parameter) => true;

        public void Execute()
            => Task.Run(() => _subject.OnNext(Unit.Default));

        public void Execute(object parameter)
            => Task.Run(() => _subject.OnNext(Unit.Default));

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}