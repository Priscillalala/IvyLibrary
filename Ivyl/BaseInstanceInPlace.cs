using BepInEx;
using BepInEx.Logging;
using BepInEx.Bootstrap;
using System.Reflection;
using System;
using System.ComponentModel;

namespace Ivyl
{
    public abstract class BaseInstanceInPlace<TSelf> where TSelf : BaseInstanceInPlace<TSelf>, new()
    {
        private static bool _init;

        protected virtual bool Init() => throw new NotImplementedException();

        protected static void TryInit()
        {
            if (_init)
            {
                return;
            }
            if (!new TSelf().Init())
            {
                throw new InvalidOperationException();
            }
            _init = true;
        }
    }
}