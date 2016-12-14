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
            // GL.Enable(EnableCap.Light0);

            // GL.Enable(EnableCap.LineSmooth);
            // GL.Enable(EnableCap.PolygonSmooth);

            GL.Light(LightName.Light0, LightParameter.Position, new float[] { 3.0f, 3.0f, 3f });
            GL.Light(LightName.Light0, LightParameter.SpotDirection, new float[] { -1.0f, -1.0f, -1.0f });
            GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { 0.2f, 0.2f, 0.2f, 0.5f });
            GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 0.2f, 0.2f, 0.2f, 0.2f });
            GL.Light(LightName.Light0, LightParameter.Specular, new float[] { 0.2f, 0.2f, 0.2f, 0.2f });
           // GL.Light(LightName.Light0, LightParameter.SpotExponent, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
           // GL.LightModel(LightModelParameter.LightModelAmbient, new float[] { 0.2f, 0.2f, 0.2f, 1.0f });
            GL.LightModel(LightModelParameter.LightModelTwoSide, 1);
            GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.ColorMaterial);

            GL.ShadeModel(ShadingModel.Smooth);

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
            //GL.LightModel(LightModelParameter.LightModelAmbient, new[] { 0.2f, 0.2f, 0.2f, 1f });

            GL.PushMatrix();

            GL.Translate(x, y, z);
            GL.Rotate(ori, 0, 1, 0);

            GL.Scale(new Vector3(0.4f, 0.4f, 0.4f));

            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.AmbientAndDiffuse, Color.Red);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, new float[] { 0.3f, 0.3f, 0.3f, 1.0f });
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, new float[] { 0.3f, 0.3f, 0.3f, 0.3f });
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, new float[] { 0.0f, 0.0f, 0.0f, 1.0f });


            if (fill)
            {
                GL.Color3(Color.WhiteSmoke);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

                //GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);

                foreach (var group in file.G)
                {
                    GL.PushMatrix();
                    if (group.name == "wheel_rf" || group.name == "wheel_rb" || group.name == "wheel_lb" || group.name == "wheel_lf")
                    {
                        //GL.PushMatrix();
                        GL.Color3(0.412f, 0.412f, 0.412f);
                        //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, Color.Red);
                        //GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.Diffuse);
                    }
                    if (group.name == "bonnet_hi")
                    {
                       // GL.PushMatrix();
                        GL.Color3(1.000f, 1.000f, 0.941f);
                    }
                    
                    if (group.name == "door_rf_hi" || group.name == "door_rr_hi" || group.name == "door_lf" || group.name == "door_lr")
                    {
                        // GL.PushMatrix();
                        GL.Color3(1.000f, 1.000f, 0.941f);
                    }
                    if (group.name == "widescreen_f" || group.name == "widescreen_r" 
                        || group.name == "door_window_fl" || group.name == "door_window_lr"
                        || group.name == "door_window_rf" || group.name == "door_window_rr")
                    {
                        // GL.PushMatrix();
                        GL.Color3(0.878f, 1.000f, 1.000f);
                    }
                    if(group.name == "grid")
                    {
                        GL.Color3(0.753f, 0.753f, 0.753f);
                    }
                    if (group.name == "light_f")
                    {
                        GL.Color3(1.000f, 0.843f, 0.0f);
                    }
                    if (group.name == "light_r")
                    {
                        GL.Color3(0.863f, 0.078f, 0.235f);
                    }

                    GL.Begin(file.PrimitiveType);
                foreach (var face in group.F)
                    {
                        
                        GL.TexCoord2(face.VT[0]);
                        GL.Normal3(face.VN[0]);
                        GL.Vertex3(face.V[0]);
                        
                        GL.TexCoord2(face.VT[1]);
                        GL.Normal3(face.VN[1]);
                        GL.Vertex3(face.V[1]);
                        
                        GL.TexCoord2(face.VT[2]);
                        GL.Normal3(face.VN[2]);
                        GL.Vertex3(face.V[2]);

                        //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, Color.Red);
                        //GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.Diffuse);
                        if (file.PrimitiveType == PrimitiveType.Quads)
                        {
                            GL.TexCoord2(face.VT[3]);
                            GL.Normal3(face.VN[3]);
                            GL.Vertex3(face.V[3]);
                        }
                    }
                    GL.End();

                    //if (group.name == "wheel_rf" || group.name == "wheel_rb" || group.name == "wheel_lb" || group.name == "wheel_lf")
                        GL.PopMatrix();
                }
                
            }

            if (wireframe)
            {
                GL.Color3(Color.Black);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line); // wireframe
                GL.Begin(file.PrimitiveType);
                foreach (var group in file.G)
                {
                    foreach (var face in group.F)
                    {
                        //GL.Normal3(new Vector3(0.0f, 1.0f, 0.0f));
                        
                        GL.TexCoord2(face.VT[0]);
                        GL.Vertex3(face.V[0]);
                        GL.Normal3(face.VN[0]);
                        GL.TexCoord2(face.VT[1]);
                        GL.Normal3(face.VN[1]);
                        GL.Vertex3(face.V[1]);                        
                        GL.TexCoord2(face.VT[2]);
                        GL.Normal3(face.VN[2]);
                        GL.Vertex3(face.V[2]);
                        if (file.PrimitiveType == PrimitiveType.Quads)
                        {
                            GL.TexCoord2(face.VT[3]);
                            GL.Normal3(face.VN[3]);
                            GL.Vertex3(face.V[3]);
                        }
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
