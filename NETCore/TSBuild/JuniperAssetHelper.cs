namespace Juniper.TSBuild;

/// <summary>
/// Juniper comes with a number of asset files that can be
/// used in projects. This class helps get references to them
/// more easily.
/// </summary>
public class JuniperAssetHelper
{
    public JuniperAssetHelper(DirectoryInfo juniperDir)
    {
        var assets = juniperDir.CD("etc", "Assets");
        CSS = new CSSHelper(assets.CD("CSS"));
        JS = new JSHelper(assets.CD("JS"));
        WebFonts = new WebFontsHelper(assets.CD("WebFonts"));
        GIS = new GISHelper(assets.CD("GIS"));
        Textures = new TexturesHelper(assets.CD("Textures"));
        Models = new ModelsHelper(assets.CD("Models"));
        Audios = new AudiosHelper(assets.CD("Audio"));
    }

    public CSSHelper CSS { get; }
    public class CSSHelper
    {
        internal CSSHelper(DirectoryInfo dir)
        {
            Bootstrap = dir.Touch("bootstrap.min.css");
            FontAwesome = dir.CD("FontAwesome");
        }

        public FileInfo Bootstrap { get; }
        public DirectoryInfo FontAwesome { get; }
    }

    public JSHelper JS { get; }
    public class JSHelper
    {
        internal JSHelper(DirectoryInfo dir)
        {
            XLSX = dir.Touch("xlsx-0.20.3.tgz");
        }

        public FileInfo XLSX { get; }
        public FileInfo[] PDFJS { get; }
    }

    public WebFontsHelper WebFonts { get; }
    public class WebFontsHelper
    {
        internal WebFontsHelper(DirectoryInfo dir)
        {
            Noto = dir.CD("Noto");
        }

        public DirectoryInfo Noto { get; }
    }

    public GISHelper GIS { get; }
    public class GISHelper
    {
        internal GISHelper(DirectoryInfo dir)
        {
            CanadaGEOJSON = dir.Touch("canada.geojson");
            CanadaKML = dir.Touch("canada.kml");
            EarthGEOJSON = dir.Touch("earth.geojson");
            MexicoGEOJSON = dir.Touch("mexico.geojson");
            MexicoKML = dir.Touch("mexico.kml");
            UnitedStatesGEOJSON = dir.Touch("united-states.geojson");
            UnitedStatesKML = dir.Touch("united-states.kml");
        }

        public FileInfo CanadaGEOJSON { get; }
        public FileInfo CanadaKML { get; }
        public FileInfo EarthGEOJSON { get; }
        public FileInfo MexicoGEOJSON { get; }
        public FileInfo MexicoKML { get; }
        public FileInfo UnitedStatesGEOJSON { get; }
        public FileInfo UnitedStatesKML { get; }
    }

    public TexturesHelper Textures { get; }
    public class TexturesHelper
    {
        internal TexturesHelper(DirectoryInfo dir)
        {
            Textures = dir;
            Bark = dir.Touch("bark.jpg");
            Compass = dir.Touch("compass.png");
            CrateDiffuse = dir.Touch("CrateDiffuse.jpg");
            CrateNormal = dir.Touch("CrateNormal.jpg");
            Exit = dir.Touch("exit.png");
            EyeChart = dir.Touch("EyeChart.jpg");
            Gear = dir.Touch("gear.png");
            Grass = dir.Touch("grass.png");
            Leaf = dir.Touch("leaf.png");
            MoonDiffuse = dir.Touch("moon.jpg");
            MoonNormal = dir.Touch("moon-normals.jpg");
            Water = dir.Touch("water.png");
        }

        public DirectoryInfo Textures { get; }
        public FileInfo Bark { get; }
        public FileInfo Compass { get; }
        public FileInfo CrateDiffuse { get; }
        public FileInfo CrateNormal { get; }
        public FileInfo Exit { get; }
        public FileInfo EyeChart { get; }
        public FileInfo Gear { get; }
        public FileInfo Grass { get; }
        public FileInfo Leaf { get; }
        public FileInfo MoonDiffuse { get; }
        public FileInfo MoonNormal { get; }
        public FileInfo Water { get; }

