using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagersParent : Singleton<ManagersParent>
{

    public List<GameObject> GameObjectsToEnable;

    // Start is called before the first frame update
    void Awake()
    {
        if (CreateSingleton(this, this.SetDontDestroy) == false)
        {
            Destroy(gameObject);
        }
        else
        {
            foreach (GameObject gObj in this.GameObjectsToEnable)
            {
                gObj.SetActive(true);
            }
        }
    }

}
