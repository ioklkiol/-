using System.Collections.Generic;
using System.Windows.Forms;

namespace _04_自发光材质
{

    /// <summary>
    /// 
    /// 本节展示自发光材质
    /// 由于本项目并没有写光源的类，就使用自发光材质充当光源了，而且还不用考虑多个光源的问题
    /// 
    /// 本节的渲染速度很慢，因为光线太少了，我们之后会解决它
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
            int width = 2000;
            int height = 1000;
            bool isSky = false;

            Vector3D lookFrom = new Vector3D(13, 15, 25);
            Vector3D lookAt = new Vector3D(0, 0, 0);
            float diskToFocus = (lookFrom - lookAt).Length();
            float aperture = 0;
            Camera camera = new Camera(lookFrom, lookAt, new Vector3D(0, 1, 0), 20,
                (float)width / (float)height, aperture, 0.7f * diskToFocus, 0, 1);


            HitableList world = new HitableList();
            List<Hitable> list = new List<Hitable>();
            Texture perText = new NoiseTexture(5f);

            list.Add(new Sphere(new Vector3D(0, -1000, 0), 1000, new Lambertian(perText)));
            list.Add(new Sphere(new Vector3D(0, 2, 0), 2, new Lambertian(perText)));
            list.Add(new Sphere(new Vector3D(0, 7, 0), 2, new DiffuseLight(new ConstantTexture(new Vector3D(10, 10, 10)))));
            list.Add(new XYRect(3, 5, 1, 3, -2, new DiffuseLight(new ConstantTexture(new Vector3D(10, 10, 10)))));
            
            world.list = list;
            scene = new Scene(width, height, world, isSky, camera,0,true);


        }
    }


}

