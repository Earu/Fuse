using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuse.Drawing
{
    internal class Vertex
    {
        private float _X;
        private float _Y;
        private float _Z;

        internal Vertex(float x, float y, float z)
        {
            this._X = x;
            this._Y = y;
            this._Z = z;
        }

        internal float X { get => this._X; }
        internal float Y { get => this._Y; }
        internal float Z { get => this._Z; }
    }
}
