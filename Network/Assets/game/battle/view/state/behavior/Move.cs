using game.battle.view.state.behavior;
using System;
using UnityEngine;
namespace game.battle.view
{
    public class Move : IState
    {
        private AbstractLogReport logReport = LoggerFactory.getInst().getUnityLogger();
        private Unit unit;
        public Move(Unit unit)
        {
            this.unit = unit;
        }
        public void Previous()
        {
        }
        public void In()
        {
            //播放移动动画
        }
        public void Exsit()
        {
        }
        public void Update(DateTime runtime)
        {
            //Vector3 bronPoint = this.bronPoint;
            //TODO unity中更新位置
            //logReport.OnLogReport("move to bronPoint x:" + bronPoint.x + ",y:" + bronPoint.y + ",z:" + bronPoint.z);
        }
        public State GetState()
        {
            return State.MOVE;
        }
    }
}
    
