﻿using ColossalFramework;
using DaylightClassic.OptionsFramework;
using UnityEngine;

namespace DaylightClassic
{
    public class DayNightCycleMonitor : MonoBehaviour
    {
        private bool _previousFogColorState;
        private bool _initialized;
        private RenderProperties renderProperties;
        private bool cachedNight;

        public void Awake()
        {
            cachedNight = Singleton<SimulationManager>.instance.m_isNightTime;
            SetUpEffects(cachedNight);
        }

        public void Update()
        {
            if (cachedNight == SimulationManager.instance.m_isNightTime && _previousFogColorState == OptionsWrapper<Options>.Options.FogColor && _initialized)
            {
                return;
            }
            cachedNight = SimulationManager.instance.m_isNightTime;
            SetUpEffects(SimulationManager.instance.m_isNightTime);
            _initialized = true;
        }

        public void OnDestroy()
        {
            SetUpEffects(true);
        }

        private void SetUpEffects(bool isNight)
        {
            var behaviors = Camera.main?.GetComponents<MonoBehaviour>();
            if (behaviors == null)
            {
                return;
            }
            foreach (var behavior in behaviors)
            {
                switch (behavior)
                {
                    case FogEffect fe:
                    {
                        fe.enabled = !isNight;
                        if (fe.enabled)
                        {
                            DaylightClassic.ReplaceFogColorImpl(false);
                        }
                        break;
                    }
                    case DayNightFogEffect dnfe:
                    {
                        dnfe.enabled = isNight;
                        if (dnfe.enabled)
                        {
                            DaylightClassic.ReplaceFogColorImpl(OptionsWrapper<Options>.Options.FogColor);
                        }
                        break;
                    }
                }
            }
            _previousFogColorState = OptionsWrapper<Options>.Options.FogColor;
        }
    }
}