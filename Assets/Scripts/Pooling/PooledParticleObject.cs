using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PooledParticleObject : PooledObject
{
    [FormerlySerializedAs("particleSystemsToStop")]
    public List<ParticleSystem> particleSystemsToUse = new List<ParticleSystem>();
    public PooledParticleStyle pooledParticleStyle = PooledParticleStyle.ReturnAfterReceivingCallAndAllParticlesHaveStopped;

    private Vector3 originalScale = Vector3.one;

    protected override void OnEnable()
    {
        originalScale = transform.localScale;
        base.OnEnable();
    }

    public void ScaleUp(float amount)
    {
        transform.localScale *= amount;
    }

    public override void SetPoolSettings(PoolObjectSettings setPoolObjectSettings)
    {
        base.SetPoolSettings(setPoolObjectSettings);

        if (this.pooledParticleStyle == PooledParticleStyle.ReturnAutomaticallyIfAllParticlesHaveStopped)
        {
            StartCoroutine(returnToPoolWhenAllParticlesHaveEnded());
        }
    }

    public override void ReturnPooledObjectBackToPool()
    {
        transform.localScale = originalScale;
        if (this.particleSystemsToUse == null)
        {
            base.ReturnPooledObjectBackToPool();
        }
        else if (this.particleSystemsToUse.Count == 0)
        {
            base.ReturnPooledObjectBackToPool();
        }
        else
        {
            switch (this.pooledParticleStyle)
            {
                case PooledParticleStyle.ReturnAfterReceivingCallAndAllParticlesHaveStopped:
                    StartCoroutine(stopParticlesAndThenReturnToPool());
                    break;
                case PooledParticleStyle.ReturnAutomaticallyIfAllParticlesHaveStopped:
                    base.ReturnPooledObjectBackToPool();
                    break;
                case PooledParticleStyle.ReturnAfterReceivingCallAndStopParticles:
                    StartCoroutine(stopParticlesAndThenReturnToPool());
                    break;
            }
        }
    }

    private IEnumerator stopParticlesAndThenReturnToPool()
    {
        if (this.particleSystemsToUse == null)
        {
            base.ReturnPooledObjectBackToPool();
            yield break;
        }

        foreach (ParticleSystem particle in this.particleSystemsToUse)
        {
            if (particle != null)
            {
                particle.Stop();
                particle.Clear(true);
            }
        }

        yield return null;

        StartCoroutine(returnToPoolWhenAllParticlesHaveEnded());
    }

    private IEnumerator returnToPoolWhenAllParticlesHaveEnded()
    {
        if (this.particleSystemsToUse == null)
        {
            base.ReturnPooledObjectBackToPool();
            yield break;
        }

        List<ParticleSystem> waitingOnTheseSystems = new List<ParticleSystem>(this.particleSystemsToUse);
        if (pooledParticleStyle != PooledParticleStyle.ReturnAfterReceivingCallAndStopParticles)
        {
            while (waitingOnTheseSystems.Count > 0)
            {
                for (int i = waitingOnTheseSystems.Count - 1; i >= 0; i--)
                {
                    ParticleSystem waitingOnThis = waitingOnTheseSystems[i];

                    if (waitingOnThis != null)
                    {
                        if (!waitingOnThis.IsAlive())
                        {
                            waitingOnTheseSystems.RemoveAt(i);
                        }
                    }
                    else
                    {
                        waitingOnTheseSystems.RemoveAt(i);
                    }
                }

                yield return null;
            }
        }
        base.ReturnPooledObjectBackToPool();
    }

}

public enum PooledParticleStyle
{
    ReturnAfterReceivingCallAndAllParticlesHaveStopped,
    ReturnAutomaticallyIfAllParticlesHaveStopped,
    ReturnAfterReceivingCallAndStopParticles
}