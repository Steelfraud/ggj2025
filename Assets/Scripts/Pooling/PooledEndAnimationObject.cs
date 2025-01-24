using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledEndAnimationObject : PooledObject
{

    public Animation LinkedAnimator;
    public AnimationClip EndClip;
    public Animator AnimatorToSet;
    public string TriggerToSet;
    public float AnimationLength;

    public override void ReturnPooledObjectBackToPool()
    {
        if (this.LinkedAnimator == null)
        {
            base.ReturnPooledObjectBackToPool();
        }
        else
        {
            StartCoroutine(ReturnAfterAnimation());
        }
    }

    private IEnumerator ReturnAfterAnimation()
    {
        this.returningToPool = true;

        if (this.LinkedAnimator != null)
        {
            LinkedAnimator.Play();
        }

        if (this.AnimatorToSet != null && string.IsNullOrEmpty(this.TriggerToSet) == false)
        {
            this.AnimatorToSet.SetTrigger(this.TriggerToSet);
        }

        if (this.EndClip != null)
        {
            if (this.EndClip.length >= this.AnimationLength)
            {
                yield return new WaitForSeconds(this.EndClip.length);
            }
            else
            {
                yield return new WaitForSeconds(this.AnimationLength);
            }
        }
        else
        {
            yield return new WaitForSeconds(this.AnimationLength);
        }

        base.ReturnPooledObjectBackToPool();
    }

}