        public void AddUITextures(BuildSystemOptions options, DirectoryInfo uiImgOUtput)
        {
            var newDep = Textures.CD("UI").CopyFiles(uiImgOUtput);
            if (options.Dependencies is null)
            {
                options.Dependencies = newDep;
            }
            else
            {
                options.Dependencies = options.Dependencies.Union(newDep);
            }
        }
    }

    public ModelsHelper Models { get; }
    public class ModelsHelper
    {
        internal ModelsHelper(DirectoryInfo dir)
        {
            Cursors = dir.CD("Cursors").Touch("Cursors.glb");
            Watch = dir.CD("Watch").Touch("watch1.glb");
            ForestGround = dir.CD("Forest").Touch("Forest-Ground.glb");
            ForestTree = dir.CD("Forest").Touch("Forest-Tree.glb");
            HandLeft = dir.CD("Hand").Touch("left.glb");
            HandRight = dir.CD("Hand").Touch("right.glb");
        }

        public FileInfo Cursors { get; }
        public FileInfo Watch { get; }
        public FileInfo ForestGround { get; }
        public FileInfo ForestTree { get; }
        public FileInfo HandLeft { get; }
        public FileInfo HandRight { get; }
    }

    public AudiosHelper Audios { get; }
    public class AudiosHelper
    {
        internal AudiosHelper(DirectoryInfo dir)
        {
            UIDragged = dir.Touch("basic_dragged.mp3");
            UIEnter = dir.Touch("basic_enter.mp3");
            UIError = dir.Touch("basic_error.mp3");
            UIExit = dir.Touch("basic_exit.mp3");
            DoorOpen = dir.Touch("door_open.mp3");
            DoorClose = dir.Touch("door_close.mp3");
            FootStepsFast = dir.Touch("footsteps_fast.mp3");
            FootStepsSlow = dir.Touch("footsteps_slow.mp3");
            Forest = dir.Touch("forest.mp3");
            ButtonPress = dir.Touch("vintage_radio_button_pressed.mp3");
            StarTrek = new StarTrekAudiosHelper(dir.CD("Star Trek"));
        }

        public FileInfo Forest { get; }
        public FileInfo FootStepsFast { get; }
        public FileInfo FootStepsSlow { get; }
        public FileInfo ButtonPress { get; }
        public FileInfo DoorOpen { get; }
        public FileInfo DoorClose { get; }
        public FileInfo UIDragged { get; }
        public FileInfo UIEnter { get; }
        public FileInfo UIError { get; }
        public FileInfo UIExit { get; }

        public StarTrekAudiosHelper StarTrek { get; }

