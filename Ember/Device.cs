using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using SharpDX;

namespace Ember
{
    public class Device
    {
        private byte[] backBuffer;
        private readonly float[] depthBuffer;
        private object[] lockBuffer;
        private WriteableBitmap bmp;
        private readonly int renderWidth;
        private readonly int renderHeight;

        public Device(WriteableBitmap bmp)
        {
            this.bmp = bmp;
            renderWidth = bmp.PixelWidth;
            renderHeight = bmp.PixelHeight;

            // the back buffer size is equal to the number of pixels to draw
            // on screen (width*height) * 4 (R,G,B & Alpha values). 
            backBuffer = new byte[renderWidth * renderHeight * 4];
            depthBuffer = new float[renderWidth * renderHeight];
            lockBuffer = new object[renderWidth * renderHeight];
            for (var i = 0; i < lockBuffer.Length; i++)
            {
                lockBuffer[i] = new object();
            }
        }

        // Called to put a pixel on screen at a specific X,Y coordinates
        public void PutPixel(int x, int y, float z, Color4 color)
        {
            // As we have a 1-D Array for our back buffer
            // we need to know the equivalent cell in 1-D based
            // on the 2D coordinates on screen
            var index = (x + y * renderWidth);
            var index4 = index * 4;

            // Protecting our buffer against threads concurrencies
            lock (lockBuffer[index])
            {
                if (depthBuffer[index] < z)
                {
                    return; // Discard
                }

                depthBuffer[index] = z;

                backBuffer[index4] = (byte)(color.Blue * 255);
                backBuffer[index4 + 1] = (byte)(color.Green * 255);
                backBuffer[index4 + 2] = (byte)(color.Red * 255);
                backBuffer[index4 + 3] = (byte)(color.Alpha * 255);
            }
        }

        // This method is called to clear the back buffer with a specific color
        public void Clear(byte r, byte g, byte b, byte a)
        {
            // Clearing Back Buffer
            for (var index = 0; index < backBuffer.Length; index += 4)
            {
                // BGRA is used by Windows instead by RGBA in HTML5
                backBuffer[index] = b;
                backBuffer[index + 1] = g;
                backBuffer[index + 2] = r;
                backBuffer[index + 3] = a;
            }

            // Clearing Depth Buffer
            for (var index = 0; index < depthBuffer.Length; index++)
            {
                depthBuffer[index] = float.MaxValue;
            }
        }

        // DrawPoint calls PutPixel but does the clipping operation before
        public void DrawPoint(Vector3 point, Color4 color)
        {
            // Clipping what's visible on screen
            if (point.X >= 0 && point.Y >= 0 && point.X < renderWidth && point.Y < renderHeight)
            {
                // Drawing a point
                PutPixel((int)point.X, (int)point.Y, point.Z, color);
            }
        }

        // Once everything is ready, we can flush the back buffer
        // into the front buffer. 
        public void Present()
        {
            using (var stream = bmp.PixelBuffer.AsStream())
            {
                // writing our byte[] back buffer into our WriteableBitmap stream
                stream.Write(backBuffer, 0, backBuffer.Length);
            }
            // request a redraw of the entire bitmap
            bmp.Invalidate();
        }

        // Clamping values to keep them between 0 and 1
        float Clamp(float value, float min = 0, float max = 1)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        // Interpolating the value between 2 vertices 
        // min is the starting point, max the ending point
        // and gradient the % between the 2 points
        float Interpolate(float min, float max, float gradient)
        {
            return min + (max - min) * Clamp(gradient);
        }

        // Project takes some 3D coordinates and transform them
        // in 2D coordinates using the transformation matrix
        // It also transform the same coordinates and the normal to the vertex 
        // in the 3D world
        public Vertex Project(Vertex vertex, Matrix transMat, Matrix world)
        {
            // transforming the coordinates into 2D space
            var point2d = Vector3.TransformCoordinate(vertex.Coordinates, transMat);
            // transforming the coordinates & the normal to the vertex in the 3D world
            var point3dWorld = Vector3.TransformCoordinate(vertex.Coordinates, world);
            var normal3dWorld = Vector3.TransformCoordinate(vertex.Normal, world);

            // The transformed coordinates will be based on coordinate system
            // starting on the center of the screen. But drawing on screen normally starts
            // from top left. We then need to transform them again to have x:0, y:0 on top left.
            var x = point2d.X * renderWidth + renderWidth / 2.0f;
            var y = -point2d.Y * renderHeight + renderHeight / 2.0f;

            return new Vertex
            {
                Coordinates = new Vector3(x, y, point2d.Z),
                Normal = normal3dWorld,
                WorldCoordinates = point3dWorld
            };
        }

