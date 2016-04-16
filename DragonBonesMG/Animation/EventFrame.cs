using DragonBonesMG.JsonData;

namespace DragonBonesMG.Animation {
    public struct EventFrame {

        public int StartFrame;
        public string Action;
        public string Event;
        public string Sound;

        internal EventFrame(int startFrame, EventFrameData data) :
            this(startFrame, data.Action, data.Event, data.Sound) {
        }

        public EventFrame(int startFrame, string action = null, string ev = null, string sound = null) {
            StartFrame = startFrame;
            Action = action;
            Event = ev;
            Sound = sound;
        }
    }
}