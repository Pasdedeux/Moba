namespace game.battle.view.state.behavior
{
    public enum State
    {
        MOVE,//移动
        IDLE,//空闲
        DIE,//死亡
    }
    public interface IState
    {
        State GetState();
        void Previous();
        void In();
        void Exsit();
    }
}
