using UnityEngine;
using SF.Events;
using TMPro;
namespace SF
{
    public class ScoreManager : MonoBehaviour, EventListener<ScoreEvent>, EventListener<RespawnEvent>
    {
        public float Score;
        public TMP_Text ScoreText;
        public float LevelHighScore;
        public float ScoreMultiplier = 1;
		#region OnEvents
		public void OnEvent(ScoreEvent scoreEvent)
        {
            switch (scoreEvent.EventType)
            {
                case ScoreEventTypes.ScoreIncrease:
                    ScoreChange(scoreEvent.ScoreChange);
                    break;
                case ScoreEventTypes.ScoreDecrease:
                    ScoreChange(scoreEvent.ScoreChange);
                    break;
                case ScoreEventTypes.ScoreSet:
                    ScoreChange(scoreEvent.ScoreChange, true);
                    break;
            }
        }

		public void OnEvent(RespawnEvent respawnEvent)
		{
			switch(respawnEvent.EventType)
			{
				case RespawnEventTypes.PlayerRespawn:
					ResetScore();
					break;
			}
		}
		#endregion
		#region Score Functions
		private void ResetScore()
		{
            Score = 0;

			if(ScoreText != null)
				ScoreText.text = ($"Score: {Score}");
		}

		private void ScoreChange(float scoreChange, bool settingScore = false)
        {
            Score = (settingScore)
                ? scoreChange
                : Score += scoreChange;

            if(ScoreText != null)
                ScoreText.text = ($"Score: {Score}");
        }
		#endregion
		#region Enable/Disable
		private void OnEnable()
        {
             this.EventStartListening<ScoreEvent>();
             this.EventStartListening<RespawnEvent>();
        }
        private void OnDisable()
        {
            this.EventStopListening<ScoreEvent>();
            this.EventStopListening<RespawnEvent>();
        }
        #endregion
    }
}
