using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;

namespace SFEditor.U2D.Sprites
{
	public class DefaultSpriteNameFileIdProvider : ISpriteNameFileIdDataProvider
	{
		protected SpriteNameFileIdPair[] _spriteNameFildIdParis;

		public DefaultSpriteNameFileIdProvider(IEnumerable<SpriteRect> spriteDataRects)
		{
			_spriteNameFildIdParis = spriteDataRects.Select(
				x => new SpriteNameFileIdPair(x.name,x.spriteID)
				).ToArray();
		}

		public IEnumerable<SpriteNameFileIdPair> GetNameFileIdPairs()
		{
			return _spriteNameFildIdParis;
		}

		public void SetNameFileIdPairs(IEnumerable<SpriteNameFileIdPair> nameFileIdPairs)
		{
			_spriteNameFildIdParis = nameFileIdPairs.ToArray();
		}
	}
}
