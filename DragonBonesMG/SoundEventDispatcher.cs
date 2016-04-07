namespace DragonBonesMG {
    public class SoundEventDispatcher {
        private static SoundEventDispatcher _instance;

        public event SoundEventHandler SoundEvent;

        private SoundEventDispatcher() {
            _instance = this;
        }

        private static SoundEventDispatcher Instance
            => _instance ?? new SoundEventDispatcher();


        public static void PlaySound() {
            _instance.SoundEvent?.Invoke(_instance, new SoundEventArgs());
        }
    }

    public delegate void SoundEventHandler(object sender, SoundEventArgs e);
}