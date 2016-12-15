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
using System.Drawing.Imaging;
using System.Threading;

namespace Viewer
{
    /// <summary>
    /// http://www.opengl-tutorial.org/ru/beginners-tutorials/tutorial-3-matrices/
    /// https://en.wikibooks.org/wiki/OpenGL_Programming
    /// http://pmg.org.ru/nehe/
    /// https://www.opengl.org/sdk/docs/man/
    /// colors http://prideout.net/archive/colors.php
    /// http://www.falloutsoftware.com/tutorials/gl/gl8.htm
    /// http://www.just.edu.jo/~yaser/courses/cs480/Tutorials/OpenGl%20-%20Light%20&%20Material%20-%20Part%20I.htm
    /// </summary>
    public partial class Form1 : Form
    {
        bool loaded = false;

        private System.Threading.Timer timer;
        private System.Threading.Timer wheelsTimer;

        private Obj model;
        private float angle;
        private float wheelAngle;
        private bool wireframe;
        private bool fill;
        private int GrassTextureID;
        private int BuildingTextureID;

        public Form1()
        {
            InitializeComponent();
            angle = -90;

            wireframe = false;
            fill = true;

            checkBox1.Checked = fill;
            checkBox2.Checked = wireframe;
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            loaded = true;

            // Setup background colour
            GL.ClearColor(0.275f, 0.510f, 0.706f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            GL.Light(LightName.Light0, LightParameter.Position, new float[] { 3.0f, 3.0f, 3f });
            GL.Light(LightName.Light0, LightParameter.SpotDirection, new float[] { -1.0f, -1.0f, -1.0f });
            GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { 0.2f, 0.2f, 0.2f, 0.5f });
            GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 0.2f, 0.2f, 0.2f, 0.2f });
            GL.Light(LightName.Light0, LightParameter.Specular, new float[] { 0.2f, 0.2f, 0.2f, 0.2f });
            GL.LightModel(LightModelParameter.LightModelTwoSide, 1);
            GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.ColorMaterial);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);  

            GrassTextureID = UploadTexture("./Textures/asphalt.jpg");
            BuildingTextureID = UploadTexture("./Textures/building_1.jpg");

            init();

            TimerCallback timeCB = new TimerCallback(TimerInvalidate);
            timer = new System.Threading.Timer(timeCB, null, 0, 10);
            TimerCallback timewheel = new TimerCallback(WheelsTimerCallback);
            wheelsTimer = new System.Threading.Timer(timewheel, null, 0, 10);

            model = new Obj();
            model.LoadFile("models/vaz_blender.obj");
        }

        void init()
        {
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);

            Matrix4 modelview = Matrix4.LookAt(new Vector3(0f, 1.2f, 5f), Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
        }

        void TimerInvalidate(object state)
        {
            glControl1.Invalidate();
        }

        void WheelsTimerCallback(object state)
        {
            this.wheelAngle -= 10 * 0.5f;
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded)
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.Texture2D);
            DrawGrass(GrassTextureID, 0, -0.5f, 0, 0);
            DrawBuilding(BuildingTextureID, 0, -0.5f, 0, 0);
            GL.Disable(EnableCap.Texture2D);
            DrawObj(model, 0, 0, 1f, this.angle);

            glControl1.SwapBuffers();
        }

        private void DrawObj(Obj file, float x, float y, float z, float ori)
        {
            GL.PushMatrix();

            GL.Translate(x, y, z);
            GL.Rotate(ori, 0, 1, 0);

            GL.Scale(new Vector3(0.4f, 0.4f, 0.4f));

            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, new float[] { 0.3f, 0.3f, 0.3f, 1.0f });
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, new float[] { 0.3f, 0.3f, 0.3f, 0.3f });
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, new float[] { 0.0f, 0.0f, 0.0f, 1.0f });

            GL.Color3(Color.WhiteSmoke);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);


            if (fill)
            {
                foreach (var group in file.G)
                {
                    GL.PushMatrix();
                    if (group.name == "wheel_rf" || group.name == "wheel_rb" || group.name == "wheel_lb" || group.name == "wheel_lf")
                    {
                        float yTmp, zTmp;
                        List<float> yList = new List<float>();
                        List<float> zList = new List<float>();
                        yTmp = zTmp = 0;
                        int count = 0;
                        foreach (var f in group.F)
                        {
                            foreach (var v in f.V)
                            {
                                if (v.Y != 0.0f)
                                    yList.Add(v.Y);
                                if (v.Z != 0.0f)
                                    zList.Add(v.Z);
                            }
                        }
                        var maxY = yList.Max();
                        var maxZ = zList.Max();
                        var minY = yList.Min();
                        var minZ = zList.Min();

                        List<Vector3> ps = new List<Vector3>();
                        ps.Add(new Vector3(0, maxY, maxZ));
                        ps.Add(new Vector3(0, maxY, minZ));
                        ps.Add(new Vector3(0, minY, maxZ));
                        ps.Add(new Vector3(0, minY, minZ));

                        count = 0;
                        yTmp = zTmp = 0;
                        foreach (var p in ps)
                        {
                            yTmp += p.Y;
                            zTmp += p.Z;
                            count++;
                        }

                        yTmp /= count;
                        zTmp /= count;
                        GL.Translate(0, yTmp, zTmp);
                        GL.Rotate(this.wheelAngle, 1, 0, 0);
                        GL.Translate(0, -yTmp, -zTmp);

                        GL.Color3(0.412f, 0.412f, 0.412f);
                    }
                    if (group.name == "bonnet_hi")
                    {
                        GL.Color3(1.000f, 1.000f, 0.941f);
                    }
                    if (group.name == "door_rf_hi" || group.name == "door_rr_hi" 
                        || group.name == "door_lf" || group.name == "door_lr")
                    {
                        GL.Color3(1.000f, 1.000f, 0.941f);
                    }
                    if (group.name == "widescreen_f" || group.name == "widescreen_r"
                        || group.name == "door_window_fl" || group.name == "door_window_lr"
                        || group.name == "door_window_rf" || group.name == "door_window_rr")
                    {
                        GL.Color4(0.878f, 1.000f, 1.000f, 0.9f);
                    }
                    if (group.name == "grid")
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
        private void DrawGrass(int texId, float x, float y, float z, float ori)
        {
            GL.PushMatrix();

            GL.Translate(x, y, z);
            GL.Rotate(ori, 0, 1, 0);
            GL.BindTexture(TextureTarget.Texture2D, texId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, Convert.ToInt32(TextureWrapMode.Repeat));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, Convert.ToInt32(TextureWrapMode.Repeat));

            GL.Begin(PrimitiveType.Quads);
            GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, Color.GhostWhite);
            GL.Color3(Color.White);

            GL.Normal3(0.0f, 1.0f, 0.0f);
            GL.TexCoord2(3.0f, 0f);
            GL.Vertex3(-3, 0, -3);

            GL.TexCoord2(0, 0);
            GL.Vertex3(-3, 0, 3);

            GL.TexCoord2(0f, 3.0f);
            GL.Vertex3(3, 0, 3);

            GL.TexCoord2(3.0f, 3.0f);
            GL.Vertex3(3, 0, -3);

            GL.End();
            GL.PopMatrix();

        }

        private void DrawBuilding(int texId, float x, float y, float z, float ori)
        {
            GL.PushMatrix();

            GL.Translate(x, y, z);
            GL.Rotate(ori, 0, 1, 0);
            GL.BindTexture(TextureTarget.Texture2D, texId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, Convert.ToInt32(TextureWrapMode.Repeat));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, Convert.ToInt32(TextureWrapMode.Repeat));

            GL.Begin(PrimitiveType.Quads);
            GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, Color.GhostWhite);
            GL.Color3(Color.White);

            GL.Normal3(0.0f, 1.0f, 0.0f);
            GL.TexCoord2(0.0f, 2.0f);
            GL.Vertex3(-3, 0, -3);

            GL.TexCoord2(3.0, 2.0);
            GL.Vertex3(3, 0, -3);

            GL.TexCoord2(3.0f, 0.0f);
            GL.Vertex3(3, 3, -3);

            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex3(-3, 3, -3);

            GL.End();
            GL.PopMatrix();

        }


        public int UploadTexture(string pathname)
        {
            int id = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, id);

            Bitmap bmp = new Bitmap(pathname);

            // Lock image data to allow direct access
            BitmapData bmp_data = bmp.LockBits(
                                      new Rectangle(0, 0, bmp.Width, bmp.Height),
                                      System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // Import the image data into the OpenGL texture
            GL.TexImage2D(TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                bmp_data.Width,
                bmp_data.Height,
                0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                OpenTK.Graphics.OpenGL.PixelType.UnsignedByte,
                bmp_data.Scan0);

            // Unlock the image data
            bmp.UnlockBits(bmp_data);

            // Configure 
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);

            return id;
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            init();
        }

        private void trackBarRotateY_Scroll(object sender, EventArgs e)
        {
            textBox1.Text = "" + trackBarRotateY.Value * 0.01f;
            this.angle = -90 + trackBarRotateY.Value * 0.01f;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            fill = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            wireframe = checkBox2.Checked;
        }

        bool mouseActive = false;
        int initMouseX;
        int initMouseY;

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Down");
            mouseActive = true;

            initMouseX = e.Location.X;
            initMouseY = e.Location.Y;
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Up");
            mouseActive = false;
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseActive)
            {
                Console.WriteLine("Move");
                if((initMouseX-e.Location.X) <= 0)
                {
                    this.angle += 2f;
                }
                else
                {
                    this.angle -= 2f;
                }
                initMouseX = e.Location.X;
            }
        }
    }
}
