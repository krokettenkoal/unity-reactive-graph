# Reactive Graph
_A node-based solution for creating reactive Unity game objects_

## Table of contents
<!-- TOC -->
* [Reactive Graph](#reactive-graph)
  * [Table of contents](#table-of-contents)
  * [Important notice](#important-notice)
  * [What is Reactive Graph?](#what-is-reactive-graph)
  * [Get started](#get-started)
  * [Reactive Nodes](#reactive-nodes)
    * [Signal Nodes](#signal-nodes)
    * [Effect Nodes](#effect-nodes)
  * [Reactive Objects](#reactive-objects)
    * [Adding the `ReactiveObject` component](#adding-the-reactiveobject-component)
    * [Selecting an input source](#selecting-an-input-source)
  * [Compatibility](#compatibility)
  * [Known issues](#known-issues)
<!-- TOC -->

## Important notice
> Reactive Graph uses Unity's **experimental** [GraphView](https://docs.unity3d.com/ScriptReference/Experimental.GraphView.GraphView.html). This means that version compatibility cannot be guaranteed in any way. As long as GraphView is in an experimental state, it is highly recommended to use the Unity version specified in the [Compatibility](#compatibility) section.

## What is Reactive Graph?
Reactive Graph provides a **designer-friendly** way to make Unity game objects react to a single input value. 
The node-based approach provides an experience similar to Unity's [Shader Graph](https://docs.unity3d.com/Manual/shader-graph.html), but on a **component-level**. 

Whether you want to dynamically change an object's position, rotation or scale, its material color or emission color, or easily set animator parameters without having to write a dedicated component, or everything at once - it's just a few clicks away. 
The great thing: What you want your objects to react to is totally up to you: be it your player's health value, the distance to a target object, an objective's progress value, an audio source's signal level. It can be virtually anything that can be broken down to a single number.

## Get started
To start using Reactive Graph, simply follow the steps described below. 
1. **Clone** the repository into your Unity project's `Assets` folder. 
2. Open a new **Reactive Graph** window by selecting `Window/Reactive Graph`.
3. Start designing your custom behaviour by adding new **nodes** via the `Add Node` dropdown in the toolbar. 
4. **Save** your new graph by clicking `File/Save` or `File/Save as...` and selecting a folder and file name for your new graph asset. 
5. **Assign** your new graph to one or more [Reactive Objects](#reactive-objects) in the scene, making them react to an input value based on your graph design.

## Reactive Nodes
Reactive Nodes are the building blocks of your Reactive Graph, defining the behaviour of your target game object based on a provided input value. In general, all Reactive Nodes are split into two categories: **Signal Nodes** and **Effect Nodes**. While _Signal Nodes_ only apply changes to the input value, _Effect Nodes_ specify how your target object reacts to the received input value.

### Signal Nodes
Signal Nodes receive an input value and provide a (potentially modified) output value. 
The table below contains all Signal Nodes that are currently available.

| Name         | Description                                                                                                                                                                                                                                                                                                            |
|--------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Pass-Through | Simply forwards the input value to the output value.                                                                                                                                                                                                                                                                   |
| Curve        | Evaluates the input value along a user-defined curve. The input value is expected to be normalized, i.e. in a range of 0..1.                                                                                                                                                                                           |
| Function     | Applies a [mathematical function](https://docs.unity3d.com/ScriptReference/Mathf.html) to the input value. Currently supported functions: `Sin`, `Cos`, `Tan`, `Abs`, `Floor`, `Ceil`, `Round`, `Sqrt`, `Pow`, `Log`, `Log10`, `Clamp`, `Clamp01`, `Min`, `Max`, `Add` (`+` operation) and `Multiply` (`*` operation). |

### Effect Nodes
Effect Nodes receive an input value and modify properties on the target object. They **do not** provide an output value. 
The table below contains all Effect Nodes that are currently available.

| Name      | Description                                                                                                                                                                                                                                                                                                                                              |
|-----------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Material  | Modifies the target object `Renderer`'s (main) material. Pre-defined properties (`Color` - main color/albedo; `Emission` - emission color) are supported as well as custom shader properties.                                                                                                                                                            |
| Transform | Split into three sub-types, corresponding to the transform components **Position**, **Rotation** and **Scale**. Transform nodes multiply the input value with the specified directional vector and add or apply the vector to the corresponding transform component (`transform.localPosition`, `transform.localEulerAngles` or `transform.localScale`). |
| Animator  | Sets one or more animator parameters to the received input value.                                                                                                                                                                                                                                                                                        |

## Reactive Objects
Reactive Objects are game objects that react to a single input value based on a Reactive Graph.
To apply the behaviour defined in a Reactive Graph asset to a game object in the scene, you need two things: the `ReactiveObject` component and an **input source**.

### Adding the `ReactiveObject` component
Creating Reactive Objects is done by adding the `ReactiveObject` component to it and assigning a Reactive Graph and a valid input source. To do this, simply follow these steps:
1. Select the game object you want to be reactive and add the `Reactive Object` component to it (found inside the `Reactive` menu item).
2. Assign a **Reactive Graph** asset to the `Reactive Graph` property. This is the graph that defines the behaviour of your reactive object.
3. Assign a valid **input source** to the `Source` property. This is the source that provides the input value for your reactive object. Check out the [Selecting an input source](#selecting-an-input-source) section for more info.

### Selecting an input source
Input sources have **one main job**: providing a `float` value that is used by a Reactive Object as an input value for its assigned Reactive Graph. To assign an input source to a Reactive Object, follow these steps:
1. **Select** the game object that has the `ReactiveObject` component attached to it.
2. **Assign** an object (can be any Unity `Object`) to the `Source` property. This can be done in two ways:
    * **Drag & Drop:** Drag the object you want to use as an input source from the Hierarchy or Project view and drop it onto the `Source` property.
    * **Object Picker:** Click the small circle on the right side of the `Source` property and select an object from the Object Picker window.
3. From the **dropdown** next to the source object, select a field, property or method to use as an input source. The following types are supported:
    * **Fields:** Any `float` field.
    * **Properties:** Any `float` property with a getter.
    * **Methods:** Any `float` method without parameters. 

## Compatibility
Reactive Graph is written and tested in Unity `2022.3.7f`. It is **highly recommended** to use a Unity version that is as close to the source code's version as possible. In general, you should be fine with any `2022.3` version.

## Known issues
* **Missing references after saving to an existing asset:** When saving a Reactive Graph to an existing asset, the old asset is deleted and a new asset with the same name is created. This causes Unity to lose all references to the asset, requiring the user to re-assign all references to the Reactive Graph asset.