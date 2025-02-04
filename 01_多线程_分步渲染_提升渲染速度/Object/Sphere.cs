﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Sphere : Hitable
{
    private Vector3D _center;
    private float _radius;
    private Material _matPtr;

    public Vector3D Center { get => _center; set => _center = value; }
    public float Radius { get => _radius; set => _radius = value; }
    public Material MatPtr { get => _matPtr; set => _matPtr = value; }

    public Sphere(Vector3D cen, float r,Material m)
    {
        Center = cen;
        Radius = r; 
        MatPtr = m;
    }
    private void GetSphereUV(Vector3D p, out float u, out float v)
    {
        float phi = Mathf.Atan2(p.Z, p.X);                  //x = cos(phi)*cos(theta)
        float theta = Mathf.Asin(p.Y);                      //z = sin(phi)*cos(theta)
                                                            //y = sin(theta)

        u = 1 - (phi + Mathf.PI) / (2 * Mathf.PI);            //将u和v规格化为0到1之间
        v = (theta + Mathf.PI / 2) / Mathf.PI;
    }

    public override  bool Hit(Ray r, float tMin, float tMax,out HitRecord rec)
    {
        rec = new HitRecord();
        Vector3D oc = r.Origin - Center;
        float a = r.Direction * r.Direction;
        float b = oc * r.Direction;                
        float c = oc * oc - Radius * Radius;       
        float discriminant = b * b -  a * c;
        if (discriminant > 0)
        {
            float temp = (-b - Mathf.Sqrt(discriminant)) / a;
            if (temp < tMax && temp > tMin)
            {
                rec.t = temp;
                rec.p = r.GetPoint(rec.t);
                rec.normal = (rec.p - Center) / Radius;
                rec.matPtr = MatPtr;
                GetSphereUV((rec.p - Center) / Radius,out rec.u,out rec.v);
                return true;

            }
            temp = (-b + Mathf.Sqrt(discriminant)) / a;
            if (temp < tMax && temp > tMin)
            {
                rec.t = temp;
                rec.p = r.GetPoint(rec.t);
                rec.normal = (rec.p - Center) / Radius;
                rec.matPtr = MatPtr;      
                GetSphereUV((rec.p - Center) / Radius,out rec.u,out rec.v);
                return true;
            }

        }
        return false;
    }

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
        box = new AABB(Center - new Vector3D(Radius, Radius, Radius), Center + new Vector3D(Radius, Radius, Radius));
        return true;
    }

    public override float PDFValue(Vector3D o, Vector3D v)
    {
        HitRecord rec;
        if (this.Hit(new Ray(o, v), 0.001f, float.MaxValue, out rec))
        {
            float cosThetaMax = Mathf.Sqrt(1 - Radius * Radius / (Center - o).SquaredMagnitude());
            float solidAngle = 2 * Mathf.PI * (1 - cosThetaMax);
            return 1 / solidAngle;
        }
        else
            return 0;
    }

    public override Vector3D Random(Vector3D o)
    {
        Vector3D direction = Center - o;
        float distanceSquared = direction.SquaredMagnitude();
        ONB uvw = new ONB();
        uvw.BuildFromW(direction);
        return uvw.Local(RandomToSphere(Radius, distanceSquared));
    }
    private Vector3D RandomToSphere(float radius, float distanceSquared)
    {
        float r1 = Mathf.Randomfloat();
        float r2 = Mathf.Randomfloat();
        float z = 1 + r2 * (Mathf.Sqrt(1 - radius * radius / distanceSquared) - 1);
        float phi = 2 * Mathf.PI * r1;
        float x = Mathf.Cos(phi) * Mathf.Sqrt(1 - z * z);
        float y = Mathf.Sin(phi) * Mathf.Sqrt(1 - z * z);
        return new Vector3D(x, y, z);
    }
}