        // Compute the cosine of the angle between the light vector and the normal vector
        // Returns a value between 0 and 1
        float ComputeNDotL(Vector3 vertex, Vector3 normal, Vector3 lightPosition) 
        {
            var lightDirection = lightPosition - vertex;

            normal.Normalize();
            lightDirection.Normalize();

            return Math.Max(0, Vector3.Dot(normal, lightDirection));
        }
        // public void DrawLine(Vector2 point0, Vector2 point1)
        // {
        //     var dist = (point1 - point0).Length();
        //
        //     // If the distance between the 2 points is less than 2 pixels
        //     // We're exiting
        //     if (dist < 2)
        //         return;
        //
        //     // Find the middle point between first & second point
        //     Vector2 middlePoint = point0 + (point1 - point0)/2;
        //     // We draw this point on screen
        //     DrawPoint(middlePoint);
        //     // Recursive algorithm launched between first & middle point
        //     // and between middle & second point
        //     DrawLine(point0, middlePoint);
        //     DrawLine(middlePoint, point1);
        // }

        // public void DrawBLine(Vector2 point0, Vector2 point1)
        // {
        //     var x0 = (int)point0.X;
        //     var y0 = (int)point0.Y;
        //     var x1 = (int)point1.X;
        //     var y1 = (int)point1.Y;
        //     
        //     var dx = Math.Abs(x1 - x0);
        //     var dy = Math.Abs(y1 - y0);
        //     var sx = (x0 < x1) ? 1 : -1;
        //     var sy = (y0 < y1) ? 1 : -1;
        //     var err = dx - dy;
        //
        //     while (true) {
        //         DrawPoint(new Vector2(x0, y0));
        //
        //         if ((x0 == x1) && (y0 == y1))
        //             break;
        //         var e2 = 2 * err;
        //         if (e2 > -dy) { err -= dy;
        //             x0 += sx;
        //         }
        //         if (e2 < dx) { err += dx;
        //             y0 += sy;
        //         }
        //     }
        // }
        
        // drawing line between 2 points from left to right
        // papb -> pcpd
        // pa, pb, pc, pd must then be sorted before
        private void ProcessScanLine(ScanLineData data, Vertex va, Vertex vb, Vertex vc, Vertex vd, Color4 color)
        {
            Vector3 pa = va.Coordinates;
            Vector3 pb = vb.Coordinates;
            Vector3 pc = vc.Coordinates;
            Vector3 pd = vd.Coordinates;

            // Thanks to current Y, we can compute the gradient to compute others values like
            // the starting X (sx) and ending X (ex) to draw between
            // if pa.Y == pb.Y or pc.Y == pd.Y, gradient is forced to 1
            var gradient1 = pa.Y != pb.Y ? (data.currentY - pa.Y) / (pb.Y - pa.Y) : 1;
            var gradient2 = pc.Y != pd.Y ? (data.currentY - pc.Y) / (pd.Y - pc.Y) : 1;

            var sx = (int)Interpolate(pa.X, pb.X, gradient1);
            var ex = (int)Interpolate(pc.X, pd.X, gradient2);

            // starting Z & ending Z
            var z1 = Interpolate(pa.Z, pb.Z, gradient1);
            var z2 = Interpolate(pc.Z, pd.Z, gradient2);

            var snl = Interpolate(data.ndotla, data.ndotlb, gradient1);
            var enl = Interpolate(data.ndotlc, data.ndotld, gradient2);

            // drawing a line from left (sx) to right (ex) 
            for (var x = sx; x < ex; x++)
            {
                var gradient = (x - sx) / (float)(ex - sx);

                var z = Interpolate(z1, z2, gradient);
                var ndotl = Interpolate(snl, enl, gradient);
                // changing the color value using the cosine of the angle
                // between the light vector and the normal vector
                DrawPoint(new Vector3(x, data.currentY, z), color * ndotl);
            }
        }

