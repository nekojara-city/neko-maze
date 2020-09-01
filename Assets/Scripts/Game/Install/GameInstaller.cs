using Game.Gimmick;
using Game.Stage;
using Zenject;

namespace Game.Install
{
    /// <summary>
    /// ゲーム画面用Installer
    /// </summary>
    internal class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // ゲームメイン
            Container
                .Bind<GameStateManager>()
                .FromComponentInHierarchy()
                .AsSingle();
            
            Container
                .Bind<GameEventDispatcher>()
                .FromComponentInHierarchy()
                .AsSingle();
            
            // ギミック
            Container
                .Bind<GoalManager>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<MazeGenerator>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<TrapEffectCamera>()
                .FromComponentInHierarchy()
                .AsSingle();
        }
    }
}