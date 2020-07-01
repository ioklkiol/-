using System.Collections.Generic;
using System.Windows.Forms;

namespace _05_Cornell盒子_平移旋转
{
    /// <summary>
    /// 
    /// 本节展示Cornell盒子
    /// Cornell盒子场景里有几个平面矩形和盒子，Box类其实是由几个平面矩形拼成的
    /// 
    /// 关于物体的位置变换，其实一般都是用的变换矩阵，但本项目为了和书上保持一致还是采用了书上的方法，在渲染进行了位置变换的物体时更改反射光线
    /// 其实我个人觉得这种方法很不好，因为使用这种方法进行的位置变换是虚假的，物体在场景中的位置其实并没有变
    /// 
    /// 本节的渲染速度同样很慢，因为光线太少了，我们会在下一节解决它
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
            scene = new Scene(width, height, world, isSky, camera, 0,true);


        }
    }


}