        public void DrawTriangle(Vertex v1, Vertex v2, Vertex v3, Color4 color)
        {
            // Sorting the points in order to always have this order on screen p1, p2 & p3
            // with p1 always up (thus having the Y the lowest possible to be near the top screen)
            // then p2 between p1 & p3
            if (v1.Coordinates.Y > v2.Coordinates.Y)
            {
                var temp = v2;
                v2 = v1;
                v1 = temp;
            }

            if (v2.Coordinates.Y > v3.Coordinates.Y)
            {
                var temp = v2;
                v2 = v3;
                v3 = temp;
            }

            if (v1.Coordinates.Y > v2.Coordinates.Y)
            {
                var temp = v2;
                v2 = v1;
                v1 = temp;
            }

            var p1 = v1.Coordinates;
            var p2 = v2.Coordinates;
            var p3 = v3.Coordinates;
            
            // Light position 
            var lightPos = new Vector3(0, 10, 10);
            // computing the cos of the angle between the light vector and the normal vector
            // it will return a value between 0 and 1 that will be used as the intensity of the color
            var nl1 = ComputeNDotL(v1.WorldCoordinates, v1.Normal, lightPos);
            var nl2 = ComputeNDotL(v2.WorldCoordinates, v2.Normal, lightPos);
            var nl3 = ComputeNDotL(v3.WorldCoordinates, v3.Normal, lightPos);

            var data = new ScanLineData { };
            

            // inverse slopes
            float dP1P2, dP1P3;

            // Computing inverse slopes
            if (p2.Y - p1.Y > 0)
                dP1P2 = (p2.X - p1.X) / (p2.Y - p1.Y);
            else
                dP1P2 = 0;

            if (p3.Y - p1.Y > 0)
                dP1P3 = (p3.X - p1.X) / (p3.Y - p1.Y);
            else
                dP1P3 = 0;

            // First case where triangles are like that:
            // P1
            // -
            // -- 
            // - -
            // -  -
            // -   - P2
            // -  -
            // - -
            // -
            // P3
            if (dP1P2 > dP1P3)
            {
                // for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
                // {
                //     if (y < p2.Y)
                //     {
                //         ProcessScanLine(y, p1, p3, p1, p2, color);
                //     }
                //     else
                //     {
                //         ProcessScanLine(y, p1, p3, p2, p3, color);
                //     }
                // }
                
                for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
                {
                    data.currentY = y;

                    if (y < p2.Y)
                    {
                        data.ndotla = nl1;
                        data.ndotlb = nl3;
                        data.ndotlc = nl1;
                        data.ndotld = nl2;
                        ProcessScanLine(data, v1, v3, v1, v2, color);
                    }
                    else
                    {
                        data.ndotla = nl1;
                        data.ndotlb = nl3;
                        data.ndotlc = nl2;
                        data.ndotld = nl3;
                        ProcessScanLine(data, v1, v3, v2, v3, color);
                    }
                }
            }
            // First case where triangles are like that:
            //       P1
            //        -
            //       -- 
            //      - -
            //     -  -
            // P2 -   - 
            //     -  -
            //      - -
            //        -
            //       P3
            else
            {
                // for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
                // {
                //     if (y < p2.Y)
                //     {
                //         ProcessScanLine(y, p1, p2, p1, p3, color);
                //     }
                //     else
                //     {
                //         ProcessScanLine(y, p2, p3, p1, p3, color);
                //     }
                // }
                
                for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
                {
                    data.currentY = y;

                    if (y < p2.Y)
                    {
                        data.ndotla = nl1;
                        data.ndotlb = nl2;
                        data.ndotlc = nl1;
                        data.ndotld = nl3;
                        ProcessScanLine(data, v1, v2, v1, v3, color);
                    }
                    else
                    {
                        data.ndotla = nl2;
                        data.ndotlb = nl3;
                        data.ndotlc = nl1;
                        data.ndotld = nl3;
                        ProcessScanLine(data, v2, v3, v1, v3, color);
                    }
                }
            }
            
            //if (dP1P2 > dP1P3)
            //{
            //    Parallel.For((int)p1.Y, (int)p3.Y + 1, y =>
            //        {
            //            if (y < p2.Y)
            //            {
            //                ProcessScanLine(y, p1, p3, p1, p2, color);
            //            }
            //            else
            //            {
            //                ProcessScanLine(y, p1, p3, p2, p3, color);
            //            }
            //        });
            //}
            //else
            //{
            //    Parallel.For((int)p1.Y, (int)p3.Y + 1, y =>
            //        {
            //            if (y < p2.Y)
            //            {
            //                ProcessScanLine(y, p1, p2, p1, p3, color);
            //            }
            //            else
            //            {
            //                ProcessScanLine(y, p2, p3, p1, p3, color);
            //            }
            //        });
            //}
        }
        
