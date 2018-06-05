using common.lib.core.collections.filter;
namespace common.game.battle.engine.action
{
    public class ActionChain:Filterchain
    {
        public void Add(Action action)
        {
            Add(new ActionFilter(action));
        }
    }
}
