using UnityEngine;
using UnityEngine.Tilemaps;

namespace SF.TileModule
{
    [CreateAssetMenu(fileName = "New SF TileBase", menuName = "SF/2D/Tiles/SF Tile Base")]
    public class SFTileBase : TileBase, ISFTiles
    {
        public Sprite DefaultSprite;
        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
        {
#if UNITY_EDITOR
            if (UnityEditor.BuildPipeline.isBuildingPlayer || Application.isBatchMode)
                return false;
#endif
            
            if (tilemap == null)
                return false;
            
            return base.StartUp(position, tilemap, go);
        }
        
        public override void RefreshTile(Vector3Int position, ITilemap tilemap)
        {
#if UNITY_EDITOR
            if (UnityEditor.BuildPipeline.isBuildingPlayer || Application.isBatchMode)
                return;
#endif
            
            if (tilemap == null)
                return;
           
            base.RefreshTile(position, tilemap);
        }
        
        public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
        {
            
#if UNITY_EDITOR
            if (UnityEditor.BuildPipeline.isBuildingPlayer || Application.isBatchMode)
                return false;
#endif
            
            if (tilemap == null)
                return false;
            
            return base.GetTileAnimationData(position, tilemap, ref tileAnimationData);
        }
        
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
#if UNITY_EDITOR
            if (UnityEditor.BuildPipeline.isBuildingPlayer || Application.isBatchMode)
                return;
#endif
            
            
            if (tilemap == null)
            {
                tileData        = new TileData();
                tileData.sprite = DefaultSprite;
                return;
            }
            
            base.GetTileData(position,tilemap,ref tileData);
        }
    }
}
