using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AutoBattleFramework.Battlefield
{
    /// <summary>
    /// Selects the color of the sprite withing the cell depending on whether it has a character in it or not, or if a character is being dragged over it.
    /// </summary>
    public class GridCellEffect : MonoBehaviour
    {
        /// <summary>
        /// Color of the cell when there is no character inside it.
        /// </summary>
        public Color Empty;

        /// <summary>
        /// Color of the cell when there is a character inside it.
        /// </summary>
        public Color NotEmpty;

        /// <summary>
        /// Color of the cell when there is a character being dragged over it.
        /// </summary>
        public Color DragOver;

        /// <summary>
        /// If there is a character being dragger over the cell.
        /// </summary>
        public bool CharacterDrag = false;

        GridCell gridCell;
        SpriteRenderer spriteRenderer;
        NavMeshObstacle obstacle;

        // Start is called before the first frame update
        void Start()
        {
            gridCell = GetComponent<GridCell>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            if (CharacterDrag)
            {
                spriteRenderer.color = DragOver;
            }
            else if (gridCell.shopItem)
            {
                spriteRenderer.color = NotEmpty;
            }
            else
            {
                spriteRenderer.color = Empty;
            }
        }

        /// <summary>
        /// Set the color of the grid cell.
        /// </summary>
        /// <param name="color">Selected color of the sprite.</param>
        public void SetColor(Color color)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.color = color;
        }


        /// <summary>
        /// Update colors of the cell
        /// </summary>
        [ContextMenu("Update Colors")]
        void DoSomething()
        {
            GetComponentInChildren<SpriteRenderer>().color = Empty;
        }
    }
}