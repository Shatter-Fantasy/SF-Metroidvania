using UnityEngine;

namespace SF.CommandModule
{
    /// <summary>
    /// Starts an async command that tints the sprite and returns it back to the original color a couple times to mimic blinking.
    /// </summary>
    [System.Serializable]
    [CommandMenu("Sprite/Blinking")]
    public class SpriteBlinkCommand : CommandNode, ICommand
    {
        /* Important: The Sprite Renderer below will be replaced with the Charctaer Renderer.
            We will eventually make the CharacterRenderer2D have an equal operator to return the attached Sprite Renderer.
            We also will be adding a tint command to the CharacterRenderer2D eventually. */
        public SpriteRenderer SpriteRenderer;
        public Color TintColor;
        private Color _originalColor = Color.white;
        public float TotalBlinkTime = 0.5f;
        [SerializeField] private bool _resetColor = true;
        private bool _isBlinking;

        protected override bool CanDoCommand()
        {
            IsAsyncCommand = true;
            _originalColor = SpriteRenderer.color;
            return SpriteRenderer != null && !_isBlinking;
        }

        protected override void DoCommand(){ }

        protected override async Awaitable DoAsyncCommand()
        {
            _isBlinking = true;
            SpriteRenderer.color = TintColor;
            await Awaitable.WaitForSecondsAsync(TotalBlinkTime);
            
            if(_resetColor)
                SpriteRenderer.color = _originalColor;
            
            _isBlinking = false;
        }

        public void StopInteruptBlinking()
        {
            SpriteRenderer.color = _originalColor;
        }
    }
}
