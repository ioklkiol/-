using System.Collections.Generic;
using System.Windows.Forms;

namespace _03_Perlin噪声_伽马矫正
{

    /// <summary>
    /// 
    /// 本节展示Perlin噪声，Perlin噪声用于生成杂乱无章的图形
    /// 使用Perlin噪声可以得到千奇百怪的图形，这里只展示了书上的一种，若想得到不同的效果，可以修改NoiseTexture类的Value函数
    /// 
    /// 使用了伽马校正后图片的更新变得更柔和了，肉眼更难发现图片的变化
    /// 
    /// 如果没看出来区别可以对比一下01和03，注意观察扫描的边界
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
            renderer = new Renderer(scene);
            
        }

        private void InitScene()
        {
            int width = 2000;
            int height = 1000;
            bool isSky = true;

            Vector3D lookFrom = new Vector3D(13, 2, 0);
            Vector3D lookAt = new Vector3D(0, 0, 0);
            float diskToFocus = (lookFrom - lookAt).Length();
            float aperture = 0;
            Camera camera = new Camera(lookFrom, lookAt, new Vector3D(0, 1, 0), 20,
                (float)width / (float)height, aperture, 0.7f * diskToFocus, 0, 1);


            HitableList world = new HitableList();
            List<Hitable> list = new List<Hitable>();
            Texture perText = new NoiseTexture(5f);

            list.Add(new Sphere(new Vector3D(0, -1000, 0), 1000, new Lambertian(perText)));
            list.Add(new Sphere(new Vector3D(0, 1, 0), 1, new Lambertian(perText)));
            world.list = list;
            scene = new Scene(width, height, world, isSky, camera,0,true);


        }
    }


}

