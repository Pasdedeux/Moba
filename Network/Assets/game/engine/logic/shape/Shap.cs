namespace common.game.battle.engine.shape
{
    /// <summary>
    /// 形状
    /// </summary>
    public class Shap
    {
        private float l;
        public float L { get { return l; } }
        private float w;
        public float W { get { return w; } }
        private float h;
        public float H { get { return h; } }
        private float sLen;
        public float SLen { get { return sLen; } }
        private float sWidth;
        public float SWidth { get { return sWidth; } }
        private float sHeight;
        public float SHeight { get { return sHeight; } }
        public Shap(float len, float width, float height)
        {
            Resize(len,width, height);
        }
        public void Resize(float len,float width, float height)
        {
            l = len;
            w = width;
            h = height;
            sLen = len / 2;
            sWidth = width / 2;
            sHeight = height / 2;
        }
        public Shap Clone()
        {
            return (Shap)MemberwiseClone();
        }
    }
}
