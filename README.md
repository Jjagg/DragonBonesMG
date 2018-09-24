This library is no longer maintained because there is an official runtime implementation for DragonBones. You can find it at https://github.com/DragonBones/DragonBonesCSharp.

_________

# DragonBonesMG
This is a [DragonBones](http://dragonbones.github.io/) runtime for [MonoGame](http://www.monogame.net/).
Get the standalone editor for DragonBones animations, DragonBonesPro, [here](http://dragonbones.github.io/download.html). There's an open Trello board for requesting DBPro features/improvements [here](https://trello.com/b/oooKrTH8/dragonbones-idea-collection).

## Features
**Done**:

+ Basic skeletal animation (bone transforms)
+ Slot transforms
  + Color transform
  + Change DrawOrder
+ Easy updating and rendering
+ Texture atlas support
+ Events
+ Timescale for slower/faster/reverse animation playback

**Needs testing, but should work**:

- Free form deformation
- Nested armatures

**To do**:

- Inverse Kinematics
- More public methods for dynamically building/changing an animation
- MonoGame content pipeline extension
- Documentation
- Cleaner code (especially meshes)

## Using DBMG
Since this is in an early state, if you want to try it out you should clone the solution.
Once you added a reference to the solution or the dll if you built it, you can use DBMG as follows:

First load the texture atlas for the animation, then let the texture atlas load the texture it needs and load a DragonBones instance, passing in the TextureAtlas and a GraphicsDevice, and retrieve an armature from it (one instance can contain multiple armatures, but the Armature property used below just retrieves the first one for convenience, since often you'll only have armature; you can have more if you have nested armatures).

    var atlas = TextureAtlas.FromJson("Content/AnimationAtlas.json");
    var atlas.LoadContent(ContentManager);
    _armature = DragonBones.FromJson("Content/Armature.json", atlas, GraphicsDevice).Armature;
That takes care of everything you need to load. You can then set the animation the armature should play:

    _armature.GotoAndPlay("animationName");
And update and draw it like you would expect.

    // in Update
    _armature.Update(gameTime.ElapsedGameTime);
    // in Draw
    _armature.Draw(spriteBatch);

## How it works
I figured JSON would be easiest to parse, but the export format is a little different from the way the library wants to use the data. Because of this the JsonData namespace contains classes for all data exactly like it is exported. A DragonBones instance is then created from the data by translating all of it to the format that's actually required. The parsing and any computation/translation should eventually be done when the content is built when the content pipeline extension is done.
An armature consists of a list of bones and slots. Bones have a transformation (scale, rotation, translation) associated with it, which will determine its position at any time. Slots can display a texture, a mesh with a texture (for free form deformation) or a nested armature and have a parent bone from which they inherit the transformation. It has a list of possible displays and keeps an index for the active one that it should draw or -1 when it should not draw anything.
An armature can have multiple animations defined for it. An animation consist of 4 timelines. A TransformTimeline, a DisplayTimeline, an FFDTimeline and an EventTimeline (event timelines are not explicitly defined, but rather as a collection of EventFrame in the animation).
The first three consist of multiple smaller timelines:

+ BoneTimelines for TransformTimeline -> defines the transform of a single bone throughout an animation
+ SlotTimelines for DisplayTimeline   -> defines the state of a single slot throughout an animation
+ MeshTimelines for FFDTimeline       -> defines the positions of all vertices for a single mesh throughout an animation
Events are handled in an animation directly.

When an armature gets updated it will update its active animation if there is one. The animation will pass time and update all of its timelines. Then the state of the animation is retrieved with the GetCurrentState function. The state of the animation is used to update the armatures bones and slots. 
To draw an armature, the list of slots is traversed and their position is calculated from their parent bone transform and their own transform so the slot can draw its active display at the correct location.
