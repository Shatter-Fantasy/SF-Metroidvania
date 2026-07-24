using SF.Graphs.Nodes;
using Unity.GraphToolkit.Editor;

namespace SFEditor.Dialogue.Graphs
{
    using SFEditor.Graphs.Nodes;
    [System.Serializable]
    [UseWithContext(typeof(ConversationContextNode))]
    [UseWithGraph(typeof(DialogueGraph))]
    public class ConversationExtensionNode : BlockNode, INodeConvertor
    {
        public string NodeCallerID = "";
        public const string NodeCallerOptionName = "Node Caller ID";

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<string>(NodeCallerOptionName);
        }

        public SFRuntimeNode ConvertToRuntimeNode()
        {
            return null;
            /*
                  ConversationExtensionBase extensionMethod = null;
                  for (int i = 0; i < Runner.ConversaNodeStageExtensions.Count; i++)
                  {
                      if (Runner.ConversaNodeStageExtensions[i].NodeCallerID == NodeCallerID)
                      {
                          extensionMethod = Runner.ConversaNodeStageExtensions[i];
                      }
                  }

                  if (extensionMethod != null)
                  {
                      extensionMethod.ControlFlow();
                  }
              */
        }
    }
}