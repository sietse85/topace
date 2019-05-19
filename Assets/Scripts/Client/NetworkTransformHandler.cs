using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using Network;
using UnityEngine;

public class NetworkTransformHandler : MonoBehaviour
{
    // Start is called before the first frame update
    private Client _client;
    private ClientGameManager _game;
    private NetworkTransformUpdate nt;
    private Vector3 loc;
    private Quaternion rot;
    private NetworkTransformsForVehicle vnt;
   
    private void Start()
    {
       _client = GetComponent<Client>();
       _game = GetComponent<ClientGameManager>();
       nt = new NetworkTransformUpdate();
       loc = new Vector3();
       rot = new Quaternion();
       vnt = new NetworkTransformsForVehicle();
       vnt.headerByte = HeaderBytes.NetworkTransFormsForVehicle;
    }

    public void UpdateNetworkTransform(NetPacketReader r)
    {
        nt.Deserialize(r);

        if (_game.networkTransforms.ContainsKey(nt.networkTransformId))
        {
            loc.x = nt.locX;
            loc.y = nt.locY;
            loc.z = nt.locZ;
            rot.x = nt.rotX;
            rot.y = nt.rotY;
            rot.z = nt.rotZ;
            rot.w = nt.rotW;
            
            _game.networkTransforms[nt.networkTransformId].SetPositionAndRotation(loc, rot);
        }
    }

    public void SetTransformIds(NetPacketReader r)
    {
        Debug.Log("set transform ids");
        vnt.Deserialize(r);

        NetworkTransform[] transforms = _game._vehicles[vnt.vehicleId].GetComponentsInChildren<NetworkTransform>();
        
        int i = 0;
        
        foreach (NetworkTransform t in transforms)
        {
            t.SetPlayerId(vnt.playerId);
            t.SetTransformId(vnt.networkTransformIds[i]);
            i++;
        }
    }
}
