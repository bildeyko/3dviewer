using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viewer
{
    public class Faces
    {
        public Vector3[] V = new Vector3[4];
        public Vector2[] VT = new Vector2[4];
        public Vector3[] VN = new Vector3[4];
    }

    public class Group
    {
        public string name;
        public List<Faces> F = new List<Faces>();
    }
    public class Obj
    {
        /// <summary>
        /// Vertices
        /// </summary>
        public List<Vector3> V = new List<Vector3>();

        /// <summary>
        /// Texture coordinates
        /// </summary>
        public List<Vector2> VT = new List<Vector2>();

        /// <summary>
        /// Normals
        /// </summary>
        public List<Vector3> VN = new List<Vector3>();

        /// <summary>
        /// Faces
        /// </summary>
        public List<Faces> F = new List<Faces>();

        /// <summary>
        /// Groups
        /// </summary>
        public List<Group> G = new List<Group>();

        /// <summary>
        /// OpenGL primitive type
        /// </summary>
        public PrimitiveType PrimitiveType;

        public Obj()
        {

        }

        public void LoadFile(string file)
        {
            int groupIndex = -1;
            using (var reader = new StreamReader(file))
            {
                while (reader.Peek() >= 0)
                {
                    float x, y, z;
                    var currentLine = reader.ReadLine();                    

                    if (currentLine.StartsWith("#"))
                        continue;
                    if (currentLine.StartsWith("v "))
                    {
                        currentLine = currentLine.Remove(0, 2);
                        var vertices = currentLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (vertices.Length != 3)
                            throw new FormatException("Input file contains vertex without 3 coordinates.");
                        x = Single.Parse(vertices[0]);
                        y = Single.Parse(vertices[1]);
                        z = Single.Parse(vertices[2]);
                        this.V.Add(new Vector3(x, y, z));
                    }
                    else if (currentLine.StartsWith("vt "))
                    {
                        currentLine = currentLine.Remove(0, 3);
                        var textures = currentLine.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                        if (textures.Length != 2)
                            throw new FormatException("Input file contains texture without 2 coordinates.");
                        x = Single.Parse(textures[0]);
                        textures[1] = textures[1].Split(new[] { ' ' }, 2)[0];
                        y = Single.Parse(textures[1]);
                        this.VT.Add(new Vector2(x, y));
                    }
                    else if (currentLine.StartsWith("vn "))
                    {
                        currentLine = currentLine.Remove(0, 3);
                        var verticesNormals = currentLine.Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                        if (verticesNormals.Length != 3)
                            throw new FormatException("Input file contains vertex normal without 3 coordinates.");
                        x = Single.Parse(verticesNormals[0]);
                        y = Single.Parse(verticesNormals[1]);
                        z = Single.Parse(verticesNormals[2]);
                        this.VN.Add(new Vector3(x, y, z));

                    }
                    else if (currentLine.StartsWith("f "))
                    {
                        currentLine = currentLine.Remove(0, 2);
                        var faces = currentLine.Split(new[] { ' ' }, 4);
                        if (faces.Length < 3)
                            throw new FormatException("Input file contains face without 3 points.");
                        if (faces.Length == 3)
                            this.PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType.Triangles;
                        if (faces.Length == 4)
                            this.PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType.Quads;

                        var face = new Faces();
                        int i = 0;
                        foreach (var f in faces)
                        {
                            var components = f.Split(new[] { '/' }, 3, StringSplitOptions.None);

                            if (!String.IsNullOrWhiteSpace(components[0]))
                                face.V[i] = this.V[Int32.Parse(components[0]) - 1];
                            if (!String.IsNullOrWhiteSpace(components[1]))
                                face.VT[i] = this.VT[Int32.Parse(components[1]) - 1];
                            if (!String.IsNullOrWhiteSpace (components [2]))
                            	face.VN [i] = this.VN [Int32.Parse (components [2]) - 1];
                            i++;
                        }
                        if(groupIndex != -1)
                            this.G[groupIndex].F.Add(face);
                        else
                            this.F.Add(face);
                    }
                    else if (currentLine.StartsWith("o "))
                    {
                        currentLine = currentLine.Remove(0, 2);
                        var group = new Group();
                        group.name = currentLine;
                        groupIndex++;
                        this.G.Add(group);
                    }
                }
            }
        }
    }
}
