namespace Network
{
    public class HeaderBytes
    {
        public const byte AskForUserName = 0x01;
        public const byte SendUserName = 0x02;
        public const byte OpenSpawnMenuOnClient = 0x03;
        public const byte SendPlayerId = 0x04;
        public const byte SpawnShip = 0x05;
        public const byte RequestSpawn = 0x06;
        public const byte SpawnAVehicleWithVehicleConfiguration = 0x07;
        public const byte GiveControlOfVehicleToClient = 0x08;
    }
}