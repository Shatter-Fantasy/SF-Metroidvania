using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SF.Utilities
{
    public static class TileMapUtilities
    {
        public static void GetUsedTiles<TTileType>(this Tilemap tilemap, out List<TTileType> usedTiles) 
            where TTileType : TileBase
        {
            usedTiles = new();

            if (tilemap == null)
                return;
            
            tilemap.CompressBounds();
            BoundsInt bounds = tilemap.cellBounds;
            var positionEnumerator = bounds.allPositionsWithin;
            
            foreach (var position in positionEnumerator)
            {
                if(tilemap.HasTile(position))
                    usedTiles.Add(tilemap.GetTile<TTileType>(position));
            }
        }
        
        public static NativeList<Vector3Int> GetTileCellPositions(this Tilemap tilemap) 
        {
            NativeList<Vector3Int> tilePositions = new NativeList<Vector3Int>(Allocator.Persistent);
            
            if (tilemap == null)
                return tilePositions;
            
            tilemap.CompressBounds();
            BoundsInt bounds = tilemap.cellBounds;
            
            var positionEnumerator = bounds.allPositionsWithin;
            
            foreach (var position in positionEnumerator)
            {
                if(tilemap.HasTile(position))
                    tilePositions.Add(position);
            }

            return tilePositions;
        }

        public static void GetUsedTileData(this Tilemap tilemap, out List<TileData> usedTileData)
        {
            usedTileData = new();

            if (tilemap == null)
                return;
            
            tilemap.CompressBounds();
            BoundsInt bounds = tilemap.cellBounds;
            var positionEnumerator = bounds.allPositionsWithin;
            
            
            foreach (var position in positionEnumerator)
            {
                if (tilemap.HasTile(position))
                {
                    var tileData = new TileData();
                    tilemap.GetTile(position).GetTileData(position,tilemap, ref tileData);
                    usedTileData.Add(tileData);
                }
            }
        }
    }

    public struct TilePositionPositionEnumeratorJob : IJob
    {
        public NativeList<Vector3Int> TilePositions;
        
        [ReadOnly]
        public BoundsInt.PositionEnumerator PositionEnumerator;
        
        public void Execute()
        {
            
        }
    }
}
