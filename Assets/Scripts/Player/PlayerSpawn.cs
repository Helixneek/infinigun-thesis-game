using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageName;

    private GameObject _spawnRoom;
    private Transform _spawnPoint;
    public void SetPlayerSpawn()
    {
        // Im too lazy to put this anywhere else
        // so just do it here
        stageName.text = SceneManager.GetActiveScene().name;

        // Actually put the player in the start room
        _spawnRoom = GameObject.FindGameObjectWithTag("Start Room");

        _spawnPoint = _spawnRoom.transform;

        transform.position = _spawnPoint.position;
    }
}
