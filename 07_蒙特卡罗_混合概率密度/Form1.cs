using System.Collections.Generic;
using System.Windows.Forms;

namespace _07_蒙特卡罗_混合概率密度_降噪
{
    /// <summary>
    /// 
    /// 之前的采样都是对物体的各个部位一视同仁地进行采样，但物体不同部位反射光线的概率是不同的，所以我们对不同部位采样的次数也应该不同
    /// 为了实现这个，我们需要使用蒙特卡罗方法模拟概率密度的分布
    /// 
    /// 为了得到更好的渲染效果，我们使用了混合概率密度，对直接光源采样和使用cos函数的pdf(概率密度分布函数)的优点进行折中，使渲染速度达到
    /// 可以接受的范围并且不会出现阴暗处无光的问题
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
            scene = new Scene(width, height, world, isSky, camera, 0.5f,true,lightShapeList);


        }
    }


}

