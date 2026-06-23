using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SFEditor.Tilemaps
{
    [CreateAssetMenu(menuName = "SF/2D/Tile Templates/SF TileTemplate",fileName = "New SF Tile Template")]
    public class SFTileTemplate : TileTemplate
    {
        [SerializeField] private int _spritePPU = 16;
        
        [SerializeField] private List<GameObject> _gameObjectForTiles;
        
        public override void CreateTileAssets(Texture2D texture2D, IEnumerable<Sprite> sprites, ref List<TileChangeData> tilesToAdd)
        {
            int xTilePosition = 0;
            int yTilePosition = 0;
            
            // For each sprite in the source texture
            foreach (Sprite sprite in sprites)
            {
                // Create a new tile instance and assign the sprite
                Tile newTile = ScriptableObject.CreateInstance<Tile>();
                newTile.name   = sprite.name;
                newTile.sprite = sprite;

                // Position the tile at the current X position in the tile palette
                tilesToAdd.Add(new TileChangeData()
                {
                    position  = new Vector3Int(xTilePosition, yTilePosition, 0),
                    tile      = newTile,
                    transform = Matrix4x4.identity,
                    color     = Color.white
                });

                // Increment the x position for the next tile
                xTilePosition++;
            }

            xTilePosition = 0;
            yTilePosition++;
            
            foreach (var gameObject in _gameObjectForTiles)
            {
                if(gameObject == null)
                    continue;
                
                // Create a new tile instance and assign the sprite
                Tile newTile = ScriptableObject.CreateInstance<Tile>();
                newTile.name       = gameObject.name;
                newTile.gameObject = gameObject;

                int xSizeOffset = 0;
                
                if (gameObject.TryGetComponent(out SpriteRenderer spriteRend))
                {
                    var renderSprite = spriteRend.sprite;
                    if(renderSprite != null)
                    {
                        newTile.sprite = renderSprite;
                        xSizeOffset    = Mathf.FloorToInt(renderSprite.rect.width / _spritePPU);
                    }
                }
                
                // Position the tile at the current X position in the tile palette
                tilesToAdd.Add(new TileChangeData()
                {
                    position  = new Vector3Int(xTilePosition + xSizeOffset, yTilePosition, 0),
                    tile      = newTile,
                    transform = Matrix4x4.identity,
                    color     = Color.white,
                });

                // Increment the x position for the next tile
                xTilePosition++;
            }
        }
    }
}
