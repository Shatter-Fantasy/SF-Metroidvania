using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SF.TileModule
{
    public interface ISFTiles { }

    [Serializable]
    [CreateAssetMenu(fileName = "New SF Tile", menuName = "SF/2D/Tiles/SF Tile")]
    public class SFTile : Tile, ISFTiles
    {
        
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

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
#if UNITY_EDITOR
            if (UnityEditor.BuildPipeline.isBuildingPlayer || Application.isBatchMode)
                return;
#endif
            
            if (tilemap == null)
            {
                tileData        = new TileData();
                tileData.sprite = sprite;
                return;
            }
            
            base.GetTileData(position, tilemap, ref tileData);
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
    }
}
