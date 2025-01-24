using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    internal string poolTypeString;
    internal GameObject poolBasePrefab;
    [Tooltip("This setting overrides anything set via the scripts, if this value is over 0.")]
    public float backToPoolTimeOverride = 0f;

    protected float returnBackToPoolTime = 0f;
    protected PoolObjectSettings poolObjectSettings;
    protected List<IPooledResettable> poolResettables;
    protected bool returningToPool = false;

    private void Awake()
    {
        if (this.poolResettables == null)
        {
            this.poolResettables = new List<IPooledResettable>(gameObject.GetComponentsInChildren<IPooledResettable>());
        }
    }

    protected virtual void OnEnable()
    {
        this.returningToPool = false;

        if (this.backToPoolTimeOverride > 0)
        {
            this.returnBackToPoolTime = this.backToPoolTimeOverride;
            StartCoroutine(TimedReturnThisBackToThePool(this.returnBackToPoolTime));
        }
    }

    protected virtual void Update()
    {
        if (this.poolObjectSettings == null)
            return;

        if (this.poolObjectSettings.transformToFollow != null)
        {
            transform.position = this.poolObjectSettings.transformToFollow.position;
        }
    }

    public virtual void SetPoolSettings(PoolObjectSettings setPoolObjectSettings)
    {
        this.poolObjectSettings = setPoolObjectSettings;

        if (this.poolResettables == null)
        {
            this.poolResettables = new List<IPooledResettable>(gameObject.GetComponentsInChildren<IPooledResettable>());
        }

        if (this.poolObjectSettings != null)
        {
            SetReturnToPoolTime(this.poolObjectSettings.timeBeforeReturningToPool);
        }
    }

    public virtual void SetReturnToPoolTime(float setTo)
    {
        if (this.backToPoolTimeOverride > 0)
        {
            this.returnBackToPoolTime = this.backToPoolTimeOverride;
        }
        else
        {
            this.returnBackToPoolTime = setTo;
        }

        if (this.returnBackToPoolTime > 0)
        {
            StartCoroutine(TimedReturnThisBackToThePool(this.returnBackToPoolTime));
        }
    }

    public virtual void ReturnPooledObjectBackToPool()
    {
        if (PoolManager.Instance != null)
        {
            if (this.poolResettables != null)
            {
                foreach (IPooledResettable resettable in this.poolResettables)
                {
                    if (resettable != null)
                    {
                        resettable.ResetSettings();
                    }
                }
            }

            if (string.IsNullOrEmpty(this.poolTypeString) == false)
            {
                if (PoolManager.Instance.ReturnObjectToPool(this.poolTypeString, gameObject) == false)
                {
                    Destroy(gameObject);
                }
            }
            else if (this.poolBasePrefab != null)
            {
                if (PoolManager.Instance.ReturnObjectToPool(this.poolBasePrefab, gameObject) == false)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public virtual void TimedReturnObjectToPool(float timeToReturnIt)
    {
        if (this.returningToPool)
        {
            return;
        }

        StartCoroutine(TimedReturnThisBackToThePool(timeToReturnIt));
    }

    private IEnumerator TimedReturnThisBackToThePool(float timeToReturn)
    {
        returningToPool = true;
        yield return new WaitForSeconds(timeToReturn);
        ReturnPooledObjectBackToPool();
    }

}

public interface IPooledResettable
{
    void ResetSettings();
}