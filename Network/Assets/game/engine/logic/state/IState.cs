using System;

namespace common.game.engine.logic.state
{
    /// <summary>
    /// 状态
    /// *状态会改变*状态,比如速度、方向改变位置
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// 状态标识码
        /// </summary>
        /// <returns></returns>
        int GetCode();
        /// <summary>
        /// 更新状态
        /// </summary>
        void RunInLogic(DateTime runtime);
        /// <summary>
        /// 是否可移除
        /// </summary>
        /// <returns></returns>
        bool CanRemove();
    }
}