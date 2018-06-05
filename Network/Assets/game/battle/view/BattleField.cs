using common.game.battle.engine.runner;
using common.game.battle.engine.shape;
using common.game.engine.scene;
using common.lib.core.collections.concurrent;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace game.battle.view
{
    /// <summary>
    /// 表现层战场类，实现战场表现层的逻辑
    /// </summary>
    public abstract class BattleField
    {
        private AbstractLogReport logReport = LoggerFactory.getInst().getUnityLogger();
        protected Runner runner;
        protected Scene scene;
        private Dictionary<int, Unit> units = new Dictionary<int, Unit>();
		private LinkedBlockingQueue<common.game.battle.engine.unit.Unit> adds = new LinkedBlockingQueue<common.game.battle.engine.unit.Unit>();
        private LinkedBlockingQueue<Unit> removes = new LinkedBlockingQueue<Unit>();
        private Unit contrller;
        public Unit Contrller { set; get; }
        public BattleField()
        {
            scene = GenScene();
            runner = GenRunner(scene);
        }
        public void Init()
        {
            scene.OnUnitCrt = OnUnitCrt;
            scene.OnUnitRemove = OnUnitRemove;
        }
        public abstract Scene GenScene();
        public abstract Runner GenRunner(Scene scene);
        public void Start()
        {
            runner.Start();
        }
        public void Stop()
        {
            runner.Stop();
        }
        public void CrtUnit(int unitCid, bool isControl,Shap shap,Vector3 position)
        {
			UnitCrtCmd crtCmd = new UnitCrtCmd(unitCid,isControl, runner.BattleField,shap, new common.game.battle.engine.Vector3(position.x, position.y, position.z));
			scene.CrtUnit(crtCmd);
        }
		public void OnUnitCrt(common.game.battle.engine.unit.Unit unit)
        {
			adds.Put(unit);
        }
        private void SynAddUnits()
        {
            while(adds.GetCount()>0)
            {
                common.game.battle.engine.unit.Unit engineUnit = adds.take();
                int unitCid = engineUnit.Cid;
                string modelprefabDir = "scenes/Boy@skin";
                GameObject modelprefab = Resources.Load<GameObject>(modelprefabDir);
                common.game.battle.engine.Vector3 position = engineUnit.Position;
				GameObject model = GameObject.Instantiate(modelprefab, new Vector3(position.X, position.Y, position.Z), new Quaternion(0, 0, 0, 1));
                Unit unit = new Unit(engineUnit, model);
                units.Add(engineUnit.Id, unit);
                if (engineUnit.IsControl)
                    contrller = unit;
            }
        }
        private void OnUnitRemove(int id)
        {
            if(units.ContainsKey(id))
            {
                Unit unit = units[id];
                unit.IsValid = false;
            }
        }
        private void SynRemoveUnits()
        {
            while(removes.GetCount()>0)
            {
               Unit unit = removes.take();
               int id = unit.Id;
               if (units.ContainsKey(id))
                    units.Remove(id);
               //TODO 在unity中移除单元
            }
        }
        private void Run(DateTime runtime)
        {
            foreach(Unit unit in units.Values)
            {
                if(!unit.IsValid)
                {
                    removes.Put(unit);
                    continue;
                }
                unit.update(runtime);
            }
        }
        public void Update(DateTime runtime)
        {
            SynAddUnits();
            SynRemoveUnits();
            Run(runtime);
        }
    }
}
