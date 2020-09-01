using Common.Audio;
using Common.Save;
using Common.Scene;
using UnityEngine;
using Zenject;

namespace Common.Install
{
    /// <summary>
    /// アプリ共通Installer
    /// </summary>
    internal class CommonInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<SaveData>()
                .AsSingle()
                .OnInstantiated((c, o) =>
                {
                    var saveData = o as SaveData;
                    saveData?.Load();
                })
                .NonLazy();

            Container
                .Bind<SoundManager>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();
        }
    }
}