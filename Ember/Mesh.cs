using System.Net.NetworkInformation;
using SharpDX;

namespace Ember
{
    public class Mesh
    {
        public string Name { get; set; }
        public Vector3[] Vertices { get; private set; }
        public Face[] Faces { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        public Mesh(string name, int verticesCount, int faceCount)
        {
            Vertices = new Vector3[verticesCount];
            Faces = new Face[faceCount];
            Name = name;
        }
    }
}