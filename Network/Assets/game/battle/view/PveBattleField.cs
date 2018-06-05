using common.game.battle.engine.runner;
using common.game.battle.engine.scene;
using game.battle.view;
/// <summary>
/// pve战场view类
/// </summary>
public class PveBattleField : BattleField
{
    public override Runner GenRunner(Scene scene)
    {
        return new common.game.battle.engine.runner.Runner(scene);
    }
    public override Scene GenScene()
    {
        return new PveScene();
    }
}
