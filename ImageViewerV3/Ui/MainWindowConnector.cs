using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using ImageViewerV3.Core;
using ImageViewerV3.Data;
using ImageViewerV3.Data.AppStates;
using ImageViewerV3.Ecs.Events;
using ImageViewerV3.Ui.Services;
using JetBrains.Annotations;
using Ookii.Dialogs.Wpf;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ui
{
    public class MainWindowConnector : EcsConnector
    {
        private readonly IAppState _appStatem;
        private int _fileListIndex;

        public OperationManager OperationManager { get; }
        public FilesManager FilesManager { get; }
        public ImageManager ImageManager { get; }

        public MainWindowConnector(IListManager listManager, IEventSystem eventSystem, IAppState appStatem,
                                   OperationManager manager, FilesManager filesManager, ImageManager imageManager) : base(listManager, eventSystem)
        {
            _appStatem = appStatem;
            ReactOn<PrepareLoadEvent>(_ => FileListIndex = 0);
            ReactOn<PostLoadingEvent>(e => _appStatem.Set<GlobalAppState>(s => s.LastLocation = e.Path));
            DisposeThis(filesManager.FilterObservable.Subscribe(_ => FileListIndex = 1));

            OperationManager = manager;
            FilesManager = filesManager;
            ImageManager = imageManager;

            OpenLocationCommand = BindToEvent(OpenLocation);
            NextImage = BindToEvent(_ => new NextPageEvnt(false));
            BackImage = BindToEvent(_ => new NextPageEvnt(true));
            DeleteCommand = BindToEvent(_ => new DeleteEvent(ImageManager.CurrentIndex));
        }

        [UsedImplicitly]
        public void OnLoadWindow()
        {
            var dic = _appStatem.Get<GlobalAppState>().LastLocation;
            if (!Directory.Exists(dic))
                return;

            SendEvent(new BeginLoadingEvent(dic));
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

        public ICommand DeleteCommand { get; }

        public ICommand OpenLocationCommand { get; }

        public ICommand NextImage { get; }

        public ICommand BackImage { get; }

        public sealed class UIWindowState : INotifyPropertyChanged
        {
            private WindowStyle _windowStyle;
            private WindowState _windowState;
            private Visibility _controlVisibility;


            public Visibility ControlVisibility
            {
                get => _controlVisibility;
                set
                {
                    if (value == _controlVisibility) return;
                    _controlVisibility = value;
                    OnPropertyChanged1();
                }
            }

            public WindowState WindowState
            {
                get => _windowState;
                set
                {
                    if (value == _windowState) return;
                    _windowState = value;
                    OnPropertyChanged1();
                }
            }

            public WindowStyle WindowStyle
            {
                get => _windowStyle;
                set
                {
                    if (value == _windowStyle) return;
                    _windowStyle = value;
                    OnPropertyChanged1();
                }
            }

            public WindowStartupLocation WindowStartupLocation { get; set; }

            public int Height { get; set; }

            public int Width { get; set; }jlk

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            private void OnPropertyChanged1([CallerMemberName] string propertyName = null) 
                => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}