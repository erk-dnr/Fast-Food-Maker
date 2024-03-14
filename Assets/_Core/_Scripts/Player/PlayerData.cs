using System;
using Unity.Collections;
using Unity.Netcode;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{

    public ulong clientId;
    public int colorId;
    // string not allowed because ca be null
    public FixedString64Bytes playerId;
    public FixedString64Bytes playerName;
    
    public bool Equals(PlayerData other)
    {
        return 
            clientId == other.clientId && 
            colorId == other.colorId &&
            playerId == other.playerId &&
            playerName == other.playerName;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
        serializer.SerializeValue(ref playerId);
        serializer.SerializeValue(ref playerName);
    }
}
