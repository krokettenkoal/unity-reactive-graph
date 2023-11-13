namespace Reactive.Editor.Nodes
{
    /// <summary>
    /// Base class for reactive nodes that apply calculations to their input value
    /// </summary>
    public abstract class ReactiveSignalNode : ReactiveNode
    {
        public override void AddClassNames()
        {
            base.AddClassNames();
            AddToClassList("node-signal");
        }
    }
}