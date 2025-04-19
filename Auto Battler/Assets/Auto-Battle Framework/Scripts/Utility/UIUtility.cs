using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Utility
{
    public class UIUtility
    {
        /// <summary>
        /// Prevents the panel to go off-screen.
        /// </summary>
        /// <param name="newPos">Position of the panel.</param>
        public static void KeepInsideScreen(RectTransform rect)
        {
            RectTransform CanvasRect = rect.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

            float minX = rect.rect.width * 0.5f;
            float maxX = CanvasRect.sizeDelta.x - rect.rect.width * 0.5f;
            float minY = rect.rect.height * 0.5f;
            float maxY = CanvasRect.sizeDelta.y - rect.rect.height * 0.5f;

            Vector3 newPos = rect.position;
            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
            newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

            rect.position = newPos;
        }
    }
}