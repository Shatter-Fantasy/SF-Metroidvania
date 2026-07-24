using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace SF.Graphs.Editor
{
    using SF.DialogueModule;
    using SFEditor.Dialogue.Graphs;
    using SFEditor.Graphs.Nodes;

    [ScriptedImporter(1, DialogueGraphExtension)]
    public class DialogueGraphImporter : ScriptedImporter
    {
        public const string DialogueGraphExtension = "sfgr";
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var graph = GraphDatabase.LoadGraphForImporter<DialogueGraph>(ctx.assetPath);

            // The `graph` may be null if the `GraphDatabase.LoadGraphForImporter` method
            // fails to load the asset from the specified `ctx.assetPath`.
            // This can occur under the following circumstances:
            // - The asset path is incorrect, or the asset does not exist at the specified location.
            // - The asset located at the specified path is not of type `VisualNovelDirectorGraph`.
            // - The asset file itself is problematic. For example, it is corrupted, or stored in an unsupported format.
            //
            // Best practice to deal with serialization is to thoroughly validate and safeguard against
            // impaired or incomplete data, to account for potential deserialization issues.
            if (graph == null)
            {
                Debug.LogError($"Failed to load the dialogue graph asset: {ctx.assetPath}");
                return;
            }

            var startingNode = graph.GetNodes().OfType<SFEditorStartNode>().FirstOrDefault();

            if (startingNode == null)
            {
                graph.UndoBeginRecordGraph("Adding Missing Start Node");
                graph.AddNode(new SFEditorStartNode());
                graph.UndoEndRecordGraph();
            }

            // No need to log an error here, as the VisualNovelDirectorGraphProcessor is already logging an error in the console
                // See VisualNovelDirectorGraph.CheckGraphErrors(GraphLogger).

            // Build the runtime asset by walking the graph and adding the relevant nodes.
            var runtimeAsset = ScriptableObject.CreateInstance<DialogueConversation>();
            runtimeAsset.GraphID = graph.ID;

            graph.ProcessGraphNodes();
            //BuildRuntimeGraph(startNodeModel, runtimeAsset);

            // Add the runtime object to the graph asset and set it to be the main asset.
            // This allows the same asset to be used in inspectors wherever a runtime asset is expected.
            // Refer to the BasicVisualNovelCanvas.prefab for an example of this.
            ctx.AddObjectToAsset("RuntimeAsset", runtimeAsset);
            ctx.SetMainObject(runtimeAsset);
        }
    }
}