using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    ///     This Brush helps draw lines of Tiles onto a Tilemap.
    ///     Use this as an example to modify brush painting behaviour to making painting quicker with less actions.
    /// </summary>
    [CustomGridBrush(true, false, false, "SF Line Brush")]
    public class SFLineBrush : GridBrush
    {
        public bool AutoAdjustTileSize = true;
        public Vector2 CurrentTileSize = new Vector2(32,32);
        public Vector2 BaseTileSize = new Vector2(16,16);
        public Vector2 OffsetAmount;
        /// <summary>
        /// Ensures that there are orthogonal connections of Tiles from the start of the line to the end.
        /// </summary>
        public bool FillGaps;

        /// <summary>
        ///     The current starting point of the line.
        /// </summary>
        public Vector3Int LineStart = Vector3Int.zero;

        /// <summary>
        ///     Whether the Line Brush has started drawing a line.
        /// </summary>
        [NonSerialized] public bool LineStartActive;

        /// <summary>
        ///     Indicates whether the brush is currently
        ///     moving something using the "Move selection with active brush" tool.
        /// </summary>
        public bool IsMoving { get; private set; }

        private void OnEnable()
        {
            LineStartActive = false;
        }

        /// <summary>
        ///     Paints tiles and GameObjects into a given position within the selected layers.
        ///     The LineBrush overrides this to provide line painting functionality.
        ///     The first paint action sets the starting point of the line.
        ///     The next paint action sets the ending point of the line and paints Tile from start to end.
        /// </summary>
        /// <param name="grid">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the paint operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to paint data to.</param>
        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            if (cells is { Length: > 0 } && cells[0].tile != null
                && AutoAdjustTileSize)
            {
                /* If we are using auto adjust tile size use the first selected tile as the base tile size.
                 * This method comes with drawbacks. You can select multiple tiles and if the tiles are different 
                 * sizes you could run into logic issues. This will be improved in the future. */
                
                // Have to geth the target EditorPreviewTilemap from the brush target for GetTileData.
                if (brushTarget.TryGetComponent(out Tilemap paintingTilemap))
                {
                    TileBase paintingTile = cells[0].tile;
                    TileData paintingData = new TileData();
                    paintingTile.GetTileData(position, paintingTilemap, ref paintingData);
                    var paintingSprite = paintingData.sprite;
                    if (paintingSprite != null)
                    {
                        var newTileSize = paintingSprite.pixelsPerUnit;
                        BaseTileSize = new Vector2(newTileSize,newTileSize);
                        CurrentTileSize = paintingSprite.rect.size;
                    }
                }
            }
            
            OffsetAmount = CurrentTileSize / BaseTileSize;
            
            int tileIndex = 0;
            if (LineStartActive)
            {
                var startPos = new Vector2Int(LineStart.x, LineStart.y);
                var endPos = new Vector2Int(position.x, position.y);
                if (startPos == endPos)
                    base.Paint(grid, brushTarget, position);
                else
                    foreach (var point in GetPointsOnLine(startPos, endPos, FillGaps))
                    {
                        /* Compare current brush position to starting position.
                         * Offset and only paint the current tile based on it's overall size compared to the base tile size.
                        */
                        
                        if (tileIndex != 0 && tileIndex % OffsetAmount.x != 0) // Not the starting
                        {
                            
                        }
                        else
                        {
                            var paintPos = new Vector3Int(point.x, point.y, position.z);
                            base.Paint(grid, brushTarget, paintPos);
                        }
                        tileIndex++;
                    }

                LineStartActive = false;
            }
            else if (IsMoving)
            {
                Debug.Log("Moving");
                base.Paint(grid, brushTarget, position);
            }
            else
            {
                LineStart = position;
                LineStartActive = true;
            }
        }

        /// <summary>
        ///     Starts the movement of tiles and GameObjects from a given position within the selected layers.
        /// </summary>
        /// <param name="grid">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the Move operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to move data from.</param>
        public override void MoveStart(GridLayout grid, GameObject brushTarget, BoundsInt position)
        {
            base.MoveStart(grid, brushTarget, position);
            IsMoving = true;
        }

        /// <summary>
        ///     Ends the movement of tiles and GameObjects to a given position within the selected layers.
        /// </summary>
        /// <param name="grid">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the Move operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to move data to.</param>
        public override void MoveEnd(GridLayout grid, GameObject brushTarget, BoundsInt position)
        {
            base.MoveEnd(grid, brushTarget, position);
            IsMoving = false;
        }

        /// <summary>
        ///     Enumerates all the points between the start and end position which are
        ///     linked diagonally or orthogonally.
        /// </summary>
        /// <param name="startPos">Start position of the line.</param>
        /// <param name="endPos">End position of the line.</param>
        /// <param name="fillGaps">
        ///     Fills any gaps between the start and end position so that
        ///     all points are linked only orthogonally.
        /// </param>
        /// <returns>
        ///     Returns an IEnumerable which enumerates all the points between the start and end position which are
        ///     linked diagonally or orthogonally.
        /// </returns>
        public static IEnumerable<Vector2Int> GetPointsOnLine(Vector2Int startPos, Vector2Int endPos, bool fillGaps)
        {
            var points = GetPointsOnLine(startPos, endPos);
            if (fillGaps)
            {
                var rise = endPos.y - startPos.y;
                var run = endPos.x - startPos.x;

                if (rise != 0 || run != 0)
                {
                    var extraStart = startPos;
                    var extraEnd = endPos;


                    if (Mathf.Abs(rise) >= Mathf.Abs(run))
                    {
                        // up
                        if (rise > 0)
                        {
                            extraStart.y += 1;
                            extraEnd.y += 1;
                        }
                        // down
                        else // rise < 0
                        {
                            extraStart.y -= 1;
                            extraEnd.y -= 1;
                        }
                    }
                    else // Mathf.Abs(rise) < Mathf.Abs(run)
                    {
                        // right
                        if (run > 0)
                        {
                            extraStart.x += 1;
                            extraEnd.x += 1;
                        }
                        // left
                        else // run < 0
                        {
                            extraStart.x -= 1;
                            extraEnd.x -= 1;
                        }
                    }

                    var extraPoints = GetPointsOnLine(extraStart, extraEnd);
                    extraPoints = extraPoints.Except(new[] { extraEnd });
                    points = points.Union(extraPoints);
                }
            }

            return points;
        }

        /// <summary>
        ///     Gets an enumerable for all the cells directly between two points
        ///     http://ericw.ca/notes/bresenhams-line-algorithm-in-csharp.html
        /// </summary>
        /// <param name="p1">A starting point of a line</param>
        /// <param name="p2">An ending point of a line</param>
        /// <returns>Gets an enumerable for all the cells directly between two points</returns>
        public static IEnumerable<Vector2Int> GetPointsOnLine(Vector2Int p1, Vector2Int p2)
        {
            var x0 = p1.x;
            var y0 = p1.y;
            var x1 = p2.x;
            var y1 = p2.y;

            var steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                int t;
                t = x0; // swap x0 and y0
                x0 = y0;
                y0 = t;
                t = x1; // swap x1 and y1
                x1 = y1;
                y1 = t;
            }

            if (x0 > x1)
            {
                int t;
                t = x0; // swap x0 and x1
                x0 = x1;
                x1 = t;
                t = y0; // swap y0 and y1
                y0 = y1;
                y1 = t;
            }

            var dx = x1 - x0;
            var dy = Math.Abs(y1 - y0);
            var error = dx / 2;
            var ystep = y0 < y1 ? 1 : -1;
            var y = y0;
            for (var x = x0; x <= x1; x++)
            {
                yield return new Vector2Int(steep ? y : x, steep ? x : y);
                error = error - dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
        }
    }

    /// <summary>
    ///     The Brush Editor for a Line Brush.
    /// </summary>
    [CustomEditor(typeof(SFLineBrush))]
    public class SFLineBrushEditor : GridBrushEditor
    {
        private static readonly string iconPath =
            "Packages/com.unity.2d.tilemap.extras/Editor/Brushes/LineBrush/LineBrush.png";

        private Tilemap lastTilemap;

        private Texture2D m_BrushIcon;
        private SFLineBrush lineBrush => target as SFLineBrush;

        /// <summary> Returns an icon identifying the Line Brush. </summary>
        public override Texture2D icon
        {
            get
            {
                if (m_BrushIcon == null)
                {
                    var gui = EditorGUIUtility.TrIconContent(iconPath);
                    m_BrushIcon = gui.image as Texture2D;
                }

                return m_BrushIcon;
            }
        }

        /// <summary>
        ///     Callback for painting the GUI for the GridBrush in the Scene View.
        ///     The CoordinateBrush Editor overrides this to draw the preview of the brush when drawing lines.
        /// </summary>
        /// <param name="grid">Grid that the brush is being used on.</param>
        /// <param name="brushTarget">
        ///     Target of the GridBrushBase::ref::Tool operation. By default the currently selected
        ///     GameObject.
        /// </param>
        /// <param name="position">Current selected location of the brush.</param>
        /// <param name="tool">Current GridBrushBase::ref::Tool selected.</param>
        /// <param name="executing">Whether brush is being used.</param>
        public override void OnPaintSceneGUI(GridLayout grid, GameObject brushTarget, BoundsInt position,
            GridBrushBase.Tool tool, bool executing)
        {
            base.OnPaintSceneGUI(grid, brushTarget, position, tool, executing);
            if (lineBrush.LineStartActive && brushTarget != null)
            {
                var tilemap = brushTarget.GetComponent<Tilemap>();
                if (tilemap != null)
                {
                    tilemap.ClearAllEditorPreviewTiles();
                    lastTilemap = tilemap;
                }

                // Draw preview tiles for tilemap
                var startPos = new Vector2Int(lineBrush.LineStart.x, lineBrush.LineStart.y);
                var endPos = new Vector2Int(position.x, position.y);
                if (startPos == endPos)
                    PaintPreview(grid, brushTarget, position.min);
                else
                    foreach (var point in SFLineBrush.GetPointsOnLine(startPos, endPos, lineBrush.FillGaps))
                    {
                        var paintPos = new Vector3Int(point.x, point.y, position.z);
                        PaintPreview(grid, brushTarget, paintPos);
                    }

                if (Event.current.type == EventType.Repaint)
                {
                    var min = lineBrush.LineStart;
                    var max = lineBrush.LineStart + position.size;

                    // Draws a box on the picked starting position
                    GL.PushMatrix();
                    GL.MultMatrix(GUI.matrix);
                    GL.Begin(GL.LINES);
                    Handles.color = Color.blue;
                    Handles.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(max.x, min.y, min.z));
                    Handles.DrawLine(new Vector3(max.x, min.y, min.z), new Vector3(max.x, max.y, min.z));
                    Handles.DrawLine(new Vector3(max.x, max.y, min.z), new Vector3(min.x, max.y, min.z));
                    Handles.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(min.x, min.y, min.z));
                    GL.End();
                    GL.PopMatrix();
                }
            }
        }

        /// <summary>
        ///     Clears all line previews.
        /// </summary>
        public override void ClearPreview()
        {
            base.ClearPreview();
            if (lastTilemap != null)
            {
                lastTilemap.ClearAllEditorPreviewTiles();
                lastTilemap = null;
            }
        }
    }
}
