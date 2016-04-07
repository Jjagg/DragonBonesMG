using System;
using System.Collections.Generic;
using System.Linq;
using DragonBonesMG.Display;
using DragonBonesMG.JsonData;
using DragonBonesMG.Mesh;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonBonesMG {
    public class DbSlot : DbObject {

        private int _originDisplayIndex;
        private int _zOrder;

        private List<DbImage> _images;
        private List<DbArmature> _armatures;
        private List<DbMesh> _meshes;

        public DbSlot(DbArmature armature, SlotData data) :
            base(data.Name, armature, data.Parent) {
            _originDisplayIndex = 0;
            ZOrder = data.Z;
            _images = new List<DbImage>();
            _armatures = new List<DbArmature>();
            _meshes = new List<DbMesh>();
        }

        public int ZOrder
        {
            get { return _zOrder; }
            protected set {
                if (value == _zOrder) return;
                _zOrder = value;
                Armature.SlotsChanged = true;
            }
        }

        /// <summary>
        /// Draw whatever is in this slot.
        /// </summary>
        public void Draw(SpriteBatch s, Matrix transform) {
            // TODO draw child armature and do something with meshes?
            foreach (var image in _images)
                image.Draw(s, Parent.CurrentGlobalTransform * transform);

        }

        public void AddDisplay(DisplayData displayData) {
            if (displayData.Type == "Armature") {
                // TODO nested armature parsing
            } else if (displayData.Type == "image") {
                _images.Add(new DbImage(displayData, Armature.Texturer));
            } else if (displayData.Type == "mesh") {
                // TODO mesh stuff
            } else {
                throw new ArgumentException(nameof(displayData));
            }
        }
    }
}