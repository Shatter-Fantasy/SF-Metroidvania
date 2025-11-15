using UnityEngine;
using SF.DataModule;
namespace SF.LootModule
{

	[CreateAssetMenu(fileName = "Region Table Data", menuName = "SF/Loot/Region Table Data")]
	public class RegionalLootTableData : DTOAssetBase
	{
		public string Region;
	}
}
