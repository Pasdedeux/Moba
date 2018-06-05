namespace common.game.battle.engine.action.move
{
    class Cmd
    {
        private Vector3 position;
        public Vector3 Position { get { return position; } }
        private Vector3 dir;
        public Vector3 Dir { get { return dir; } }
        private float? speed;
        public float? Speed { get { return speed; } }
        public Cmd(Vector3 position,Vector3 dir,float? speed)
        {
            this.position = position;
            this.dir = dir;
            this.speed = speed;
        }
        public object Clone()
        {
            Cmd clone = (Cmd)MemberwiseClone();
            clone.position = position.Clone();
            clone.dir = dir.Clone();
            return clone;
        }
    }
}
