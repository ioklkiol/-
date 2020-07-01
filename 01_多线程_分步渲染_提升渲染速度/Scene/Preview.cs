using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcDx;

public class Preview : DxWindow
{
    public bool gamma;

    public override void Update()
    {
        for (int i = 0; i < Buff.Length; i++)
        {
            if (gamma)
                Buff[i] = (byte)Mathf.Range(Mathf.Sqrt(Renderer.main.buff[i] * 255 / Renderer.main.changes[i / 4]), 0, 255);
            else
                Buff[i] = (byte)Mathf.Range(Renderer.main.buff[i] * 255 / Renderer.main.changes[i / 4], 0, 255);
        }
    }

    public void Start(string title, int width, int height)
    {
        Run(new DxConfiguration(title, width, height));
    }

}