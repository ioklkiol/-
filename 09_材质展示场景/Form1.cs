using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _09_材质展示场景
{
    /// <summary>
    /// 
    /// 本节展示该渲染器可以渲染的材质
    /// 
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
            Vector3D lookFrom = new Vector3D(500, 300, -600);
            Vector3D lookAt = new Vector3D(278, 278, 0);
            float diskToFocus = 10;
            float aperture = 0;
            float vfov = 40;
            Camera camera = new Camera(lookFrom, lookAt, new Vector3D(0, 1, 0), vfov,
                (float)width / (float)height, aperture, diskToFocus, 0, 1);

            List<Hitable> list = new List<Hitable>();
            List<Hitable> boxList = new List<Hitable>();
            List<Hitable> boxList2 = new List<Hitable>();

            int nb = 20;
            Material white = new Lambertian(new ConstantTexture(new Vector3D(0.73f, 0.73f, 0.73f)));
            Material ground = new Lambertian(new ConstantTexture(new Vector3D(0.48f, 0.83f, 0.53f)));
            Material light = new DiffuseLight(new ConstantTexture(new Vector3D(7, 7, 7)));

            for (int i = 0; i < nb; i++)
            {
                for (int j = 0; j < nb; j++)
                {
                    float w = 100;
                    float x0 = -1000 + i * w;
                    float z0 = -1000 + j * w;
                    float y0 = 0;
                    float x1 = x0 + w;
                    float y1 = 100 * (Mathf.Randomfloat() + 0.01f);
                    float z1 = z0 + w;
                    boxList.Add(new Box(new Vector3D(x0, y0, z0), new Vector3D(x1, y1, z1), ground));
                }
            }
            list.Add(new BVHNode(boxList, boxList.Count, 0, 1));
            list.Add(new FlipNormals(new XZRect(123, 423, 147, 412, 554, light)));
            Vector3D center = new Vector3D(400, 400, 200);
            list.Add(new MovingSphere(center, center + new Vector3D(30, 0, 0), 0, 1, 50,
                new Lambertian(new ConstantTexture(new Vector3D(0.7f, 0.3f, 0.1f)))));
            list.Add(new Sphere(new Vector3D(260, 150, 45), 50, new Dielectric(1.5f)));
            list.Add(new Sphere(new Vector3D(0, 150, 145), 50, new Metal(
               new Vector3D(0.8f, 0.8f, 0.9f), 10)));
            Hitable boundary = new Sphere(new Vector3D(360, 150, 145), 70, new Dielectric(1.5f));
            list.Add(boundary);
            list.Add(new ConstantMedium(boundary, 0.2f, new ConstantTexture(new Vector3D(0.2f, 0.4f, 0.9f))));
            boundary = new Sphere(new Vector3D(0, 0, 0), 5000, new Dielectric(1.5f));
            list.Add(new ConstantMedium(boundary, 0.0001f, new ConstantTexture(new Vector3D(1, 1, 1))));

            Material emat = new Lambertian(new Imagetexture("Earth.jpg"));
            list.Add(new Sphere(new Vector3D(400, 200, 400), 100, emat));
            Texture pertext = new NoiseTexture(1f);
            list.Add(new Sphere(new Vector3D(220, 280, 300), 80, new Lambertian(pertext)));
            int ns = 1000;
            for (int j = 0; j < ns; j++)
            {
                boxList2.Add(new Sphere(new Vector3D(165 * Mathf.Randomfloat(), 165 * Mathf.Randomfloat(), 165 * Mathf.Randomfloat()), 10, white));
            }
            list.Add(new Translate(new RotateY(new BVHNode(boxList2, ns, 0, 1), 15), new Vector3D(-100, 270, 395)));


            BVHNode b = new BVHNode(list, list.Count, 0, 1);
            HitableList world = new HitableList();
            world.list.Add(b);


            HitableList lightShapeList = new HitableList();
            Hitable lightShape = new XZRect(123, 423, 147, 412, 554, null);
            lightShapeList.list.Add(lightShape);
            scene = new Scene(width, height, world, isSky, camera, 0.5f,false,lightShapeList);


        }
    }


}

