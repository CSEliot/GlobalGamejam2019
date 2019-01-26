using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum PersonalityType
{
    Perfectionist, Hoarder, Nerd, Tacky, Cheap, Expensive, Hermit, Athletic
}


public class PlayerHome : MonoBehaviour
{
    public PersonalityType houseType;
    public List<Room> allRooms = new List<Room>();

    //public List<objItems> needs = new List<objItems>();

    public void DropItemInRoom(int roomInt)
    {

    }
}
