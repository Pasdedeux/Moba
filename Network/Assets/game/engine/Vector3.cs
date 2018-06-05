using System;
namespace common.game.engine
{
    /// <summary>
    /// 3D向量类
    /// </summary>
    public class Vector3
    {
        float x, y, z;
        public float X { get { return x; } }
        public float Y { get { return y; } }
        public float Z { get { return z; } }
        public Vector3()
        {
            x = y = z = 0;
        }
        public Vector3(Vector3 input)
        {
            x = input.x;
            y = input.y;
            z = input.z;
        }
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public bool Equals(Vector3 input)
        {
            return x == input.x && y == input.y && z == input.z;
        }
        public void Zero()
        {
            x = y = z = 0.0f;
        }
        public Vector3 Negative()
        {
            return new Vector3(-x, -y, -z);
        }
        public Vector3 NegativeDir()
        {
            Vector3 rs = Negative();
            return rs.Normalize();
        }
        public Vector3 Plus(Vector3 input1, Vector3 input2)
        {
            return new Vector3(input1.x + input2.x, input1.y + input2.y, input1.z + input2.z);
        }

        public Vector3 plusDir(Vector3 input1, Vector3 input2)
        {
            Vector3 rs = Plus(input1, input2);
            return rs.Normalize();
        }
        public static Vector3 Reduce(Vector3 input1, Vector3 input2)
        {
            return new Vector3(input1.x - input2.x, input1.y - input2.y, input1.z - input2.z);
        }
        public Vector3 ReduceDir(Vector3 input1, Vector3 input2)
        {
            Vector3 rs = Reduce(input1, input2);
            return rs.Normalize();
        }
        public static Vector3 Multiply(Vector3 input, float rate)
        {
            Vector3 dir = input.Normalize();
            return new Vector3(dir.x * rate, dir.y * rate, dir.z * rate);
        }
        public Vector3 Divide(Vector3 input, float rate)
        {
            float oneOverA = 1.0f / rate;//注意这里不对"除零"进行处理
            return new Vector3(input.x * oneOverA, input.y * oneOverA, input.z * oneOverA);
        }
        public Vector3 Plus(Vector3 input)
        {
            x += input.x;
            y += input.y;
            z += input.z;
            return this;
        }
        public Vector3 Reduce(Vector3 input)
        {
            x -= input.x;
            y -= input.y;
            z -= input.z;
            return this;
        }
        public Vector3 Divide(float input)
        {
            float oneOverA = 1.0f / input;
            x *= oneOverA;
            y *= oneOverA;
            z *= oneOverA;
            return this;
        }
        /**
         * 获得标准化向量
         */
        public Vector3 Normalize()
        {
            float magSq = x * x + y * y + z * z;
            if (magSq > 0.0f)
            {//检查除零
                float oneOverMag = 1.0f / (float)Math.Sqrt(magSq);
                return new Vector3(x * oneOverMag, y * oneOverMag, z * oneOverMag);
            }
            return new Vector3(0, 0, 0);
        }
        /**
         * 点乘-用于算夹角
         * @param input
         * @return
         */
        public float Multiply(Vector3 input1, Vector3 input2)
        {
            return input1.x * input2.x + input1.y * input2.y + input1.z * input2.z;
        }
        /**
         * 计算两向量的叉乘,求法向向量(垂直于两个向量构成的平面的向量)
         * Cross Product叉乘公式 
         * aXb = | i   j   k  | 
         *       | a.x a.y a.z| 
         *       | b.x b.y b.z| = (a.y*b.z -a.z*b.y)i + (a.z*b.x - a.x*b.z)j + (a.x+b.y - a.y*b.x)k 
         * @param input1
         * @param input2
         * @return
         */
        public Vector3 CrossProduct(Vector3 input1, Vector3 input2)
        {
            Vector3 rs = new Vector3(
                    input1.y * input2.z - input1.z * input2.y,
                    input1.z * input2.x - input1.x * input2.z,
                    input1.x * input2.y - input1.y * input2.x
            );
            return rs.Normalize();
        }
        /**
         * 向量夹角cos值
         * @return
         */
        public float IncludedAngle(Vector3 input1, Vector3 input2)
        {
            return Multiply(input1, input2) / (input1.Mag() * input2.Mag());
        }
        /**
         * 求向量模
         * @return
         */
        public float Mag()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }
        /**
         * 两点间距离
         * @param input
         * @return
         */
        public float Distance(Vector3 input)
        {
            float dx = input.x - x;
            float dy = input.y - y;
            float dz = input.z - z;
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
        /// <summary>
        /// 判断两点是否相等
        /// </summary>
        /// <param name="input1"></param>
        /// <param name="input2"></param>
        /// <returns></returns>        
        public static bool Equal(Vector3 input1,Vector3 input2)
        {
            return input1.x == input2.x && input1.y == input2.y && input1.z == input2.z;
        }
        public Vector3 Clone()
        {
            return (Vector3)MemberwiseClone();
        }
    }
}
