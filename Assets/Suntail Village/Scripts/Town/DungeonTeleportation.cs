using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonTeleportation : MonoBehaviour
{
    public string transferDungeon;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") 
        {
            SceneManager.LoadScene("Dungeon");
        }
    }
}
