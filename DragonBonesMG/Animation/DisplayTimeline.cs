using System.Collections.Generic;
using DragonBonesMG.JsonData;

namespace DragonBonesMG.Animation {
    /// <summary>
    /// Holds a collection of slot timelines.
    /// <seealso cref="SlotTimeline"/>
    /// </summary>
    internal class DisplayTimeline {

        private readonly Dictionary<string, SlotTimeline> _slotTimelines;

        public DisplayTimeline(SlotTimelineData[] datas) {
            _slotTimelines = new Dictionary<string, SlotTimeline>();
            foreach (var d in datas) {
                var startFrame = 0;
                var frames = new SlotFrame[d.SlotFrames.Length];
                for (var i = 0; i < d.SlotFrames.Length; i++) {
                    var f = d.SlotFrames[i];
                    frames[i] = new SlotFrame(startFrame, f);
                    startFrame += f.Duration;
                }
                _slotTimelines.Add(d.SlotName, new SlotTimeline(frames));
            }
        }

        public void Update(float currentFrame) {
            foreach (var tl in _slotTimelines.Values)
                tl.Update(currentFrame);
        }

        public DisplayTimelineState GetState() {
            return new DisplayTimelineState(_slotTimelines);
        }

    }
}