using System.Text;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UnityEngine;
using Microsoft.Win32.SafeHandles;
using Mono.Cecil;

namespace GunfireRebornMods
{
    static class Program
    {
        static unsafe void Main()
        {
            if (Process.GetCurrentProcess().ProcessName == typeof(Program).Namespace)
            {
                var dll = AssemblyDefinition.ReadAssembly(Assembly.GetEntryAssembly().Location);
                var rand = Guid.NewGuid().ToString().Replace("-", "");
                dll.Name.Name += rand;
                dll.MainModule.Name += rand;
                dll.MainModule.Types.ToList().ForEach(t => t.Namespace += rand);
                var dllBytes = new Byte[0];
                using (var newDll = new MemoryStream())
                {
                    dll.Write(newDll);
                    dllBytes = newDll.ToArray();
                }
                NativeNetSharp.Inject("Gunfire Reborn", dllBytes);
            }
            else
            {
                try
                {
                    NativeNetSharp.AllocConsole();
                    var standardOutput = new StreamWriter(new FileStream(new SafeFileHandle(NativeNetSharp.GetStdHandle(-11), true), FileAccess.Write), Encoding.GetEncoding(437)) { AutoFlush = true };
                    Console.SetOut(standardOutput);
                    LogSupport_TraceHandler("C# DLL loaded");
                    Setup();
                }
                catch (Exception e)
                {
                    LogSupport_TraceHandler(e.Message.ToString());
                }
            }
        }

        public static GameObject BaseObject;
        public static void Setup()
        {
            Console.WriteLine(Environment.Version);
            Console.WriteLine(Application.unityVersion);
            Console.WriteLine(Directory.GetCurrentDirectory());
            UnhollowerBaseLib.Runtime.UnityVersionHandler.Initialize(2018, 4, 20);
            LogSupport.RemoveAllHandlers();
            LogSupport.TraceHandler += LogSupport_TraceHandler;
            LogSupport.ErrorHandler += LogSupport_TraceHandler;
            LogSupport.InfoHandler += LogSupport_TraceHandler;
            LogSupport.WarningHandler += LogSupport_TraceHandler;

            //ClassInjector.Detour = new DoHookDetour();
            IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());
            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            ClassInjector.DoHook?.GetInvocationList().ToList().ForEach(d => ClassInjector.DoHook -= (Action<IntPtr, IntPtr>)d);
            ClassInjector.DoHook += JmpPatch;
            ClassInjector.RegisterTypeInIl2Cpp<ModManager>();
            while (BaseObject = GameObject.Find("ModManager")) GameObject.DestroyImmediate(BaseObject);
            BaseObject = new GameObject("ModManager");
            GameObject.DontDestroyOnLoad(BaseObject);

            var modMgr = BaseObject.AddComponent<ModManager>();
            var types = Assembly.GetExecutingAssembly().GetTypes().ToList().Where(t => t.BaseType == typeof(ModBase) && !t.IsNested);
            foreach (var type in types) modMgr.Mods.Add((ModBase)Activator.CreateInstance(type));
        }
        /*unsafe class DoHookDetour : IManagedDetour
        {
            private static readonly List<object> PinnedDelegates = new List<object>();
            public T Detour<T>(IntPtr @from, T to) where T : Delegate
            {
                IntPtr* targetVarPointer = &from;
                PinnedDelegates.Add(to);
                //JmpPatch((IntPtr)targetVarPointer, Marshal.GetFunctionPointerForDelegate(to));
                return Marshal.GetDelegateForFunctionPointer<T>(from);
            }
        }*/

        private static void LogSupport_TraceHandler(string obj)
        {
            File.AppendAllText(@"log.txt", obj + "\n");
            Console.WriteLine(obj);
        }
        [DllImport("kernel32")] static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);
        [DllImport("kernel32")] static extern bool FlushInstructionCache(IntPtr hProcess, IntPtr lpBaseAddress, UIntPtr dwSize);
        [DllImport("kernel32")] static extern IntPtr GetCurrentProcess();
        [DllImport("kernel32")] static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
        [DllImport("kernel32")] static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, Int32 dwSize, Int32 flAllocationType, Int32 flProtect);
        [DllImport("kernel32")] static extern IntPtr GetModuleHandle(string lpModuleName);
        public static void JmpPatch(IntPtr originalPtr, IntPtr replacement)
        {
            var origCodeLoc = Marshal.ReadIntPtr(originalPtr);
            var jmpToNew = new List<Byte>();
            jmpToNew.AddRange(new Byte[] { 0x49, 0xBB }); // mov r11, replacement
            jmpToNew.AddRange(BitConverter.GetBytes(replacement.ToInt64()));
            jmpToNew.AddRange(new Byte[] { 0x41, 0xFF, 0xE3 }); // jmp r11
            var origCode = new byte[0x12];
            Marshal.Copy(origCodeLoc, origCode, 0, origCode.Length);
            var jmpToOrig = new List<Byte>();
            jmpToOrig.AddRange(origCode);
            jmpToOrig.AddRange(new Byte[] { 0x49, 0xBB }); // mov r11, replacement
            jmpToOrig.AddRange(BitConverter.GetBytes((origCodeLoc + origCode.Length).ToInt64()));
            jmpToOrig.AddRange(new Byte[] { 0x41, 0xFF, 0xE3 }); // jmp r11
            var newFuncLocation = VirtualAllocEx(GetCurrentProcess(), IntPtr.Zero, 0x100, 0x3000, 0x40);
            Marshal.Copy(jmpToOrig.ToArray(), 0, newFuncLocation, jmpToOrig.ToArray().Length);

            VirtualProtect(origCodeLoc, (UIntPtr)jmpToNew.ToArray().Length, (UInt32)0x40, out UInt32 old);
            Marshal.Copy(jmpToNew.ToArray(), 0, origCodeLoc, jmpToNew.ToArray().Length);
            FlushInstructionCache(GetCurrentProcess(), origCodeLoc, (UIntPtr)jmpToNew.ToArray().Length);
            VirtualProtect(origCodeLoc, (UIntPtr)jmpToNew.ToArray().Length, old, out UInt32 _);

            Marshal.WriteIntPtr(originalPtr, newFuncLocation);
        }
        public static void JmpUnPatch(IntPtr originalPtr, IntPtr replacement)
        {
            // todo
        }
        unsafe public static void Hook(IntPtr original, IntPtr target)
        {
            IntPtr originalPtr = original;
            IntPtr* targetVarPointer = &originalPtr;
            JmpPatch((IntPtr)targetVarPointer, target);
        }
    }
}
