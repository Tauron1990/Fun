using System.Windows;
using System.Windows.Input;
using EcsRx.Collections;
using EcsRx.Events;
using ImageViewerV3.Core;
using ImageViewerV3.Ecs.Events;
using ImageViewerV3.Ui.Services;
using Ookii.Dialogs.Wpf;

namespace ImageViewerV3.Ui
{
    public class MainWindowConnector : EcsConnector
    {
        public OperationManager OperationManager { get; }
        public FilesManager FilesManager { get; }

        public MainWindowConnector(IEntityCollectionManager entityCollectionManager, IEventSystem eventSystem,
            OperationManager manager, FilesManager filesManager) : base(entityCollectionManager, eventSystem)
        {
            OperationManager = manager;
            FilesManager = filesManager;

            OpenLocationCommand = BindToEvent(OpenLocation);
        }

        private BeginLoadingEvent? OpenLocation(IEntityCollectionManager manager)
        {
            var dialog = new VistaFolderBrowserDialog
            {
                ShowNewFolderButton = false,
                Description = "Ordner mit Bildern"
            };

            return dialog.ShowDialog(Application.Current.MainWindow) == true ? new BeginLoadingEvent(dialog.SelectedPath) : null;
        }

        public ICommand OpenLocationCommand { get; }
    }
}