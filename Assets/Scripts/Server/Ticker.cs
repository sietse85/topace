using System;
using System.Collections;
using System.Diagnostics;
using LiteNetLib;
using Network;
using UnityEngine;
using Vehicle;
using Debug = UnityEngine.Debug;

namespace Server
{
    public class Ticker : MonoBehaviour
    {
        //buffers to fill with various data
        private byte[] _snapshot;
        
        private FireCommand[] _fireCommands;
        private CollisionReport[] _reportedCollisions;
        private ReportCollision _reportCollision;
        private int _reportCollisionIndex;
        private FireWeapon _firePacket;
        private int _fireCommandIndex;
        private Stopwatch s;
        public byte tickNr;
        private int index;
        public NetworkTransformHistory[] transformHistory;
        
        public NetworkTransformStruct[] networkTransforms;
        
        private void Awake()
        {
            _snapshot = new byte[65000];
            _firePacket = new FireWeapon();
            _reportCollision = new ReportCollision();
            _fireCommands = new FireCommand[1024];
            _reportedCollisions = new CollisionReport[1024];
            s = new Stopwatch();
            networkTransforms = new NetworkTransformStruct[1024];
        }

        private void Start()
        {
            GameServer.instance.updateSpeed = 1f / GameServer.instance.ticksPerSecond;
            transformHistory = new NetworkTransformHistory[GameServer.instance.maxPlayers * GameServer.instance.ticksPerSecond];
            StartTicking();
        }

        public void StartTicking()
        {
            StartCoroutine(Tick());
        }

        public IEnumerator Tick()
        {
            while (true)
            {
                s.Start();
                s.Restart();

                //indicate a snapshot packet
                index = 0;
                _snapshot[0] = HeaderBytes.IncomingSnapShot;
                _snapshot[1] = tickNr;
                index +=2;

                

                //process the gamestate and add info to snapshot
                RecordNetworkTransformHistory();
                ProcessCollisions();
                AddVehicleInfoToSnapShot();
                ProcessFireCommands();
                AddNetworkTransformsToSnapshot();
                AddPlayerInfoToSnapShot();
                
                s.Stop();
                
                //send the snapshot to all connected clients
                GameServer.instance.SendBytesToAll(_snapshot, index);
                tickNr++;
                if (tickNr == GameServer.instance.ticksPerSecond)
                {
                    Debug.Log("The tick took " + (s.ElapsedTicks /10000f).ToString("#.000") + " ms to process");
                    tickNr = 0;
                }

                yield return new WaitForSeconds(GameServer.instance.updateSpeed);
            }
        }

        public void RecordNetworkTransformHistory()
        {
            for (int i = 0; i < GameManager.instance.vehicleEntities.Length; i++)
            {
                VehicleEntity v = GameManager.instance.vehicleEntities[i];
                if (!v.processInTick)
                {
                    continue;
                }
                transformHistory[i * GameServer.instance.maxPlayers + tickNr].v = v.obj.transform.position;
                transformHistory[i * GameServer.instance.maxPlayers + tickNr].q = v.obj.transform.rotation;
            }
        }

        private void ProcessCollisions()
        {
            _reportCollisionIndex = 0;
            for (int i = 0; i < _reportedCollisions.Length; i++)
            {
                if (!_reportedCollisions[i].Process)
                    break;

               
                if (!GameManager.instance.vehicleEntities[_reportedCollisions[i].PlayerIdThatWasHit].processInTick)
                {
                    _reportedCollisions[i].Process = false;
                    continue;
                }
                
                Debug.Log("Processing collision");

                VehicleEntity e = GameManager.instance.vehicleEntities[_reportedCollisions[i].PlayerIdThatWasHit];
                
                Vector3 oldPos = e.obj.transform.position;
                Quaternion oldRot = e.obj.transform.rotation; 

                e.obj.transform.position =
                    transformHistory[
                        _reportedCollisions[i].PlayerIdThatWasHit * GameServer.instance.maxPlayers +
                        _reportedCollisions[i].TickWhenCollisionOccurred
                        ].v;
                
                e.obj.transform.rotation =
                    transformHistory[
                        _reportedCollisions[i].PlayerIdThatWasHit * GameServer.instance.maxPlayers +
                        _reportedCollisions[i].TickWhenCollisionOccurred
                    ].q;
                
                // do raycast from referenced projectile
                
                ProjectileReference r = GameManager.instance.projectiles[
                    _reportedCollisions[i].PlayerIdThatShotThisProjectile * 100 +
                    _reportedCollisions[i].UniqueProjectileId
                ];

                if (!r.active)
                {
                    _reportedCollisions[i].Process = false;
                    continue;
                }
                

                r.entity.transform.position = _reportedCollisions[i].impactPos;
                int? playerId = r.entity.DoRayCast(false);

                if (playerId != null)
                {
                    //process damage and flash players hitmarker
                }

                //put vehicle that was checked back to old position
                e.obj.transform.position = oldPos;
                e.obj.transform.rotation = oldRot;
                
                _reportedCollisions[i].Process = false;
            }
        }

