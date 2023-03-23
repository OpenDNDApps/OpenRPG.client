using System;
using UnityEngine;

namespace VGDevs
{
    public class Counter
    {
        private readonly int m_target = 0;
        private int m_currentCount = 0;
        private readonly bool m_repeat;

        private event Action OnReached;

        public Counter(int target, Action onReached, bool repeat = true)
        {
            m_target = target;
            m_repeat = repeat;
            OnReached += onReached;
        }

        public void Add(int toAdd = 1)
        {
            m_currentCount += toAdd;
            if (m_currentCount >= m_target)
            {
                OnReached?.Invoke();
            }

            if (m_repeat)
            {
                m_currentCount = (int) Mathf.Repeat(m_currentCount, m_target);
            }
        }
    }
}