using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using SharpMonoInjector;
using System.IO.Compression;
using System.Reflection;
using Mono.Cecil;
using NativeNetSharp;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace AutoGunfireReborn
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            var managedDlls = Directory.GetFiles("Managed");
            foreach(var managedDll in managedDlls) if (!File.Exists(Path.GetFileName(managedDll))) File.Copy(managedDll, Path.GetFileName(managedDll));
            var monoInjector = new Injector("Gunfire Reborn");

            var dll = AssemblyDefinition.ReadAssembly("GunfireRebornMods.dll");
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
            monoInjector.Inject(dllBytes, dll.Name.Name, "Init", "Setup");
            Application.Exit();
            Environment.Exit(0);
            InitializeComponent();
        }
    }
}
