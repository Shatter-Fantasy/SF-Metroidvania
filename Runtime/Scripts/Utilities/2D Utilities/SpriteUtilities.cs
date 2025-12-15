using UnityEngine;

namespace SF.Utilities
{
    public static class SpriteUtilities
    {
        public static Vector3 GetSpriteWorldSize(Sprite sprite)
        {
            if(sprite != null && sprite.rect.size.magnitude > 0f)
            {
                return new Vector3(
                    sprite.rect.size.x / sprite.pixelsPerUnit,
                    sprite.rect.size.y / sprite.pixelsPerUnit,
                    1f);
            }

            return Vector3.one;
        }
    }
}
