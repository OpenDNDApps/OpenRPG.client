using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ORC;

namespace VGDevs
{
    public partial class GameUIResourcesCollection
    {
        [Space(4f)]
        [Header("ORC Resources")]
        public PageStyle DefaultPageStyle;
        public List<PageStyle> PageStyles = new List<PageStyle>();
		
        public bool TryGetPageStyle<T>(string itemName, out T uiWindow) where T : PageStyle
        {
            uiWindow = null;
            if (string.IsNullOrEmpty(itemName)) 
                return false;
            
            foreach (PageStyle window in PageStyles)
            {
                if (!window.name.Equals(itemName)) continue;
				
                uiWindow = window as T;
                return true;
            }
            return false;
        }
    }
}