using Zenject;

public class SignalsInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        
        Container.DeclareSignal<GameStartedSignal>().OptionalSubscriber();
        Container.DeclareSignal<GenerateMazeSignal>().OptionalSubscriber();
        Container.DeclareSignal<MazeGeneratedSignal>().OptionalSubscriber();
        Container.DeclareSignal<MazeUpdatedSignal>().OptionalSubscriber();
        Container.DeclareSignal<MazeFinishedSignal>().OptionalSubscriber();
        Container.DeclareSignal<MazeRedrawStarted>().OptionalSubscriber();
        Container.DeclareSignal<MazeRedrawFinished>().OptionalSubscriber();

        Container.DeclareSignal<SpawnPlayerSignal>().OptionalSubscriber();
        Container.DeclareSignal<PlayerMoveSignal>().OptionalSubscriber();
        Container.DeclareSignal<PlayerMovedSignal>().OptionalSubscriber();
        Container.DeclareSignal<PlayerStartMoveSignal>().OptionalSubscriber();
        Container.DeclareSignal<PlayerEndMoveSignal>().OptionalSubscriber();
        
        Container.DeclareSignal<EmptyFuelSignal>().OptionalSubscriber();
        Container.DeclareSignal<FuelUpdateSignal>().OptionalSubscriber();

        Container.DeclareSignal<ScoreUpdatedSignal>().OptionalSubscriber();
    }
}
