using System.Collections.Generic;
using System.Windows.Forms;

namespace _01_多线程_分步渲染_提升渲染速度
{
    /// <summary>
    /// 本节展示的这张图大家都是渲染过的，这是一张2000*1000的图，没有经过优化的话渲染是要花十几个小时的。本项目采用多个线程同时渲染，
    /// 逐步增加采样率的方式进行渲染，同时使用了层次包围盒(BVH树)来管理场景中的物体，轴对齐包围盒(AABB)来处理物体的碰撞检测
    /// 并优化了用于生成随机数的随机函数，使用directX11的窗口来展示结果，大大加快了渲染速度
    /// 
    /// 运行项目后，一开始是全黑的窗口，然后从上往下将当前渲染的结果展示出来。第一轮扫描后会出现一张不清晰的图，然后会继续进行扫描
    /// 每一轮扫描都是有多个线程的，仔细看的话是能看出来的。
    /// 每一轮扫描都会使图片变得更加清晰，因为采样率增加了。
    /// 一开始的采样率是0，每一个线程都会使采样率增加一，展示的结果会根据采样率取平均值。
    /// 经过两轮扫描后，肉眼已经已经很难看出图片的变化，但是采样仍在继续，过一段时间再看就会发现画质变好了
    /// 可以在Renderer类中设置最大的采样率和最大线程数，默认的最大采样率是1000,最大线程数是15。线程数设置得太高并没有意义，电脑性能跟不上
    /// 
    /// 若不能运行该项目，请导入dll文件，并将项目的生成目标平台改为X64
    /// 
    /// 也可以不导入dll文件，将项目文件里的Windows文件夹导入项目，在项目上右击->管理NuGet程序包然后下载sharpDX插件，
    /// 这样不改为64位也可以运行，但不改的话可能会导致卡顿(然而渲染速度并不会变慢)
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
            renderer = new Renderer(scene);
            
        }

        private void InitScene()
        {
            int width = 2000;
            int height = 1000;
            bool isSky = true;

            Vector3D lookFrom = new Vector3D(13, 2, 3);
            Vector3D lookAt = new Vector3D(0, 0, 0);
            float diskToFocus = (lookFrom - lookAt).Length();
            float aperture = 0;
            Camera camera = new Camera(lookFrom, lookAt, new Vector3D(0, 1, 0), 20,
                (float)width / (float)height, aperture, 0.7f * diskToFocus, 0, 1);

            List<Hitable> list = new List<Hitable>();
            list.Add(new Sphere(new Vector3D(0, -1000, 0), 1000, new Lambertian(new ConstantTexture(new Vector3D(0.5f, 0.5f, 0.5f)))));
            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    double chooseMat = Mathf.Randomfloat();
                    Vector3D center = new Vector3D(a + 0.9f * Mathf.Randomfloat(), 0.2f, b + 0.9f * Mathf.Randomfloat());
                    if ((center - new Vector3D(4, 0.2f, 0)).Length() > 0.9)
                    {
                        if (chooseMat < 0.8)
                        {
                            list.Add(new Sphere(center, 0.2f, new Lambertian(new ConstantTexture(new Vector3D(
                                                            Mathf.Randomfloat() * Mathf.Randomfloat(),
                                                            Mathf.Randomfloat() * Mathf.Randomfloat(),
                                                            Mathf.Randomfloat() * Mathf.Randomfloat())))));
                        }
                        else if (chooseMat < 0.95)
                        {
                            list.Add(new Sphere(center, 0.2f, new Metal(new Vector3D(
                                                                0.5f * (1 + Mathf.Randomfloat()),
                                                                0.5f * (1 + Mathf.Randomfloat()),
                                                                0.5f * (1 + Mathf.Randomfloat())),
                                                                0.5f * (1 + Mathf.Randomfloat()))));
                        }
                        else
                        {
                            list.Add(new Sphere(center, 0.2f, new Dielectric(1.5f)));
                        }
                    }
                }
            }
            list.Add(new Sphere(new Vector3D(0, 1, 0), 1, new Dielectric(1.5f)));
            list.Add(new Sphere(new Vector3D(-4, 1, 0), 1, new Lambertian(new ConstantTexture(new Vector3D(0.4f, 0.2f, 0.1f)))));
            list.Add(new Sphere(new Vector3D(4, 1, 0), 1, new Metal(new Vector3D(0.7f, 0.6f, 0.5f), 0)));
            BVHNode bb = new BVHNode(list, list.Count, 0, 1);
            HitableList world = new HitableList();
            world.list.Add(bb);
            scene = new Scene(width, height, world, isSky, camera,0,false);


        }
    }


}

