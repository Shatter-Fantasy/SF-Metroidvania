namespace SF.DialogueModule.Nodes
{
    /// <summary>
    /// Informs a <see cref="DialogueGraph"/> about what type of comparison is being done
    /// on a <see cref="IComparisonNode"/>.
    /// </summary>
    public enum ComparisonNodeType
    {
        Int,
        Float,
        Bool
    }
    
    
    /// <summary>
    /// Implement to inform a <see cref="DialogueRuntimeGraph"/> a node is
    /// used for comparing a type of value.
    /// </summary>
    public interface IComparisonNode
    {
        public bool Compare();
    }

    public interface IComparisonNode<TValueType> : IComparisonNode
    {
        /// <summary>
        /// The value you want to use a the base to check.
        /// This is set by the <see cref="BranchingRuntimeNode"/>
        /// </summary>
        public TValueType ValueToCheck { get; set; }
        /// <summary>
        /// The value to check against.
        /// </summary>
        public TValueType ComparisonValue{ get; set; }
    }
}
