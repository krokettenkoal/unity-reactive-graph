# Reactive Graph
_A node-based solution for creating reactive game objects_

## Important notice
> Reactive Graph uses Unity's **experimental** [GraphView](https://docs.unity3d.com/ScriptReference/Experimental.GraphView.GraphView.html). This means that version compatibility cannot be guaranteed in any way. As long as GraphView is in an experimental state, it is highly recommended to use the Unity version specified in the [Compatibility](#compatibility) section.

## What is Reactive Graph?
Reactive Graph provides a **designer-friendly** way to make Unity Game Objects react to a single input value. The node-based approach provides an experience similar to Unity's [Shader Graph](https://docs.unity3d.com/Manual/shader-graph.html), but on a **component-level**. Whether you want to dynamically change an object's position, rotation or scale, its material color or emission color, or easily set animator parameters without having to write a dedicated component, or everything at once - it's just a few clicks away. The great thing: What you want your objects to react to is totally up to you! By writing just a few (very simple) lines of code, you can react to virtually anything that can be broken down to a single number.

## Get started
To start using Reactive Graph, simply follow the steps described below. 
1. **Clone** the repository into your Unity project's `Assets` folder. 
2. Open a new **Reactive Graph** window by selecting `Window/Reactive Graph`.
3. Start designing your custom behaviour by adding new **nodes** via the `Add Node` dropdown in the toolbar. 
4. **Save** your new graph by clicking `File/Save` or `File/Save as...` and selecting a folder and file name for your new graph asset. This asset can now be used on your game objects, making them react to an input value based on your graph design. Check out the [Reactive Objects](#reactive-objects) section for more info.

## Reactive Nodes
Reactive Nodes are the building blocks of your Reactive Graph, defining the behaviour of your target game object based on a provided input value. In general, all Reactive Nodes are split into two categories: **Signal Nodes** and **Effect Nodes**. While _Signal Nodes_ only apply changes to the input value, _Effect Nodes_ specify how your target object reacts to the received input value.

### Signal Nodes
Signal Nodes receive an input value and provide a (potentially modified) output value. 
The table below contains all Signal Nodes that are currently available.
| Name 			| Description 											|
| ------------- | ----------------------------------------------------- |
| Pass-Through	| Simply forwards the input value to the output value. 	|
| Curve 		| Evaluates the input value along a user-defined curve. The input value is expected to be normalized, i.e. in a range of 0..1. |
| Function 		| Applies a [mathematical function](https://docs.unity3d.com/ScriptReference/Mathf.html) to the input value. Currently supported functions: `Sin`, `Cos`, `Tan`, `Abs`, `Floor`, `Ceil`, `Round`, `Sqrt`, `Pow`, `Log`, `Log10`, `Clamp`, `Clamp01`, `Min`, `Max`, `Add` (`+` operation) and `Multiply` (`*` operation). |

### Effect Nodes
Effect Nodes receive an input value and modify properties on the target object. They **do not** provide an output value. 
The table below contains all Effect Nodes that are currently available.
| Name 		| Description |
| --------- | ----------- |
| Material 	| Modifies the target object's `Renderer` (main) material. Pre-defined properties (`Material.color` and `_EmissionColor`) are supported as well as custom shader properties. |
| Transform | Split into three sub-types, corresponding to the transform components **Position**, **Rotation** and **Scale**. Transform nodes multiply the input value with the specified directional vector and add or apply the vector to the corresponding transform component (`transform.localPosition`, `transform.localEulerAngles` or `transform.localScale`). |
| Animator 	| Sets one or more animator parameters to the received input value. |

## Reactive Objects
To apply the behaviour defined in a Reactive Graph asset to a Game Object in the scene, you need two things: a `ReactiveObject` component and an **input source**. Since Reactive Graph is meant to provide as much flexibility as possible, no out-of-the-box solution for those components is available - you need to implement your own classes that extend the provided base classes. **But fear not!** All the required functionality is already there, you only need to override some input parameters.

### Writing Reactive Object components
Writing your own Reactive Object components is as simple as can be. Just follow the few steps described below.
1. Create a **new script** somewhere in your `Assets` folder
2. Make your class **inherits** from `ReactiveObject` (namespace: `Reactive.Runtime`) instead of `MonoBehaviour`
3. **Override** the `Awake` method and, inside the method, assign a valid input source to the object's `Input` property. The input source needs to implement the `IReactiveInputSource` interface. Check out the (Creating input sources)[#creating-input-sources] section for details.
4. **Make sure to call `base.Awake()` after you have assigned the input source!**

### Creating input sources
Input sources have **one main job**: providing a `float` value that is used by a Reactive Object as an input value for its assigned Reactive Graph. To do this, simply create a **new class** that implements the `IReactiveInputSource` interface and its `Value` property. Usually, an input source also is a `MonoBehaviour`, but this is purely **optional** and may not be necessary in many use cases. 
How the `Value` property is calculated is fully up to you and heavily depends on the thing you are reacting to - it could be your player's health value, the distance to a target object, an objective's progress value, an audio source's signal level or virtually any other single `float` value.

## Compatibility
Reactive Graph is written and tested in Unity `2022.3.7f`. It is **highly recommended** to use a Unity version that is as close to the source code's version as possible. In general, you should be fine with any `2022.3` version.

## Known issues
* **Missing references after saving to an existing asset:** When saving a Reactive Graph to an existing asset, the old asset is deleted and a new asset with the same name is created. This causes Unity to lose all references to the asset, requiring the user to re-assign all references to the Reactive Graph asset.