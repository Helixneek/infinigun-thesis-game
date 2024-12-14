using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    private GameObject _spawnRoom;
    private Transform _spawnPoint;

    public void SetPlayerSpawn()
    {
        _spawnRoom = GameObject.FindGameObjectWithTag("Start Room");

        _spawnPoint = _spawnRoom.transform;

        transform.position = _spawnPoint.position;
    }
}
