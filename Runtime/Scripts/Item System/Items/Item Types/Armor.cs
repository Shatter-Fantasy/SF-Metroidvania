namespace SF.ItemModule
{
	[System.Serializable]
	public class Armor : ItemData
	{
		public Armor() : this("New Armor") { }
		public Armor(string name = "New Armor", string description = "I am a new Armor")
			: base(name, description) { }

		public override void Use()
		{
		}
	}
}