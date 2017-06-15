/********************************************************************************
** All rights reserved
** Auth： kay.yang
** Date： 6/9/2017 4:52:51 PM
** Version:  v1.0.0
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BVH
{
    public abstract class BVHObject
    {
        public abstract bool GetIntersection(ref BVHRay ray, ref BVHIntersectionInfo intersection);
        public abstract Vector3 GetNormal(ref BVHIntersectionInfo i);
        public abstract BVHBox GetBBox();
        public abstract Vector3 GetCentroid();

    }
}
