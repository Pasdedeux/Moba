using game.battle.view.state.attr;
using System;
using UnityEngine;
namespace game.battle.view
{
    /// <summary>
    /// 表现层单元类
    /// </summary>
    public class Unit
    {
        private AbstractLogReport logReport = LoggerFactory.getInst().getUnityLogger();
        private int id;
        public int Id { get { return id; } }
        private common.game.battle.engine.unit.Unit engineUnit;
        private GameObject model;
        public GameObject Model { get { return model; } }
        private bool isValid;
        public bool IsValid { get; set; }
        private State state;
        public Unit(common.game.battle.engine.unit.Unit unit,GameObject model)
        {
            id = unit.Id;
            engineUnit = unit;
            engineUnit.OnPositionChange = OnPositionChange;
            this.model = model;
            isValid = true;
        }
        public void OnPositionChange(common.game.battle.engine.Vector3 position)
        {
            state.OnPositionChange(position);
        }
        /// <summary>
        /// 获取移动动作
        /// </summary>
        /// <returns></returns>
        public common.game.battle.engine.action.move.Move GetMove()
        {
            return engineUnit.Move;
        }
        public void OnDestroy()
        {
            isValid = false;
        }
        public void update(DateTime runtime)
        {
            state.Update(runtime);
        }

    }
}
