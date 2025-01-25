using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

public class FloatingTextUI : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textLabel;
    public Vector3 textMovementPerSecond;
    public AnimationCurve scaleCurve;
    public Transform followTarget;

    internal FloatingTextHandler HandlerParent;
    internal bool skipScaleCurve;

    private Camera cameraToFollow = null;
    private Vector3 target = new Vector3();
    private float textOnForTime = 0f;
    private float timeToStayAliveFor = 1.5f;
    private Vector3 followOffset = new Vector3();
    private bool coordsAreInScreenSpace = false;


    #region Update functions

    private void Update()
    {
        GetCamera();
        SetPosition();
        SetScale();

        if (this.HandlerParent == null)
        {
            return;
        }

        this.textOnForTime += Time.deltaTime;

        if (this.textOnForTime >= this.timeToStayAliveFor)
        {
            ReturnBackToHandler();
        }
    }

    private void SetPosition()
    {
        if (this.cameraToFollow == null || this.textLabel == null)
        {
            return;
        }

        Vector2 screenPos;

        if (this.followTarget != null)
        {
            if (this.coordsAreInScreenSpace)
            {
                screenPos = this.cameraToFollow.WorldToScreenPoint(this.followTarget.position);
                screenPos += new Vector2(followOffset.x, followOffset.y);
            }
            else
            {
                screenPos = this.cameraToFollow.WorldToScreenPoint(this.followTarget.position + this.followOffset);
            }

            followOffset += this.textMovementPerSecond * Time.deltaTime;
        }
        else
        {
            if (this.coordsAreInScreenSpace)
            {
                screenPos = this.target;
            }
            else
            {
                Vector3 offset;

                //if (DebugTools.UsePerspectiveCamera)
                //{
                    offset = new Vector3(0f, 0f, 0f);
                //}
                //else
                //{
                //    offset = BattleUI.instance.CurrentCameraOffset * 0.25f;
                //}

                screenPos = this.cameraToFollow.WorldToScreenPoint(this.target + offset);
            }

            this.target += this.textMovementPerSecond * Time.deltaTime;
        }

        transform.position = screenPos;
    }

    private void SetScale()
    {
        if (this.skipScaleCurve)
        {
            return;
        }

        if (this.scaleCurve == null)
        {
            return;
        }

        float animValue = this.scaleCurve.Evaluate(this.textOnForTime / timeToStayAliveFor);
        this.transform.localScale = Vector3.one * animValue;
    }

    private void GetCamera()
    {
        if (this.cameraToFollow == null)
        {
            this.cameraToFollow = Camera.main;
        }
    }

    #endregion

    internal void SetDataToThis(FloatingTextData data)
    {
        if (data == null)
        {
            return;
        }

        if (this.textLabel != null)
        {
            this.textLabel.text = data.textToSet;
            this.textLabel.color = data.textColor;
        }

        this.textOnForTime = 0;
        this.timeToStayAliveFor = data.timeToStayActiveFor;
        this.textMovementPerSecond = data.textMovementPerSecond;
        this.skipScaleCurve = data.skipScaleCurve;
        followOffset = Vector3.zero;

        if (data.transformToFollow != null)
        {
            this.followTarget = data.transformToFollow;
        }
        else
        {
            this.followTarget = null;
            this.target = data.positionToSet;
            this.coordsAreInScreenSpace = data.usePosAsScreenCords;
        }

        SetPosition();
        SetScale();
    }

    internal void SetTextAndColor(string text, Color color, Transform targetToFollow)
    {
        if (this.textLabel != null)
        {
            this.textLabel.text = text;
            this.textLabel.color = color;
            this.textOnForTime = 0;
            this.followTarget = targetToFollow;
        }
    }

    internal void ReturnBackToHandler()
    {
        if (this.HandlerParent != null)
        {
            this.HandlerParent.ReturnFloatingTextBackToPool(this);
        }
    }

    internal void ShowTextLabel(bool show)
    {
        if (textLabel)
        {
            textLabel.gameObject.SetActive(show);
        }
    }

}