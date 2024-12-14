using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorTriggerInteraction : TriggerInteractionBase
{
    public int currentRoomID;
    public int nextRoomID;

    private List<DoorTriggerInteraction> otherDoors;

    private void Start()
    {
        otherDoors = FindObjectsByType<DoorTriggerInteraction>(FindObjectsSortMode.None).ToList();
    }

    public override void Interact()
    {
        // Teleport to the door with:
        // 1. Same currentRoomID as this door's nextRoomID;
        // 2. Same nextRoomID as this door's currentRoomID;
        // Basically to make sure that you can go back and forth between these rooms.
        DoorTriggerInteraction target = otherDoors.Find(x => x.nextRoomID == this.currentRoomID && x.currentRoomID == this.nextRoomID);

        if (target != null)
        {
            Debug.Log("[Interact] Teleporting player to " + target.name);
            Player.transform.position = target.transform.position;
        }
    }
}
