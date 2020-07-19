using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


public class Renderer
{
    private int width;
    private int height;
    private HitableList world = new HitableList();  //場景中的物體
    private bool isSky = true;                      //是否顯示天空
    private Scene scene;
    private int samples;
    private Camera camera;

    public static Renderer main;
    public Preview preview = new Preview();         //展示圖片的窗口
    public float[] buff;        //保存顔色信息，每四個值對應一個點的顔色，即(r,g,b,a)
    public int[] changes;       //記錄每個點的采樣率

    public Renderer(Scene scene, int samples=1000)
    {
        this.scene = scene;
        this.width = scene.Width;
        this.height = scene.Height;
        this.isSky = scene.IsSky;
        this.world = scene.World;
        this.camera = scene.Camera;
        this.samples = samples;
        Init();
    }

    public void Init()
    {
        main = this;
        buff = new float[width * height * 4];
        changes = new int[width * height];
        Start();
        preview.gamma = scene.Gamma;
        preview.Start("Preview", width, height);
        System.Environment.Exit(0);
    }

    private class ScannerCofig
    {
        public int width;
        public int height;

        public ScannerCofig(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }
    /// <summary>
    /// 綫程池
    /// </summary>
    private async void Start()
    {
        ThreadPool.SetMaxThreads(15, 15);
        await Task.Factory.StartNew(
            delegate { LinearScanner(new ScannerCofig(width, height)); });
        for (int i = 1; i < samples; i++)
        {
            ThreadPool.QueueUserWorkItem(LinearScanner, new ScannerCofig(width, height));
        }
    }

    private Vector3D DeNaN(Vector3D c)
    {
        Vector3D temp = c;
        if (!(temp[0] == temp[0])) temp[0] = 0;
        if (!(temp[1] == temp[1])) temp[1] = 0;
        if (!(temp[2] == temp[2])) temp[2] = 0;
        return temp;
    }
    private void LinearScanner(object o)
    {
        ScannerCofig config = (ScannerCofig)o;

        for (int j = 0; j < config.height; j++)
        {
            for (int i = 0; i < config.width; i++)
            {
                Vector3D color = new Vector3D(0, 0, 0);
                float u = (float)(i + Mathf.Randomfloat()) / (float)width;
                float v = 1 - (float)(j + Mathf.Randomfloat()) / (float)height;
                Ray ray = camera.GetRay(u, v);
                color = DeNaN(GetColor(ray, world, scene.LightShapeList, 0));
                if (!scene.Gamma)
                    color = new Vector3D(Mathf.Sqrt(color.X), Mathf.Sqrt(color.Y), Mathf.Sqrt(color.Z));
                SetPixel(i, j, color);
            }
        }
    }


    private Vector3D GetColor(Ray r, HitableList world, Hitable lightShape, int depth)
    {
        HitRecord hRec;
        /*这里的0.001不能改为0，当tmin设0的时候会导致，遍历hitlist时候，ray的t求解出来是0，
         * hit的时候全走了else，导致递归到50层的时候，最后return的是0，* attenuation结果还是0。
         * 距离越远，散射用到random_in_unit_sphere生成的ray误差越大
         */
        if (world.Hit(r, 0.001f, float.MaxValue, out hRec))
        {
            ScatterRecord sRec;
            Vector3D emitted = hRec.matPtr.Emitted(r, hRec, hRec.u, hRec.v, hRec.p);
            if (depth < 50 && hRec.matPtr.Scatter(r, hRec, out sRec))
            {
                if (sRec.isSpecular)
                {
                    Vector3D c = GetColor(sRec.specularRay, world, lightShape, depth + 1);
                    return new Vector3D(sRec.attenuation.X * c.X, sRec.attenuation.Y * c.Y, sRec.attenuation.Z * c.Z);
                }
                PDF p;
                if (lightShape != null)
                {
                    HitablePDF p0 = new HitablePDF(lightShape, hRec.p);
                    p = new MixturePDF(p0, sRec.pdfPtr);
                    ((MixturePDF)p).MixRatio = scene.MixRatio;
                }
                else
                {
                    p = sRec.pdfPtr;
                }
                //CosinePDF p = new CosinePDF(hRec.normal);
                Ray scattered = new Ray(hRec.p, p.Generate(), r.Time);
                float pdfVal = p.Value(scattered.Direction);
                Vector3D color = GetColor(scattered, world, lightShape, depth + 1);      //每次光线衰减之后深度加一
                return emitted + hRec.matPtr.ScatteringPDF(r, hRec, scattered)
                   * new Vector3D(sRec.attenuation.X * color.X, sRec.attenuation.Y
                   * color.Y, sRec.attenuation.Z * color.Z) / pdfVal;
            }
            else
            {
                return emitted;
            }
        }
        else
        {
            if (isSky)
            {
                Vector3D unitDirection = r.Direction.UnitVector();
                float t = 0.5f * (unitDirection.Y + 1f);
                return (1 - t) * new Vector3D(1, 1, 1) + t * new Vector3D(0.5f, 0.7f, 1);

            }
            return new Vector3D(0, 0, 0);
        }
    }

    /// <summary>
    /// 設置顔色(r,g,b,a)
    /// </summary>
    private void SetPixel(int x, int y, Vector3D color)
    {
        var i = width * 4 * y + x * 4;
        changes[width * y + x]++;
        buff[i] += color.X;
        buff[i + 1] += color.Y;
        buff[i + 2] += color.Z;
        buff[i + 3] += 1;
    }
}