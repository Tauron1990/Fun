using System;
using System.Drawing;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using ImageViewerV3.Ecs.Components;
using ImageViewerV3.Ui.Libs.Gif;
using Vlc.DotNet.Core;
using Vlc.DotNet.Wpf;
using Image = System.Windows.Controls.Image;

namespace ImageViewerV3.Ecs.Services
{
    public class ImageControlFactory : IImageControlFactory, IDisposable
    {
        private readonly IDisposable _subscription;
        private readonly Subject<ImageComponent> _input = new Subject<ImageComponent>();
        private readonly Subject<object> _output = new Subject<object>();

        public IObserver<ImageComponent> Input => _input.AsObserver();

        public IObservable<object> Output => _output.AsObservable();

        public ImageControlFactory()
        {
            _subscription = _input
               .DistinctUntilChanged()
               .ObserveOnDispatcher()
               .Select(CreateView)
               .Subscribe(e => _output.OnNext(e));
        }

        private readonly Lazy<Image> _imageControl = new Lazy<Image>(() => new Image());
        private readonly Lazy<Image> _gifControl = new Lazy<Image>(() =>
                                                                   {
                                                                       var gifImg = new Image();
                                                                       GifAnimationBehavior.SetAutoStart(gifImg, true);
                                                                       GifAnimationBehavior.SetRepeatBehavior(gifImg, RepeatBehavior.Forever);
                                                                       return gifImg;
                                                                   });
        private readonly Lazy<VlcControl> _video = new Lazy<VlcControl>(CreateVideoControl);

        private static VlcControl CreateVideoControl()
        {
            var control = new VlcControl();

            var path = AppDomain.CurrentDomain.BaseDirectory;

            path = Path.Combine(path!, Environment.Is64BitProcess ? @"libvlc\win-x64" : @"libvlc\win-x86");
            var sourceProvider = control.SourceProvider;

            sourceProvider.CreatePlayer(new DirectoryInfo(path), "--repeat");
            sourceProvider.MediaPlayer!.EndReached += (s, e) => Task.Run(() => sourceProvider.MediaPlayer.Play());
            sourceProvider.MediaPlayer!.VideoOutChanged += (sender, args) => Task.Run(() => ((VlcMediaPlayer)sender!).Audio.IsMute = true);

            return control;
        }

        private object CreateView(ImageComponent imageComponent)
        {
            var mem = new MemoryStream(File.ReadAllBytes(imageComponent.FilePath));
            try
            {
                var source = BitmapFrame.Create(mem);
                

                if (imageComponent.FilePath.EndsWith(".gif"))
                {
                    var gifImg = _gifControl.Value;
                    GifAnimationBehavior.SetAnimatedSource(gifImg, source);
                    return gifImg;
                }

                var img = _imageControl.Value;
                img.Source = source;

                return img;
            }
            catch (Exception)
            {
                // ignored
            }

            mem.Position = 0;
            var video = _video.Value;
            video.SourceProvider.MediaPlayer!.Play(mem);

            return video;
        }

        public void Dispose()
        {
            _subscription.Dispose();
            _output.Dispose();
            _input.Dispose();
            if (_video.IsValueCreated)
                _video.Value.Dispose();
        }
    }
}