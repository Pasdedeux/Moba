using common.game.engine.logic.action;
using common.game.engine.logic.effect;
using common.game.battle.engine.shape;
using common.game.engine.logic.state;
using common.lib.core.collections.concurrent;
using System;
using System.Collections.Generic;
using common.game.engine.logic.controller;

namespace common.game.engine.logic.unit
{
    public delegate void OnPositionChange(Vector3 position);
    /// <summary>
    /// 单元类
    /// </summary>
    public class Unit
    {
        protected int id;
        public int Id { get { return id; }set { id = value; } }
        private int cid;
        public int Cid { get { return cid; } }
        private bool isControl;
        public bool IsControl { get { return isControl; } }
        private Shap shap;
        private volatile Vector3 bronPoint;
        public Vector3 BronPoint { get { return bronPoint; } }
        public Shap Shap { get { return shap; } }
        public OnPositionChange OnPositionChange { get; set; }
        protected LinkedBlockingQueue<IAction> actions;//输入动作
        protected Queue<IEffect> effects;//受到的效果
        protected Dictionary<int, IState> states;//状态
        public Dictionary<int, IState> States { get { return states; } }
        private bool isRemove = false;//是否移除
        public bool IsRemove { get { return isRemove; } }
        public Unit(int cid, bool isControl, Shap shap):this(cid,isControl,shap,new Vector3(0,0,0)){}
        private IMoveController moveController;
        public IMoveController MoveController { get { return moveController; } set { moveController = value; } }
        public Unit(int cid,bool isControl,Shap shap,Vector3 bronPoint)
        {
            this.cid = cid;
            this.isControl = isControl;
            this.shap = shap;
            this.bronPoint = bronPoint;
            actions = new LinkedBlockingQueue<IAction>();
            effects = new Queue<IEffect>();
            states = new Dictionary<int, IState>();
        }
        /// <summary>
        /// 动作输入
        /// </summary>
        /// <param name="action"></param>
        public void Cmd(IAction action)
        {
            actions.Put(action);
        }
        /// <summary>
        /// 获取单元类状态
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        public IState getState(int stateCode)
        {
            if (!states.ContainsKey(stateCode))
                return null;
            return states[stateCode];
        }
        /// <summary>
        /// 添加单元类状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool AddState(IState state)
        {
            int stateCode = state.GetCode();
            if (states.ContainsKey(stateCode))
                return false;
            states.Add(stateCode, state);
            return true;
        }
        public void Remove()
        {
            isRemove = true;
        }
        /// <summary>
        /// 刷新-逻辑线程调用
        /// </summary>
        /// <param name="runtime"></param>
        public void RunInLogic(DateTime runtime)
        {
            for (IAction action = null; (action = actions.Poll())!=null;)
            {
                IEffect effect = action.Proc(this,runtime);
                if(effect!=null)
                    effects.Enqueue(effect);
            }
            for(IEffect effect = null;(effect = effects.Dequeue()) != null;)
                effect.changeState();
            List<int> removeStates = null;
            foreach(IState o in states.Values)
            {
                o.RunInLogic(runtime);
                if(o.CanRemove())
                {
                    if (removeStates == null)
                        removeStates = new List<int>();
                    removeStates.Add(o.GetCode());
                }
            }
            if (removeStates == null)
                return;
            for(int i = 0;i<removeStates.Count;i++)
            {
                int code = removeStates[i];
                if (states.ContainsKey(code))
                    states.Remove(code);
            }
        }
        /// <summary>
        /// 获得移动的状态
        /// </summary>
        /// <returns></returns>
        public MoveState getMoveState()
        {
            MoveState state = (MoveState)getState(MoveState.CODE);
            if (state == null)
            {
                state = new MoveState(BronPoint);
                state.Controller = moveController;
                AddState(state);
            }
            return state;
        }
    }
}
