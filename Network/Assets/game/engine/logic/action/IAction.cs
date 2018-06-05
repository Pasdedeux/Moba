using common.game.engine.logic.effect;
using common.game.engine.logic.unit;
using System;
namespace common.game.engine.logic.action
{
    /// <summary>
    /// 动作指令
    /// 分为主动指令和被动指令
    /// 主动指令:比如行走,施法技能等
    /// 被动指令:比如收到攻击等
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// 执行动作
        /// </summary>
        /// <param name="unit">动作处理者</param>
        /// <param name="runtime">执行时间</param>
        /// <returns></returns>
        IEffect Proc(Unit unit,DateTime runtime);
    }
}
