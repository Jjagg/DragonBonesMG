using System.Collections.Generic;

namespace DragonBonesMG.Animation {
    internal class DisplayTimelineState {
        private readonly Dictionary<string, SlotTimeLine> _slotTimelines;

        public DisplayTimelineState(Dictionary<string, SlotTimeLine> slotTimelines) {
            _slotTimelines = slotTimelines;
        }

        public SlotState? GetState(string slotName) {
            if (_slotTimelines.ContainsKey(slotName))
                return _slotTimelines[slotName].State;
            return null;
        }
    }
}