using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;

namespace SFEditor.DialogueModule
{
    using SF.DialogueModule;
    using SF.DialogueModule.Nodes;
    using SFEditor.Dialogue.Graphs;
    using SFEditor.Nodes;
    using static DialogueNodeUtilities;
    
    public class DialogueNodeProcessor
    {
        private DialogueGraph _dialogueGraph;
        private DialogueConversation _dialogueConversation;
        
#region Graph Nodes
        public List<INode> GraphNodes = new();
        public List<IRuntimeNode> RuntimeNodes = new();
        
        /// <summary>
        /// StartDialogueNode tells each graph where the conversation starts.
        /// </summary>
        private StartDialogueNode _startDialogueNode;
#endregion

        public DialogueNodeProcessor(DialogueGraph dialogueGraph)
        {
            _dialogueGraph        = dialogueGraph;
            
            if(_dialogueGraph != null)
                _dialogueConversation = _dialogueGraph.LinkedConversationAsset;
        }

        public void StartNodeProcessing()
        {
            // Cache all the graph nodes so we don't have to retrieve them each time and waste allocations.
            GraphNodes = _dialogueGraph?.GetNodes().ToList();
            
            // StartDialogueNode tells each graph where the conversation starts.
            _startDialogueNode = GraphNodes?.OfType<StartDialogueNode>().FirstOrDefault();
            
            // This can happen when first creating a dialogue graph asset and Unity imports the created asset into the project.
            // Not an error when this happens on asset creation and expected behavior. 
            if(_startDialogueNode == null)
                return;

            ProcessNodesToConversations(_startDialogueNode);
        }
        
        private void ProcessNodesToConversations(IDialogueNode nodeModel)
        {
            if (_dialogueConversation == null)
                return;
            
            // Node check.
            switch (nodeModel)
            {
                case ConversationContextNode conversationEntryContextNode:
                {
                    conversationEntryContextNode
                    .GetNodeOptionByName(ConversationContextNode.ConversationTitleName)
                    .TryGetValue(out _dialogueConversation.ConversationName);
#if UNITY_6000_4_OR_NEWER
                    for(int i = 0; i < conversationEntryContextNode.BlockCount; i++) 
#else
					    for(int i = 0; i < conversationEntryContextNode.blockCount; i++) 
#endif
                    {
                        var conversationNode = conversationEntryContextNode.GetBlock(i);
						    
                        if (conversationNode is INodeConvertor convertor)
                        {
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
                    }
                }
                    break;
            }

		   
            if (nodeModel is IContextNodeConvertor contextNodeConvertor)
            {
                RuntimeNodes.Add(contextNodeConvertor.ConvertToRuntimeNode());
                //_runtimeNodes.AddRange(contextNodeConvertor.ConvertToRuntimeNodes(_dialogueConversation));
            }
		    
            var nextNode = GetNextNode(nodeModel);
		    
            if (nextNode != null)
            {
                ProcessNodesToConversations(nextNode);
            }
        }
    }
}
