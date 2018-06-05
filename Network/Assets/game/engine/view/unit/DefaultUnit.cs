using common.game.battle.engine.shape;

namespace common.game.engine.view.unit
{
    public class DefaultUnit : Unit
    {
        public DefaultUnit(int cid, bool isControl, Shap shap, Vector3 bronPoint):base(cid, isControl, shap, bronPoint)
        {
        }

        public override void BindController()
        {
            
        }
    }
}
