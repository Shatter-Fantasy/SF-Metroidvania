using System;
using System.Collections.Generic;
using SF.DialogueModule;
using SF.DialogueModule.Nodes;
using SFEditor.Nodes;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace SFEditor.Dialogue.Graphs
{
	
	[Serializable]
	[UseWithGraph(typeof(DialogueGraph))]
	public class ConversationContextNode : ContextNode, IDialogueNode, IContextNodeConvertor
	{
		public string ExecutionPortName { get; } = "Dialogue Entry";
		
		public const string ConversationTitleName = "Conversation Name";
		
		public List<IRuntimeNode> RuntimeNodes = new ();
		/// <summary>
		/// Conversation that is only set and used during the <see cref="DialogueGraphImporter"/> processing. 
		/// </summary>
		[NonSerialized] public DialogueConversation Conversation;

		public IRuntimeNode ExecutionNode;
		
		protected override void OnDefineOptions(IOptionDefinitionContext  context)
		{ 
			context.AddOption<string>(ConversationTitleName);
		}
	    
		protected override void OnDefinePorts(IPortDefinitionContext context)
		{
			context.AddInputPort<string>("Input Node");
		    
			context.AddOutputPort<string>(ExecutionPortName);
		}

		public IRuntimeNode ConvertToRuntimeNode()
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

		public List<IRuntimeNode> ConvertToRuntimeNodes(DialogueConversation dialogueConversation)
		{
			if (dialogueConversation != null)
			{
				GetNodeOptionByName(ConversationTitleName)
					.TryGetValue(out dialogueConversation.Name);
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
	}
}
