/********************************************************************************
** All rights reserved
** Auth： kay.yang
** E-mail: 1025115216@qq.com
** Date： 6/30/2017 11:13:04 AM
** Version:  v1.0.0
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using KayMath;

namespace KayAlgorithm
{
    public class RectPartionNode
    {
        Vector2 mStart;
        Vector2 mSize;
        bool mIsChildren;
        public int mIndex;
        RectPartionNode mLeftChild;
        RectPartionNode mRightChild;

        public float this[int idx]
        {
            get
            {
                return mStart[idx];
            }
        }

        public RectPartionNode()
        {
            mStart = new Vector2();
            mSize = new Vector2();
            mLeftChild = null;
            mRightChild = null;
            mIsChildren = false;
            mIndex = 0;
        }
        public RectPartionNode(float x, float y, float w, float h)
        {
            mStart = new Vector2(x, y);
            mSize = new Vector2(w, h);
            mLeftChild = null;
            mRightChild = null;
            mIsChildren = false;
            mIndex = 0;
        }

        public RectPartionNode Insert(float w, float h, float intervalW = 0.0f, float intervalH = 0.0f)
	    {
            RectPartionNode newNode = null;
		    float dw, dh;
            if (!mIsChildren && 0 == mIndex)
		    {
			    dw = mSize[0] - w;
			    dh = mSize[1] - h;
                if (dw >= 0 && dh >= 0)
			    {
                    if (dw < 0.01f && dh < 0.01f)
				    {
					    return this;
				    }
				    else
				    {
					    float t = dh - dw;
                        if (t < 0)
					    {
                            mLeftChild = new RectPartionNode(mStart[0], mStart[1], w, mSize[1]);
                            mRightChild = new RectPartionNode(mStart[0] + w + intervalW, mStart[1], dw - intervalW, mSize[1]);
						    mIsChildren = true;
					    }
					    else
					    {
                            mLeftChild = new RectPartionNode(mStart[0], mStart[1], mSize[0], h);
                            mRightChild = new RectPartionNode(mStart[0], mStart[1] + h + intervalH, mSize[0], dh - intervalH);
                            mIsChildren = true;
					    }
                        return mLeftChild.Insert(w, h, intervalW, intervalH);
				    }
			    }
			    else
				    return null;
		    }
            else if (mIsChildren)
            {
                newNode = mLeftChild.Insert(w, h, intervalW, intervalH);
                if (newNode == null)
                {
                    newNode = mRightChild.Insert(w, h, intervalW, intervalH);
                }
            }
            else
            {
                return null;
            }
		    return newNode;
	    }
    }

    public class RectPartion
    {
        private List<RectPartionNode> mRectList;
        private Dictionary<int, Vector2> mMinRect = new Dictionary<int, Vector2>();
        public RectPartion()
        {
            mRectList = new List<RectPartionNode>();
        }
        public void AddRect(float x, float y, float w, float h)
        {
            mMinRect.Add(mRectList.Count, new Vector2(w, h));
            mRectList.Add(new RectPartionNode(x, y, w, h));
            
        }
        public RectPartionNode this[int index]
        {
            get
            {
                return mRectList[index];
            }
            set
            {
                mRectList[index] = value;
            }
        }

        public bool GetNextImage(float w, float h, int texIndex, out float lmX, out float lmY, out int mLInd, float intervalW = 0.0f, float intervalH = 0.0f)
        {
            lmX = lmY = mLInd = 0;
            int lIndex = 0;
            RectPartionNode node = null;
            for (int i = 0; i < mRectList.Count; ++i)
            {
                if (mMinRect[i][0] > w && mMinRect[i][1] > h)
                {
                    if ((node = mRectList[i].Insert(w, h, intervalW, intervalH)) != null)
                    {
                        break;
                    }
                    else
                    {
                        mMinRect[i] = new Vector2(Math.Min(w, mMinRect[i][0]), Math.Min(h, mMinRect[i][1]));
                    }
                }
                lIndex++;
            }
            if (node != null)
            {
                lmX = node[0];
                lmY = node[1];
                mLInd = lIndex;
                node.mIndex = texIndex;
                return true;
            }
            else
                return false;
        }

    };
}
