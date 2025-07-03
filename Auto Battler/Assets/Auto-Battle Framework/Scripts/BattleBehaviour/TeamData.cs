using AutoBattleFramework.BattleBehaviour.GameActors;
using System.Collections.Generic;

namespace AutoBattleFramework.BattleBehaviour {

    /// <summary>
    /// Information about the characters in a player's field. 
    /// </summary>
    [System.Serializable]
    public class TeamData
    {
        /// <summary>
        /// List of the characters in a player's field. 
        /// </summary>
        public List<GameCharacter> team;

    }
}