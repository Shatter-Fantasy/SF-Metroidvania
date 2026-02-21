using System;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEngine;

namespace SFEditor.Dialogue.Graphs
{
	[Icon("AssetIconPath")]
    [Graph(AssetExtension)] [Serializable]
    public class DialogueGraph : Graph
    {
	    public const string AssetIconPath = "Assets/Editor Default Resources/SF Dialogue Graph/Icons/Dialogue Graph Icon.png";
	    public const string AssetExtension = "diagr";
	    
	    /// <summary>
	    /// If the graph has changed we need to update it during the next graph importer.
	    /// </summary>
	    public bool HasGraphChanged;
	    
	    [MenuItem("Assets/Create/SF/DialogueGraph/Dialogue Graph", false)]
	    static void CreateAssetFile()
	    {
		    GraphDatabase.PromptInProjectBrowserToCreateNewAsset<DialogueGraph>();
		    
	    }

	    public override void OnGraphChanged(GraphLogger graphLogger)
	    {
		    HasGraphChanged = true;
	    }
    }

    [Serializable]
    class DialogueInterruptionNode : Node
    {
	    protected override void OnDefinePorts(IPortDefinitionContext context)
	    {
		    context.AddInputPort<string>("Input").Build();
		    context.AddOutputPort<string>("Output").Build();
	    }
    }
    
	[Serializable]
    class DialoguePersonalityContextNode : ContextNode
    {
	    protected override void OnDefinePorts(IPortDefinitionContext context)
	    {
		    context.AddInputPort<string>("Speaker");
		    context.AddInputPort<string>("Conversation Entry");
		    context.AddOutputPort<string>("Conversation Entry Output");
	    }
    }
    
    [UseWithContext(typeof(DialoguePersonalityContextNode))] [Serializable]
    class DialoguePersonalityBlockNode : BlockNode
    {
	    protected override void OnDefineOptions(IOptionDefinitionContext  context)
	    {
		    context.AddOption<PersonalityTraits>("Personality");
	    }
	    protected override void OnDefinePorts(IPortDefinitionContext context)
	    {
		    context.AddInputPort<string>("Speaker");
		    context.AddInputPort<string>("Conversation Entry");
		    context.AddOutputPort<string>("Conversation Entry Output");
	    }
    }

    public enum PersonalityTraits
    {
	    Quirky,
	    Proud,
	    Social
    }
    
    [Serializable]
    class SpriteNode : Node
    {
	    protected override void OnDefineOptions(IOptionDefinitionContext  context)
	    {
		    context.AddOption<Sprite>("Sprite");
	    }

	    protected override void OnDefinePorts(IPortDefinitionContext context)
	    {
		    context.AddOutputPort<Sprite>("Output").Build();
	    }
    }
    
    [Serializable]
    class SpriteRenderNode : Node
    {
	    protected override void OnDefineOptions(IOptionDefinitionContext  context)
	    {
		    context.AddOption<SpriteRenderer>("Sprite");
	    }

	    protected override void OnDefinePorts(IPortDefinitionContext context)
	    {
		    context.AddOutputPort<SpriteRenderer>("Output").Build();
	    }
    }
    
    [Serializable]
    class SpriteBlinkNode : Node
    {
	    protected override void OnDefineOptions(IOptionDefinitionContext  context)
	    {
		    context.AddOption<Color>("Blink Color");
		    context.AddOption<float>("Blink Duration");
	    }

	    protected override void OnDefinePorts(IPortDefinitionContext context)
	    {
		    context.AddInputPort<Sprite>("Input").Build();
	    }
    }
}
