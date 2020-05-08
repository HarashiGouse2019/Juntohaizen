using System.IO;
using UnityEngine;

namespace DSL
{
    //Directories for all resources
    public static class Directory
    {
        //An enumerator of all directories
        public enum Path
        {
            EXPRESSION,
            POSE,
            SOUND,
            MUSIC,
            VOICE,
            SCENE,
            STYLE
        }

        //Default Paths
        readonly static string DEFAULT_EXPRESSION_PATH = Application.dataPath + @"";
        readonly static string DEFAULT_POSE_PATH = Application.dataPath + @"";
        readonly static string DEFAULT_SOUND_PATH = Application.dataPath + @"";
        readonly static string DEFAULT_MUSIC_PATH = Application.dataPath + @"";
        readonly static string DEFAULT_VOICE_PATH = Application.dataPath + @"";
        readonly static string DEFAULT_SCENE_PATH = Application.dataPath + @"";
        readonly static string DEFAULT_STYLE_PATH = Application.dataPath + @"";

        //Directory for all expressions
        public static DirectoryInfo ExpressionDirectory { get; private set; } = new DirectoryInfo(DEFAULT_EXPRESSION_PATH);

        //Directory for all poses
        public static DirectoryInfo PoseDirectory { get; private set; } = new DirectoryInfo(DEFAULT_POSE_PATH);

        //Directory for all sound effects
        public static DirectoryInfo SoundDirectory { get; private set; } = new DirectoryInfo(DEFAULT_SOUND_PATH);

        //Directory for all music
        public static DirectoryInfo MusicDirectory { get; private set; } = new DirectoryInfo(DEFAULT_MUSIC_PATH);

        //Directory for all voices (if they're character voices involved)
        public static DirectoryInfo VoiceDirectory { get; private set; } = new DirectoryInfo(DEFAULT_VOICE_PATH);

        //Directory for all background scenes
        public static DirectoryInfo SceneDirectory { get; private set; } = new DirectoryInfo(DEFAULT_SCENE_PATH);

        //Directory for all style aesthetics of the game
        public static DirectoryInfo StyleDirectory { get; private set; } = new DirectoryInfo(DEFAULT_STYLE_PATH);

        //An array of all directories
        public static DirectoryInfo[] ResourceDirectories { get; } =
        {
            ExpressionDirectory,
            PoseDirectory,
            SoundDirectory,
            MusicDirectory,
            VoiceDirectory,
            SceneDirectory,
            StyleDirectory,
        };

        //Constructor
        static Directory()
        {
            Init();
        }

        //Update a directory path
        public static void UpdateDirectoryPath(Path _pathType, DirectoryInfo _newDirectory)
        {
            switch (_pathType)
            {
                case Path.EXPRESSION:
                    ChangeExpressionPath(_newDirectory);
                    break;

                case Path.POSE:
                    ChangePosePath(_newDirectory);
                    break;

                case Path.SOUND:
                    ChangeSoundPath(_newDirectory);
                    break;

                case Path.MUSIC:
                    ChangeMusicPath(_newDirectory);
                    break;

                case Path.VOICE:
                    ChangeVoicePath(_newDirectory);
                    break;

                case Path.SCENE:
                    ChangeScenePath(_newDirectory);
                    break;

                case Path.STYLE:
                    ChangeScenePath(_newDirectory);
                    break;

                default:
                    break;
            }
        }

        //Initalize object
        static void Init()
        {
            //We want to check if the current directories do exist. If not, we'll create one ourselves.

            if (!ExpressionDirectory.Exists) throw new IOException("Expressions file doesn't exist.");

            if (!PoseDirectory.Exists) throw new IOException("Poses file doesn't exist.");

            if (!SoundDirectory.Exists) throw new IOException("Sounds file doesn't exist.");

            if (!MusicDirectory.Exists) throw new IOException("Music file doesn't exist.");

            if (!VoiceDirectory.Exists) throw new IOException("Voices file doesn't exist.");

            if (!SceneDirectory.Exists) throw new IOException("Scenes file doesn't exist.");

            return;
        }

        static void ChangeExpressionPath(DirectoryInfo _newDirectory)
        {
            ExpressionDirectory = _newDirectory;
        }

        static void ChangePosePath(DirectoryInfo _newDirectory)
        {
            PoseDirectory = _newDirectory;
        }

        static void ChangeSoundPath(DirectoryInfo _newDirectory)
        {
            SoundDirectory = _newDirectory;
        }

        static void ChangeMusicPath(DirectoryInfo _newDirectory)
        {
            MusicDirectory = _newDirectory;
        }

        static void ChangeVoicePath(DirectoryInfo _newDirectory)
        {
            VoiceDirectory = _newDirectory;
        }

        static void ChangeScenePath(DirectoryInfo _newDirectory)
        {
            SceneDirectory = _newDirectory;
        }

        static void ChangeStylePath(DirectoryInfo _newDirectory)
        {
            StyleDirectory = _newDirectory;
        }
    }
}
