using Assets.Script.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

namespace Assets.Script.Utils
{
    public static class GizmosHelper
    {
        private static long _key = 0;
        private static List<(long, Action)> _drawAction = new List<(long, Action)>();
        static GizmosHelper()
        {
            GameEvents.GameGlobalEvent.OnDrawGizmosCall += () =>
            {
                for (int i = 0; i < _drawAction.Count; i++)
                {
                    _drawAction[i].Item2.Invoke();
                }
            };
        }
        /// <summary>
        /// 取消绘制任务
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveDraw(long key)
        {
            for (int i = _drawAction.Count - 1; i >= 0; i--)
            {
                if (_drawAction[i].Item1 == key)
                {
                    _drawAction.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// 添加自定义绘制任务 (适合单个绘制目标动态变更)
        /// </summary>
        /// <param name="drawAction"></param>
        /// <returns></returns>
        public static long AddDrawTask(Action drawAction)
        {
            long key = ++_key;
            _drawAction.Add((key, drawAction));
            return key;
        }



        /// <summary>
        /// 绘制线段
        /// </summary>
        /// <param name="color"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static long DrawLine(Color color, Vector3 from, Vector3 to)
        {
            long key = ++_key;
            Action drawAction = () =>
            {
                Gizmos.color = color;
                Gizmos.DrawLine(from, to);
            };
            _drawAction.Add((key, drawAction));
            return key;
        }

        /// <summary>
        /// 绘制镂空球形
        /// </summary>
        /// <param name="color"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static long DrawWireSphere(Color color, Vector3 center, float radius)
        {
            long key = ++_key;
            Action drawAction = () =>
            {
                Gizmos.color = color;
                Gizmos.DrawWireSphere(center, radius);
            };
            _drawAction.Add((key, drawAction));
            return key;
        }
        /// <summary>
        /// 绘制球形
        /// </summary>
        /// <param name="color"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static long DrawSphere(Color color, Vector3 center, float radius)
        {
            long key = ++_key;
            Action drawAction = () =>
            {
                Gizmos.color = color;
                Gizmos.DrawSphere(center, radius);
            };
            _drawAction.Add((key, drawAction));
            return key;
        }


        /// <summary>
        /// 绘制方形
        /// </summary>
        /// <param name="color"></param>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static long DrawCube(Color color, Vector3 center, Vector3 size)
        {
            long key = ++_key;
            Action drawAction = () =>
            {
                Gizmos.color = color;
                Gizmos.DrawCube(center, size);
            };
            _drawAction.Add((key, drawAction));
            return key;
        }
        /// <summary>
        /// 绘制镂空方形
        /// </summary>
        /// <param name="color"></param>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static long DrawWireCube(Color color, Vector3 center, Vector3 size)
        {
            long key = ++_key;
            Action drawAction = () =>
            {
                Gizmos.color = color;
                Gizmos.DrawWireCube(center, size);
            };
            _drawAction.Add((key, drawAction));
            return key;
        }

        /// <summary>
        /// 绘制网格
        /// </summary>
        /// <param name="color"></param>
        /// <param name="mesh"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static long DrawMesh(Color color, Mesh mesh, Vector3 position, Quaternion rotation)
        {
            long key = ++_key;
            Action drawAction = () =>
            {
                Gizmos.color = color;
                Gizmos.DrawMesh(mesh, position,rotation);
            };
            _drawAction.Add((key, drawAction));
            return key;
        }

    }

}
