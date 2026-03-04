using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MetroidvaniaToolkitEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private VisualElement _root;
    [MenuItem("SF/Metroidvania Toolkit Editor")]
    public static void ShowExample()
    {
        MetroidvaniaToolkitEditor wnd = GetWindow<MetroidvaniaToolkitEditor>();
        wnd.titleContent = new GUIContent("SF Metroidvania Toolkit Editor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
         _root = rootVisualElement;
        
        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        _root.Add(labelFromUXML);
    }
}
