using System.Collections;
using UnityEngine;
using VGDevs;

namespace ORC
{
    public class GameScene : OrcMono
    {
        [SerializeField] private BoardUI m_boardUI;

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => GameResources.Runtime.IsReady);
            yield return new WaitUntil(() => GameResources.UIRuntime.IsReady);

            if(UIRuntime.TryShowWindow("BoardUI", out m_boardUI))
            {
                m_boardUI.AnimatedShow();
            }
        }
    }
}