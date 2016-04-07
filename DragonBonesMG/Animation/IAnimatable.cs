using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonBonesMG.Animation {
    public interface IAnimatable {
        void PlayAnimation();
        void StopAnimation();
        void GotoAndPlay(string animationName, int playTimes = -1);
        void GotoAndPlay(string animationName, float time, int playTimes = -1);
        void GotoAndStop(string animationName, float time, int playTimes = -1);
        void SetTimeScale(double value);
        bool IsAnimating();
        bool IsDoneAnimating();
        void Update(TimeSpan elapsed);
        void Draw(SpriteBatch s, Matrix transform);
    }
}