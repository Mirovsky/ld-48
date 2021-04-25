using UnityEngine;
using Zenject;

public class MemoryPoolsInstaller : MonoInstaller
{
    [SerializeField] private MazeObject _mazeObjectPrefab;
    [SerializeField] private ScoreItem _scoreItemPrefab;
    [SerializeField] private FuelBlock _fuelBlockPrefab;
    
    public override void InstallBindings()
    {
        Container.BindMemoryPool<MazeObject, MazeObject.Pool>().FromComponentInNewPrefab(_mazeObjectPrefab);
        Container.BindMemoryPool<ScoreItem, ScoreItem.Pool>().FromComponentInNewPrefab(_scoreItemPrefab);
        Container.BindMemoryPool<FuelBlock, FuelBlock.Pool>().FromComponentInNewPrefab(_fuelBlockPrefab);
    }
}
