using System;
using common.lib.core.collections.filter;
namespace common.game.battle.engine.action
{
    class ActionFilter : Filter
    {
        private Action action;
        public ActionFilter(Action action)
        {
            this.action = action;
        }
        public override void doFilter(DateTime runtime)
        {
            action.Proc(runtime);
        }
    }
}
