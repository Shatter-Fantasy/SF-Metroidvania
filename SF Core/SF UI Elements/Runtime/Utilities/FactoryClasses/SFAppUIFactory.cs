
using Unity.Properties;
#if SF_APPUI // If the Unity AppUI package is installed.
using Unity.AppUI.Core;
using TextField = Unity.AppUI.UI.TextField;
using Unity.AppUI.UI;
using UnityEngine.UIElements;
#endif

namespace SF.AppUI
{
    public class SFAppUIFactory
    {
        
        #if SF_APPUI // If the Unity AppUI package is installed.
        
        /// <summary>
        /// Creates an AppUI InputLabel and link it to a TextField while setting the TextFields size and the InputLabels label text.
        /// Than returns the base InputLabel element for further use. 
        /// </summary>
        /// <param name="labelText"></param>
        /// <param name="size"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static InputLabel CreateLabeledTextField(string labelText,Size size = Size.M, Direction direction = Direction.Horizontal)
        {
            InputLabel label = new InputLabel(labelText) { direction = direction };
            TextField textField = new TextField()
            {
                size = size,
            };
            
            label.Add(textField);
            return label;
        }
        
        /// <summary>
        /// Creates an AppUI InputLabel and link it to a TextField while setting the TextFields size and the InputLabels label text.
        /// Then returns the base InputLabel element for further use. 
        /// </summary>
        /// <param name="labelText"></param>
        /// <param name="field"></param>
        /// <param name="size"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static InputLabel CreateLabeledTextField(string labelText,TextField textField,Size size = Size.M, Direction direction = Direction.Horizontal)
        {
            InputLabel label = new InputLabel(labelText) { direction = direction };
            textField.size = size;
            
            label.Add(textField);
            return label;
        }
#endif
    }
}
