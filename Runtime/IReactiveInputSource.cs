namespace Reactive.Runtime
{
    /// <summary>
    /// Interface to implement for any class that provides a reactive input value.
    /// </summary>
    /// <seealso cref="ReactiveObject.Input"/>
    public interface IReactiveInputSource
    {
        /// <summary>
        /// The current value of the input source.
        /// This value is used as the starting value for the <see cref="ReactiveObject.graph"/> of the <see cref="ReactiveObject"/>.
        /// </summary>
        public float Value { get; }
    }
}