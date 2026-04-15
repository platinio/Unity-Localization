using ArcaneOnyx.EditorExtension;
using Zenject;

namespace ArcaneOnyx.Localization
{
    [AutoAssetGeneration("Installers/Static" , "LocalizationStaticInstaller")]
    [StaticInstaller(StaticInstallerExecutionOrder.NonImportant)]
    public class LocalizationStaticInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.Rebind<LocalizationManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}