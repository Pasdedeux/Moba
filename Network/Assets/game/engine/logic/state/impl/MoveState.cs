using common.game.engine.logic.controller;
using System;

namespace common.game.engine.logic.state
{
    /// <summary>
    /// 移动状态
    /// </summary>
    public class MoveState : IState
    {
        public const int CODE = BaseStateConst.MOVE;
        private Vector3 position;
        private float curSpeed = 0;//当前速度
        private Vector3 curVector3;//当前方向向量
        private float? nextSpeed;//将被改变的速度,在下一逻辑帧改变为当前速度并置空
        private Vector3 nextVector3;//将被改变的方向向量,在下一逻辑帧改变为当前向量并置空
        private DateTime synTime;//同步状态时间
        private Vector3 synPosition;//同步位置
        private const int SYN_PERIOD = 100;//同步时间间隔,单位毫秒
        private IMoveController controller;
        public IMoveController Controller { get { return controller; } set { controller = value; } }
        public MoveState(Vector3 position)
        {
            synPosition = this.position = position;
            synTime = DateTime.Now;
        }
        public bool CanRemove()
        {
            return false;
        }

        public int GetCode()
        {
            return CODE;
        }

        public void RunInLogic(DateTime runtime)
        {
            if (curSpeed != 0 && curVector3 != null)
            {
                Vector3 delta = Vector3.Multiply(curVector3, curSpeed);
                position.Plus(delta);
            }
            SwitchToNewStatePreFrame();
            SynState(runtime);
        }

        //public void Update(Unit unit)
        //{
        //    unit.transform.position = new UnityEngine.Vector3(synPosition.X, synPosition.Y, synPosition.Z);
        //}

        /// <summary>
        /// 改变移动状态
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="vector3"></param>
        public void ChangeMove(float? speed, Vector3 vector3)
        {
            nextSpeed = speed;
            nextVector3 = vector3;
        }
        /// <summary>
        /// 每帧调用切换更新最新运动状态
        /// </summary>
        public void SwitchToNewStatePreFrame()
        {
            if (nextSpeed != null)
            {
                curSpeed = (float)nextSpeed;
                nextSpeed = null;
            }  
            if (nextVector3 != null)
            {
                curVector3 = nextVector3;
                nextVector3 = null;
            }  
        }
        /// <summary>
        /// 按移动的频率同步状态
        /// </summary>
        /// <param name="runtime"></param>
        public void SynState(DateTime runtime)
        {
            if (runtime.Millisecond - synTime.Millisecond < SYN_PERIOD)
                return;
            if (Vector3.Equal(position, synPosition))
                return;
            controller.move(synPosition);
        }
    }
}