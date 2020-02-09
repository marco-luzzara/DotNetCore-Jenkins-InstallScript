using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using InstallScript;
using System.IO;
using Moq;

namespace InstallScript
{
    [TestClass]
    public class ProgramTest
    {
        public Mock<CommandExecutor> InitExecutor()
        {
            Mock<CommandExecutor> executorMock = new Mock<CommandExecutor>();
            executorMock.Setup(x => x.ExecuteCommandOnDefaultShell($"git config core.hooksPath {Program.HOOKS_DIRECTORY}", It.IsAny<bool>()));

            Program.executor = executorMock.Object;

            return executorMock;
        }

        [TestMethod]
        public void CreateCIFolder_CheckProcedure_OK()
        {
            var executorMock = InitExecutor();
            var curDir = Directory.GetCurrentDirectory();

            var dirInfo = Directory.CreateDirectory(Program.CI_DIRECTORY);
            Directory.SetCurrentDirectory(Program.CI_DIRECTORY);

            Program.CreateResourcesFolder();

            Directory.SetCurrentDirectory(curDir);
            Assert.IsTrue(Directory.Exists(Program.CI_DIRECTORY));

            Directory.SetCurrentDirectory(Program.CI_DIRECTORY);
            Assert.IsTrue(Directory.Exists(Program.RESOURCES_DIRECTORY));

            Directory.SetCurrentDirectory(Program.RESOURCES_DIRECTORY);
            Assert.IsTrue(Directory.Exists(Program.HOOKS_DIRECTORY));

            Directory.SetCurrentDirectory(Program.HOOKS_DIRECTORY);
            Assert.IsTrue(File.Exists(Program.COMMIT_MSG_HOOK));

            Directory.SetCurrentDirectory(Path.Combine("..", "..", ".."));
            Directory.Delete(Program.CI_DIRECTORY, true);

            executorMock.Verify(x => x.ExecuteCommandOnDefaultShell(
                $"git config core.hooksPath {Program.CI_DIRECTORY}/{Program.RESOURCES_DIRECTORY}/{Program.HOOKS_DIRECTORY}", It.IsAny<bool>()),
                Times.Once);
        }

        [TestMethod]
        public void CreateCIFolder_ResourcesFolderAlreadyExist_DoNothing()
        {
            var executorMock = InitExecutor();
            var curDir = Directory.GetCurrentDirectory();

            var dirInfo = Directory.CreateDirectory(Program.CI_DIRECTORY);
            Directory.SetCurrentDirectory(Program.CI_DIRECTORY);

            var resourcesInfo = Directory.CreateDirectory(Program.RESOURCES_DIRECTORY);

            Program.StartScript();

            Directory.SetCurrentDirectory(curDir);
            Directory.Delete(Program.CI_DIRECTORY, true);

            executorMock.Verify(x => x.ExecuteCommandOnDefaultShell(
                $"git config core.hooksPath {Program.CI_DIRECTORY}/{Program.RESOURCES_DIRECTORY}/{Program.HOOKS_DIRECTORY}", It.IsAny<bool>()),
                Times.Never);
        }
    }
}
