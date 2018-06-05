using common.game.engine.logic.state;
using common.game.engine.logic.unit;

namespace common.game.engine.logic.effect
{
    /// <summary>
    /// 移动效果
    /// </summary>
    public class MoveEffect : IEffect
    {
        private Unit owner;//被作用者
        private float? changeSpeed;//改变的速度
        private Vector3 changeVector3;//改变的方向
        public MoveEffect(Unit owner,float? speed,Vector3 vector3)
        {
            this.owner = owner;
            changeSpeed = speed;
            changeVector3 = vector3;
        }
        public void changeState()
        {
            MoveState state = owner.getMoveState();
            state.ChangeMove(changeSpeed, changeVector3);
        }
    }
}