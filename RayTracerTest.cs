/********************************************************************************
** Copyright 深圳市全名点游科技有限公司
** All rights reserved
** Auth： kay.yang
** Date： 6/9/2017 4:53:32 PM
** Desc： JoinGame 全名交游
** Version:  v1.0.0
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KayUtils;
namespace BVH
{
    public class RayTracerTest
    {
        public static void BuildTriangles(ref List<BVHObject> triObjects)
        {
            int w = 100;
            int h = 100;
            float startW = 0;
            float startH = 0;
            float wStep = 5f;
            float hStep = 5f;
            float y = 4.0f;
            Vector3 v1;
            Vector3 v2;
            Vector3 v3;
            Vector3 v4;
            Vector3 v5;
            Vector3 v6;
            while (startW < w)
            {
                while (startH < h)
                {
                    v1 = new Vector3(startW, y, startH);
                    v2 = new Vector3(startW, y, startH + hStep);
                    v3 = new Vector3(startW + wStep, y, startH + hStep);

                    v4 = new Vector3(startW + wStep, y, startH + hStep);
                    v5 = new Vector3(startW + wStep, y, startH);
                    v6 = new Vector3(startW, y, startH);
                    BVHTriangleObject tri1 = new BVHTriangleObject(v1, v2, v3);
                    BVHTriangleObject tri2 = new BVHTriangleObject(v4, v5, v6);
                    triObjects.Add(tri1);
                    triObjects.Add(tri2);
                    startH += hStep;
                }
                startW += wStep;
                startH = 0;
            }
        }

        public static void BuildRay(ref List<BVHRay> rayList, ref List<BVHObject> triObjects)
        {
            BVHIntersectionInfo info = new BVHIntersectionInfo();
            for (int i = 0; i < triObjects.Count; ++i)
            {
                Vector3 pos = triObjects[i].GetCentroid();
                BVHRay ray = new BVHRay(new Vector3(pos.x, 0.0f, pos.z), Vector3.up);
                rayList.Add(ray);
            }
        }

        public static void TestBVH()
        {
            List<BVHObject> triObjects = new List<BVHObject>();
            BuildTriangles(ref triObjects);
            List<BVHRay> rayList = new List<BVHRay>();
            BuildRay(ref rayList, ref triObjects);
            float start = Time.realtimeSinceStartup;
            BVH bvh = new BVH(triObjects);
            float end1 = Time.realtimeSinceStartup;
            Debug.Log(string.Format("time initialized: {0}", end1 - start));
            BVHIntersectionInfo insect = new BVHIntersectionInfo();
            int insectC = 0;
            int missC = 0;
            for (int i = 0; i < 200; ++i )
            {
                foreach (BVHRay ray in rayList)
                {
                    int test = bvh.GetIntersection(ray, ref insect, false) ? insectC++ : missC++;
                }
            }
            float end2 = Time.realtimeSinceStartup;
            Debug.Log(string.Format("time slapped: {0}, insect: {1}, miss: {2}", end2 - end1, insectC, missC));
        }

    }
}
