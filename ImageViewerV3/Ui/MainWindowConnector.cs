using System;
using System.ComponentModel;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
using Syncfusion.Windows.Shared;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ui
{
    public class MainWindowConnector : EcsConnector
    {
        private readonly IAppStates _appStatem;
        private readonly FullScreenManager _fullScreenManager;

        private int _fileListIndex;

        public OperationManager OperationManager { get; }
        public FilesManager FilesManager { get; }
        public ImageManager ImageManager { get; }
        public UIWindowState WindowState { get; }

        public MainWindowConnector(IListManager listManager, IEventSystem eventSystem, IAppStates appStatem,
                                   OperationManager manager, FilesManager filesManager, ImageManager imageManager) : base(listManager, eventSystem)
        {
            _appStatem = appStatem;
            
            ReactOn<PrepareLoadEvent>(_ => FileListIndex = 0);
            ReactOn<PostLoadingEvent>(e => _appStatem.Set<GlobalAppState>(s => s.LastLocation = e.Path));
            DisposeThis(filesManager.FilterObservable.Subscribe(_ => FileListIndex = 1));

            OperationManager = manager;
            FilesManager = filesManager;
            ImageManager = imageManager;
            WindowState = new UIWindowState(_appStatem);
            _fullScreenManager = new FullScreenManager(WindowState);

            OpenLocationCommand = BindToEvent(OpenLocation);
            NextImage = BindToEvent(_ => new NextPageEvnt(false));
            BackImage = BindToEvent(_ => new NextPageEvnt(true));
            DeleteCommand = BindToEvent(
                _ => MessageBox.Show("Wírklich Löschen?", "Löschen", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes
                    ? new DeleteEvent(ImageManager.CurrentIndex)
                    : null);
            ToogleFavoriteCommand = BindToEvent(_ => new ToogleFavoritesEvent(ImageManager.CurrentIndex), postExec:() => FilesManager.ForceUpdate());

            FullScreen = new DelegateCommand(_ => _fullScreenManager.EnableFullScreen(), _ => true);
        }

        [UsedImplicitly]
        public void OnLoadWindow()
        {
            var dic = _appStatem.Get<GlobalAppState>().LastLocation;
            if (!Directory.Exists(dic))
                return;

            SendEvent(new BeginLoadingEvent(dic));
        }

        public void OnKeyDowm(KeyEventArgs args)
        {
            if(args.Key == Key.Escape)
                _fullScreenManager.DisableFullScreen();
        }

        private BeginLoadingEvent? OpenLocation(IListManager manager)
        {
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

        public ICommand FullScreen { get; }

        public ICommand ToogleFavoriteCommand { get; }

        public sealed class UIWindowState : INotifyPropertyChanged
        {
            private readonly IAppStates _stats;
            private readonly Subject<Unit> _subject = new Subject<Unit>();

            private WindowStyle _windowStyle;
            private WindowState _windowState;
            private Visibility _controlVisibility;
            private double _height;
            private double _width;
            private bool _topMost;


            public Visibility ControlVisibility
            {
                get => _controlVisibility;
                set
                {
                    if (value == _controlVisibility) return;
                    _controlVisibility = value;
                    OnPropertyChanged();
                }
            }

            public WindowState WindowState
            {
                get => _windowState;
                set
                {
                    if (value == _windowState) return;
                    _windowState = value;
                    OnPropertyChanged();
                }
            }

            public WindowStyle WindowStyle
            {
                get => _windowStyle;
                set
                {
                    if (value == _windowStyle) return;
                    _windowStyle = value;
                    OnPropertyChanged();
                }
            }

            public double Height
            {
                get => _height;
                set
                {
                    if (value == _height) return;
                    _height = value;
                    OnPropertyChanged();
                }
            }

            public double Width
            {
                get => _width;
                set
                {
                    if (value == _width) return;
                    _width = value;
                    OnPropertyChanged();
                }
            }

            public bool TopMost
            {
                get => _topMost;
                set
                {
                    if (value == _topMost) return;
                    _topMost = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            [NotifyPropertyChangedInvocator]
            private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                _subject.OnNext(Unit.Default);
            }

            public void Save()
            {
                _stats
                   .Set<AppWindowState>(
                        s =>
                        {
                            s.Height = Height;
                            s.Width = Width;
                            s.WindowStyle = WindowStyle;
                            s.WindowState = WindowState;
                            s.ControlVisibility = ControlVisibility;
                            s.TopMost = TopMost;
                        });
            }

            public UIWindowState(IAppStates stats)
            {
                _stats = stats;
                var window = stats.Get<AppWindowState>();

                Height = window.Height;
                Width = window.Height;
                WindowStyle = window.WindowStyle;
                WindowState = window.WindowState;
                ControlVisibility = window.ControlVisibility;
                TopMost = window.TopMost;

                _subject.Delay(TimeSpan.FromMilliseconds(2000)).Subscribe(_ => Save());
            }
        }

        private sealed class FullScreenManager
        {
            private readonly UIWindowState _state;
            private bool _isFullScreen;

            private WindowState _oldState = System.Windows.WindowState.Maximized;

            public FullScreenManager(UIWindowState state)
            {
                _state = state;
                _isFullScreen = state.TopMost;
            }

            public void DisableFullScreen()
            {
                lock (this)
                {
                    if(!_isFullScreen) return;
                    _isFullScreen = false;
                }

                _state.ControlVisibility = Visibility.Visible;
                _state.TopMost = false;
                _state.WindowState = _oldState;
                _state.WindowStyle = WindowStyle.SingleBorderWindow;

                _state.Save();
            }

            public void EnableFullScreen()
            {
                lock (this)
                {
                    if(_isFullScreen) return;
                    _isFullScreen = true;
                }

                _oldState = _state.WindowState;
                _state.ControlVisibility = Visibility.Collapsed;
                _state.TopMost = true; 
                _state.WindowStyle = WindowStyle.None;

                _state.Save();
            }
        }
    }
}