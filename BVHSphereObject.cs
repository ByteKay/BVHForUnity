/********************************************************************************
** Copyright 深圳市全名点游科技有限公司
** All rights reserved
** Auth： kay.yang
** Date： 6/12/2017 9:12:46 AM
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
    public class BVHSphereObject : BVHObject
    {
        public Vector3 mCenter;
        public float mRadius, mRadius2; // Radius, Radius^2

        public BVHSphereObject(Vector3 center, float r)
        {
            mCenter = center;
            mRadius = r;
            mRadius2 = mRadius * mRadius;
        }

        override
        public bool GetIntersection(ref BVHRay ray, ref BVHIntersectionInfo intersection)
        {
            Vector3 s = mCenter - ray.mOrigin;
            float sd = Vector3.Dot(s, ray.mDirection);
            float ss = s.magnitude * s.magnitude;
            float disc = sd * sd + mRadius2 - ss;
            if (disc < 0.0f)
            {
                return false;
            }
            intersection.mObject = this;
            intersection.mLength = sd - Mathf.Sqrt(disc);
            return true;
        }

        override
        public Vector3 GetNormal(ref BVHIntersectionInfo i)
        {
            Vector3 nor = i.mHitPoint - mCenter;
            nor.Normalize();
            return nor;
        }

        override
        public BVHBox GetBBox()
        {
            return new BVHBox(mCenter - new Vector3(mRadius, mRadius, mRadius), mCenter + new Vector3(mRadius, mRadius, mRadius)); ;
        }

        override
        public Vector3 GetCentroid()
        {
            return mCenter;
        }
    }
}
