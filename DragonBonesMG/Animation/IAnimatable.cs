using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonBonesMG.Animation {
    public interface IAnimatable {
        void PlayAnimation(bool loop = false);
        void StopAnimation();
        void GotoAndPlay(string animationName, bool loop = false);
        void GotoAndPlay(string animationName, float time, bool loop = false);
        void GotoAndStop(string animationName, float time);
        void SetTimeScale(double value);
        bool IsAnimating();
        bool IsDoneAnimating();
        void Update(TimeSpan elapsed);
        void Draw(SpriteBatch s, Matrix transform);
    }
}