using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

namespace HSM
{
    public abstract class State //状态树节点
    {
        public readonly StateMachine Machine;//发送状态转换请求
        public readonly State Parent;//回溯到父级状态节点
        public State ActiveChild;//活跃子节点
        readonly List<IActivity> activities = new List<IActivity>();
        public IReadOnlyList<IActivity> Activities => activities;

        protected bool isLookingChildren = true;

        public State(StateMachine machine, State parent = null)
        {
            Machine = machine;
            Parent = parent;
        }

        public void Add(IActivity a) { if (a != null) activities.Add(a); }

        protected virtual State GetInitialState() => null;//默认激活的子状态(nul=l则为最终状态节点)

        protected virtual State GetTransition() => null;//当前帧是否需要切换状态 (null=不需要切换)

        // 生命周期钩子
        protected virtual void OnEnter() { }

        protected virtual void OnExit() { }

        protected virtual void OnUpdate(float deltaTime) { }

        internal void Enter()
        {
            if (Parent != null) Parent.ActiveChild = this;
            OnEnter();
            State init = GetInitialState();
            if (init != null) init.Enter();
        }

        internal void Exit()
        {
            if (ActiveChild != null) ActiveChild.Exit();
            ActiveChild = null;
            OnExit();
        }

        internal void Update(float deltaTime)
        {
            State to = GetTransition();
            if (to != null)
            {
                State form = LookingChildren();

                Machine.Sequencer.RequestTransition(form, to);
                return;
            }
            if (ActiveChild != null) ActiveChild.Update(deltaTime);

            OnUpdate(deltaTime);
        }

        public State Leaf()
        {
            State s = this;
            while (s.ActiveChild != null) s = s.ActiveChild;
            return s;
        }

        public IEnumerable<State> PathToRoot()
        {
            for (State s = this; s != null; s = s.Parent) yield return s;
        }

        protected State LookingChildren()
        {
            if (isLookingChildren && ActiveChild != null)
                return ActiveChild.LookingChildren();

            return this;
        }
    }


    public class InteractionState : State
    {
        public InteractionState(StateMachine machine, State parent = null) : base(machine, parent) { }

        public void TriggerEnter(Collider2D collision)
        {
            OnTriggerEnter(collision);

            if (ActiveChild != null) ((InteractionState)ActiveChild).TriggerEnter(collision);

        }
        public void TriggerExit(Collider2D collision)
        {
            if (ActiveChild != null) ((InteractionState)ActiveChild).TriggerExit(collision);

            OnTriggerExit(collision);
        }

        public void TriggerUpdate(Collision2D collision)
        {
            if (ActiveChild != null) ((InteractionState)ActiveChild).TriggerUpdate(collision);

            OnTriggerUpdate(collision);
        }

        public void CollisionEnter(Collision2D collision)
        {
            OnCollisionEnter(collision);
            if (ActiveChild != null) ((InteractionState)ActiveChild).CollisionEnter(collision);


        }

        public void CollisionExit(Collision2D collision)
        {
            if (ActiveChild != null) ((InteractionState)ActiveChild).CollisionExit(collision);

            OnCollisionExit(collision);
        }

        public void CollisionUpdate(Collision2D collision)
        {
            if (ActiveChild != null) ((InteractionState)ActiveChild).CollisionUpdate(collision);

            OnCollisionUpdate(collision);
        }

        protected virtual void OnTriggerEnter(Collider2D collision) { }
        protected virtual void OnTriggerExit(Collider2D collision) { }
        protected virtual void OnTriggerUpdate(Collision2D collision) { }
        protected virtual void OnCollisionEnter(Collision2D collision) { }
        protected virtual void OnCollisionExit(Collision2D collision) { }
        protected virtual void OnCollisionUpdate(Collision2D collision) { }
    }

}