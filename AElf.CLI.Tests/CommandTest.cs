﻿using System;
using System.IO;
using System.Linq;
using AElf.CLI.Command;
using AElf.CLI.Screen;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Security;
using Xunit;

namespace AElf.CLI.Tests
{
    public class CommandTest
    {
        [Fact]
        public void TestListCommands()
        {
            var ctx = new AElfClientProgramContext(new ScreenManager());
            var command = new RootCommand();
            var cmds = command.Process(new string[]
            {
                "get_commands"
            }, ctx);
            Assert.Equal(cmds.Split('\n').Length, command.SubCommands.Count);
        }


        private class MockScreenManager : ScreenManager
        {
            public override string AskInvisible(string prefix)
            {
                return "12345";
            }
        }
        
        [Fact]
        public void TestCreateListAccount()
        {   
            string tmpPath = Path.Combine(Path.GetTempPath(), "TestCreateListAccount");
            if (Directory.Exists(tmpPath))
            {
                Directory.Delete(tmpPath, true);
            }

            Directory.CreateDirectory(tmpPath);
            var ctx = new AElfClientProgramContext(new MockScreenManager(), tmpPath);
            var command = new RootCommand();
            for (var i = 0; i < 2; ++i)
            {
                var result = command.Process(new string[]
                {
                    "account",
                    "new"
                }, ctx);
                Assert.Equal(result, AccountCommand.MsgAccountCreated);
            }

            var listResult = command.Process(new string[]
            {
                "account",
                "list"
            }, ctx);
            Assert.Equal(2, listResult.Split('\n').Length);

            var addr = ctx.KeyStore.ListAccounts()[0];
            var unlockResult = command.Process(new string[]
            {
                "account",
                "unlock",
                addr
            }, ctx);
            Assert.Equal(unlockResult, "account successfully unlocked!");
        }

        [Fact]
        public void TestGetUsage()
        {
            var rootCmd = new RootCommand();
            var usage = rootCmd.Usage;
            // TODO Make a better way to check usage correct.
            Assert.NotEmpty(usage);
        }
    }
}