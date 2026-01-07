using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.GraphToolkit.Editor;

using UnityEditor.AssetImporters;
using UnityEngine;

using SFEditor.Nodes;
using SF.DialogueModule.Nodes;
using SF.DialogueModule;
using UnityEditor;


namespace SFEditor.Dialogue.Graphs
{
	
	/// <summary>
	/// DialogueGraphImporter is a <see cref="ScriptedImporter"/> that imports the <see cref="DialogueGraph"/>
	/// and builds the corresponding <see cref="DialogueRuntimeGraph"/>.
	/// </summary>
	[ScriptedImporter(1, DialogueGraph.AssetExtension)]
    public class DialogueGraphImporter : ScriptedImporter
    {
	    private DialogueDatabase _dialogueDatabase;
	    private DialogueConversation _dialogueConversation;
	    private int _conversationID;
	    
	    #region Graph Nodes
	    private List<INode> _graphNodes = new();
	    private List<IRuntimeNode> _runtimeNodes = new();
	    #endregion
	   
	    public override void OnImportAsset(AssetImportContext ctx)
	    {
		    var graph = GraphDatabase.LoadGraphForImporter<DialogueGraph>(ctx.assetPath);
		    if(graph is null)
		    {
			     Debug.LogError($"Failed to load the dialogue graph asset at: {ctx.assetPath}");
			     return;
		    }
		    
		    //If the graph hasn't had any changes since last import don't waste time going through the graph import again.
		    if(!graph.HasGraphChanged)
			    return;

		    // Cache all the graph nodes so we don't have to retrieve them each time and waste allocations.
		    _graphNodes = graph.GetNodes().ToList();
			    
		    // StartDialogueNode tells each graph where the conversation starts.
		    StartDialogueNode startDialogueNode = _graphNodes.OfType<StartDialogueNode>().FirstOrDefault();
			
			// This can happen when first creating a dialogue graph asset and Unity imports the created asset into the project.
			// Not an error when this happens on asset creation and expected behavior. 
		    if(startDialogueNode == null)
		    {
			    return;
		    }

		    startDialogueNode.GetNodeOptionByName(StartDialogueNode.DialogueDBName).TryGetValue(out _dialogueDatabase);
		    startDialogueNode.GetNodeOptionByName(StartDialogueNode.ConversationIDName).TryGetValue(out _conversationID);

		    if(_dialogueDatabase == null)
			    return;

		    // Database conversation checks
		    if(_dialogueDatabase.GetConversation(_conversationID, out _dialogueConversation))
		    {
			    // Only reason I am hesitant on doing an update here is this would prevent
			    // just opening the Dialogue Database from inspector an editing it.
			    // we should first read in the conversation when opening the graph first to make sure the graph matches the 
			    // already existing dialogue conversation.
		    }
		    else
		    {
			    _dialogueConversation = ScriptableObject.CreateInstance<DialogueConversation>();
			    _dialogueConversation.GUID = Guid.NewGuid().GetHashCode();
			    
			    // Default make the conversation asset and put it in the same folder as the graph it is linked to.
			    // Get the path to the graph.
			    StringBuilder stringBuilder = new(ctx.assetPath);
#if UNITY_6000_4_OR_NEWER
			    stringBuilder.Replace($"{graph.Name}.{DialogueGraph.AssetExtension}", "");
			    stringBuilder.Append($"{graph.Name} Conversation.asset");
#else
				stringBuilder.Replace($"{graph.name}.{DialogueGraph.AssetExtension}", "");
			    stringBuilder.Append($"{graph.name} Conversation.asset");
#endif   
			    //Debug.Log(stringBuilder.ToString());
			    AssetDatabase.CreateAsset(_dialogueConversation,stringBuilder.ToString());

		
			    /*var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(
				    "Assets/Editor Default Resources/SF Dialogue Graph/Icons/Dialogue Graph Icon.png");
			    EditorGUIUtility.SetIconForObject(graph, icon);
			    */
			    _conversationID = _dialogueConversation.GUID;

			    _dialogueDatabase.Conversations.Add(_dialogueConversation);
		    }
		    
		    ProcessNodesToConversations(startDialogueNode);

		    if (_dialogueConversation == null)
			    return;
		    
		    
		    // Bad we need to fix this. Only create graph at runtime.
		    _dialogueConversation.RuntimeGraph
			    = new DialogueRuntimeGraph(_dialogueConversation, _runtimeNodes);

		    EditorUtility.SetDirty(_dialogueConversation);
		    graph.HasGraphChanged = false;
	    }

	    private static IDialogueNode GetNextNode<T>(T currentNode) where T : IDialogueNode
	    {
		    var outputPort = currentNode.GetOutputPortByName(currentNode.ExecutionPortName);
		    
#if UNITY_6000_4_OR_NEWER
		    var nextNodePort = outputPort.FirstConnectedPort;
#else
		    var nextNodePort = outputPort.firstConnectedPort;
#endif
		    
		    var nextNode = nextNodePort?.GetNode() as IDialogueNode;
		    return nextNode;
	    }
	    
	    private void ProcessNodesToConversations(IDialogueNode nodeModel)
	    {
		    // Node check.
		    switch (nodeModel)
		    {
			    case ConversationContextNode conversationEntryContextNode:
				    {
					    conversationEntryContextNode
						    .GetNodeOptionByName(ConversationContextNode.ConversationTitleName)
						    .TryGetValue(out _dialogueConversation.Name);
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
								    _runtimeNodes.Add(entryNode);
							    }
							    else
							    {
								    _runtimeNodes.Add(convertedNode);
							    }
						    }
					    }
				    }
				    break;
		    }

		   
		    if (nodeModel is IContextNodeConvertor contextNodeConvertor)
		    {
			    
			    _runtimeNodes.Add(contextNodeConvertor.ConvertToRuntimeNode());
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