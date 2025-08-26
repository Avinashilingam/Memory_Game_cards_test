using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayoutHandler : MonoBehaviour
{
    public GridLayoutGroup gridLayoutGroup;
    public RectTransform cardParent;

    private int rows;
    private int columns;
    private float spacing = 10f;

    public void SetLayout(int rows, int columns)
    {
        this.rows = rows;
        this.columns = columns;

        float parentWidth = cardParent.rect.width;
        float parentHeight = cardParent.rect.height;

        float totalSpacingX = spacing * (columns - 1);
        float totalSpacingY = spacing * (rows - 1);

        float cellWidth = (parentWidth - totalSpacingX) / columns;
        float cellHeight = (parentHeight - totalSpacingY) / rows;

        gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
        gridLayoutGroup.spacing = new Vector2(spacing, spacing);
    }
}
