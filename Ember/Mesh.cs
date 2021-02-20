using System.Net.NetworkInformation;
using SharpDX;

namespace Ember
{
    public class Mesh
    {
        public string Name { get; set; }
        public Vertex[] Vertices { get; private set; }
        public Face[] Faces { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        public Mesh(string name, int verticesCount, int facesCount)
        {
            Vertices = new Vertex[verticesCount];
            Faces = new Face[facesCount];
            Name = name;
        }
    }

    public struct Vertex
    {
        public Vector3 Normal;
        public Vector3 Coordinates;
        public Vector3 WorldCoordinates;
    }
}