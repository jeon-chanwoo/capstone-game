using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonTeleportation : MonoBehaviour
{
    //던전이동
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") 
        {
            SceneManager.LoadScene("Dungeon");
        }
    }
}
