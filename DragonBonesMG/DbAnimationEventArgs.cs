using System;

namespace DragonBonesMG {
    public class DbAnimationEventArgs : EventArgs {

        public readonly string Name;

        public DbAnimationEventArgs(string name) {
            Name = name;
        }
    }
}