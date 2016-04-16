using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonBonesMG.Animation {
    public interface IAnimatable {
        void PlayAnimation(bool loop = false);
        void PauseAnimation();
        void GotoAndPlay(string animationName, bool loop = false);
        void GotoAndPlay(string animationName, float time, bool loop = false);
        void GotoAndStop(string animationName, float time);
        double TimeScale { get; set; }
        bool IsAnimating();
        bool IsDoneAnimating();
        void Update(TimeSpan elapsed);
        void Draw(SpriteBatch s, Matrix transform);
    }
}