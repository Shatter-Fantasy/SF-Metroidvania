using UnityEngine;
using UnityEngine.Tilemaps;

namespace SF.TileModule
{
    [CreateAssetMenu(menuName = "SF/2D/Tiles/SFNeighbour Rule Tile", fileName = "Neighbor Rule Tile")]
    public class SFNeighbourRuleTile : RuleTile<SFNeighbourRuleTile.Neighbor>, 
        ISFTiles
    {

        public class Neighbor : RuleTile.TilingRuleOutput.Neighbor {
            public const int Null = 3;
            public const int NotNull = 4;
        }

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject instantiatedGameObject)
        {
            if (tilemap == null)
                return false;
            
            return base.StartUp(position, tilemap, instantiatedGameObject);
        }

        public override void RefreshTile(Vector3Int position, ITilemap tilemap)
        {
            /*
#if UNITY_EDITOR
            if (UnityEditor.BuildPipeline.isBuildingPlayer || Application.isBatchMode)
                return;
            
#endif*/
            if (tilemap == null || tilemap is not ITilemap)
                return;
            
            if (tilemap.GetTile(position) == null)
                return;

            
            
            base.RefreshTile(position,tilemap);
        }

        public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
        {
            /*
#if UNITY_EDITOR
            if (UnityEditor.BuildPipeline.isBuildingPlayer || Application.isBatchMode)
                return false;
#endif */
            
            if (tilemap == null)
            {
                tileAnimationData = new TileAnimationData();
                return false;
            }
            
            return base.GetTileAnimationData(position, tilemap, ref tileAnimationData);
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
/* #if UNITY_EDITOR
            if (UnityEditor.BuildPipeline.isBuildingPlayer || Application.isBatchMode)
                return;
#endif*/

            if (tilemap == null)
            {
                tileData = new TileData();
                return;
            }

            base.GetTileData(position, tilemap, ref tileData);
        }

        /// <summary>
        ///     Does a Rule Match given a Tiling Rule and neighboring Tiles.
        /// </summary>
        /// <param name="rule">The Tiling Rule to match with.</param>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="tilemap">The tilemap to match with.</param>
        /// <param name="transform">A transform matrix which will match the Rule.</param>
        /// <returns>True if there is a match, False if not.</returns>
        public override bool RuleMatches(TilingRule rule, Vector3Int position, ITilemap tilemap, ref Matrix4x4 transform)
        {
/*#if UNITY_EDITOR
            if (UnityEditor.BuildPipeline.isBuildingPlayer || Application.isBatchMode)
                return false;
#endif*/

            if (tilemap == null)
                return false;

            return base.RuleMatches(rule, position, tilemap, ref transform);
        }

        public override bool RuleMatch(int neighbor, TileBase tile)
        {
/*#if UNITY_EDITOR
            if (UnityEditor.BuildPipeline.isBuildingPlayer || Application.isBatchMode)
                return false;
#endif*/

            if (neighbor == Neighbor.Null)
                return tile == null;

            if (tile is RuleOverrideTile overrideTile)
                tile = overrideTile.m_InstanceTile;

            switch (neighbor)
            {
                case TilingRuleOutput.Neighbor.This:
                {
                    return tile is ISFTiles;
                }
                case TilingRuleOutput.Neighbor.NotThis:
                {
                    return tile != this;
                }
                case Neighbor.NotNull: return tile != null;
            }

            return true;
        }
    }
}