        private void ProcessFireCommands()
        {
            _fireCommandIndex = 0;
            for (int i = 0; i < _fireCommands.Length; i++)
            {
                if (!_fireCommands[i].Process)
                {
                    break;
                }
                
                _snapshot[index] = HeaderBytes.FireWeapon;
                index++;
                
                _fireCommands[i].Process = false;

                if (!GameManager.instance.turrets[_fireCommands[i].VehicleId][_fireCommands[i].WeaponSlotFired].inCoolDown)
                {
                    Vector3 pos =
                        GameManager.instance.turrets[_fireCommands[i].VehicleId][_fireCommands[i].WeaponSlotFired]
                            .gameObject.transform.position;
                    
                    Quaternion rot =
                        GameManager.instance.turrets[_fireCommands[i].VehicleId][_fireCommands[i].WeaponSlotFired]
                            .gameObject.transform.rotation;

                    FireCommand f = _fireCommands[i];
                    
                    Buffer.BlockCopy(ByteHelper.instance.IntToByte(f.ProjectileId), 0,  _snapshot, index, sizeof(int));
                    index += sizeof(int);
                    _snapshot[index] = f.VehicleId;
                    index += sizeof(byte);
                    _snapshot[index] = f.UniqueProjectileId;
                    index += sizeof(byte);
                    Buffer.BlockCopy(ByteHelper.instance.Vector3ToByte(pos), 0, _snapshot, index, sizeof(float) * 3);
                    index += sizeof(float) * 3;
                    Buffer.BlockCopy(ByteHelper.instance.QuaternionToByte(rot), 0, _snapshot, index, sizeof(float) * 4);
                    index += sizeof(float) * 4;

                    f.Process = false;
                    
                    GameManager.instance.turrets[_fireCommands[i].VehicleId][_fireCommands[i].WeaponSlotFired].Fire(_fireCommands[i].Rotation);
                }
            }
        }

        private void AddNetworkTransformsToSnapshot()
        {
            for(int i = 0; i < networkTransforms.Length; i++)
            {
                if (!networkTransforms[i].processInTick)
                    continue;
                
                _snapshot[index] = HeaderBytes.NetworkTransFormId;
                index += sizeof(byte);
                Buffer.BlockCopy(ByteHelper.instance.Vector3ToByte(networkTransforms[i].transform.position), 0, _snapshot, index, sizeof(float) * 3);
                index += sizeof(float) * 3;
                Buffer.BlockCopy(ByteHelper.instance.QuaternionToByte(networkTransforms[i].transform.rotation), 0, _snapshot, index, sizeof(float) * 4);
                index += sizeof(float) * 4;
                Buffer.BlockCopy(ByteHelper.instance.IntToByte(i), 0, _snapshot, index, sizeof(int));
                index += sizeof(int);
                _snapshot[index] = networkTransforms[i].playerId;
                index += sizeof(byte);
            }
        }

        public void AddVehicleInfoToSnapShot()
        {
            
            for (int i = 0; i < GameManager.instance.vehicleEntities.Length; i++)
            {
                VehicleEntity entity = GameManager.instance.vehicleEntities[i];
                if (!entity.processInTick)
                    continue;
                
                _snapshot[index] = HeaderBytes.SendVehicleData;
                index += sizeof(byte);
                _snapshot[index] = (byte) i;
                index += sizeof(byte);
                Buffer.BlockCopy(ByteHelper.instance.FloatToByte(entity.currentArmor), 0, _snapshot, index, sizeof(float));
                index += sizeof(float);
                Buffer.BlockCopy(ByteHelper.instance.FloatToByte(entity.currentHealth), 0, _snapshot, index, sizeof(float));
                index += sizeof(float);
                Buffer.BlockCopy(ByteHelper.instance.FloatToByte(entity.currentShield), 0, _snapshot, index, sizeof(float));
                index += sizeof(float);
                Buffer.BlockCopy(ByteHelper.instance.FloatToByte(entity.battery), 0, _snapshot, index, sizeof(float));
                index += sizeof(float);
            }
        }