        public class StarTrekAudiosHelper
        {
            internal StarTrekAudiosHelper(DirectoryInfo dir)
            {
                Alarm01 = dir.Touch("alarm01.mp3");
                Alarm02 = dir.Touch("alarm02.mp3");
                Alarm03 = dir.Touch("alarm03.mp3");
                AlarmDamage = dir.Touch("damagealarm.mp3");
                AlarmRomulan = dir.Touch("romulan_alarm.mp3");
                Alert03 = dir.Touch("alert03.mp3");
                Alert04 = dir.Touch("alert04.mp3");
                Alert05 = dir.Touch("alert05.mp3");
                Alert07 = dir.Touch("alert07.mp3");
                Alert08 = dir.Touch("alert08.mp3");
                Alert09 = dir.Touch("alert09.mp3");
                Alert12 = dir.Touch("alert12.mp3");
                Alert13 = dir.Touch("alert13.mp3");
                Alert14 = dir.Touch("alert14.mp3");
                Alert15 = dir.Touch("alert15.mp3");
                Alert16 = dir.Touch("alert16.mp3");
                Alert17 = dir.Touch("alert17.mp3");
                Alert18 = dir.Touch("alert18.mp3");
                Alert19 = dir.Touch("alert19.mp3");
                Alert20 = dir.Touch("alert20.mp3");
                Alert21 = dir.Touch("alert21.mp3");
                Alert22 = dir.Touch("alert22.mp3");
                Alert23 = dir.Touch("alert23.mp3");
                Alert24 = dir.Touch("alert24.mp3");
                AlertVoyagerBlue = dir.Touch("voybluealert.mp3");
                AlertVoyagerRed = dir.Touch("voy_redalert.mp3");
                ComputerBeep3 = dir.Touch("computerbeep_3.mp3");
                ComputerBeep4 = dir.Touch("computerbeep_4.mp3");
                ComputerBeep5 = dir.Touch("computerbeep_5.mp3");
                ComputerBeep6 = dir.Touch("computerbeep_6.mp3");
                ComputerBeep9 = dir.Touch("computerbeep_9.mp3");
                ComputerBeep10 = dir.Touch("computerbeep_10.mp3");
                ComputerBeep11 = dir.Touch("computerbeep_11.mp3");
                ComputerBeep12 = dir.Touch("computerbeep_12.mp3");
                ComputerBeep13 = dir.Touch("computerbeep_13.mp3");
                ComputerBeep16 = dir.Touch("computerbeep_16.mp3");
                ComputerBeep18 = dir.Touch("computerbeep_18.mp3");
                ComputerBeep19 = dir.Touch("computerbeep_19.mp3");
                ComputerBeep20 = dir.Touch("computerbeep_20.mp3");
                ComputerBeep21 = dir.Touch("computerbeep_21.mp3");
                ComputerBeep23 = dir.Touch("computerbeep_23.mp3");
                ComputerBeep26 = dir.Touch("computerbeep_26.mp3");
                ComputerBeep30 = dir.Touch("computerbeep_30.mp3");
                ComputerBeep31 = dir.Touch("computerbeep_31.mp3");
                ComputerBeep32 = dir.Touch("computerbeep_32.mp3");
                ComputerBeep33 = dir.Touch("computerbeep_33.mp3");
                ComputerBeep34 = dir.Touch("computerbeep_34.mp3");
                ComputerBeep35 = dir.Touch("computerbeep_35.mp3");
                ComputerBeep36 = dir.Touch("computerbeep_36.mp3");
                ComputerBeep38 = dir.Touch("computerbeep_38.mp3");
                ComputerBeep39 = dir.Touch("computerbeep_39.mp3");
                ComputerBeep41 = dir.Touch("computerbeep_41.mp3");
                ComputerBeep42 = dir.Touch("computerbeep_42.mp3");
                ComputerBeep43 = dir.Touch("computerbeep_43.mp3");
                ComputerBeep44 = dir.Touch("computerbeep_44.mp3");
                ComputerBeep45 = dir.Touch("computerbeep_45.mp3");
                ComputerBeep46 = dir.Touch("computerbeep_46.mp3");
                ComputerBeep50 = dir.Touch("computerbeep_50.mp3");
                ComputerBeep51 = dir.Touch("computerbeep_51.mp3");
                ComputerBeep52 = dir.Touch("computerbeep_52.mp3");
                ComputerBeep53 = dir.Touch("computerbeep_53.mp3");
                ComputerBeep54 = dir.Touch("computerbeep_54.mp3");
                ComputerBeep55 = dir.Touch("computerbeep_55.mp3");
                ComputerBeep68 = dir.Touch("computerbeep_68.mp3");
                ComputerBeep70 = dir.Touch("computerbeep_70.mp3");
                ComputerBeep72 = dir.Touch("computerbeep_72.mp3");
                ComputerBeep73 = dir.Touch("computerbeep_73.mp3");
                ComputerBeep74 = dir.Touch("computerbeep_74.mp3");
                ComputerBeep75 = dir.Touch("computerbeep_75.mp3");
                CommunicationsEndTransmission = dir.Touch("communications_end_transmission.mp3");
                CommunicationsStartTransmission = dir.Touch("communications_start_transmission.mp3");
                JeffriesTube = dir.Touch("jefferies_tube.mp3");
                ComputerError = dir.Touch("computer_error.mp3");
                ConsoleWarning = dir.Touch("consolewarning.mp3");
                Critical = dir.Touch("critical.mp3");
                DenyBeep1 = dir.Touch("denybeep1.mp3");
                DeskViewer1 = dir.Touch("deskviewer1.mp3");
                DeskViewer2 = dir.Touch("deskviewer2.mp3");
                ReplicatorDS9 = dir.Touch("ds9_replicator.mp3");
                Energize = dir.Touch("energize.mp3");
                Engage = dir.Touch("engage.mp3");
                OldCommunicator1 = dir.Touch("ent_communicator1.mp3");
                OldCommunicator3 = dir.Touch("ent_communicator3.mp3");
                ForcefieldOff = dir.Touch("forcefield_off.mp3");
                ForcefieldOff2 = dir.Touch("forcefield_off_2.mp3");
                ForcefieldOn = dir.Touch("forcefield_on.mp3");
                ForcefieldTouch = dir.Touch("forcefield_touch.mp3");
                ForcefieldTouch2 = dir.Touch("forcefield_touch2.mp3");
                ForceFieldHit = dir.Touch("force_field_hit.mp3");
                ForceFieldOn2 = dir.Touch("force_field_on2.mp3");
                HailAlert1 = dir.Touch("hailalert_1.mp3");
                HailAlert2 = dir.Touch("hailalert_2.mp3");
                HailBeep3 = dir.Touch("hailbeep3_clean.mp3");
                HailBeep4 = dir.Touch("hailbeep4_clean.mp3");
                HailBeep5 = dir.Touch("hailbeep_5.mp3");
                HailingFrequenciesOpen2 = dir.Touch("hailingfrequencies_open2.mp3");
                HailAllShip = dir.Touch("hail_allship_ep.mp3");
                HelmEngage = dir.Touch("helm_engage_clean.mp3");
                HologramOff2 = dir.Touch("hologram_off_2.mp3");
                HologridFailing = dir.Touch("hologrid_failing.mp3");
                HologridOnline = dir.Touch("hologrid_online.mp3");
                InputFailed2 = dir.Touch("input_failed2_clean.mp3");
                InputFailed = dir.Touch("input_failed_clean.mp3");
                InputOk2 = dir.Touch("input_ok_2_clean.mp3");
                InputOk3 = dir.Touch("input_ok_3_clean.mp3");
                KeyOk1 = dir.Touch("keyok1.mp3");
                KeyOk2 = dir.Touch("keyok2.mp3");
                KeyOk5 = dir.Touch("keyok5.mp3");
                KeyOk6 = dir.Touch("keyok6.mp3");
                PowerDown = dir.Touch("power_down.mp3");
                PowerUp1 = dir.Touch("power_up1_clean.mp3");
                PowerUp2 = dir.Touch("power_up2_clean.mp3");
                Processing = dir.Touch("processing.mp3");
                Processing2 = dir.Touch("processing2.mp3");
                Processing3 = dir.Touch("processing3.mp3");
                ScreenScroll1 = dir.Touch("scrscroll1.mp3");
                ScreenScroll2 = dir.Touch("scrscroll2.mp3");
                ScreenScroll3 = dir.Touch("scrscroll3.mp3");
                ScreenScroll4 = dir.Touch("scrscroll4.mp3");
                ScreenScroll5 = dir.Touch("scrscroll5.mp3");
                ScreenSearch = dir.Touch("scrsearch.mp3");
                ScreenShow = dir.Touch("scrshow.mp3");
                Chime = dir.Touch("tng_chime_clean.mp3");
                Chirp = dir.Touch("tng_chirp_clean.mp3");
                Door = dir.Touch("tng_doors2.mp3");
                DoorClose = dir.Touch("tng_door_close.mp3");
                DoorOpen = dir.Touch("tng_door_open.mp3");
                EngineeringDevice1 = dir.Touch("tng_engineering_device_1.mp3");
                EngineeringDevice2 = dir.Touch("tng_engineering_device_2.mp3");
                EngineeringDevice3 = dir.Touch("tng_engineering_device_3.mp3");
                Phaser = dir.Touch("tng_phaser3_clean.mp3");
                Torpedo = dir.Touch("tng_torpedo3_clean.mp3");
                Tricorder4 = dir.Touch("tng_tricorder4.mp3");
                Tricorder5 = dir.Touch("tng_tricorder5.mp3");
                Tricorder6 = dir.Touch("tng_tricorder6.mp3");
                Tricorder8 = dir.Touch("tng_tricorder8.mp3");
                Tricorder9 = dir.Touch("tng_tricorder9.mp3");
                Tricorder10 = dir.Touch("tng_tricorder10.mp3");
                Tricorder11 = dir.Touch("tng_tricorder11.mp3");
                Tricorder12 = dir.Touch("tng_tricorder12.mp3");
                ViewscreenOff = dir.Touch("tng_viewscreen_off.mp3");
                ViewscreenOn = dir.Touch("tng_viewscreen_on.mp3");
            }

