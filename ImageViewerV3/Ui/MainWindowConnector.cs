using System.Windows.Input;
using ImageViewerV3.Core;
using ImageViewerV3.Ecs.Events;
using ImageViewerV3.Ui.Services;

namespace ImageViewerV3.Ui
{
    public class MainWindowConnector : EcsConnector
    {
        public OperationManager OperationManager { get; }

        public MainWindowConnector(OperationManager manager)
        {
            OperationManager = manager;

            TestOp = BindToEvent((k, m) => new StartOperationEvent("Hallo"));
        }

        public ICommand TestOp { get; }
    }
}