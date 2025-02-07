using System;
using UnityEngine;

namespace SF.CommandModule
{
    public class SpriteBlinkCommand : CommandNode, ICommand
    {
        public SpriteRenderer SpriteRenderer;
        public Color TintColor;
        private Color _originalColor;
        public float BlinkIntervalTimer = 0.5f;
        public float TotalBlinkTime = 1;
        private int _blinkAmount;

        public async override Awaitable Use()
        {
            if (SpriteRenderer == null)
                return;

            _blinkAmount = Mathf.RoundToInt(TotalBlinkTime/BlinkIntervalTimer);
            _originalColor = SpriteRenderer.color;

            await TintSprite();
        }

        private async Awaitable TintSprite()
        {
            while(_blinkAmount > 0)
            {
                SpriteRenderer.color = TintColor;
                await Awaitable.WaitForSecondsAsync(BlinkIntervalTimer);
                SpriteRenderer.color = _originalColor;
                await Awaitable.WaitForSecondsAsync(BlinkIntervalTimer);
                _blinkAmount--;
            }      
        }
    }
}
