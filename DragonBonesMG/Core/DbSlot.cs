using System;
using System.Collections.Generic;
using DragonBonesMG.Animation;
using DragonBonesMG.Display;
using DragonBonesMG.JsonData;
using DragonBonesMG.Mesh;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonBonesMG.Core {
    public class DbSlot : DbObject {

        public int DisplayIndex
        {
            get { return _displayIndex; }
            set { _displayIndex = MathHelper.Clamp(value, -1, Displays.Count - 1); }
        }

        private int _zOrder;

        public int ZOrder
        {
            get { return _zOrder; }
            private set {
                if (value == _zOrder) return;
                Armature.SlotsChanged = true;
                _zOrder = value;
            }
        }

        public Color ColorTransform { get; private set; }

        /// <summary>
        /// This list is up for grabs for easier editing, but be careful with the transforms.
        /// </summary>
        // reformatted so transform is not INSIDE a display, in case you want the same display 
        // nested in different other animations. Why the hell do you want that?
        public readonly List<DisplayTransform> Displays;

        private int _displayIndex;

        public bool Visible => DisplayIndex != -1;

        internal DbSlot(DbArmature armature, SlotData data) :
            base(data.Name, armature, data.Parent) {
            ZOrder = data.Z;
            Displays = new List<DisplayTransform>();
            ColorTransform = Color.White;
        }

        /// <summary>
        /// Draw whatever is in this slot.
        /// </summary>
        public void Draw(SpriteBatch s, Matrix transform, Color parentColor) {
            if (Displays.Count <= 0 || !Visible) return;

            var display = Displays[DisplayIndex];
            display.Display.Draw(s,
                display.Transform * Parent.CurrentGlobalTransform * transform,
                ColorEx.Multiply(parentColor, ColorTransform));
        }

        /// <summary>
        /// Add the display to the list of displays and set it to be the active display.
        /// </summary>
        /// <param name="display">The display to add and show.</param>
        /// <param name="transform">The transform of the display.</param>
        public void SetNewDisplay(DbDisplay display, Matrix transform) {
            Displays.Add(new DisplayTransform(display, transform));
            DisplayIndex = Displays.Count - 1;
        }

        public void SetNewDisplay(DbDisplay display) {
            SetNewDisplay(display, Matrix.Identity);
        }

        /// <summary>
        /// Set the display of this slot to the one with the given name, or nothing no display with the given name is present.
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
                Displays.Add(new DisplayTransform(Armature.GetCreator().GetArmature(data.Name),
                    transform));
                break;
            case "image":
                Displays.Add(new DisplayTransform(new DbImage(data.Name, Armature.Texturer),
                    transform));
                break;
            case "mesh":
                Displays.Add(new DisplayTransform(new DbMesh(data, Armature.Texturer),
                    transform));
                break;
            default:
                throw new ArgumentException(nameof(data));
            }
        }

        internal void Update(DisplayTimelineState displayState) {
            var val = displayState.GetState(Name);
            if (val == null) return;
            var state = (SlotState) val;
            DisplayIndex = state.DisplayIndex;
            ZOrder = state.ZOrder;
            ColorTransform = state.Color;
        }

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