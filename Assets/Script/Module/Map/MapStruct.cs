using GameFramework;
using System;
using UnityEngine;

namespace Assets.Script.Module.Map
{

    public static class EMapLayer
    {
        /// <summary>
        /// 地面层
        /// </summary>
        public static string MapGround = "MapGround";


    }


    /// <summary>
    /// 地图格子索引
    /// </summary>
    public struct MapIndex
    {
        public int x; public int y;

        public MapIndex(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void Set(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj is MapIndex)
            {
                MapIndex temp = (MapIndex)obj;
                return this.x == temp.x && this.y == temp.y;
            }
            return false;
        }
        public override int GetHashCode()
        {

            return ShiftAndWrap(x.GetHashCode(), 2) ^ y.GetHashCode();
        }

        private int ShiftAndWrap(int value, int positions)
        {
            positions = positions & 0x1F;
            uint number = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
            uint wrapped = number >> (32 - positions);
            return BitConverter.ToInt32(BitConverter.GetBytes((number << positions) | wrapped), 0);
        }
    }

    /// <summary>
    /// 矩形范围
    /// </summary>
    public struct MapRect
    {
        public float x1;
        public float x2;
        public float y1;
        public float y2;
        public MapRect(float x1, float x2, float y1, float y2)
        {
            if (x1 > x2)
            {
                x1.swapTo(ref x2);
            }
            if (y1 > y2)
            {
                y1.swapTo(ref y2);
            }

            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
        }

        /// <summary>
        /// 区域四角点
        /// </summary>
        public Vector3[] rectPostions
        {
            get
            {
                Vector3[] result = new Vector3[4]
                {
                    new Vector3(x1,y1,0),
                    new Vector3(x1,y2,0),
                    new Vector3(x2,y1,0),
                    new Vector3(x2,y2,0)
                };
                return result;
            }
        }
        /// <summary>
        /// 区域中心点
        /// </summary>
        public Vector3 centerPosition
        {
            get
            {
                return new Vector3((x2 + x1) / 2, (y2 + y1) / 2, 0);
            }
        }
        /// <summary>
        /// 检查目标点是否在此范围内
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool checkScope(Vector3 position)
        {
            return checkNum(position.x, x1, x2) && checkNum(position.y, y1, y2);
        }

        private bool checkNum(float num, float v1, float v2)
        {
            float min = Math.Min(v1, v2);
            float max = Math.Max(v1, v2);
            return num >= min && num <= max;
        }
    }
}
