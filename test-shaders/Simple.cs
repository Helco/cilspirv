using System;
using System.Numerics;

namespace test_shaders
{
    public class Simple
    {
        public Vector4 Frag()
        {
            return new Vector4(0.1f, 0.3f, 0.7f, 1f);
        }
    }
}
