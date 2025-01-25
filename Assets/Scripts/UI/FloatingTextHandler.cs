using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextHandler : MonoBehaviour
{
    public List<FloatingTextUI> UsedFloatingTexts = new List<FloatingTextUI>();
    public FloatingTextData DefaultTextSettings;

    private List<FloatingTextData> textsInQueue = new List<FloatingTextData>();
    private List<FloatingTextUI> activeFloatingTexts = new List<FloatingTextUI>();
    private List<FloatingTextUI> waitingFloatingTexts = new List<FloatingTextUI>();

    private void Start()
    {
        foreach (FloatingTextUI floatText in this.UsedFloatingTexts)
        {
            if (floatText == null)
            {
                continue;
            }

            floatText.HandlerParent = this;
            ReturnFloatingTextBackToPool(floatText);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        HandleQueue();
    }

    private void HandleQueue()
    {
        if (this.textsInQueue == null)
        {
            this.textsInQueue = new List<FloatingTextData>();
        }

        if (this.waitingFloatingTexts == null)
        {
            this.waitingFloatingTexts = new List<FloatingTextUI>();
        }

        if (this.activeFloatingTexts == null)
        {
            this.activeFloatingTexts = new List<FloatingTextUI>();
        }

        if (this.textsInQueue.Count > 0 && this.waitingFloatingTexts.Count > 0)
        {
            int textsActivated = 0;

            List<FloatingTextData> waitingForActivation = new List<FloatingTextData>();

            for (int i = 0; i < this.textsInQueue.Count; i++)
            {
                FloatingTextData data = this.textsInQueue[i];

                if (data == null)
                {
                    continue;
                }

                if (data.textDelay > 0)
                {
                    data.textDelay -= Time.deltaTime;
                }

                if (data.textDelay <= 0)
                {
                    waitingForActivation.Add(data);
                }
            }

            for (int i = 0; i < waitingForActivation.Count && i < this.waitingFloatingTexts.Count; i++)
            {
                FloatingTextUI textUI = this.waitingFloatingTexts[i];

                if (textUI == null)
                {
                    this.waitingFloatingTexts.RemoveAt(i);
                    continue;
                }

                textUI.SetDataToThis(waitingForActivation[i]);
                textsActivated++;
            }

            for (int i = 0; i < textsActivated; i++)
            {
                FloatingTextUI textUI = this.waitingFloatingTexts[0];

                this.textsInQueue.Remove(waitingForActivation[i]);
                this.waitingFloatingTexts.Remove(textUI);
                this.activeFloatingTexts.Add(textUI);
                textUI.gameObject.SetActive(true);
            }
        }
    }

    internal void AddNewTextToQueue(FloatingTextData newTextData)
    {
        if (this.textsInQueue == null)
        {
            this.textsInQueue = new List<FloatingTextData>();
        }

        this.textsInQueue.Add(newTextData);
    }

    internal void ReturnFloatingTextBackToPool(FloatingTextUI returnedTextUI)
    {
        if (this.activeFloatingTexts.Contains(returnedTextUI))
        {
            this.activeFloatingTexts.Remove(returnedTextUI);
        }

        if (this.waitingFloatingTexts.Contains(returnedTextUI) == false)
        {
            this.waitingFloatingTexts.Add(returnedTextUI);
        }

        if (returnedTextUI != null)
        {
            returnedTextUI.gameObject.SetActive(false);
        }
    }

    internal void ShowTextWithDefaultSettings(Vector3 textPosition, string text)
    {
        FloatingTextData newTextData = new FloatingTextData()
        {
            positionToSet = textPosition,
            textColor = this.DefaultTextSettings.textColor,
            textMovementPerSecond = this.DefaultTextSettings.textMovementPerSecond,
            textToSet = text,
            timeToStayActiveFor = this.DefaultTextSettings.timeToStayActiveFor
        };

        AddNewTextToQueue(newTextData);
    }

    internal void ShowTextWithDefaultSettings(Transform followTransform, string text)
    {
        FloatingTextData newTextData = new FloatingTextData()
        {
            transformToFollow = followTransform,
            textColor = this.DefaultTextSettings.textColor,
            textMovementPerSecond = this.DefaultTextSettings.textMovementPerSecond,
            textToSet = text,
            timeToStayActiveFor = this.DefaultTextSettings.timeToStayActiveFor
        };

        AddNewTextToQueue(newTextData);
    }

    internal FloatingTextData GetDefaultSettingsCopy()
    {
        FloatingTextData newTextData = new FloatingTextData()
        {
            textColor = this.DefaultTextSettings.textColor,
            textMovementPerSecond = this.DefaultTextSettings.textMovementPerSecond,
            timeToStayActiveFor = this.DefaultTextSettings.timeToStayActiveFor
        };

        return newTextData;
    }

}

[System.Serializable]
public class FloatingTextData
{
    internal Vector3 positionToSet;
    internal Transform transformToFollow;
    internal string textToSet;

    public float textDelay = 0;
    public Vector3 textMovementPerSecond;
    public Color textColor;
    public float timeToStayActiveFor;
    public bool skipScaleCurve;
    public bool usePosAsScreenCords = false;

}