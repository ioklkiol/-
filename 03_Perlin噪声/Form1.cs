using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _03_Perlin噪声_伽马矫正
{
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

