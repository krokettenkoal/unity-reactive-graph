namespace Reactive.Runtime
{
    /// <summary>
    /// Data class for storing node links (connections) in a <see cref="ReactiveGraphData"/> asset
    /// </summary>
    [System.Serializable]
    public class NodeLinkData
    {
        /// <summary>
        /// The GUID of the node that the link starts from
        /// </summary>
        public string baseNodeGuid;
        
        /// <summary>
        /// The GUID of the node that the link ends at
        /// </summary>
        public string targetNodeGuid;
    }
}