using System;
using System.IO;
using System.Text;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEngine;

namespace SFEditor.DialogueModule
{
    using SF.DataModule;
    using SF.DialogueModule;
    using SFEditor.Dialogue.Graphs;
    public class DialogueAssetPostProcessor : AssetPostprocessor
    {
        public static event Action<DialogueGraph> OnImported; 
        
        // TODO: Implement the OnDeleted and OnMovedAssetFromTo events as well.
        //public static event Action<DialogueGraph> OnDeleted;
        //public static event Action<string, string> OnMovedAssetFromTo;
        
        internal static bool IsPathToGraphAsset(string assetPath) => string.Equals(Path.GetExtension(assetPath), DialogueGraph.AssetExtension);
        internal const string AssetExtensionWithPeriod = "." + DialogueGraph.AssetExtension;
        
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            
            DialogueGraph dialogueGraph = default; 

            foreach (string assetPath in importedAssets)
            {
                // Make sure we are only processing Dialogue Graph assets that are SFGraphBase derived.
                if(Path.GetExtension(assetPath) != AssetExtensionWithPeriod)
                    continue;

                dialogueGraph = GraphDatabase.LoadGraph<DialogueGraph>(assetPath);

                if (dialogueGraph == null)
                    continue;

                // If there is already a linked DialogueConversation asset just update it.
                if (dialogueGraph.LinkedConversationAsset != null)
                {
                    UpdateLinkedAsset(dialogueGraph);
                }
                else // If there is no linked DialogueConversation asset create one and link it.
                {
                    CreateLinkedAsset(dialogueGraph,assetPath);
                }
                //If the graph hasn't had any changes since last import don't waste time going through the graph import again.
                if(dialogueGraph.HasGraphChanged)
                {
                    dialogueGraph.ProcessGraphNodes();
                }
                
                
                if (OnImported != null)
                {
                    OnImported.Invoke(dialogueGraph);
                }
                
                dialogueGraph.HasGraphChanged = false;
            } // End of Foreach loop for importedAssets
        }

        private static void UpdateLinkedAsset(DialogueGraph dialogueGraph)
        {
            //Debug.Log(dialogueGraph.LinkedConversationAsset.name);
        }

        private static void CreateLinkedAsset(DialogueGraph dialogueGraph, string assetPath)
        {
            // Default make the conversation asset and put it in the same folder as the graph it is linked to.
            StringBuilder stringBuilder = new(assetPath);
#if UNITY_6000_4_OR_NEWER
            stringBuilder.Replace($"{dialogueGraph.Name}.{DialogueGraph.AssetExtension}", "");
            stringBuilder.Append($"{dialogueGraph.Name} Conversation.asset");
#else
				stringBuilder.Replace($"{graph.name}.{DialogueGraph.AssetExtension}", "");
			    stringBuilder.Append($"{graph.name} Conversation.asset");
#endif   
            
            DialogueConversation _dialogueConversation = ScriptableObject.CreateInstance<DialogueConversation>();
            _dialogueConversation.GUID = Guid.NewGuid().GetHashCode();
            AssetDatabase.CreateAsset(_dialogueConversation,stringBuilder.ToString());
            dialogueGraph.LinkedConversationAsset = _dialogueConversation;
            
            if (DatabaseRegistry.TryGetDatabase(out DialogueDatabase dialogueDatabase))
            {
                dialogueDatabase.AddData(dialogueGraph.LinkedConversationAsset);
            }
            EditorUtility.SetDirty(_dialogueConversation);
        }
    }
}
