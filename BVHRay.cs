/********************************************************************************
** All rights reserved
** Auth： kay.yang
** Date： 6/9/2017 4:53:08 PM
** Version:  v1.0.0
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace BVH
{
    public class BVHRay
    {
        public Vector3 mOrigin; 
        public Vector3 mDirection;
        public Vector3 mInvDirection;

        public BVHRay(Vector3 o, Vector3 d)
        {
            mOrigin = o;
            mDirection = d;
            mInvDirection = new Vector3(1 / d[0], 1 / d[1], 1 / d[2]);
        }
    }
}
