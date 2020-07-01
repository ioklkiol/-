using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _06_直接光源采样
{
    /// <summary>
    /// 
    /// 前两节渲染的速度都很慢，主要原因就是光线太少，要解决这个问题，我们想到的是:对光线多的地方优先进行采样
    /// 本节展示的就是直接对光源进行采样之后的渲染结果
    /// 可以看到，渲染速度非常的快，而且渲染的效果非常的好
    /// 
    /// 但是直接光源采样也有问题，就是没有直接受到光源照射的地方太暗了，比如天花板上，直接就是黑色，这不是我们想要的
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
            Material light = new DiffuseLight(new ConstantTexture(new Vector3D(15, 15, 15)));

            list.Add(new FlipNormals(new XZRect(213, 343, 227, 332, 554, light)));
            list.Add(new FlipNormals(new YZRect(0, 555, 0, 555, 555, green)));
            list.Add(new YZRect(0, 555, 0, 555, 0, red));
            list.Add(new FlipNormals(new XZRect(0, 555, 0, 555, 555, white)));
            list.Add(new XZRect(0, 555, 0, 555, 0, white));
            list.Add(new FlipNormals(new XYRect(0, 555, 0, 555, 555, white)));

            list.Add(new Translate(new RotateY(new Box(new Vector3D(0, 0, 0),
               new Vector3D(165, 165, 165), white), -18), new Vector3D(130, 0, 65)));
            list.Add(new Translate(new RotateY(new Box(new Vector3D(0, 0, 0),
                new Vector3D(165, 330, 165), white), 15), new Vector3D(265, 0, 295)));
            BVHNode b = new BVHNode(list, list.Count, 0, 1);
            HitableList world = new HitableList();
            world.list.Add(b);

            HitableList lightShapeList = new HitableList();
            Hitable lightShape = new XZRect(213, 343, 227, 332, 554, null);
            lightShapeList.list.Add(lightShape);
            scene = new Scene(width, height, world, isSky, camera, 1,false,lightShapeList);


        }
    }


}

