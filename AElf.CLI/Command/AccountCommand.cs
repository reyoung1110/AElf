﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace AElf.CLI.Command
{
    public class AccountCommand : ComposedCommand
    {
        public AccountCommand()
        {
            SubCommands = new Dictionary<string, ICommand>
            {
                ["list"] = new ListAccountCommand(),
                ["new"] = new NewAccountCommand()
            };
            CurrentCommandName = "account";
        }

        public const string MsgAccountCreated = "account successfully created!";

        private class ListAccountCommand : ICommand
        {
            public string Process(IEnumerable<string> args, AElfClientProgramContext context)
            {
                if (args.Count() != 0)
                {
                    throw new CommandException("account list does not need to take any params");
                }

                var accounts = context.KeyStore.ListAccounts();
                if (accounts.Count != 0)
                {
                    return string.Join("\n",
                        accounts.Zip(Enumerable.Range(0, accounts.Count),
                            (account, id) => $"account #{id} : {account}"));
                }
                else
                {
                    return "no accounts available";
                }
            }

            public string Usage { get; } = "account list";
        }

        private class NewAccountCommand : ICommand
        {
            public string Process(IEnumerable<string> args, AElfClientProgramContext context)
            {
                string pwd;
                var argc = args.Count();
                switch (argc)
                {
                    case 0:
                        pwd = context.ScreenManager.AskInvisible("Password:");
                        break;
                    case 1:
                        pwd = args.First();
                        break;
                    default:
                        throw new CommandException("Invalid parameter number");
                }

                var pair = context.KeyStore.Create(pwd);
                if (pair != null)
                {
                    return MsgAccountCreated;
                }
                else
                {
                    throw new CommandException("Cannot create new account");
                }
            }

            public string Usage { get; } = "account new";
        }
    }
}