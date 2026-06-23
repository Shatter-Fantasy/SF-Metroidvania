using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;

namespace SFEditor.TileModule
{
    [Overlay(typeof(SceneView), "Tilemap Measure", true)]
    public class TilemapMeasureOverlay : Overlay
    {
        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement() { name = "My Toolbar Root" };
            root.Add(new Label() { text           = "Hello" });
            return root;
        }
    }
}
