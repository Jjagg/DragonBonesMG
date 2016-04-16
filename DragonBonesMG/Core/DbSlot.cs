using System;
using System.Collections.Generic;
using DragonBonesMG.Animation;
using DragonBonesMG.Display;
using DragonBonesMG.JsonData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonBonesMG.Core {
    /// <summary>
    /// Slots can be attached to a bone to inherit its transform. Add displays (<see cref="DbDisplay"/>)
    /// to a slot to have them rendered when a slot is drawn. 
    /// </summary>
    public class DbSlot : DbObject {
        #region Fields

        /// <summary>
        /// Index of the active display of this slot in its list of displays, or -1 if no display is active.
        /// </summary>
        public int DisplayIndex
        {
            get { return _displayIndex; }
            set { _displayIndex = MathHelper.Clamp(value, -1, Displays.Count - 1); }
        }

        private int _originalZOrder;

        private int _zOrder;

        /// <summary>
        /// Determines the drawing order of slots within an armature.
        /// </summary>
        public int ZOrder
        {
            get { return _zOrder; }
            private set {
                if (value == _zOrder) return;
                Armature.SlotsChanged = true;
                _zOrder = value;
            }
        }

        /// <summary>
        /// The current color transform applied to a slot. This is used in 
        /// <see cref="SpriteBatch.Draw(Texture2D,Vector2?,Rectangle?,Rectangle?,Vector2?,float,Vector2?,Color?,SpriteEffects,float)"/>
        /// as the passed in color.
        /// </summary>
        public Color ColorTransform { get; private set; }

        #endregion

        /// <summary>
        /// This list is up for grabs for easier editing, but be careful with the transforms.
        /// </summary>
        // reformatted so transform is not INSIDE a display, in case you want the same display 
        // nested in different other animations. Why the hell do you want that?
        // TODO is this ever userful? Would be neater without the extra indirection
        public readonly List<DisplayTransform> Displays;

        private int _displayIndex;

        /// <summary>
        /// True if DisplayIndex != -1, false otherwise.
        /// </summary>
        public bool Visible => DisplayIndex != -1;

        internal DbSlot(DbArmature armature, SlotData data) :
            base(data.Name, armature, data.Parent) {
            _originalZOrder = data.Z;
            ZOrder = data.Z;
            Displays = new List<DisplayTransform>();
            ColorTransform = Color.White;
        }

        #region Draw and Update

        /// <summary>
        /// Draw this slot's active display, if any, applying the current matrix and color transform.
        /// </summary>
        public void Draw(SpriteBatch s, Matrix transform, Color parentColor) {
            if (Displays.Count == 0 || !Visible) return;

            var display = Displays[DisplayIndex];
            display.Display.Draw(s,
                display.Transform * Parent.CurrentGlobalTransform * transform,
                ColorEx.Multiply(parentColor, ColorTransform));
        }

        /// <summary>
        /// Update this slot, applying the given states to find current matrix and color transforms
        /// and updating meshes using the FFDTimelineState if necessary.
        /// </summary>
        /// <param name="displayState"></param>
        /// <param name="ffdState"></param>
        internal void Update(DisplayTimelineState displayState, FFDTimelineState ffdState) {
            var s = displayState.GetState(Name);
            if (s != null) {
                var slotState = (SlotState) s;
                DisplayIndex = slotState.DisplayIndex;
                if (slotState.ZOrder != null)
                    ZOrder = (int) slotState.ZOrder;
                ColorTransform = slotState.Color;
            }

            if (Displays.Count == 0 || !Visible) return;
            var display = Displays[DisplayIndex].Display;
            // update the current display if it's a mesh
            var mesh = display as DbMesh;
            mesh?.Update(ffdState.GetVertices(Name));
        }

        #endregion

        #region Add and Remove Displays

        /// <summary>
        /// Add the display to the list of displays and set it to be the active display.
        /// </summary>
        /// <param name="display">The display to add and show.</param>
        /// <param name="transform">The transform of the display.</param>
        public void SetNewDisplay(DbDisplay display, Matrix transform) {
            Displays.Add(new DisplayTransform(display, transform));
            DisplayIndex = Displays.Count - 1;
        }

        /// <summary>
        /// Add the display to the list of displays and set it to be the active display.
        /// Uses the Identity matrix as transfomr for the display.
        /// </summary>
        /// <param name="display">The display to add and show.</param>
        public void SetNewDisplay(DbDisplay display) {
            SetNewDisplay(display, Matrix.Identity);
        }

        /// <summary>
        /// Set the display of this slot to the one with the given name, or nothing if no display with the given name is present.
        /// </summary>
        /// <param name="name">The name of the display to set the active display to.</param>
        public void SetDisplay(string name) {
            DisplayIndex = Displays.FindIndex(d => d.Display.Name == name);
        }

        internal void AddDisplay(DisplayData data) {
            var transform = data.Transform?.ToMatrix() ?? Matrix.Identity;
            switch (data.Type) {
            case "Armature":
                // TODO defaultActions.
                Displays.Add(new DisplayTransform(Armature.Creator.GetArmature(data.Name),
                    transform));
                break;
            case "image":
                Displays.Add(new DisplayTransform(new DbImage(data.Name, Armature.Texturer),
                    transform));
                break;
            case "mesh":
                Displays.Add(new DisplayTransform(new DbMesh(data, Armature.Texturer, Armature.GraphicsDevice),
                    transform));
                break;
            default:
                throw new ArgumentException(nameof(data));
            }
        }

        #endregion

        /// <summary>
        /// Wraps a display with its current transform so a display can be reused and have a different transform
        /// in another slot.
        /// </summary>
        public class DisplayTransform {
            public DbDisplay Display;
            public Matrix Transform;

            public DisplayTransform(DbDisplay display) : this(display, Matrix.Identity) {
            }

            public DisplayTransform(DbDisplay display, Matrix transform) {
                Display = display;
                Transform = transform;
            }

        }

    }
}