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

        public Counter(int p_target, Action p_onReached, bool p_repeat = true)
        {
            this.m_target = p_target;
            this.m_repeat = p_repeat;
            OnReached += p_onReached;
        }

        public void Add(int p_toAdd = 1)
        {
            m_currentCount += p_toAdd;
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