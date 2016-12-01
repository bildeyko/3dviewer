using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Viewer
{
    public partial class Form1 : Form
    {
        bool loaded = false;

        private Obj model;
        public Form1()
        {
            InitializeComponent();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            loaded = true;

            // Setup background colour
            GL.ClearColor(Color.Gray);

            // Setup OpenGL capabilities
            GL.Enable(EnableCap.DepthTest);
            //GL.Enable(EnableCap.CullFace);
            //GL.Enable(EnableCap.ColorMaterial);
            // GL.Enable(EnableCap.Lighting);
            //GL.Enable(EnableCap.Light0);

           // GL.Enable(EnableCap.LineSmooth);
           // GL.Enable(EnableCap.PolygonSmooth);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);

            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            //Matrix4 modelview = Matrix4.LookAt(-10, -10, 0, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            //GL.Viewport(0, 0, this.Width, this.Height);

            model = new Obj();
            model.LoadFile("models/vaz_blender.obj");
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded)
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Initialise the model view matrix
            // GL.MatrixMode(MatrixMode.Modelview);
            // GL.LoadIdentity();


            DrawObj(model, 0, 0, 1f, -90);

            //GL.Begin(PrimitiveType.Triangles);
            //GL.Color3(Color.OrangeRed);
            //GL.Vertex3(0, 0, 0.5f);
            //GL.Color3(Color.Blue);
            //GL.Vertex3(1, 0, 0.5f);
            //GL.Vertex3(0, 1, 0.5f);

            //GL.Color3(Color.Yellow);
            //GL.Vertex3(0, 0, 0.3f);
            //GL.Vertex3(0.5, 0, 0.3f);
            //GL.Vertex3(-0.3, 1, 0.8f);
            //GL.End();

            glControl1.SwapBuffers();
        }

        private void DrawObj(Obj file, float x, float y, float z, float ori)
        {
            GL.PushMatrix();

            GL.Translate(x, y, z);
            GL.Rotate(ori, 1, 0, 0);

            //GL.BindTexture (TextureTarget.Texture2D, BallTextureID);
            GL.Scale(new Vector3(0.2f, 0.2f, 0.2f));

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            //GL.Begin(file.PrimitiveType);
            //foreach (var face in file.F)
            //{
            //    //GL.Color3(Color.White);
            //    GL.Normal3(new Vector3(0.0f, 1.0f, 0.0f));
            //    //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, Color.White); //specify material parameters for the lighting model
            //    // GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line); // это врорде для отрисовки wireframe
            //    GL.TexCoord2(face.VT[0]);
            //    GL.Vertex3(face.V[0]);
            //    GL.TexCoord2(face.VT[1]);
            //    GL.Vertex3(face.V[1]);
            //    GL.TexCoord2(face.VT[2]);
            //    GL.Vertex3(face.V[2]);
            //    if (file.PrimitiveType == PrimitiveType.Quads)
            //    {
            //        GL.TexCoord2(face.VT[3]);
            //        GL.Normal3(face.VN[3]);
            //        GL.Vertex3(face.V[3]);
            //    }
            //}

            //GL.End();

            GL.Color3(Color.Black);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Begin(file.PrimitiveType);
            foreach (var face in file.F)
            {
                //GL.Color3(Color.White);
                GL.Normal3(new Vector3(0.0f, 1.0f, 0.0f));
               // GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, Color.White);
               // GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line); // это врорде для отрисовки wireframe
                GL.TexCoord2(face.VT[0]);
                GL.Vertex3(face.V[0]);
                GL.TexCoord2(face.VT[1]);
                GL.Vertex3(face.V[1]);
                GL.TexCoord2(face.VT[2]);
                GL.Vertex3(face.V[2]);
                if (file.PrimitiveType == PrimitiveType.Quads)
                {
                    GL.TexCoord2(face.VT[3]);
                    GL.Normal3(face.VN[3]);
                    GL.Vertex3(face.V[3]);
                }
            }

            GL.End();
            GL.PopMatrix();
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);

            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            //Matrix4 modelview = Matrix4.LookAt(-10, -10, 0, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
        }
    }
}
