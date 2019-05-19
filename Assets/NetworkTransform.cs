using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using TMPro.EditorUtilities;
using UnityEngine;

public class NetworkTransform : MonoBehaviour
{
    public Transform t;
    [SerializeField] 
    private int ownedByPlayerId;
    [SerializeField]
    public int networkTransformId;
    private Client client;
    public bool isLocalTransform = false;

    public NetworkTransformUpdate u;
    private ClientGameManager _game;

    private void Awake()
    {
        networkTransformId = 0;
        ownedByPlayerId = 0;
        client = FindObjectOfType<Client>();
        u = new NetworkTransformUpdate();
        u.headerByte = HeaderBytes.NetworkTransFormId;
        _game = FindObjectOfType<ClientGameManager>();
        t = gameObject.transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_game == null)
        {
            Debug.Log("Could not find the Client Game Manager for Network Transform");
        }

        if (client != null)
            StartCoroutine(SendUpdateToServer());
    }


    IEnumerator SendUpdateToServer()
    {
        while (true)
        {
           
            u.locX = t.position.x;
            u.locY = t.position.y;
            u.locZ = t.position.z;
            u.rotX = t.rotation.x;
            u.rotY = t.rotation.y;
            u.rotZ = t.rotation.z;
            u.rotW = t.rotation.w;
            u.networkTransformId = networkTransformId;
            
            client.Send(u);
            
            yield return new WaitForSeconds(client.updateRatePerSecond);
        }
    }
    
    public int GetTransformId()
    {
        return this.networkTransformId;
    }

    public int GetPlayerId()
    {
        return this.ownedByPlayerId;
    }

    public void SetTransformId(int id)
    {
        this.networkTransformId = id;
    }

    public void SetPlayerId(int id)
    {
        this.ownedByPlayerId = id;
    }
}
