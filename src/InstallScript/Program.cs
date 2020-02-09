using InstallScript.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: InternalsVisibleTo("InstallScript.Test")]
namespace InstallScript
{
    public class Program
    {
        public static CommandExecutor executor = new CommandExecutor();
        public static OSPlatform currentOS;

        public static readonly string CI_DIRECTORY = "CI";
        public static readonly string RESOURCES_DIRECTORY = "Resources";
        public static readonly string HOOKS_DIRECTORY = "githooks";
        public static readonly string COMMIT_MSG_HOOK = "commit-msg";

        static Program()
        {
            currentOS = GetOS();
        }

        internal static OSPlatform GetOS()
        {
            Func<bool> isWin = () => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            Func<bool> isLinux = () => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            Func<bool> isMac = () => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

            return
                isLinux() ? OSPlatform.Linux : (
                    isMac() ? OSPlatform.OSX : (
                        isWin() ? OSPlatform.Windows : throw new NotSupportedException("OS not supported")
                ));
        }

        internal static Stream ReadEmbeddedResource(string resourceFileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceId = $"{assembly.GetName().Name}.Resources.{resourceFileName}";

            Stream stream = assembly.GetManifestResourceStream(resourceId);

            return stream;
        }

        // CI folder must exist
        internal static void CreateResourcesFolder()
        {
            Console.WriteLine($"{RESOURCES_DIRECTORY} folder creation...");
            var resDirInfo = Directory.CreateDirectory(RESOURCES_DIRECTORY);
            Directory.SetCurrentDirectory(RESOURCES_DIRECTORY);

            Console.WriteLine($"{HOOKS_DIRECTORY} folder creation...");
            var dirInfo = Directory.CreateDirectory(HOOKS_DIRECTORY);
            var commitmsgPath = Path.Combine(HOOKS_DIRECTORY, COMMIT_MSG_HOOK);

            using (FileStream fs = new FileStream(commitmsgPath, FileMode.CreateNew, FileAccess.ReadWrite))
            {
                byte[] commitmsgContent = ReadEmbeddedResource(COMMIT_MSG_HOOK).ToByteArray();
                fs.Write(commitmsgContent);
            }

            executor.ExecuteCommandOnDefaultShell($"git config core.hooksPath {CI_DIRECTORY}/{RESOURCES_DIRECTORY}/{HOOKS_DIRECTORY}");
        }

        internal static void StartScript()
        {
            Console.WriteLine("Install Script started...");

            if (!Directory.Exists(RESOURCES_DIRECTORY))
            {
                Console.WriteLine($"{Path.Combine(CI_DIRECTORY, RESOURCES_DIRECTORY)} does not exist");
                CreateResourcesFolder();
            }
            else
            {
                Console.WriteLine($"{Path.Combine(CI_DIRECTORY, RESOURCES_DIRECTORY)} already exists");
            }

            Console.WriteLine("Install Script ended...");
        }

        public static void Main(string[] args)
        {
            var slnPath = Path.Combine("..", "..", CI_DIRECTORY);
            Directory.SetCurrentDirectory(slnPath);

            StartScript();
        }
    }
}

