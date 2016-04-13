using Microsoft.Xna.Framework;

namespace DragonBonesMG.Animation {
    public struct SlotState {

        public int DisplayIndex { get; private set; }
        public int ZOrder { get; private set; }
        public Color Color { get; private set; }

        public SlotState(int displayIndex, int z, Color color) {
            DisplayIndex = displayIndex;
            ZOrder = z;
            Color = color;
        }

    }
}