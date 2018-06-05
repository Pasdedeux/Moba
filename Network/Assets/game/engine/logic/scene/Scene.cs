using System;
using common.lib.core.collections.concurrent;
using common.game.engine.logic.unit;
using System.Collections.Generic;
using common.game.engine.logic.runner;

namespace common.game.engine.logic.scene
{
    /// <summary>
    /// 场景类
    /// </summary>
    public class Scene:IRunable
    {
        private LinkedBlockingQueue<Unit> addUnits = new LinkedBlockingQueue<Unit>();//物件add缓冲
        private Dictionary<int, Unit> units = new Dictionary<int, Unit>();//物件表
        private static int seq = 0;
        private static byte[] seqLock = new byte[0];
        public int getSeq()
        {
            int rs = 0;
            lock (seqLock)
            {
                seq++;
                rs = seq;
            }
            return rs;
        }
        public Scene(){}
        public void RunInLogic(DateTime runtime)
        {
            Add();
            Remove();
            RunUnitsInLogic(runtime);
        }
        public void Add(Unit unit)
        {
            int id = getSeq();
            unit.Id = id;
            addUnits.Put(unit);
        }
        /// <summary>
        /// 执行添加
        /// </summary>
        private void Add()
        {
            Unit o = null;
            while ((o = addUnits.Poll()) != null)
                units.Add(o.Id, o);
        }
        /// <summary>
        /// 执行移除
        /// </summary>
        public void Remove()
        {
            List<int> removeList = null;
            foreach (Unit o in units.Values)
            {
                if(o.IsRemove)
                {
                    if (removeList == null)
                        removeList = new List<int>();
                    removeList.Add(o.Id);
                }
            }
            if (removeList == null)
                return;
            for (int i = 0; i < removeList.Count; i++)
                units.Remove(removeList[i]);
        }
        public void RunUnitsInLogic(DateTime runtime)
        {
            foreach (Unit o in units.Values)
                o.RunInLogic(runtime);
        }
    }
}
