namespace Network
{
    public class HeaderBytes
    {
        public const byte AskClientForUsername = 0x01;
        public const byte SendUserNameToServer = 0x02;
        public const byte OpenSpawnMenuOnClient = 0x03;
        public const byte SendPlayerId = 0x04;
        public const byte SpawnShip = 0x05;
        public const byte RequestSpawn = 0x06;
        public const byte SpawnVehicle = 0x07;
        public const byte GiveControlOfVehicleToClient = 0x08;
        public const byte NetworkTransFormId = 0x09;
        public const byte NetworkTransFormsForVehicle = 0x0A;
        public const byte RemoveVehicle = 0x0B;
        public const byte SendPlayerData = 0x0C;
        public const byte FireWeapon = 0x0D;
    }
}