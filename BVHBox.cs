/********************************************************************************
** Copyright 深圳市全名点游科技有限公司
** All rights reserved
** Auth： kay.yang
** Date： 6/9/2017 4:51:47 PM
** Desc： JoinGame 全名交游
** Version:  v1.0.0
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BVH
{
    public class BVHBox
    {
        public Vector3 mMin = new Vector3(), mMax = new Vector3(), mExtentSize = new Vector3();

        public BVHBox()
        {

        }

        public BVHBox(Vector3 _min, Vector3 _max)
        {
            mMin = _min;
            mMax = _max;
            mExtentSize = mMax - mMin;
        }

        public BVHBox(Vector3 p)
        {
            mMin = p;
            mMax = p;
            mExtentSize = mMax - mMin;
        }

        public bool Intersect(BVHRay ray, ref float dnear, ref float dfar)
        {
            // Intermediate calculation variables.
            float tmin = 0.0f;
            float tmax = 0.0f;
            // X direction.
            float div = ray.mInvDirection.x;
            if (div >= 0.0f)
            {
                tmin = (mMin.x - ray.mOrigin.x) * div;
                tmax = (mMax.x - ray.mOrigin.x) * div;
            }
            else
            {
                tmin = (mMax.x - ray.mOrigin.x) * div;
                tmax = (mMin.x - ray.mOrigin.x) * div;
            }
            dnear = tmin;
            dfar = tmax;
    
            // Check if the ray misses the box.
            if (dnear > dfar || dfar < 0.0f)
            {
                return false;
            }
            // Y direction.
            div = ray.mInvDirection.y;
            if (div >= 0.0f)
            {
                tmin = (mMin.y - ray.mOrigin.y) * div;
                tmax = (mMax.y - ray.mOrigin.y) * div;
            }
            else
            {
                tmin = (mMax.y - ray.mOrigin.y) * div;
                tmax = (mMin.y - ray.mOrigin.y) * div;
            }
            // Update the near and far intersection distances.
            if (tmin > dnear)
            {
                dnear = tmin;
            }
            if (tmax < dfar)
            {
                dfar = tmax;
            }
            // Check if the ray misses the box.
            if (dnear > dfar || dfar < 0.0f)
            {
		        return false;
            }
            // Z direction.
            div = ray.mInvDirection.z;
            if (div >= 0.0f)
            {
                tmin = (mMin.z - ray.mOrigin.z) * div;
                tmax = (mMax.z - ray.mOrigin.z) * div;
            }
            else
            {
                tmin = (mMax.z - ray.mOrigin.z) * div;
                tmax = (mMin.z - ray.mOrigin.z) * div;
            }
            // Update the near and far intersection distances.
            if (tmin > dnear)
            {
                dnear = tmin;
            }
            if (tmax < dfar)
            {
                dfar = tmax;
            }
            // Check if the ray misses the box.
            if (dnear > dfar || dfar < 0.0f)
            {
                return false;
            }
            return true;
        }

        public void ExpandToInclude(Vector3 p)
        {
            mMin = Vector3.Min(mMin, p);
            mMax = Vector3.Max(mMax, p);
            mExtentSize = mMax - mMin;
        }

        public void ExpandToInclude(BVHBox b)
        {
            mMin = Vector3.Min(mMin, b.mMin);
            mMax = Vector3.Max(mMax, b.mMax);
            mExtentSize = mMax - mMin;
        }

        public int MaxDimension()
        {
            int result = 0;
            if (mExtentSize.y > mExtentSize.x)
            {
                result = 1;
                if (mExtentSize.z > mExtentSize.y)
                {
                    result = 2;
                }
            }
            else
            {
                if (mExtentSize.z > mExtentSize.x)
                {
                    result = 2;
                }
            }
            return result;
        }

        public float SurfaceArea()
        {
            return 2.0f * (mExtentSize.x * mExtentSize.z + mExtentSize.x * mExtentSize.y + mExtentSize.y * mExtentSize.z);
        }
    }
}
