/********************************************************************************
** All rights reserved
** Auth： kay.yang
** Date： 6/9/2017 5:59:13 PM
** Version:  v1.0.0
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BVH
{
    public class BVHTriangleObject : BVHObject
    {
        public Vector3 mP1;
        public Vector3 mP2;
        public Vector3 mP3;
        public Vector3 mCenter;
        public BVHBox mBox;
        public Vector3 mNormal;
        public BVHTriangleObject(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            mP1 = p1;
            mP2 = p2;
            mP3 = p3;
            mCenter = (mP1 + mP2 + mP3) * 0.33333f;
            Vector3 min = Vector3.Min(Vector3.Min(mP1, mP2), mP3);
            Vector3 max = Vector3.Max(Vector3.Max(mP1, mP2), mP3);
            mBox = new BVHBox(min, max);
            mNormal = Vector3.Cross(mP2 - mP1, mP3 - mP1);
            mNormal.Normalize();
        }

        override
        public bool GetIntersection(ref BVHRay ray, ref BVHIntersectionInfo intersection)
        {
            Vector3 edge1, edge2, tvec, pvec, qvec;  
            double det, inv_det;
            double t;
            //find vectors for two edges sharing vert0  
            edge1 = mP2 - mP1;
            edge2 = mP3 - mP1;
            // begin calculating determinant - also used to calculate U parameter  
            pvec = Vector3.Cross(ray.mDirection, edge2);
            // if determinant is near zero, ray lies in plane of triangle  
            det = Vector3.Dot(edge1, pvec);  
  
#if TEST_CULL // define TEST_CULL if culling is desired  
            if (det < 1e-5f)  
                return false;  
            tvec = ray.mOrigin - mP1;
            // calculate U parameter and test bounds  
            double u = Vector3.Dot(tvec, pvec); 
            if (u < 0.0 || u > det)  
                return false;  
  
            // prepare to test V parameter  
            qvec = Vector3.Cross(tvec, edge1);  
            // calculate V parameter and test bounds  
            double v = Vector3.Dot(ray.mDirection, qvec);  
            if (v < 0.0 || u +v > det)  
                return false;  
            // calculate t, scale parameters, ray intersects triangle  
            t = Vector3.Dot(edge2, qvec);
            inv_det = 1.0 / det;  
            t *= inv_det;  
            u *= inv_det;  
            v *= inv_det;  
#else           // the non-culling branch  
            if (det > -1e-5f && det < 1e-5f)  
                return false;  
            inv_det = 1.0 / det;  
  
            // calculate distance from vert0 to ray origin   
            tvec = ray.mOrigin - mP1;
            // calculate U parameter and test bounds  
            double u = Vector3.Dot(tvec, pvec) * inv_det;  
            if (u < 0.0 || u > 1.0)  
                return false;  
  
            // prepare to test V parameters  
            qvec = Vector3.Cross(tvec, edge1);  
  
            // calculate V paremeter and test bounds  
            double v = Vector3.Dot(ray.mDirection, qvec) * inv_det;  
            if (v < 0.0 || u + v > 1.0)  
                return false;  
  
            //calculate t, ray intersects triangle  
            t = Vector3.Dot(edge2, qvec) * inv_det;  
#endif  
            intersection.mLength = (float)t;
            intersection.mObject = this;
            return true;
        }
         
        override
        public Vector3 GetNormal(ref BVHIntersectionInfo i)
        {
            return mNormal;
        }

        override
        public BVHBox GetBBox()
        {
            return mBox;
        }

        override
        public Vector3 GetCentroid()
        {
            return mCenter;
        }
    }
}
