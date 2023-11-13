namespace Reactive
{
    /// <summary>
    /// Supported reactive node types
    /// </summary>
    public enum ReactiveNodeType
    {
        Entry,
        PassThrough,
        Material,
        TransformPosition,
        TransformRotation,
        TransformScale,
        Curve,
        Math,
        Animator,
    }
    
    /// <summary>
    /// Supported properties that can be modified by a reactive material node
    /// </summary>
    [System.Flags]
    public enum ReactiveMaterialProperty : int
    {
        None = 0,
        Color = 1 << 0,
        Emission = 1 << 1,
    }

    /// <summary>
    /// Supported functions that can be used by a reactive math node
    /// </summary>
    public enum ReactiveMathNodeFunction
    {
        Sin,
        Cos,
        Tan,
        Abs,
        Floor,
        Ceil,
        Round,
        Sqrt,
        Pow,
        Log,
        Log10,
        Clamp,
        Clamp01,
        Min,
        Max,
        Add,
        Multiply,
    }
}