        public void AddPlayerInfoToSnapShot()
        {
            for (int i = 0; i < GameManager.instance.players.Length; i++)
            {
                if(!GameManager.instance.players[i].processInTick)
                    continue;
                
                _snapshot[index] = HeaderBytes.SendPlayerData;
                index += sizeof(byte);
                _snapshot[index] = (byte) i;
                index += sizeof(byte);
                Buffer.BlockCopy(ByteHelper.instance.IntToByte(GameManager.instance.players[i].latency), 0, _snapshot, index, sizeof(int));
                index += sizeof(int);
                Buffer.BlockCopy(ByteHelper.instance.IntToByte(GameManager.instance.players[i].score), 0, _snapshot, index, sizeof(int));
                index += sizeof(int);
                Buffer.BlockCopy(ByteHelper.instance.IntToByte(GameManager.instance.players[i].kills), 0, _snapshot, index, sizeof(int));
                index += sizeof(int);
                Buffer.BlockCopy(ByteHelper.instance.IntToByte(GameManager.instance.players[i].deaths), 0, _snapshot, index, sizeof(int));
                index += sizeof(int);
                Buffer.BlockCopy(ByteHelper.instance.IntToByte(GameManager.instance.players[i].shotsFired), 0, _snapshot, index, sizeof(int));
                index += sizeof(int);
                Buffer.BlockCopy(ByteHelper.instance.IntToByte(GameManager.instance.players[i].shotsHit), 0, _snapshot, index, sizeof(int));
                index += sizeof(int);
            }
        }

        public static void ClearBuf(ref byte[] buf)
        {
            foreach (byte b in buf)
            {
                buf[b] = 0x00;
            }
        }

        public void AddFireCommand(NetPacketReader r)
        {
            _firePacket.Deserialize(r);
            if (_firePacket.PlayerPin != GameManager.instance.players[_firePacket.PlayerId].securityPin)
            {
                return;
            }
            
            _fireCommands[_fireCommandIndex].ProjectileId = _firePacket.ProjectileDatabaseId;
            _fireCommands[_fireCommandIndex].VehicleId = _firePacket.PlayerId;
            _fireCommands[_fireCommandIndex].WeaponSlotFired = _firePacket.WeaponSlotFired;
            _fireCommands[_fireCommandIndex].UniqueProjectileId = _firePacket.UniqueProjectileId;
            _fireCommands[_fireCommandIndex].Process = true;
            _fireCommands[_fireCommandIndex].Rotation.x = _firePacket.RotX;
            _fireCommands[_fireCommandIndex].Rotation.y = _firePacket.RotY;
            _fireCommands[_fireCommandIndex].Rotation.z = _firePacket.RotZ;
            _fireCommandIndex++;
        }

        public void AddCollisionReport(NetPacketReader r)
        {
            Debug.Log("Add collision report");
            _reportCollision.Deserialize(r);
            if (GameManager.instance.players[_reportCollision.PlayerIdThatShotThisProjectile].securityPin ==
                _reportCollision.PlayerPin)
            {
                _reportedCollisions[_reportCollisionIndex].UniqueProjectileId = _reportCollision.UniqueProjectileId;
                _reportedCollisions[_reportCollisionIndex].PlayerIdThatWasHit = _reportCollision.PlayerIdThatWasHit;
                _reportedCollisions[_reportCollisionIndex].ByProjectileDatabaseId =
                    _reportCollision.ByProjectileDatabaseId;
                _reportedCollisions[_reportCollisionIndex].PlayerIdThatShotThisProjectile =
                    _reportCollision.PlayerIdThatShotThisProjectile;
                _reportedCollisions[_reportCollisionIndex].TickWhenCollisionOccurred =
                    _reportCollision.TickWhenCollisionOccurred;
                _reportedCollisions[_reportCollisionIndex].Process = true;
                _reportedCollisions[_reportCollisionIndex].impactPos = new Vector3(_reportCollision.impactPosX, _reportCollision.impactPosY, _reportCollision.impactPosZ);
                _reportCollisionIndex++;
            }
        }
    }
}