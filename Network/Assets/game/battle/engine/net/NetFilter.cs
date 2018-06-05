using common.lib.core.collections.filter;
using common.net.socket;
using System;
namespace common.game.battle.engine.net
{
    class NetFilter:Filter
    {
        private Client client;
        public NetFilter(Client client)
        {
            this.client = client;
        }
        public override void doFilter(DateTime runtime)
        {
            client.Update(runtime);
        }
    }
}
