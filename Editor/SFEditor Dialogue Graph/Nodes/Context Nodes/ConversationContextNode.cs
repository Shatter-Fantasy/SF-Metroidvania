using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;

namespace SFEditor.Dialogue.Graphs
{
	using SF.Graphs.Nodes;
	using SFEditor.Graphs.Nodes;
	using SF.DialogueModule;
	using SF.DialogueModule.Nodes;

	[Serializable]
	[UseWithGraph(typeof(DialogueGraph))]
	public class ConversationContextNode : SFEditorContextNode,
		IDialogueNode, 
		IContextNodeConvertor
	{
		public override string ExecutionPortName { get; } = "Dialogue Entry";
		
		public const string ConversationTitleName = "Conversation Name";
		
		public List<SFRuntimeNode> RuntimeNodes = new ();
		/// <summary>
		/// Conversation that is only set and used during the <see cref="DialogueGraphImporter"/> processing. 
		/// </summary>
		[NonSerialized] public DialogueConversation Conversation;

		public SFRuntimeNode ExecutionNode;
		
		protected override void OnDefineOptions(IOptionDefinitionContext  context)
		{ 
			context.AddOption<string>(ConversationTitleName);
		}
	    
		protected override void OnDefinePorts(IPortDefinitionContext context)
		{
			context.AddInputPort<string>("Input Node").Build();
		    
			context.AddOutputPort<string>(ExecutionPortName).Build();
		}

		public SFRuntimeNode ConvertToRuntimeNode()
		{
			RuntimeNodes = ConvertToRuntimeNodes(Conversation);

#if UNITY_6000_4_OR_NEWER
			var executionNode = GetOutputPortByName(ExecutionPortName).FirstConnectedPort.GetNode();
#else
			var executionNode = GetOutputPortByName(ExecutionPortName).firstConnectedPort.GetNode();
#endif
			if (executionNode is INodeConvertor nodeConvertor)
			{
				return new ConversationRuntimeNode(RuntimeNodes,nodeConvertor.ConvertToRuntimeNode());
			}
			
			return new ConversationRuntimeNode(RuntimeNodes);
		}

		public List<SFRuntimeNode> ConvertToRuntimeNodes()
		{
			throw new NotImplementedException();
		}

		public List<SFRuntimeNode> ConvertToRuntimeNodes(DialogueConversation dialogueConversation)
		{
			if (dialogueConversation != null)
			{
				GetNodeOptionByName(ConversationTitleName)
					.TryGetValue(out dialogueConversation.ConversationName);
			}
			
#if UNITY_6000_4_OR_NEWER
						for(int i = 0; i < BlockCount; i++) 
#else
			for(int i = 0; i < blockCount; i++) 
#endif
			{
				var conversationNode = GetBlock(i);
						    
				if (conversationNode is not INodeConvertor convertor)
					return null;
				
				var convertedNode = convertor.ConvertToRuntimeNode();
						    
				if(convertedNode == null)
					continue;
						    
				// The below should be a switch statement after testing is done.
				if (convertedNode is ConversationEntryRuntimeNode entryNode)
				{
					RuntimeNodes.Add(entryNode);
				}
				else
				{
					RuntimeNodes.Add(convertedNode);
				}
			}
			

			return RuntimeNodes;
		}

		SFRuntimeNode INodeConvertor.ConvertToRuntimeNode()
		{
			throw new NotImplementedException();
		}
	}
}