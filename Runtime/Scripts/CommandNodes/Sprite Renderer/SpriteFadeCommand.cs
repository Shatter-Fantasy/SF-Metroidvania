using UnityEngine;

namespace SF.CommandModule
{
    [System.Serializable]
    [CommandMenu("Sprite/Fade")]
    public class SpriteFadeCommand : CommandNode, ICommand
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        public bool FadingOut = true;

        [SerializeField] private Timer _fadeTimer;
        private Color _originalColor;
        protected override bool CanDoCommand()
        {
            return _spriteRenderer != null;
        }

        protected override void DoCommand()
        {
            _fadeTimer = new Timer(_fadeTimer.Duration);
            _ = _fadeTimer.StartTimerAsync();

            _originalColor = _spriteRenderer.color;
            float completionPercent = 0;
            
            while (_fadeTimer.RemainingTime > 0)
            {
                if (_fadeTimer.TimerStopped)
                    break;

                completionPercent = _fadeTimer.ElapsedTimer / _fadeTimer.Duration;

                _spriteRenderer.color = Color.Lerp(_originalColor, new Color(0, 0, 0, 0), completionPercent);
            }
        }

        protected override async Awaitable DoAsyncCommand()
        {
            _fadeTimer = new Timer(_fadeTimer.Duration);
            _ = _fadeTimer.StartTimerAsync();

            float completionPercent = 0;
            _originalColor = _spriteRenderer.color;
            
            while (_fadeTimer.RemainingTime > 0)
            {
                if (_fadeTimer.TimerStopped)
                    break;

                completionPercent = _fadeTimer.ElapsedTimer / _fadeTimer.Duration;
                _spriteRenderer.color = Color.Lerp(_originalColor, new Color(0, 0, 0, 0), completionPercent);
                
                await Awaitable.EndOfFrameAsync();
            }
        }
    }
}
