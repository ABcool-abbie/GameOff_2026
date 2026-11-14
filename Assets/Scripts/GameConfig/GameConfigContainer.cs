using UnityEngine;
using Utility;

public class GameConfigContainer : SingletonBehaviour<GameConfigContainer>
{
    public GameProgressConfig Config => config;
    [SerializeField] GameProgressConfig config;
}
