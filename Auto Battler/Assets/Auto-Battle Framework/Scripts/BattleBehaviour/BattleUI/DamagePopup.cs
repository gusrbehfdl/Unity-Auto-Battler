using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace AutoBattleFramework.BattleUI
{
    /// <summary>
    /// Displays a text when a character gets damaged.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class DamagePopup : MonoBehaviour
    {
        /// <summary>
        /// Text to display
        /// </summary>
        [HideInInspector]
        public string displayText;

        /// <summary>
        /// Set as invisible
        /// </summary>
        public bool Invisible = false;

        // Start is called before the first frame update
        void Start()
        {
            TMP_Text tmp_text = GetComponent<TMP_Text>();
            tmp_text.text = displayText;
            Destroy(gameObject, 0.7f);
        }

        private void Update()
        {
            Vector3 pos = transform.position;
            pos.y += 0.0005f;
            transform.position = pos;
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        }

        /// <summary>
        /// Set the color of the text.
        /// </summary>
        /// <param name="color">Color of displayed text.</param>
        public void Setcolor(Color color)
        {
            TMP_Text tmp_text = GetComponent<TMP_Text>();
            tmp_text.color = color;
            if (Invisible)
            {
                tmp_text.color = new Color(0, 0, 0, 0);
            }
        }
    }
}
