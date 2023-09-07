namespace Juniper.TSBuild
{
    public class PathHelper
    {
        public PathHelper(DirectoryInfo juniperDir, DirectoryInfo nodeModules)
        {
            JuniperDir = juniperDir;

            Assets = juniperDir.CD("etc", "Assets");

            Textures = Assets.CD("Textures");
            Audios = Assets.CD("Audio");
            Models = Assets.CD("Models");
            StarTrekAudios = Audios.CD("Star Trek");

            var pdfJsIn = nodeModules.CD("pdfjs-dist", "build");
            PDFJSWorkerBundle = pdfJsIn.Touch("pdf.worker.js");
            PDFJSWorkerMap = pdfJsIn.Touch("pdf.worker.js.map");
            PDFJSWorkerMinBundle = pdfJsIn.Touch("pdf.worker.min.js");

            var jQueryIn = nodeModules.CD("jquery", "dist");
            JQueryBundle = jQueryIn.Touch("jquery.js");
            JQueryMinBundle = jQueryIn.Touch("jquery.min.js");

            CursorModel = Models.CD("Cursors").Touch("Cursors.glb");
            WatchModel = Models.CD("Watch").Touch("watch1.glb");

            var forestModels = Models.CD("Forest");
            ForestGroundModel = forestModels.Touch("Forest-Ground.glb");
            ForestTreeModel = forestModels.Touch("Forest-Tree.glb");

            ForestAudio = Audios.Touch("forest.mp3");
            StarTrekComputerBeep55Audio = StarTrekAudios.Touch("computerbeep_55.mp3");
            FootStepsAudio = Audios.Touch("footsteps_fast.mp3");
            ButtonPressAudio = Audios.Touch("vintage_radio_button_pressed.mp3");
            DoorOpenAudio = Audios.Touch("door_open.mp3");
            DoorCloseAudio = Audios.Touch("door_close.mp3");
            UIDraggedAudio = Audios.Touch("basic_dragged.mp3");
            UIEnterAudio = Audios.Touch("basic_enter.mp3");
            UIErrorAudio = Audios.Touch("basic_error.mp3");
            UIExitAudio = Audios.Touch("basic_exit.mp3");
        }

        public void AddUITextures(BuildSystemOptions options, DirectoryInfo uiImgOUtput)
        {
            var newDeps = new List<BuildSystemDependency>();
            foreach (var file in Textures.CD("UI").EnumerateFiles())
            {
                newDeps.Add(file.MakeDependency(uiImgOUtput));
            }

            options.Dependencies = options.Dependencies is null
                ? newDeps
                : options.Dependencies.Union(newDeps);
        }


        public DirectoryInfo JuniperDir { get; }
        public DirectoryInfo Assets { get; }
        public DirectoryInfo Textures { get; }
        public DirectoryInfo Audios { get; }
        public DirectoryInfo Models { get; }

        public DirectoryInfo NodeModules { get; }
        public DirectoryInfo StarTrekAudios { get; }

        public FileInfo PDFJSWorkerBundle { get; }
        public FileInfo PDFJSWorkerMap { get; }
        public FileInfo PDFJSWorkerMinBundle { get; }
        public FileInfo JQueryBundle { get; }
        public FileInfo JQueryMinBundle { get; }

        public FileInfo CursorModel { get; }
        public FileInfo WatchModel { get; }
        public FileInfo ForestGroundModel { get; }
        public FileInfo ForestTreeModel { get; }
        public FileInfo ForestAudio { get; }
        public FileInfo StarTrekComputerBeep55Audio { get; }
        public FileInfo FootStepsAudio { get; }
        public FileInfo ButtonPressAudio { get; }
        public FileInfo DoorOpenAudio { get; }
        public FileInfo DoorCloseAudio { get; }
        public FileInfo UIDraggedAudio { get; }
        public FileInfo UIEnterAudio { get; }
        public FileInfo UIErrorAudio { get; }
        public FileInfo UIExitAudio { get; }
    }
}
