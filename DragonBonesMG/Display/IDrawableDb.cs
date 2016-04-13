using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonBonesMG.Display {
    public interface IDrawableDb {
        void Draw(SpriteBatch s);
        void Draw(SpriteBatch s, Color color, SpriteEffects effect = SpriteEffects.None);
        Texture2D RenderToTexture(SpriteBatch s);
    }
}