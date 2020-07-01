using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _08_体恒量介质_平面三角形
{
    /// <summary>
    /// 本节展示体恒量介质和平面三角形，场景中有两个气态矩形，要达到书上那种效果的话要多等一会儿
    /// </summary>
    public partial class Form1 : Form
    {
        public static Form1 main;
        private Scene scene;
        private Renderer renderer;

        public Form1()
        {
            InitializeComponent();
            main = this;
            InitScene();
            renderer = new Renderer(scene,1000);
            
        }

        private void InitScene()
        {
            int width = 512;
            int height = 512;
            bool isSky = false;
            Vector3D lookFrom = new Vector3D(278, 278, -800);
            Vector3D lookAt = new Vector3D(278, 278, 0);
            float diskToFocus = 10;
            float aperture = 0;
            float vfov = 40;
            Camera camera = new Camera(lookFrom, lookAt, new Vector3D(0, 1, 0), vfov,
                 (float)width / (float)height, aperture, diskToFocus, 0, 1);

            List<Hitable> list = new List<Hitable>();

            Material red = new Lambertian(new ConstantTexture(new Vector3D(0.65f, 0.05f, 0.05f)));
            Material white = new Lambertian(new ConstantTexture(new Vector3D(0.73f, 0.73f, 0.73f)));
            Material green = new Lambertian(new ConstantTexture(new Vector3D(0.12f, 0.45f, 0.15f)));
            Material light = new DiffuseLight(new ConstantTexture(new Vector3D(7, 7, 7)));

            list.Add(new FlipNormals(new XZTriangle(113, 443, 278, 127, 127, 432, 554, light)));
            list.Add(new FlipNormals(new YZRect(0, 555, 0, 555, 555, green)));
            list.Add(new YZRect(0, 555, 0, 555, 0, red));
            list.Add(new FlipNormals(new XZRect(0, 555, 0, 555, 555, white)));
            list.Add(new XZRect(0, 555, 0, 555, 0, white));
            list.Add(new FlipNormals(new XYRect(0, 555, 0, 555, 555, white)));
            Hitable b1 = new Translate(new RotateY(new Box(new Vector3D(0, 0, 0),
               new Vector3D(165, 165, 165), white), -18), new Vector3D(130, 0, 65));
            Hitable b2 = new Translate(new RotateY(new Box(new Vector3D(0, 0, 0),
                new Vector3D(165, 330, 165), white), 15), new Vector3D(265, 0, 295));
            list.Add(new ConstantMedium(b1, 0.01f, new ConstantTexture(new Vector3D(1, 1, 1))));
            list.Add(new ConstantMedium(b2, 0.01f, new ConstantTexture(new Vector3D(0, 0, 0))));
            BVHNode b = new BVHNode(list, list.Count, 0, 1);
            HitableList world = new HitableList();
            world.list.Add(b);


            HitableList lightShapeList = new HitableList();
            Hitable lightShape = new XZRect(113, 443, 127, 432, 554, null);
            lightShapeList.list.Add(lightShape);
            scene = new Scene(width, height, world, isSky, camera, 0.5f,true,lightShapeList);


        }
    }


}

