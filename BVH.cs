/********************************************************************************
** All rights reserved
** Auth： kay.yang
** Date： 6/9/2017 4:52:07 PM
** Version:  v1.0.0
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace BVH
{
    public class BVHFlatNode
    {
        public BVHBox mBox = new BVHBox();
        public uint mStartIndex;
        public uint mLeafCount;
        public uint mRightOffset;
    }

    public class BVHTraversal
    {
        public int mIndex; 
        public float mLength;
        public BVHTraversal() { }
        public BVHTraversal(int idx, float len)
        {
            mIndex = idx;
            mLength = len;
        }
    }

    public class BVHBuildEntry
    {
        public uint mParent;
        public uint mStart;
        public uint mEnd;
    }

    /// <summary>
    /// 1 对静态的射线检测性能不错
    /// 2 对于动态的，需要重构bvh tree，性能比不上静态
    /// Note: 最好静态的使用
    /// </summary>
    public class BVH
    {
        static BVHBuildEntry[] PREALLOC;
        static BVH()
        {
            PREALLOC = new BVHBuildEntry[128];
            for (int i = 0; i < 128; ++i)
            {
                PREALLOC[i] = new BVHBuildEntry();
            }
        }

        private int mNumNodes, mNumLeafs, mNodeMaxLeafSize;
        private List<BVHObject> mBuildPrims;
        private List<BVHFlatNode> mFlatTreeList = null;
        public BVH(List<BVHObject> objects, int _leafSize = 4)
        {
            mBuildPrims = objects;
            mNodeMaxLeafSize = _leafSize;
            mNumNodes = mNumLeafs = 0;
            Build();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ray">射线</param>
        /// <param name="intersection">交点信息</param>
        /// <param name="occlusion">是否找到最短的。  true if 找到交叉就行； false if 找到最短的 </param>
        /// <returns></returns>
        public bool GetIntersection(BVHRay ray, ref BVHIntersectionInfo intersection, bool occlusion)
        {
            intersection.mLength = 999999999.0f;
            intersection.mObject = null;
            float[] bbhits = new float[4];
            int closer, other;
            BVHTraversal[] todo = new BVHTraversal[64];
            todo[0] = new BVHTraversal();
            int stackptr = 0;
            todo[stackptr].mIndex = 0;
            todo[stackptr].mLength = -9999999.0f;
            while(stackptr >= 0) 
            {
                int ni = todo[stackptr].mIndex;
                float near = todo[stackptr].mLength;
                stackptr--;
                BVHFlatNode node = mFlatTreeList[ni];
                if(near > intersection.mLength)
                    continue;
                // 对叶节点做相交测试
                if( node.mRightOffset == 0 )
                {
                    bool hit = false;
                    for(int o = 0; o < node.mLeafCount; ++o) 
                    {
                        BVHIntersectionInfo current = new BVHIntersectionInfo();
                        BVHObject obj = mBuildPrims[(int)node.mStartIndex + o];
                        hit = obj.GetIntersection(ref ray, ref current);
                        if (hit) 
                        {
                            if(occlusion)
                            {
                                return true;
                            }
                            if (current.mLength < intersection.mLength) 
                            {
                                intersection = current;
                            }
                        }
                    }
                } 
                else
                { 
                    // 对父结点做测试
                    bool hitc0 = mFlatTreeList[ni + 1].mBox.Intersect(ray, ref bbhits[0], ref bbhits[1]);
                    bool hitc1 = mFlatTreeList[ni + (int)node.mRightOffset].mBox.Intersect(ray, ref bbhits[2], ref bbhits[3]);
                    if(hitc0 && hitc1)
                    {
                        closer = ni + 1;
                        other = ni + (int)node.mRightOffset;
                        if(bbhits[2] < bbhits[0])
                        {
                            float temp = bbhits[0];
                            bbhits[0] = bbhits[2];
                            bbhits[2] = temp;
                            temp = bbhits[1];
                            bbhits[1] = bbhits[3];
                            bbhits[3] = temp;
                            int itemp = closer;
                            closer = other;
                            other = itemp;
                        }
                        todo[++stackptr] = new BVHTraversal(other, bbhits[2]);
                        todo[++stackptr] = new BVHTraversal(closer, bbhits[0]);
                    }
                    else if (hitc0) 
                    {
                        todo[++stackptr] = new BVHTraversal(ni + 1, bbhits[0]);
                    }
                    else if (hitc1)
                    {
                        todo[++stackptr] = new BVHTraversal(ni + (int)node.mRightOffset, bbhits[2]);
                    }
                }
            } 
            if (intersection.mObject != null)
            {
                intersection.mHitPoint = ray.mOrigin + ray.mDirection * intersection.mLength;
            }
            return intersection.mObject != null;
        }

        // this is not property.but just support dynamic add operator
        public void AddObject(BVHObject obj, bool imme = false)
        {
            mBuildPrims.Add(obj);
            if (imme)
            {
                Build();
            }
        }
        // this is not property.but just support dynamic delete operator
        public void DeleteObject(BVHObject obj, bool imme = false)
        {
            bool success = mBuildPrims.Remove(obj);
            if (success && imme)
            {
                Build();
            }
        }

        private void Build()
        {
            int stackptr = 0;
            uint Untouched    = 0xffffffff;
            uint TouchedTwice = 0xfffffffd;
            PREALLOC[stackptr].mStart = 0;
            PREALLOC[stackptr].mEnd = (uint)mBuildPrims.Count;
            PREALLOC[stackptr].mParent = 0xfffffffc;
            stackptr++;
            List<BVHFlatNode> buildnodes = new List<BVHFlatNode>(mBuildPrims.Count * 2);
            while(stackptr > 0) 
            {
                BVHBuildEntry bnode = PREALLOC[--stackptr];
                uint start = bnode.mStart;
                uint end = bnode.mEnd;
                uint nPrims = end - start;
                mNumNodes++;
                BVHFlatNode node = new BVHFlatNode();
                node.mStartIndex = start;
                node.mLeafCount = nPrims;
                node.mRightOffset = Untouched;
                BVHBox bb = new BVHBox(mBuildPrims[(int)start].GetBBox().mMin, mBuildPrims[(int)start].GetBBox().mMax);
                BVHBox bc = new BVHBox(mBuildPrims[(int)start].GetCentroid());
                for(uint p = start + 1; p < end; ++p)
                {
                    bb.ExpandToInclude(mBuildPrims[(int)p].GetBBox());
                    bc.ExpandToInclude(mBuildPrims[(int)p].GetCentroid());
                }
                node.mBox = bb;
                if(nPrims <= mNodeMaxLeafSize)
                {
                    node.mRightOffset = 0;
                    mNumLeafs++;
                }

                buildnodes.Add(node);
                // 记录父节点关于右孩子结点相对父结点的偏移值mRightOffset
                // 第一次为左孩子，相对父结点的偏移值为1
                // 每个父节点最多被两次 hit
                if(bnode.mParent != 0xfffffffc)
                {
                    buildnodes[(int)bnode.mParent].mRightOffset--;
                    if( buildnodes[(int)bnode.mParent].mRightOffset == TouchedTwice)
                    {
                        buildnodes[(int)bnode.mParent].mRightOffset = (uint)mNumNodes - 1 - bnode.mParent;
                    }
                }
                if(node.mRightOffset == 0)
                    continue;
                // 选择合适的分割维度
                uint split_dim = (uint)bc.MaxDimension();
                float split_coord = 0.5f * (bc.mMin[(int)split_dim] + bc.mMax[(int)split_dim]);
                uint mid = start;
                // 交换 start 和 end 之间 的数据
                for(uint i = start; i < end; ++i)
                {
                    if(mBuildPrims[(int)i].GetCentroid()[(int)split_dim] < split_coord ) 
                    {
                        BVHObject temp = mBuildPrims[(int)i];
                        mBuildPrims[(int)i] = mBuildPrims[(int)mid];
                        mBuildPrims[(int)mid] = temp;
                        ++mid;
                    }
                }
                if(mid == start || mid == end)
                {
                    mid = start + (end - start) / 2;
                }
                // 右孩子
                PREALLOC[stackptr].mStart = mid;
                PREALLOC[stackptr].mEnd = end;
                PREALLOC[stackptr].mParent = (uint)mNumNodes - 1;
                stackptr++;
                // 左孩子
                PREALLOC[stackptr].mStart = start;
                PREALLOC[stackptr].mEnd = mid;
                PREALLOC[stackptr].mParent = (uint)mNumNodes - 1;
                stackptr++;
            }
            if (mFlatTreeList != null)
                mFlatTreeList.Clear();
            mFlatTreeList = new List<BVHFlatNode>(mNumNodes);
            for (uint n = 0; n < mNumNodes; ++n)
            {
                mFlatTreeList.Add(buildnodes[(int)n]);
            }
        }
    }
}
