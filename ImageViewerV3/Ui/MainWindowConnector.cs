using System;
using System.Windows;
using System.Windows.Input;
using ImageViewerV3.Core;
using ImageViewerV3.Ecs.Events;
using ImageViewerV3.Ui.Services;
using Ookii.Dialogs.Wpf;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ui
{
    public class MainWindowConnector : EcsConnector
    {
        private int _fileListIndex;
        public OperationManager OperationManager { get; }
        public FilesManager FilesManager { get; }
        public ImageManager ImageManager { get; }

        public MainWindowConnector(IListManager listManager, IEventSystem eventSystem,
                                   OperationManager manager, FilesManager filesManager, ImageManager imageManager) : base(listManager, eventSystem)
        {
            DisposeThis(eventSystem.Receive<PrepareLoadEvent>().Subscribe(_ => FileListIndex = 0));
            DisposeThis(filesManager.FilterObservable.Subscribe(_ => FileListIndex = 1));

            OperationManager = manager;
            FilesManager = filesManager;
            ImageManager = imageManager;

            OpenLocationCommand = BindToEvent(OpenLocation);
        }

        private BeginLoadingEvent? OpenLocation(IListManager manager)
        {
            //.libvlc\win - x64
            var dialog = new VistaFolderBrowserDialog
            {
                ShowNewFolderButton = false,
                Description = "Ordner mit Bildern"
            };

            return Application.Current.Dispatcher != null && Application.Current.Dispatcher.Invoke(() => dialog.ShowDialog(Application.Current.MainWindow)) == true 
                ? new BeginLoadingEvent(dialog.SelectedPath) : null;
        }

        public int FileListIndex
        {
            get => _fileListIndex;
            set
            {
                if(_fileListIndex == value) return;
                _fileListIndex = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenLocationCommand { get; }
    }
}