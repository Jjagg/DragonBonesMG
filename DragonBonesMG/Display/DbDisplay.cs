using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonBonesMG.Display {
    public abstract class DbDisplay {

        public string Name { get; private set; }

        public DbDisplay(string name) {
            Name = name;
        }

        public abstract void Draw(SpriteBatch s, Matrix transform, Color parentColor);
    }
}