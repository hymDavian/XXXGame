using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script
{
    public static class ExtensionFunctions
    {
        /// <summary>
        /// 获取包括自身在内的所有子物体的包围盒 (一定会新生成包围盒对象)
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Bounds GetDeepBounds( this Transform transform)
        {
            
            Bounds _bounds = new Bounds();
            _bounds.center = transform.position;
            var rootRender = transform.GetComponent<Renderer>();
            if (rootRender != null)
            {
                _bounds.Encapsulate(rootRender.bounds);
            }
            var childRenderers = transform.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < childRenderers.Length; i++)
            {
                _bounds.Encapsulate(childRenderers[i].bounds);
            }
            return _bounds;
        }
    }
}
