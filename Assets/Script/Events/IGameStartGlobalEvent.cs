using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Events
{
    public interface IGameStartGlobalEvent
    {
        /// <summary>
        /// 绘制更新 执行 Gizmos
        /// </summary>
        event Action OnDrawGizmosCall;
    }
}
