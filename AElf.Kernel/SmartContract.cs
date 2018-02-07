﻿using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AElf.Kernel.Extensions;

namespace AElf.Kernel
{
    public class SmartContract : ISmartContract
    {
        private IAccountDataProvider _accountDataProvider;
        public async Task InititalizeAsync(IAccountDataProvider dataProvider)
        {
            _accountDataProvider = dataProvider;
            await Task.CompletedTask;
        }

        public async Task InvokeAsync(IHash<IAccount> caller, string methodname, params object[] objs)
        {
            // get smartContractRegistration by accountDataProvider 
            var smartContractRegistration = (SmartContractRegistration) _accountDataProvider.GetDataProvider()
                .GetDataProvider("SmartContract")
                .GetAsync(new Hash<SmartContractRegistration>(_accountDataProvider.CalculateHashWith("SmartContract")))
                .Result;
            
            // load assembly with bytes
            Assembly assembly = Assembly.Load(smartContractRegistration.Bytes);
            Type type = assembly.GetTypes().ElementAt(0);
            var method = type.GetMethod(methodname);
            await (Task) method.Invoke(this, objs);
        }
    }
}