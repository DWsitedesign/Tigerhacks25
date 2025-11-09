using UnityEngine;

public class RoomManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float RoomDistance = 30f;
    void Start()
    {
        // loop through children and set active to false unless inside 30f of player
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        foreach (Transform child in transform)
        {
            float distance = Vector3.Distance(player.position, child.position);
            if (distance > RoomDistance)
            {
                child.gameObject.SetActive(false);
            } else
            {
                child.gameObject.SetActive(true);
            }
        }

    }

    public void CheckRooms()
    {
        // loop through children and set active to false unless inside 30f of player
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        foreach (Transform child in transform)
        {
            float distance = Vector3.Distance(player.position, child.position);
            if (distance > RoomDistance)
            {
                child.gameObject.SetActive(false);
            }
            else
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}