            public FileInfo Alarm01 { get; }
            public FileInfo Alarm02 { get; }
            public FileInfo Alarm03 { get; }
            public FileInfo Alert03 { get; }
            public FileInfo AlarmDamage { get; }
            public FileInfo AlarmRomulan { get; }
            public FileInfo Alert04 { get; }
            public FileInfo Alert05 { get; }
            public FileInfo Alert07 { get; }
            public FileInfo Alert08 { get; }
            public FileInfo Alert09 { get; }
            public FileInfo Alert12 { get; }
            public FileInfo Alert13 { get; }
            public FileInfo Alert14 { get; }
            public FileInfo Alert15 { get; }
            public FileInfo Alert16 { get; }
            public FileInfo Alert17 { get; }
            public FileInfo Alert18 { get; }
            public FileInfo Alert19 { get; }
            public FileInfo Alert20 { get; }
            public FileInfo Alert21 { get; }
            public FileInfo Alert22 { get; }
            public FileInfo Alert23 { get; }
            public FileInfo Alert24 { get; }
            public FileInfo AlertVoyagerBlue { get; }
            public FileInfo AlertVoyagerRed { get; }
            public FileInfo CommunicationsEndTransmission { get; }
            public FileInfo CommunicationsStartTransmission { get; }
            public FileInfo ComputerBeep10 { get; }
            public FileInfo ComputerBeep11 { get; }
            public FileInfo ComputerBeep12 { get; }
            public FileInfo ComputerBeep13 { get; }
            public FileInfo ComputerBeep16 { get; }
            public FileInfo ComputerBeep18 { get; }
            public FileInfo ComputerBeep19 { get; }
            public FileInfo ComputerBeep20 { get; }
            public FileInfo ComputerBeep21 { get; }
            public FileInfo ComputerBeep23 { get; }
            public FileInfo ComputerBeep26 { get; }
            public FileInfo ComputerBeep3 { get; }
            public FileInfo ComputerBeep30 { get; }
            public FileInfo ComputerBeep31 { get; }
            public FileInfo ComputerBeep32 { get; }
            public FileInfo ComputerBeep33 { get; }
            public FileInfo ComputerBeep34 { get; }
            public FileInfo ComputerBeep35 { get; }
            public FileInfo ComputerBeep36 { get; }
            public FileInfo ComputerBeep38 { get; }
            public FileInfo ComputerBeep39 { get; }
            public FileInfo ComputerBeep4 { get; }
            public FileInfo ComputerBeep41 { get; }
            public FileInfo ComputerBeep42 { get; }
            public FileInfo ComputerBeep43 { get; }
            public FileInfo ComputerBeep44 { get; }
            public FileInfo ComputerBeep45 { get; }
            public FileInfo ComputerBeep46 { get; }
            public FileInfo ComputerBeep5 { get; }
            public FileInfo ComputerBeep50 { get; }
            public FileInfo ComputerBeep51 { get; }
            public FileInfo ComputerBeep52 { get; }
            public FileInfo ComputerBeep53 { get; }
            public FileInfo ComputerBeep54 { get; }
            public FileInfo ComputerBeep55 { get; }
            public FileInfo ComputerBeep6 { get; }
            public FileInfo ComputerBeep68 { get; }
            public FileInfo ComputerBeep70 { get; }
            public FileInfo ComputerBeep72 { get; }
            public FileInfo ComputerBeep73 { get; }
            public FileInfo ComputerBeep74 { get; }
            public FileInfo ComputerBeep75 { get; }
            public FileInfo ComputerBeep9 { get; }
            public FileInfo ComputerError { get; }
            public FileInfo ConsoleWarning { get; }
            public FileInfo Critical { get; }
            public FileInfo DenyBeep1 { get; }
            public FileInfo DeskViewer1 { get; }
            public FileInfo DeskViewer2 { get; }
            public FileInfo ReplicatorDS9 { get; }
            public FileInfo Energize { get; }
            public FileInfo Engage { get; }
            public FileInfo OldCommunicator1 { get; }
            public FileInfo OldCommunicator3 { get; }
            public FileInfo ForcefieldOff { get; }
            public FileInfo ForcefieldOff2 { get; }
            public FileInfo ForcefieldOn { get; }
            public FileInfo ForcefieldTouch { get; }
            public FileInfo ForcefieldTouch2 { get; }
            public FileInfo ForceFieldHit { get; }
            public FileInfo ForceFieldOn2 { get; }
            public FileInfo HailAlert1 { get; }
            public FileInfo HailAlert2 { get; }
            public FileInfo HailBeep3 { get; }
            public FileInfo HailBeep4 { get; }
            public FileInfo HailBeep5 { get; }
            public FileInfo HailingFrequenciesOpen2 { get; }
            public FileInfo HailAllShip { get; }
            public FileInfo HelmEngage { get; }
            public FileInfo HologramOff2 { get; }
            public FileInfo HologridFailing { get; }
            public FileInfo HologridOnline { get; }
            public FileInfo InputFailed2 { get; }
            public FileInfo InputFailed { get; }
            public FileInfo InputOk2 { get; }
            public FileInfo InputOk3 { get; }
            public FileInfo JeffriesTube { get; }
            public FileInfo KeyOk1 { get; }
            public FileInfo KeyOk2 { get; }
            public FileInfo KeyOk5 { get; }
            public FileInfo KeyOk6 { get; }
            public FileInfo PowerDown { get; }
            public FileInfo PowerUp1 { get; }
            public FileInfo PowerUp2 { get; }
            public FileInfo Processing { get; }
            public FileInfo Processing2 { get; }
            public FileInfo Processing3 { get; }
            public FileInfo ScreenScroll1 { get; }
            public FileInfo ScreenScroll2 { get; }
            public FileInfo ScreenScroll3 { get; }
            public FileInfo ScreenScroll4 { get; }
            public FileInfo ScreenScroll5 { get; }
            public FileInfo ScreenSearch { get; }
            public FileInfo ScreenShow { get; }
            public FileInfo Chime { get; }
            public FileInfo Chirp { get; }
            public FileInfo Door { get; }
            public FileInfo DoorClose { get; }
            public FileInfo DoorOpen { get; }
            public FileInfo EngineeringDevice1 { get; }
            public FileInfo EngineeringDevice2 { get; }
            public FileInfo EngineeringDevice3 { get; }
            public FileInfo Phaser { get; }
            public FileInfo Torpedo { get; }
            public FileInfo Tricorder4 { get; }
            public FileInfo Tricorder5 { get; }
            public FileInfo Tricorder6 { get; }
            public FileInfo Tricorder8 { get; }
            public FileInfo Tricorder9 { get; }
            public FileInfo Tricorder10 { get; }
            public FileInfo Tricorder11 { get; }
            public FileInfo Tricorder12 { get; }
            public FileInfo ViewscreenOff { get; }
            public FileInfo ViewscreenOn { get; }

        }
    }
}
