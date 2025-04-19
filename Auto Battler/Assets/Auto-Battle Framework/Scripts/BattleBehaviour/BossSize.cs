using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.BattleBehaviour
{
    /// <summary>
    /// Attach it to a character that is supposed to be a boss to resize it.
    /// </summary>
    public class BossSize : MonoBehaviour
    {
        /// <summary>
        /// Scale of the character.
        /// </summary>
        public float Size = 1.25f;

        // Start is called before the first frame update
        void Start()
        {
            gameObject.transform.localScale *= Size;
            enabled = false;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}