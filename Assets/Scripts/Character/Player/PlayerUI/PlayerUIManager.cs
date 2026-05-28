using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    private static PlayerUIManager Instance { get; set; }

    [Header("Network Join")]
    [SerializeField] bool StartGameAsClient;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if(StartGameAsClient)
        {
            StartGameAsClient = false;
            //If restarting as client must shutdown first since we started the game as host
            NetworkManager.Singleton.Shutdown();
            NetworkManager.Singleton.StartClient();
        }
    }
}
