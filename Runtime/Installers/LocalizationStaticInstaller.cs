using ArcaneOnyx.EditorExtension;
using Zenject;

namespace ArcaneOnyx.Localization
{
    [AutoAssetGeneration("Installers/Resources/Static" , "LocalizationStaticInstaller")]
    [StaticInstaller(StaticInstallerExecutionOrder.NonImportant)]
    public class LocalizationStaticInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.Rebind<LocalizationManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}