using Unity.GraphToolkit.Editor;

namespace SFEditor.Dialogue.Graphs
{
	[System.Serializable]
	[UseWithGraph(typeof(DialogueGraph))]
	class ConversationChoiceNode : Node
	{
		public const string InputPortName = "Input Port";
		protected override void OnDefinePorts(IPortDefinitionContext context)
		{
			context.AddInputPort<ConversationChoiceNode>(InputPortName);
		}
	}
}
