namespace SF.StateMachine.Core
{
	public interface IState
    {
		public void Init()
		{
			OnInit();
		}
		protected void OnInit();
	}
}
