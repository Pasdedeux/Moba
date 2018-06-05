using common.game.battle.engine.unit;
using System;
namespace common.game.battle.engine.action.move
{
    public class Move : IAction
    {
        private Unit owner;
        public Unit Owner { get; set; }
        private Vector3 position;
        public Vector3 Position { get; set; }
        private Vector3 dir;
        public Vector3 Dir { get; set; }
        private float speed;
        public float Speed { get; set; }
        private DateTime lastRuntime;
        public DateTime LastRuntime { get; set; }
        private volatile Cmd cmd;
        public Move(Unit owner)
        {
            this.owner = owner;
        }
        public void Change(Vector3 dir, float speed)
        {
            Change(null, dir, speed);
        }
        public void Flash(Vector3 position)
        {
            Change(position, null, 0);
        }
        public void Change(Vector3 postion,Vector3 dir,float speed)
        {
            cmd = new Cmd(position, dir, speed);
        }
        public void Run(DateTime runtime)
        {
            long detalTime = runtime == null ? 1 : runtime.Millisecond - lastRuntime.Millisecond;
            exeCmd();
            if (dir == null || speed == 0)
                return;
            Vector3 deltalPostion = Vector3.Multiply(Vector3.Multiply(dir, speed), detalTime);
            position.Plus(deltalPostion);
            owner.OnPositionChange(position);
        }
        private void exeCmd()
        {
            if (cmd == null)
                return;
            if (cmd.Position != null)
                position = cmd.Position;
            if (cmd.Dir != null)
                dir = cmd.Dir;
            if (cmd.Speed != null)
                speed = (float)cmd.Speed;
        }
        public void Proc(DateTime runtime)
        {
            Run(runtime);
        }
    }
}
