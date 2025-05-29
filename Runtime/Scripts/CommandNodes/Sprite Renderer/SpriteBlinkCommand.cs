using UnityEngine;

namespace SF.CommandModule
{
    [System.Serializable]
    public class SpriteBlinkCommand : CommandNode, ICommand
    {
        public SpriteRenderer SpriteRenderer;
        public Color TintColor;
        private Color _originalColor;
        public float TotalBlinkTime = 0.5f;
        private bool _isBlinking;
        public override async  Awaitable Use()
        {
            if (SpriteRenderer == null)
                return;

            // Don't start a new blink if we are already doing one.
            if (_isBlinking)
                return;
            
            _originalColor = SpriteRenderer.color;

            await TintSprite();
        }

        private async Awaitable TintSprite()
        {
            _isBlinking = true;
            SpriteRenderer.color = TintColor;
            await Awaitable.WaitForSecondsAsync(TotalBlinkTime);
            SpriteRenderer.color = _originalColor;
            _isBlinking = false;
        }

        public void StopInteruptBlinking()
        {
            SpriteRenderer.color = _originalColor;
        }
    }
}
