using SharpDX;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Ember
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        // private Device device;
        // Mesh _mesh = new Mesh("Cube", 8, 12);
        // Camera _camera = new Camera();
        //
        // private void Page_Loaded(object sender, RoutedEventArgs e)
        // {
        //     // Choose the back buffer resolution here
        //     WriteableBitmap bmp = new WriteableBitmap(640, 480);
        //
        //     device = new Device(bmp);
        //
        //     // Our Image XAML control
        //     frontBuffer.Source = bmp;
        //
        //     _mesh.Vertices[0] = new Vector3(-1, 1, 1);
        //     _mesh.Vertices[1] = new Vector3(1, 1, 1);
        //     _mesh.Vertices[2] = new Vector3(-1, -1, 1);
        //     _mesh.Vertices[3] = new Vector3(1, -1, 1);
        //     _mesh.Vertices[4] = new Vector3(-1, 1, -1);
        //     _mesh.Vertices[5] = new Vector3(1, 1, -1);
        //     _mesh.Vertices[6] = new Vector3(1, -1, -1);
        //     _mesh.Vertices[7] = new Vector3(-1, -1, -1);
        //     
        //     _mesh.Faces[0] = new Face { A = 0, B = 1, C = 2 };
        //     _mesh.Faces[1] = new Face { A = 1, B = 2, C = 3 };
        //     _mesh.Faces[2] = new Face { A = 1, B = 3, C = 6 };
        //     _mesh.Faces[3] = new Face { A = 1, B = 5, C = 6 };
        //     _mesh.Faces[4] = new Face { A = 0, B = 1, C = 4 };
        //     _mesh.Faces[5] = new Face { A = 1, B = 4, C = 5 };
        //     
        //     _mesh.Faces[6] = new Face { A = 2, B = 3, C = 7 };
        //     _mesh.Faces[7] = new Face { A = 3, B = 6, C = 7 };
        //     _mesh.Faces[8] = new Face { A = 0, B = 2, C = 7 };
        //     _mesh.Faces[9] = new Face { A = 0, B = 4, C = 7 };
        //     _mesh.Faces[10] = new Face { A = 4, B = 5, C = 6 };
        //     _mesh.Faces[11] = new Face { A = 4, B = 6, C = 7 };
        //
        //     _camera.Position = new Vector3(0, 0, 10.0f);
        //     _camera.Target = Vector3.Zero;
        //
        //     // Registering to the XAML rendering loop
        //     CompositionTarget.Rendering += CompositionTarget_Rendering;
        // }
        //
        // // Rendering loop handler
        // private void CompositionTarget_Rendering(object sender, object e)
        // {
        //     device.Clear(0, 0, 0, 255);
        //
        //     // rotating slightly the cube during each frame rendered
        //     _mesh.Rotation = new Vector3(_mesh.Rotation.X + 0.01f, _mesh.Rotation.Y + 0.01f, _mesh.Rotation.Z);
        //
        //     // Doing the various matrix operations
        //     device.Render(_camera, _mesh);
        //     // Flushing the back buffer into the front buffer
        //     device.Present();
        // }
        
        private Device _device;
        private Mesh[] _meshes;
        private readonly Camera _mera = new Camera();
        private DateTime _previousDate;
        private readonly Collection<double> _lastFpsValues = new Collection<double>();

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Choose the back buffer resolution here
            var bmp = new WriteableBitmap((int)ActualWidth, (int)ActualHeight);

            // Our Image XAML control
            FrontBuffer.Source = bmp;
            
            _device = new Device(bmp);
            _meshes = await Device.LoadJsonFileAsync("monkey.babylon");
            _mera.Position = new Vector3(0, 0, 10.0f);
            _mera.Target = Vector3.Zero;

            // Registering to the XAML rendering loop
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        // Rendering loop handler
        void CompositionTarget_Rendering(object sender, object e)
        {
            // Fps
            var now = DateTime.Now;
            var currentFps = 1000.0 / (now - _previousDate).TotalMilliseconds;
            _previousDate = now;

            Fps.Text = $"instant {currentFps:0.00} fps";

            if (_lastFpsValues.Count < 60)
            {
                _lastFpsValues.Add(currentFps);
            }
            else
            {
                _lastFpsValues.RemoveAt(0);
                _lastFpsValues.Add(currentFps);
                var totalValues = _lastFpsValues.Sum();

                var averageFps = totalValues / _lastFpsValues.Count;
                AverageFps.Text = $"average {averageFps:0.00} fps";
            }

            _device.Clear(0, 0, 0, 255);

            foreach (var mesh in _meshes) {
                // rotating slightly the meshes during each frame rendered
                //mesh.Rotation = new Vector3(mesh.Rotation.X + 0.01f, mesh.Rotation.Y + 0.01f, mesh.Rotation.Z);
                mesh.Rotation = new Vector3(mesh.Rotation.X, mesh.Rotation.Y + 0.01f, mesh.Rotation.Z);
                //mesh.Position = new Vector3(0, 0, (float)(5 * Math.Cos(alpha)));
            }

            // Doing the various matrix operations
            _device.Render(_mera, _meshes);
            // Flushing the back buffer into the front buffer
            _device.Present();
        }

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
