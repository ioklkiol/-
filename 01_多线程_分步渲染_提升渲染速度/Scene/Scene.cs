using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Scene
{
    private int width = 2000;
    private int height = 1000;
    private HitableList world = new HitableList();
    private bool isSky = true;
    private float mixRatio;
    private Camera camera;
    private bool gamma;
    private HitableList lightShapeList;

    public int Width { get => width; set => width = value; }
    public int Height { get => height; set => height = value; }
    public HitableList World { get => world; set => world = value; }
    public bool IsSky { get => isSky; set => isSky = value; }
    public float MixRatio { get => mixRatio; set => mixRatio = value; }
    public Camera Camera { get => camera; set => camera = value; }
    public bool Gamma { get => gamma; set => gamma = value; }
    public HitableList LightShapeList { get => lightShapeList; set => lightShapeList = value; }

    public Scene(int width, int height, HitableList world, bool isSky, Camera camera,float mixRatio=0.0f,bool gamma=false,HitableList lightShapeList=null)
    {
        this.width = width;
        this.height = height;
        this.world = world;
        this.isSky = isSky;
        this.camera = camera;
        this.mixRatio = mixRatio;
        this.gamma = gamma;
        this.lightShapeList = lightShapeList;
    }


}