        // The main method of the engine that re-compute each vertex projection
        // during each frame
        public void Render(Camera camera, params Mesh[] meshes)
        {
            var viewMatrix = Matrix.LookAtLH(camera.Position, camera.Target, Vector3.UnitY);
            var projectionMatrix = Matrix.PerspectiveFovRH(0.78f, 
                                                           (float)renderWidth / renderHeight, 
                                                           0.01f, 1.0f);

            foreach (var mesh in meshes) 
            {
                // Beware to apply rotation before translation 
                var worldMatrix = Matrix.RotationYawPitchRoll(mesh.Rotation.Y, mesh.Rotation.X, mesh.Rotation.Z) * 
                                  Matrix.Translation(mesh.Position);

                var transformMatrix = worldMatrix * viewMatrix * projectionMatrix;

                // foreach (var vertex in mesh.Vertices)
                // {
                //     // First, we project the 3D coordinates into the 2D space
                //     var point = Project(vertex, transformMatrix);
                //     // Then we can draw on screen
                //     DrawPoint(point);
                // }
                
                // for (var i = 0; i < mesh.Vertices.Length - 1; i++)
                // {
                //     var point0 = Project(mesh.Vertices[i], transformMatrix);
                //     var point1 = Project(mesh.Vertices[i + 1], transformMatrix);
                //     DrawLine(point0, point1);
                // }
                
                // var faceIndex = 0;
                //
                // foreach (var face in mesh.Faces)
                // {
                //     var vertexA = mesh.Vertices[face.A];
                //     var vertexB = mesh.Vertices[face.B];
                //     var vertexC = mesh.Vertices[face.C];
                //
                //     var pixelA = Project(vertexA, transformMatrix);
                //     var pixelB = Project(vertexB, transformMatrix);
                //     var pixelC = Project(vertexC, transformMatrix);
                //
                //     // DrawBLine(pixelA, pixelB);
                //     // DrawBLine(pixelB, pixelC);
                //     // DrawBLine(pixelC, pixelA);
                //     
                //     var color = 0.25f + (faceIndex % mesh.Faces.Length) * 0.75f / mesh.Faces.Length;
                //     DrawTriangle(pixelA, pixelB, pixelC, new Color4(color, color, color, 1));
                //     faceIndex++;
                // }
                
                Parallel.For(0, mesh.Faces.Length, faceIndex =>
                {                    
                    var face = mesh.Faces[faceIndex];
                    var vertexA = mesh.Vertices[face.A];
                    var vertexB = mesh.Vertices[face.B];
                    var vertexC = mesh.Vertices[face.C];

                    var pixelA = Project(vertexA, transformMatrix, worldMatrix);
                    var pixelB = Project(vertexB, transformMatrix, worldMatrix);
                    var pixelC = Project(vertexC, transformMatrix, worldMatrix);

                    //var color = 0.25f + (faceIndex % mesh.Faces.Length) * 0.75f / mesh.Faces.Length;
                    var color = 1.0f;
                    DrawTriangle(pixelA, pixelB, pixelC, new Color4(color, color, color, 1));

                    faceIndex++;
                });
            }
        }
        
        // Loading the JSON file in an asynchronous manner
        public async Task<Mesh[]> LoadJSONFileAsync(string fileName)
        {
            var meshes = new List<Mesh>();
            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(fileName);
            var data = await Windows.Storage.FileIO.ReadTextAsync(file);
            dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(data);

            for (var meshIndex = 0; meshIndex < jsonObject.meshes.Count; meshIndex++)
            {
                var verticesArray = jsonObject.meshes[meshIndex].vertices;
                // Faces
                var indicesArray = jsonObject.meshes[meshIndex].indices;

                var uvCount = jsonObject.meshes[meshIndex].uvCount.Value;
                var verticesStep = 1;

                // Depending of the number of texture's coordinates per vertex
                // we're jumping in the vertices array  by 6, 8 & 10 windows frame
                switch ((int)uvCount)
                {
                    case 0:
                        verticesStep = 6;
                        break;
                    case 1:
                        verticesStep = 8;
                        break;
                    case 2:
                        verticesStep = 10;
                        break;
                }

                // the number of interesting vertices information for us
                var verticesCount = verticesArray.Count / verticesStep;
                // number of faces is logically the size of the array divided by 3 (A, B, C)
                var facesCount = indicesArray.Count / 3;
                var mesh = new Mesh(jsonObject.meshes[meshIndex].name.Value, verticesCount, facesCount);

                // Filling the Vertices array of our mesh first
                for (var index = 0; index < verticesCount; index++)
                {
                    var x = (float)verticesArray[index * verticesStep].Value;
                    var y = (float)verticesArray[index * verticesStep + 1].Value;
                    var z = (float)verticesArray[index * verticesStep + 2].Value;
                    // Loading the vertex normal exported by Blender
                    var nx = (float)verticesArray[index * verticesStep + 3].Value;
                    var ny = (float)verticesArray[index * verticesStep + 4].Value;
                    var nz = (float)verticesArray[index * verticesStep + 5].Value;
                    mesh.Vertices[index] = new Vertex{ Coordinates= new Vector3(x, y, z), Normal= new Vector3(nx, ny, nz) };
                }

                // Then filling the Faces array
                for (var index = 0; index < facesCount; index++)
                {
                    var a = (int)indicesArray[index * 3].Value;
                    var b = (int)indicesArray[index * 3 + 1].Value;
                    var c = (int)indicesArray[index * 3 + 2].Value;
                    mesh.Faces[index] = new Face { A = a, B = b, C = c };
                }

                // Getting the position you've set in Blender
                var position = jsonObject.meshes[meshIndex].position;
                mesh.Position = new Vector3((float)position[0].Value, (float)position[1].Value, (float)position[2].Value);
                meshes.Add(mesh);
            }
            return meshes.ToArray();
        }
    }
}