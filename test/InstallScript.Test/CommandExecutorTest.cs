using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace InstallScript
{
    [TestClass]
    public class CommandExecutorTest
    {
        public void InitTest()
        {
            Program.currentOS = Program.GetOS();
        }

        [TestMethod]
        public void ExecuteCommandOnDefaultShell_GenericCommand_OK()
        {
            InitTest();

            CommandExecutor executor = new CommandExecutor();
            executor.ExecuteCommandOnDefaultShell("echo 'ciao'");
        }
    }
}
