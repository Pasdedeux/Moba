namespace common.game.engine.logic.effect
{   
    /// <summary>
    /// 效果,特定的效果改变特定的状态
    /// </summary>
    public interface IEffect
    {
        /// <summary>
        /// 效果改变状态
        /// </summary>
        void changeState();
    }
}