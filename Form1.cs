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
    /// <summary>
    /// http://www.opengl-tutorial.org/ru/beginners-tutorials/tutorial-3-matrices/
    /// https://en.wikibooks.org/wiki/OpenGL_Programming
    /// http://pmg.org.ru/nehe/
    /// https://www.opengl.org/sdk/docs/man/
    /// </summary>
    public partial class Form1 : Form
    {
        bool loaded = false;

        private Obj model;
        private float angle;
        private bool wireframe;
        private bool fill;
        public Form1()
        {
            InitializeComponent();
            angle = -90;

            wireframe = true;
            fill = false;

            checkBox1.Checked = fill;
            checkBox2.Checked = wireframe;
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

            init();

            model = new Obj();
            model.LoadFile("models/vaz_blender.obj");
        }

        void init()
        {
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);

            Matrix4 modelview = Matrix4.LookAt(new Vector3(0f, 0.6f, 3.5f), Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded)
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            DrawObj(model, 0, 0, 1f, this.angle);

            glControl1.SwapBuffers();
        }

        private void DrawObj(Obj file, float x, float y, float z, float ori)
        {
            GL.PushMatrix();

            GL.Translate(x, y, z);
            GL.Rotate(ori, 0, 1, 0);

            GL.Scale(new Vector3(0.4f, 0.4f, 0.4f));

            if (fill)
            {
                GL.Color3(Color.Bisque);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.Begin(file.PrimitiveType);
                foreach (var face in file.F)
                {
                    //GL.Normal3(new Vector3(0.0f, 1.0f, 0.0f));
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
            }

            if (wireframe)
            {
                GL.Color3(Color.Black);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line); // wireframe
                GL.Begin(file.PrimitiveType);
                foreach (var face in file.F)
                {
                    //GL.Normal3(new Vector3(0.0f, 1.0f, 0.0f));
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
            }
            GL.PopMatrix();
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            init();
        }

        private void trackBarRotateY_Scroll(object sender, EventArgs e)
        {
            textBox1.Text = "" + trackBarRotateY.Value * 0.01f;
            this.angle = -90 + trackBarRotateY.Value * 0.01f;
            glControl1.Invalidate();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            fill = checkBox1.Checked;
            glControl1.Invalidate();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            wireframe = checkBox2.Checked;
            glControl1.Invalidate();
        }
    }
}
