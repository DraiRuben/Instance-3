using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance;
    private CinemachineImpulseDefinition _ImpulseDefinition;
    private CinemachineImpulseListener _ImpulseListener;
    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;

        _ImpulseListener = GetComponent<CinemachineImpulseListener>();
    }
    public void ShakeCamera(CinemachineImpulseSource impulseSource, CameraShakeProfile profile)
    {
        SetupShakeParameters(impulseSource, profile);
        impulseSource.GenerateImpulseWithForce(profile.impactForce);
    }
    private void SetupShakeParameters(CinemachineImpulseSource impulseSource, CameraShakeProfile profile)
    {
        _ImpulseDefinition = impulseSource.m_ImpulseDefinition;

        _ImpulseDefinition.m_ImpulseDuration = profile.impactTime;
        impulseSource.m_DefaultVelocity = profile.defaultVelocity;
        _ImpulseDefinition.m_CustomImpulseShape = profile.impulseCurve;

        _ImpulseListener.m_ReactionSettings.m_AmplitudeGain = profile.listenerAmplitude;
        _ImpulseListener.m_ReactionSettings.m_FrequencyGain = profile.listenerFrequency;
        _ImpulseListener.m_ReactionSettings.m_Duration = profile.listenerDuration;
    }
}
