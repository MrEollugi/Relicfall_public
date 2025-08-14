using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerFactory
{
    PlayerController Spawn(Vector3 spawnPosition);
}

public class LocalPlayerFactory : IPlayerFactory
{
    private readonly PlayerController _playerPrefab;

    public LocalPlayerFactory(PlayerController playerPrefab)
    {
        _playerPrefab = playerPrefab;
    }

    public PlayerController Spawn(Vector3 spawnPosition)
    {
        return UnityEngine.Object.Instantiate(
            _playerPrefab,
            spawnPosition,
            Quaternion.identity
        );
    }
}

//public class NetworkPlayerFactory : IPlayerFactory
//{
//    private readonly PlayerController _playerPrefab;

//    public NetworkPlayerFactory(PlayerController playerPrefab)
//    {
//        _playerPrefab = playerPrefab;
//    }

//    public PlayerController Spawn(Vector3 spawnPosition, PlayerData data)
//    {
//        // Mirror�� ���, ����/������ �ʱ�ȭ ������ ���� �ݹ鿡�� ó��
//        NetworkServer.Spawn(_playerPrefab.gameObject);
//        // ���� Init(data)�� OnServerAddPlayer �ݹ� �ȿ���
//        return _playerPrefab;
//    }
//}
