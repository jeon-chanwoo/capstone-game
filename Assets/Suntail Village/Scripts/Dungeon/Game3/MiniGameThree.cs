using Suntail;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameThree : MonoBehaviour
{
    private GameObject player;
    private CharacterController characterController;
    private PlayerController playerController;
    private Vector3 stageStartPosition;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        characterController = player.GetComponent<CharacterController>();
        playerController = player.GetComponent<PlayerController>();
        stageStartPosition = new Vector3(2.8f, -4.0f, 68.7f);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerController._maxHP > 20)
            {
                playerController._maxHP -=10.0f ;
                if(playerController._maxHP < playerController._hp)
                {
                    playerController._hp = playerController._maxHP;
                }
            }
            ForceMove();
        }
    }

    private void ForceMove()
    {
        characterController.enabled = false;
        player.transform.position = stageStartPosition;
        characterController.enabled = true;
    }
}
