using common.game.battle.engine.shape;

namespace common.game.engine.view.unit
{
    public abstract class Unit
    {
        private logic.unit.Unit module;
        public Unit(int cid, bool isControl, Shap shap, Vector3 bronPoint)
        {
            module = new logic.unit.Unit(cid, isControl, shap, bronPoint);
        }
        public abstract void BindController();
    }
}
