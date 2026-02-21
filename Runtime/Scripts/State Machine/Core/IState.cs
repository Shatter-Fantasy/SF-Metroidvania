namespace SF.StateMachine
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
