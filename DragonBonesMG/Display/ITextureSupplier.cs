namespace DragonBonesMG.Display {
    public interface ITextureSupplier {
        /// <summary>
        /// Get a drawable that will draw the given texture when drawn.
        /// Returns null if the given texture does not exist.
        /// </summary>
        IDrawableDb Get(string name);
    }
}