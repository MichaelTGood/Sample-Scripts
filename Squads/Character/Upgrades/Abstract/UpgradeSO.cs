using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Squads.CharacterElements
{
    [CreateAssetMenu(menuName = "Squads/Upgrade", fileName = "New Upgrade")]
    public class UpgradeSO : SerializedScriptableObject
    {
        #region Variables

            public string UpgradeName;

            [TextArea]
            public string Description;

            public UpgradeTypes UpgradeType;

            [Space(25)]

            // [SerializeReference] // For use without OdinInspector
            [TypeFilter(nameof(GetFilteredUpgradeList))]
            public List<Upgrade> upgradeList = new List<Upgrade>();




        #endregion
        
        
        public IEnumerable<Type> GetFilteredUpgradeList()
        {
            var upgradeList = typeof(Upgrade).Assembly.GetTypes()
                .Where(x => !x.IsAbstract)
                .Where(x => !x.IsGenericTypeDefinition)
                .Where(x => typeof(Upgrade).IsAssignableFrom(x))
                .Where(x => !typeof(MonoBehaviour).IsAssignableFrom(x));

            return upgradeList;
        }



    }



}
