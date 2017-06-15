/********************************************************************************
** All rights reserved
** Auth： kay.yang
** Date： 6/12/2017 9:51:31 AM
** Version:  v1.0.0
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace BVH
{
    public class BVHAABBObject : BVHObject
    {
        public BVHBox mAABB;
        public BVHAABBObject(Vector3 min, Vector3 max)
        {
            mAABB = new BVHBox(min, max);
        }
        override
        public bool GetIntersection(ref BVHRay ray, ref BVHIntersectionInfo intersection)
        {
            float near = 0.0f;
            float far = 0.0f;
            bool isect = mAABB.Intersect(ray, ref near, ref far);
            if (isect)
            {
                intersection.mObject = this;
                intersection.mHitPoint = ray.mOrigin + ray.mDirection * near;
                intersection.mLength = near;
            }
            return isect;
        }

        // here not debug test
        override
        public Vector3 GetNormal(ref BVHIntersectionInfo i)
        {
            Vector3 v = i.mHitPoint - mAABB.mMin;
            if (v.x == 0.0f || v.y == 0.0f || v.z == 0.0f)
            {
                if (v.x == 0.0f)
                {
                    return Vector3.left;
                }
                else if (v.y == 0.0f)
                {
                    return Vector3.down;
                }
                else if (v.z == 0.0f)
                {
                    return Vector3.back;
                }
            }
            else
            {
                if (v.x == 0.0f)
                {
                    return Vector3.right;
                }
                else if (v.y == 0.0f)
                {
                    return Vector3.up;
                }
                else if (v.z == 0.0f)
                {
                    return Vector3.forward;
                }
            }
            // won't be exist
            return Vector3.zero;
        }

        override
        public BVHBox GetBBox()
        {
            return mAABB;
        }

        override
        public Vector3 GetCentroid()
        {
            return (mAABB.mMin + mAABB.mMax) * 0.5f;
        }
    }
}
