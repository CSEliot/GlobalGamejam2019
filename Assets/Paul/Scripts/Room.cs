using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotSpace
{
    public Transform anchorSpot;
}


[System.Serializable]
public class Room : MonoBehaviour
{
    public List<SlotSpace> slots = new List<SlotSpace>();


    public void TakeThisObject()
    {

    }
}

