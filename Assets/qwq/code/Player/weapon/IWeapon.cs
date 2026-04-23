using UnityEngine;

namespace qwq
{
    public interface IWeapon
    {
        public void Initialize(PlayerContext ctx);
        public void OnEnter();
        public void OnUpdate(float deltaTime);
        public void OnExit();
        public bool IsEnter();
    }

    public abstract class Weapon : MonoBehaviour, IWeapon
    {
        protected PlayerContext ctx;
        public virtual void Initialize(PlayerContext ctx)
        {
            this.ctx = ctx;
        }

        public abstract bool IsEnter();
        public abstract void OnEnter();

        public abstract void OnExit();

        public abstract void OnUpdate(float deltaTime);
    }
}