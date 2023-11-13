namespace Reactive.Editor.Nodes
{
    /// <summary>
    /// Base class for reactive nodes that cause side effects on the target object (e.g. material, transform, etc.)
    /// </summary>
    public abstract class ReactiveEffectNode : ReactiveNode
    {
        public override bool HasOutput { get; } = false;
        
        public override void AddClassNames()
        {
            base.AddClassNames();
            AddToClassList("node-effect");
        }
    }
}