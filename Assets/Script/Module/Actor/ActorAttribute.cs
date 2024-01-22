using GameFramework;
using GameFramework.Module;

namespace Assets.Script.Module.Actor
{
    public enum EActorAttr
    {
        Hp,
        Damage,

        MoveSpeed,
    }


    public class ActorAttribute
    {
        public readonly EActorAttr attrType;
        public readonly int ownerId;
        public float value { get { return this._value; } }

        private float _base = 0;
        private float _add = 0;
        private float _pct = 0;
        private float _value = 0;

        private (float, float)? _limit = null;//上下限

        public ActorAttribute(int owner, EActorAttr type, float _base, (float, float)? limit = null)
        {
            this.ownerId = owner;
            this.attrType = type;
            this._base = _base;
            this._limit = limit;
        }

        public void Reset(float value, (float, float)? limit = null)
        {
            this._base = value;
            _add = 0;
            _pct = 0;
            this._limit = limit;
            _Calc();
        }

        private void _Calc()
        {
            this._value = (_base + _add) * (1 + _pct);
            if (_limit != null)
            {
                _value = _value.Clamp(_limit.Value.Item1, _limit.Value.Item2);

            }
            ModuleManager.GetModule<ActorModule>().CallActorAttrChange(ownerId, attrType, _value);
        }

        /// <summary>
        /// 修改基础值
        /// </summary>
        /// <param name="add"></param>
        public void AddBase(float add)
        {
            _base += add;
            _Calc();
        }

        /// <summary>
        /// 修改额外值
        /// </summary>
        /// <param name="add"></param>
        public void AddExtra(float add)
        {
            _add += add;
            _Calc();
        }

        /// <summary>
        /// 修改额外百分比值 (0-1)
        /// </summary>
        /// <param name="add"></param>
        public void AddPercent(float add)
        {
            _pct += add;
            _Calc();
        }

        public void SetBase(float v)
        {
            _base = v;
            _Calc();
        }
        public void SetExtra(float v)
        {
            _add = v;
            _Calc();
        }
        public void SetPercent(float v)
        {
            _pct = v;
            _Calc();
        }



        public override string ToString()
        {
            return $"{attrType.GetEName()}:({_base}+{_add})*{1 + _pct}={_value}";
        }

    }
}