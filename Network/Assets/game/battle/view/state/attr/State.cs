using System;
using UnityEngine;
namespace game.battle.view.state.attr
{
    public class State
    {
        private AbstractLogReport logReport = LoggerFactory.getInst().getUnityLogger();
        private Vector3 position;
        public void OnPositionChange(common.game.battle.engine.Vector3 position)
        {
            this.position = new Vector3(position.X, position.Y, position.Z);
        }
        /// <summary>
        /// 同步单元位置
        /// </summary>
        public void SynPosition()
        {
            Vector3 position = this.position;
            //TODO unity中更新位置
            logReport.OnLogReport("move to bronPoint x:" + position.x + ",y:" + position.y + ",z:" + position.z);
        }
        public void Update(DateTime runtime)
        {
            SynPosition();
        }
    }
}
