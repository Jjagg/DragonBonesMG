using System.Collections.Generic;
using System.IO;
using System.Linq;
using DragonBonesMG.Core;
using DragonBonesMG.Display;
using DragonBonesMG.JsonData;
using Microsoft.Xna.Framework.Graphics;

namespace DragonBonesMG {
    /// <summary>
    /// Use this class to load a DragonBones file. It contains some general data exported by DragonBones
    /// and more importantly the armatures loaded with the file. Use <see cref="Armature"/> to get the
    /// first loaded armature if you're not using nested armatures. You can also get Armatures by name using
    /// <see cref="GetArmature"/>.
    /// <seealso cref="DbArmature"/>
    /// </summary>
    public class DragonBones {

        // The big TODO list:
        // - pivot? (not sure if used)
        // - IK
        // - dynamic animation building (more add/remove stuff)
        // - playtimes? Is loop bool enough?
        // - no tween animation
        // - events
        // - mesh code cleanup
        // - mesh anti-alias
        // - content pipeline extension!
        // - clean up timeline initialization (~ duplicate code)
        // - some more documentation
        // - nested armature (might work, needs testing)
        // - testing

        // DONE: 
        // - bone transform 
        // - slot transform
        //   + color transform
        //   + z-order transform 
        // - rendering
        // - texture atlas
        // - tweening
        // - ffd
        // - negative timescale

        /// <summary>The name of this DragonBones instance.</summary>
        public readonly string Name;

        /// <summary>The version of DragonBones this instance was created with.</summary>
        public readonly string Version;

        /// <summary>
        /// True if all transform are global as opposed to being relative to their parent. 
        /// </summary>
        /// <remarks>
        /// This is always false in newer versions of DragonBones and this runtime does not support
        /// older versions with global positions.
        /// </remarks>
        public readonly bool IsGlobal;

        /// <summary>The framerate set in DragonBonesPro editor. Used to determine expected playback speed.</summary>
        public readonly int FrameRate;

        /// <summary>List of armatures loaded by this DragonBones instance.</summary>
        private readonly List<DbArmature> Armatures;

        /// <summary>
        /// Get the first loaded armature from this DragonBones instance or null if none is present.
        /// </summary>
        /// <remarks>Pretty convenient for a DB instance with just one armature, which is often the case.</remarks>
        public DbArmature Armature => Armatures.FirstOrDefault();

        internal DragonBones(ITextureSupplier texturer, GraphicsDevice graphics, DbData data) {
            Name = data.Name;
            Version = data.Version;
            IsGlobal = data.IsGlobal;
            FrameRate = data.FrameRate;
            Armatures = new List<DbArmature>();
            foreach (var armatureData in data.Armatures) {
                var armature = new DbArmature(armatureData.Name, texturer, graphics, this);
                armature.Initialize(armatureData);
                Armatures.Add(armature);
            }
        }

        /// <summary>
        /// Get an armature that was loaded by this DragonBones instance by its name.
        /// </summary>
        /// <param name="name">The name of the armature</param>
        /// <returns>A DbArmature with <see cref="DbDisplay.Name"/>name or null if no armature with
        /// the given name is loaded</returns>
        public DbArmature GetArmature(string name) {
            return Armatures.FirstOrDefault(a => a.Name == name);
        }

        /// <summary>
        /// Load a DragonBones instance from the JSON file at the given path.
        /// </summary>
        /// <param name="path">Relative or absolute path to the json DragonBones file.</param>
        /// <param name="texturer">The supplier that has all textures for the DragonBones file at the given path.</param>
        /// <param name="graphics">A GraphicsDevice used to initialize meshes for FFD if necessary.</param>
        /// <returns></returns>
        public static DragonBones FromJson(string path, ITextureSupplier texturer, GraphicsDevice graphics) {
            if (!File.Exists(path))
                throw new FileNotFoundException("Could not resolve the given path", path);
            return new DragonBones(texturer, graphics, DbData.FromJson(path));
        }
    }
}