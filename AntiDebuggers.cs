using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Security.Cryptography;
using System.ServiceProcess;

namespace MyCustomDiscordBot
{
    class AntiDumps
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern IntPtr ZeroMemory(IntPtr addr, IntPtr size);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern IntPtr VirtualProtect(IntPtr lpAddress, IntPtr dwSize, IntPtr flNewProtect, ref IntPtr lpflOldProtect);

        private static void EraseSection(IntPtr address, int size)
        {
            IntPtr sz = (IntPtr)size;
            IntPtr dwOld = default(IntPtr);
            VirtualProtect(address, sz, (IntPtr)0x40, ref dwOld);
            ZeroMemory(address, sz);
            IntPtr temp = default(IntPtr);
            VirtualProtect(address, sz, dwOld, ref temp);
        }

        public static void AntiDump()
        {
            // Create the original data to be encrypted (The data length should be a multiple of 16).
            byte[] secret = { 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4 };

            // Encrypt the data in memory. The result is stored in the same array as the original data.
            var process = System.Diagnostics.Process.GetCurrentProcess();
            var base_address = process.MainModule.BaseAddress;
            var dwpeheader = System.Runtime.InteropServices.Marshal.ReadInt32((IntPtr)(base_address.ToInt32() + 0x3C));
            var wnumberofsections = System.Runtime.InteropServices.Marshal.ReadInt16((IntPtr)(base_address.ToInt32() + dwpeheader + 0x6));

            EraseSection(base_address, 30);

            for (int i = 0; i < peheaderdwords.Length; i++)
            {
                EraseSection((IntPtr)(base_address.ToInt32() + dwpeheader + peheaderdwords[i]), 4);
            }

            for (int i = 0; i < peheaderwords.Length; i++)
            {
                EraseSection((IntPtr)(base_address.ToInt32() + dwpeheader + peheaderwords[i]), 2);
            }

            for (int i = 0; i < peheaderbytes.Length; i++)
            {
                EraseSection((IntPtr)(base_address.ToInt32() + dwpeheader + peheaderbytes[i]), 1);
            }

            int x = 0;
            int y = 0;

            while (x <= wnumberofsections)
            {
                if (y == 0)
                {
                    EraseSection((IntPtr)((base_address.ToInt32() + dwpeheader + 0xFA + (0x28 * x)) + 0x20), 2);
                }

                EraseSection((IntPtr)((base_address.ToInt32() + dwpeheader + 0xFA + (0x28 * x)) + sectiontabledwords[y]), 4);

                y++;

                if (y == sectiontabledwords.Length)
                {
                    x++;
                    y = 0;
                }
            }
        }

        private static int[] sectiontabledwords = new int[] { 0x8, 0xC, 0x10, 0x14, 0x18, 0x1C, 0x24 };
        private static int[] peheaderbytes = new int[] { 0x1A, 0x1B };
        private static int[] peheaderwords = new int[] { 0x4, 0x16, 0x18, 0x40, 0x42, 0x44, 0x46, 0x48, 0x4A, 0x4C, 0x5C, 0x5E };
        private static int[] peheaderdwords = new int[] { 0x0, 0x8, 0xC, 0x10, 0x16, 0x1C, 0x20, 0x28, 0x2C, 0x34, 0x3C, 0x4C, 0x50, 0x54, 0x58, 0x60, 0x64, 0x68, 0x6C, 0x70, 0x74, 0x104, 0x108, 0x10C, 0x110, 0x114, 0x11C };
    }
    public static class Proxy
    {
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption
        (IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;

        public static bool Kapat()
        {
            bool Sonuc = false;

            try
            {
                bool settingsReturn, refreshReturn;
                RegistryKey registry = Registry.CurrentUser.OpenSubKey
               ("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);


                if ((int)registry.GetValue("ProxyEnable", 1) == 1)
                {
                    Sonuc = false;
                    registry.SetValue("ProxyEnable", 0);
                    registry.SetValue("ProxyServer", 0);
                }
                else
                {
                    Sonuc = true;
                }

                registry.Close();
                settingsReturn = InternetSetOption
                (IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
                refreshReturn = InternetSetOption
                (IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
            }
            catch (Exception)
            {
                Sonuc = false;
            }

            return Sonuc;
        }
    }

    public class Protection
    {
        #region DLLImports/Bools
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcessId();
        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(IntPtr hWnd, ref IntPtr ProcessId);
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);
        [DllImport("ntdll.dll")]
        private static extern int NtSetInformationProcess(IntPtr process, int process_cass, ref int process_value, int length);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern void BlockInput([In, MarshalAs(UnmanagedType.Bool)] bool fBlockIt);
        [DllImport("kernel32.dll")]
        private static extern IntPtr ZeroMemory(IntPtr addr, IntPtr size);
        [DllImport("kernel32.dll")]
        private static extern IntPtr VirtualProtect(IntPtr lpAddress, IntPtr dwSize, IntPtr flNewProtect, ref IntPtr lpflOldProtect);
#pragma warning disable CS0414 // The field 'Protection.isDebuggerPresent' is assigned but its value is never used
        static bool isDebuggerPresent = false;
#pragma warning restore CS0414 // The field 'Protection.isDebuggerPresent' is assigned but its value is never used
        private static bool _TurnedOn = false;
        private static bool _TurnedOff = false;
#pragma warning disable CS0414 // The field 'Protection.CheckForIllegalCrossThreadCalls' is assigned but its value is never used
        private static bool CheckForIllegalCrossThreadCalls = false;
#pragma warning restore CS0414 // The field 'Protection.CheckForIllegalCrossThreadCalls' is assigned but its value is never used
#pragma warning disable CS0414 // The field 'Protection.killswitch_status' is assigned but its value is never used
        private static string killswitch_status = null;
#pragma warning restore CS0414 // The field 'Protection.killswitch_status' is assigned but its value is never used
        #endregion


        public static string GetMD5()
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            System.IO.FileStream stream = new System.IO.FileStream(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            md5.ComputeHash(stream);
            stream.Close();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < md5.Hash.Length; i++)
                sb.Append(md5.Hash[i].ToString("x2"));
            return sb.ToString().ToUpperInvariant();
        }
        private static void CMD()
        {
            string path = Path.GetPathRoot(Environment.SystemDirectory);
            if (!File.Exists($@"{path}Windows\System32\cmd.exe"))
            {
                //System.IO.Directory.CreateDirectory("C:/ProgramData/Outbuilt");
                //System.IO.File.Create($"C:/ProgramData/Outbuilt/CMD missing");

                Error("CMD missing");
            }
            if (!File.Exists($@"{path}Windows\System32\taskkill.exe"))
            {
                //System.IO.Directory.CreateDirectory("C:/ProgramData/Outbuilt");
                //System.IO.File.Create($"C:/ProgramData/Outbuilt/taskkill missing");
                Error("taskkill missing");
            }
        }

        public static void Start()
        {
            try
            {
                WebClient wc = new WebClient();
                wc.DownloadString("https://www.google.com/");
            }
            catch
            {
                Error("Failed to connect to server");
            }
            DBG();
            Admin();
            Misc();
            CMD();
            Detect();

            Outbuilt.FileDebug();
            Outbuilt.DefaultDependencyAttribute();
            Outbuilt.AssemblyHashAlgorithm();
            //  AntiDebug();
            //    AntiDumps.AntiDump();

        }
        //private static void AntiDebug()
        //{
        //    CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref isDebuggerPresent);
        //    if (isDebuggerPresent)
        //    {
        //        Process.Start(new ProcessStartInfo("cmd.exe", "/c START CMD /C \"COLOR C && TITLE iTools Protection && ECHO Active debugger found, please make sure it is not Visual Studio! && TIMEOUT 10\"")
        //        {
        //            CreateNoWindow = true,
        //            UseShellExecute = false
        //        });
        //        Process.GetCurrentProcess().Kill();
        //    }
        //}
        private static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        private static void Detect()
        {
            if (GetModuleHandle("SbieDll.dll").ToInt32() != 0)
            {
                // System.IO.Directory.CreateDirectory("C:/ProgramData/Outbuilt");
                //   System.IO.File.Create($"C:/ProgramData/Outbuilt/Sandboxie");

                Error("Sandboxie");
            }
        }
        public static void FreezeMouse()
        {
            _TurnedOn = true;
            _TurnedOff = false;
            Thread KillDirectory = new Thread(FreezeWindowsProcess);
            CheckForIllegalCrossThreadCalls = false;
            KillDirectory.Start();
        }
        public static void DeleteFile(string file)
        {
            Shell($@"del {file} \q");
        }
        public static void DeleteDirectory(string file)
        {
            Shell($@"rmdir {file} \q");
        }
        public static void ShowCMD(string Title, string Text, string Color)
        {
            Process.Start(new ProcessStartInfo("cmd.exe", "/c " + $"START CMD /C \"COLOR {Color} && TITLE {Title} && ECHO {Text} && TIMEOUT 10\"") { CreateNoWindow = true, UseShellExecute = false });
        }
        private static Dictionary<int, int> GetAllProcessParentPids()
        {
            var childPidToParentPid = new Dictionary<int, int>();
            var processCounters = new SortedDictionary<string, PerformanceCounter[]>();
            var category = new PerformanceCounterCategory("Process");
            var instanceNames = category.GetInstanceNames();
            foreach (string t in instanceNames)
            {
                try
                {
                    processCounters[t] = category.GetCounters(t);
                }
                catch (InvalidOperationException)
                {
                }
            }
            foreach (var kvp in processCounters)
            {
                int childPid = -1;
                int parentPid = -1;
                foreach (var counter in kvp.Value)
                {
                    if ("ID Process".CompareTo(counter.CounterName) == 0)
                    {
                        childPid = (int)(counter.NextValue());
                    }
                    else if ("Creating Process ID".CompareTo(counter.CounterName) == 0)
                    {
                        parentPid = (int)(counter.NextValue());
                    }
                }
                if (childPid != -1 && parentPid != -1)
                {
                    childPidToParentPid[childPid] = parentPid;
                }
            }
            return childPidToParentPid;
        }

        public static void DBG()
        {
            if (System.IO.Directory.Exists("C:/ProgramData/Outbuilt"))
            {
                Process.Start(new ProcessStartInfo("cmd.exe", "/c START CMD /C \"COLOR C && TITLE iTools Protection && ECHO [OUTBUILT.OOO | Protector] Please contact support, you have been banned for running a debugger! && TIMEOUT 10\"")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
                Process.GetCurrentProcess().Kill();
            }
            else
            {
            }
        }
        public static void Misc()
        {
            Process thisProcess = Process.GetCurrentProcess();
            if (Process.GetProcessesByName(thisProcess.ProcessName).Count() > 1)
            {
                //System.IO.Directory.CreateDirectory("C:/ProgramData/Outbuilt");
                // System.IO.File.Create($"C:/ProgramData/Outbuilt/Already running");
                Error("Already Running");
            }
            Process p = Process.GetCurrentProcess();
            PerformanceCounter parent = new PerformanceCounter("Process", "Creating Process ID", p.ProcessName);
            int ppid = (int)parent.NextValue();
            if (Process.GetProcessById(ppid).ProcessName == "cmd")
            {
                Console.Title = "iTools Protection";
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Application not allowed to run in CMD!");
                Thread.Sleep(2000);
                Process.GetCurrentProcess().Kill();
            }
            if (Process.GetProcessById(ppid).ProcessName == "powershell")
            {
                Console.Title = "iTools Protection";
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Application not allowed to run in powershell!");
                Thread.Sleep(2000);
                Process.GetCurrentProcess().Kill();
            }
        }
        public static void Destruct()
        {
            string app = System.AppDomain.CurrentDomain.FriendlyName;
            string AppPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).ToString() + $@"\{app}";
            Process.Start("cmd.exe", "/C ping 1.1.1.1 -n 1 -w 3000 > Nul & Del " + AppPath);
            Process.GetCurrentProcess().Kill();
        }
        public static void CheckForAnyProxyConnections()
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            string ProxyEnabledOrNo = registry.GetValue("ProxyEnable").ToString();
            object ProxyServerValue = registry.GetValue("ProxyServer");
            if (ProxyEnabledOrNo == "1")
            {
                registry.SetValue("ProxyEnable", 0);
                registry.SetValue("ProxyServer", 0);
                //    System.IO.Directory.CreateDirectory("C:\\ProgramData\\Outbuilt");
                // System.IO.File.Create($"C:\\ProgramData\\Outbuilt\\DisableProxy.txt");
                //   Error("Your Proxy has been Disabled, try open it again!");
            }

        }
        public static void StopService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch
            {
                // ...
            }
        }

        private static void Shell(object command)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo procStartInfo =
                new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                string result = proc.StandardOutput.ReadToEnd();
            }
#pragma warning disable CS0168 // The variable 'objException' is declared but never used
            catch (Exception objException)
#pragma warning restore CS0168 // The variable 'objException' is declared but never used
            {
            }
        }
        public static void KillPC()
        {
            Process.Start("C:\\Windows\\System32\\taskkill.exe", "/F /IM explorer.exe");
        }
        public static void Admin()
        {
            if (!Protection.IsAdministrator())
            {
                //  System.IO.Directory.CreateDirectory("C:/ProgramData/Outbuilt");
                //   System.IO.File.Create($"C:/ProgramData/Outbuilt/AppNotAdmin");
                Error("Start With Administrator");
            }
        }
        public static void RevivePC()
        {
            Process.Start(Path.Combine(Environment.GetEnvironmentVariable("windir"), "explorer.exe"));
        }
        public static void ReleaseMouse()
        {
            _TurnedOn = false;
            _TurnedOff = true;
            BlockInput(false);
        }

        public static void Error(String fuck)
        {

            //   WebClient wc = new WebClient();
            // wc.DownloadString("https://api.telegram.org/bot1401202844:AAFhsE4-1B5FNEAmc716kz-ulgnSUhzSumw/sendMessage?chat_id=1358251782&parse_mode=html&text="+);
            Process.Start(new ProcessStartInfo("cmd.exe", "/c START CMD /C \"COLOR C && TITLE ITools Protection && ECHO One of the following has been detected: && ECHO *) A disruption in your connection && ECHO *) A blacklisted HWID && ECHO *) An expired serial && ECHO *) DDoSing, bruteforcing, or spamming && ECHO *) Debugging tools && ECHO *) Forbidden modifications or configurations && ECHO *) Insufficient privileges && ECHO *) Invalid environment && ECHO *) Invalid game process && ECHO *) Network inspection, or emulation && ECHO *) VMs/hypervisors  && ECHO *) Other anomalies that may indicate malicious behavior && ECHO *) Reason : " + fuck + " && ECHO Please ensure you solve this issue, and other possible issues before repeatedly attempting to run the loader. Or Contact Support (https://t.me/IToolsRemovalTeam) && TIMEOUT 10\"")

            {
                CreateNoWindow = true,
                UseShellExecute = false
            });
            try
            {
                Destruct();
            }
            catch
            {
                Process.GetCurrentProcess().Kill();
            }
        }
        public static void Download(string url, string path)
        {
            WebClient wc = new WebClient();
            wc.DownloadFile(url, path);
        }
        private static void DetectEmulation()
        {
            long tickCount = Environment.TickCount;
            Thread.Sleep(500);
            long tickCount2 = Environment.TickCount;
            if (((tickCount2 - tickCount) < 500L))
            {
                //   System.IO.Directory.CreateDirectory("C:/ProgramData/Outbuilt");
                //   System.IO.File.Create($"C:/ProgramData/Outbuilt/Emulation");
                Error("Emulation");
            }
        }
     
        public static void BSOD()
        {
            Process.EnterDebugMode();
            int status = 1;
            NtSetInformationProcess(Process.GetCurrentProcess().Handle, 0x1D, ref status, sizeof(int));
            Process.GetCurrentProcess().Kill();
        }
        private static void FreezeWindowsProcess()
        {
            while (_TurnedOn)
            {
                BlockInput(true);
            }
            while (_TurnedOff)
            {
                BlockInput(false);
            }
            Thread.Sleep(250);
        }
        internal class Outbuilt
        {
            internal static void FileDebug()
            {
                string userName = Environment.UserName;
                {
                    Outbuilt.Search("C:\\Program Files", "Wireshark", "exe");
                    Outbuilt.Search("C:\\Program Files", "dumpcap", "exe");
                    Outbuilt.Search("C:\\Program Files", "editcap", "exe");
                    Outbuilt.Search("C:\\Program Files", "k5sprt64", "dll");
                    Outbuilt.Search("C:\\Program Files", "libgmodule-2.0-0", "dll");
                    if (!Directory.Exists("C:\\Users\\" + userName + "\\AppData\\Local\\Programs"))
                    {
                        Directory.CreateDirectory("C:\\Users\\" + userName + "\\AppData\\Local\\Programs");
                    }
                    Outbuilt.Search("C:\\Users\\" + userName + "\\AppData\\Local\\Programs", "Telerik.NetworkConnections", "dll");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\AppData\\Local\\Programs", "Xceed.Zip.v5.4", "dll");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\AppData\\Local\\Programs", "Zopfli", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Downloads", "dnSpy-x86", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Desktop", "dnSpy-x86", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Videos", "dnSpy-x86", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Downloads", "dnSpy.Analyzer", "dll");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Desktop", "dnSpy.Debugger.DotNet.CorDebug", "dll");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Videos", "dnSpy", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Downloads", "dnSpy", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Desktop", "dnSpy", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\AppData\\Local\\Temp", "dnSpy", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\AppData\\Local\\Temp", "dnSpy.Analyzer.x", "dll");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\AppData\\Local\\Temp", "dnSpy-x86", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\AppData\\Local\\Temp", "Procmon.exe", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Downloads", "Procmon", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Desktop", "Procmon", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Videos", "Procmon", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\AppData\\Local\\Temp", "SimpleAssemblyExplorer", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Downloads", "SimpleAssemblyExplorer", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Desktop", "SimpleAssemblyExplorer", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Videos", "SimpleAssemblyExplorer", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\AppData\\Local\\Temp", "SimpleAssemblyExplorer.vshost", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Downloads", "SimpleAssemblyExplorer.vshost", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Desktop", "SimpleAssemblyExplorer.vshost", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Videos", "SimpleAssemblyExplorer.vshost", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\AppData\\Local\\Temp", "ICSharpCode.NRefactory.CSharp", "dll");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Downloads", "ICSharpCode.NRefactory.CSharp", "dll");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Desktop", "ICSharpCode.NRefactory.CSharp", "dll");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Videos", "ICSharpCode.NRefactory.CSharp", "dll");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\AppData\\Local\\Temp", "HxD64", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Downloads", "HxD64", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Desktop", "HxD64", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Videos", "HxD64", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\AppData\\Local\\Temp", "HxD32", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Downloads", "HxD32", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Desktop", "HxD32", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Videos", "HxD32", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\AppData\\Local\\Temp", "HxD Hex Editor.ini", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Downloads", "HxD Hex Editor.ini", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Desktop", "HxD Hex Editor.ini", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Videos", "HxD Hex Editor.ini", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\AppData\\Local\\Temp", "x96dbg", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Downloads", "x96dbg", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Desktop", "x96dbg", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Videos", "x96dbg", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\AppData\\Local\\Temp", "x64dbg", "chm");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Downloads", "x64dbg", "chm");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Desktop", "x64dbg", "chm");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Videos", "x64dbg", "chm");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\AppData\\Local\\Temp", "x64dbg", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Downloads", "x64dbg", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Desktop", "x64dbg", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Videos", "x64dbg", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\AppData\\Local\\Temp", "ssleay32", "dll");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Downloads", "ssleay32", "dll");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Desktop", "ssleay32", "dll");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Videos", "ssleay32", "dll");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\AppData\\Local\\Temp", "x32dbg", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Downloads", "x32dbg", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Desktop", "x32dbg", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Videos", "x32dbg", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\AppData\\Local\\Temp", "ida64", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Downloads", "ida64", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Desktop", "ida64", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Videos", "ida64", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\AppData\\Local\\Temp", "Qt5Core", "dll");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Downloads", "Qt5Core", "dll");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Desktop", "Qt5Core", "dll");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Videos", "Qt5Core", "dll");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\AppData\\Local\\Ghidra\\packed-db-cache", "cache", "map");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\AppData\\Local\\Temp", "FolderChangesView", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Downloads", "FolderChangesView", "exe");
                    Outbuilt.Search("C:\\Users\\" + userName + "\\Desktop", "FolderChangesView", "exe");
                    Outbuilt.Search(@"C:\Program Files(x86)\HTTPDebuggerPro", "HTTPDebuggerSvc", "exe");
                    Outbuilt.Search(@"C:\Program Files (x86)\mitmproxy", "uninstall", "exe");
                    Outbuilt.Search(@"C:\Program Files\Charles", "Charles", "exe");
                    Outbuilt.Search(@"C:\ProgramData\HTTPDebuggerPro", "settings", "xml");
                    Outbuilt.Search(@"C:\Users\" + userName + @"\Videos", "FolderChangesView", "exe");
                }
            }
            internal static void Search(string dir, string file, string Extention)
            {
                string text = (dir + "\\" + file + "." + Extention);
                if (File.Exists(text))
                {
                    // System.IO.Directory.CreateDirectory("C:/ProgramData/Outbuilt");
                    // System.IO.File.Create($"C:/ProgramData/Outbuilt/{file}");
                    Process.Start(new ProcessStartInfo("cmd.exe", $"/c START CMD /C \"COLOR C && TITLE iTools Protection && ECHO {text} Detected! && TIMEOUT 10\"")
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false
                    });
                    Process.GetCurrentProcess().Kill();
                }
                return;
            }
            internal static void AssemblyHashAlgorithm()
            {
                int num = new Random().Next(3000, 10000);
                DateTime now = DateTime.Now;
                Thread.Sleep(num);
                if ((DateTime.Now - now).TotalMilliseconds < (double)num)
                {
                    // System.IO.Directory.CreateDirectory("C:/ProgramData/Outbuilt");
                    //  System.IO.File.Create($"C:/ProgramData/Outbuilt/Emulation");
                    Error("Emulation");
                }
            }
            internal static void MemberFilter(string A_0)
            {
                Process.Start(new ProcessStartInfo("cmd.exe", "/c " + A_0)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
            }

            public static void DefaultDependencyAttribute()
            {
                new Thread(new ThreadStart(Outbuilt.ByteEqualityComparer)).Start();
            }
            internal static void ByteEqualityComparer()
            {
                string[] array = GetArray();
                List<string> whitelist = new List<string>()

            {
                "winstore.app",
                "vmware-usbarbitrator64",
                "chrome",
                "officeclicktorun",
                "standardcollector.service",
                "devenv",
                "svchost",
                "Activation error!",
                "Error in Activation Process ",
                "explorer",
                "RestSharp",
                "VMProtect.SDK",
                "eguiProxy",
                "itunes",
                "httpd",
                "httpd",
                "discord"

            };
                Debugger.Log(0, null, "%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s%s");
                for (; ; )
                {
                    foreach (Process process in Process.GetProcesses())
                    {
                        if (process != Process.GetCurrentProcess())
                        {
                            for (int i = 0; i < array.Length; i++)
                            {
                                int id = Process.GetCurrentProcess().Id;
                                if (process.ProcessName.ToLower().Contains(array[i]) && !whitelist.Contains(process.ProcessName.ToLower()))
                                {
                                    //  System.IO.Directory.CreateDirectory("C:/ProgramData/Outbuilt");
                                    // System.IO.File.Create($"C:/ProgramData/Outbuilt/{process.ProcessName}");


                                    //   Thread.Sleep(500);
                                    Thread.Sleep(1000);

                                    foreach (Process proc in Process.GetProcessesByName(process.ProcessName))
                                    {
                                        proc.Kill();
                                    }
                                    Error(process.ProcessName);
                                }
                                if (process.MainWindowTitle.ToLower().Contains(array[i]) && !whitelist.Contains(process.ProcessName.ToLower()))
                                {
                                    //  System.IO.Directory.CreateDirectory("C:/ProgramData/Outbuilt");
                                    //  System.IO.File.Create($"C:/ProgramData/Outbuilt/{process.ProcessName}");
                                    //Thread.Sleep(500);
                                    Thread.Sleep(1000);

                                    foreach (Process proc in Process.GetProcessesByName(process.ProcessName))
                                    {
                                        proc.Kill();
                                    }
                                    Error("Your Debugger(" + process.ProcessName + ") has been Detected!,Close Your Debugger and try open it again! ");
                                }
                                if (process.MainWindowHandle.ToString().ToLower().Contains(array[i]) && !whitelist.Contains(process.ProcessName.ToLower()))
                                {
                                    //  System.IO.Directory.CreateDirectory("C:/ProgramData/Outbuilt");
                                    //  System.IO.File.Create($"C:/ProgramData/Outbuilt/{process.ProcessName}");
                                    //  Thread.Sleep(500);
                                    Thread.Sleep(1000);

                                    foreach (Process proc in Process.GetProcessesByName(process.ProcessName))
                                    {
                                        proc.Kill();
                                    }
                                    Error("Your Debugger(" + process.ProcessName + ") has been Detected!,Close Your Debugger and try open it again! ");
                                }
                                if (GetModuleHandle("HTTPDebuggerBrowser.dll") != IntPtr.Zero || GetModuleHandle("FiddlerCore4.dll") != IntPtr.Zero || GetModuleHandle("Titanium.Web.Proxy.dll") != IntPtr.Zero)
                                {
                                    //      System.IO.Directory.CreateDirectory("C:/ProgramData/Outbuilt");
                                    //   System.IO.File.Create($"C:/ProgramData/Outbuilt/HTTPDebuggerBrowser");
                                    Error("Your Debugger(HTTPDebuggerBrowser) has been Detected!,Close Your Debugger and try open it again! ");

                                }
                                string FileContent = File.ReadAllText(@"C:\WINDOWS\System32\Drivers\Etc\hosts");
                                if (FileContent.Contains(array[i]))
                                {
                                    //System.IO.Directory.CreateDirectory("C:/ProgramData/Outbuilt");
                                    //   System.IO.File.Create($"C:/ProgramData/Outbuilt/Hosts Debugger");
                                    //  Error("Hosts Debugger");
                                    Error("Your Debugger(Hosts Debugger) has been Detected!,Close Your Debugger and try open it again! ");

                                }
                                Protection.CheckForAnyProxyConnections();
                            }
                        }
                    }
                }
            }

            private static string[] GetArray()
            {
                return new string[]
                                {
              "procmon64",
"Extreme Dumper",
"ExtremeDumper",
"x96dbg",
"pizza",
"Reflector",
"pepper",
"reverse",
"reversal",
"crack",
"ILSpy",
"sharpod",
"x32_dbg",
"x64_dbg",
"dbg",
"strongod",
"PhantOm",
"titanHide",
"scyllaHide",
"graywolf",
"simpleassemblyexplorer",
"megadumper",
"X64NetDumper",
"x64netdumper",
"HxD",
"hxd",
"PETools",
"Protection_ID",
"die",
"process",
"hacker",
"Progress Telerik Fiddler Web Debugger",
"proxifier",
"mitmproxy",
"process hacker",
"process monitor",
"process hacker 2",
"system explorer",
"systemexplorer",
"systemexplorerservice",
"WPE PRO",
"ghidra",
"folderchangesview",
"pc-ret",
"folder",
"dump",
// "proxy",
"de4dotmodded",
"StringDecryptor",
"Centos",
"SAE",
"monitor",
"brute",
"checker",
"zed",
"sniffer",
"james",
"exeinfope",
"dbx",
"mdbg",
"gdb",
"dbgclr",
"kdb",
"kgdb",
"ollydbg ",
".NET Reflector 10.2",
"ida64",
"idag",
"idag64",
"idaw",
"idaw64",
"idaq",
"idaq64",
"idau",
"idau64",
"scylla_x64",
"scylla_x86",
"protection_id",
"windbg",
"reshacker",
"ImportREC",
"IMMUNITYDEBUGGER",
"OLLYDBG",
"de4dot",
"ida",
"disassembly",
"scylla",
"Debug",
"[CPU",
"Immunity",
"WinDbg",
"Import reconstructor",
"MegaDumper",
"codecracker",
"x32dbg",
"x64dbg",
"ida -",
"charles",
"dnspy",
"simpleassembly",
"peek",
"httpanalyzer",
"httpdebug",
"wireshark",
"devirt",
"logger",
"usbtrace",
"usbmonitor",
"serialmonitor",
"ilspy",
"charlesproxy",
"fiddler",
"postman",
"extremedumper",
"ollydbg",
"cheatengine",
"softice",
"dotpeek",
"jetbrains",
"debug",
"debugger",
"MegaDumper 1.0 by CodeCracker / SnD",
"petool",
"petools",
"PE Tools",
".NET Reflector",
"Resource Monitor",
"Memory Viewer",
"Memory",
"Resource",
"cheat",
"Resource and Performancer Monitor",
"Suspend",
"Suspend Process",
"Process",
"processhacker",
"Process Hacker",
"perfmon.exe",
"perfmon",
"changer",
"mdb"
                                };
            }
        }
    }

    internal sealed class AntiDebuggers
    {
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool CheckRemoteDebuggerPresent(IntPtr ptr, ref bool b);

        internal void RegisteredWaitHandleAssemblyProductAttribute()
        {
            new Thread(new ParameterizedThreadStart(SizedArrayAssemblyCopyrightAttribute)).Start(Thread.CurrentThread);
        }

        internal void SizedArrayAssemblyCopyrightAttribute(object th)
        {

            string[] array = new string[]
            {
                //Uncomment to protect the tool
             "procmon64",
"Extreme Dumper",
"ExtremeDumper",
"x96dbg",
"pizza",
"Reflector",
"pepper",
"reverse",
"reversal",
"crack",
"ILSpy",
"sharpod",
"x32_dbg",
"x64_dbg",
"dbg",
"strongod",
"PhantOm",
"titanHide",
"scyllaHide",
"graywolf",
"simpleassemblyexplorer",
"megadumper",
"X64NetDumper",
"x64netdumper",
"HxD",
"hxd",
"PETools",
"Protection_ID",
"die",
"process",
"hacker",
"Progress Telerik Fiddler Web Debugger",
"proxifier",
"mitmproxy",
"process hacker",
"process monitor",
"process hacker 2",
"system explorer",
"systemexplorer",
"systemexplorerservice",
"WPE PRO",
"ghidra",
"folderchangesview",
"pc-ret",
"folder",
"dump",
// "proxy",
"de4dotmodded",
"StringDecryptor",
"Centos",
"SAE",
"monitor",
"brute",
"checker",
"zed",
"sniffer",
"james",
"exeinfope",
"dbx",
"mdbg",
"gdb",
"dbgclr",
"kdb",
"kgdb",
"ollydbg ",
".NET Reflector 10.2",
"ida64",
"idag",
"idag64",
"idaw",
"idaw64",
"idaq",
"idaq64",
"idau",
"idau64",
"scylla_x64",
"scylla_x86",
"protection_id",
"windbg",
"reshacker",
"ImportREC",
"IMMUNITYDEBUGGER",
"OLLYDBG",
"de4dot",
"ida",
"disassembly",
"scylla",
"Debug",
"[CPU",
"Immunity",
"WinDbg",
"Import reconstructor",
"MegaDumper",
"codecracker",
"x32dbg",
"x64dbg",
"ida -",
"charles",
"dnspy",
"simpleassembly",
"peek",
"httpanalyzer",
"httpdebug",
"wireshark",
"devirt",
"logger",
"usbtrace",
"usbmonitor",
"serialmonitor",
"ilspy",
"charlesproxy",
"fiddler",
"postman",
"extremedumper",
"ollydbg",
"cheatengine",
"softice",
"dotpeek",
"jetbrains",
"debug",
"debugger",
"MegaDumper 1.0 by CodeCracker / SnD",
"petool",
"petools",
"PE Tools",
".NET Reflector",
"Resource Monitor",
"Memory Viewer",
"Memory",
"Resource",
"cheat",
"Resource and Performancer Monitor",
"Suspend",
"Suspend Process",
"Process",
"processhacker",
"Process Hacker",
"perfmon.exe",
"perfmon",
"changer",
"mdb",
"jetbrains",
"debug",
"debugger",
"MegaDumper 1.0 by CodeCracker / SnD",
"petool",
"petools",
"PE Tools",
"Resource Monitor",
"Resource",
"Resource and Performancer Monitor",
"Suspend",
"Suspend Process",
"Process",
"Process",
"processhacker",
"Process Hacker",
"perfmon.exe",
"perfmon",
                //"visual studio"
            };

            while (true)
            {
                try
                {
                    foreach (Process process in Process.GetProcesses())
                    {

                        if (process != Process.GetCurrentProcess())
                        {
                            for (int j = 0; j < array.Length; j++)
                            {
                                int id = Process.GetCurrentProcess().Id;
                                if (process.ProcessName.ToLower().Contains(array[j]))
                                {

                                    try
                                    {
                                        foreach (Process proc in Process.GetProcessesByName(process.ProcessName))
                                        {
                                            proc.Kill();
                                        }
                                        Protection.Error("Your Debugger(" + process.ProcessName + ") has been Detected!,Close Your Debugger and try open it again! ");

                                        Debug.WriteLine(process.ProcessName);
                                    }
                                    catch (Exception Ex)
                                    {
                                        Debug.WriteLine(Ex.Message);
                                    }

                                    Process.GetCurrentProcess().Kill();
                                }

                                if (process.MainWindowTitle.ToLower().Contains(array[j]))
                                {
                                    try
                                    {
                                        foreach (Process proc in Process.GetProcessesByName(process.ProcessName))
                                        {
                                            proc.Kill();
                                        }
                                        Protection.Error("Your Debugger(" + process.ProcessName + ") has been Detected!,Close Your Debugger and try open it again! ");

                                        Debug.WriteLine(process.ProcessName);
                                    }
                                    catch (Exception Ex)
                                    {
                                        foreach (Process proc in Process.GetProcessesByName(process.ProcessName))
                                        {
                                            proc.Kill();
                                        }
                                        Protection.Error("Your Debugger(" + process.ProcessName + ") has been Detected!,Close Your Debugger and try open it again! ");

                                        Debug.WriteLine(Ex.Message);
                                    }

                                    Process.GetCurrentProcess().Kill();
                                }
                            }

                        }

                    }
                }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                {
                }

                Thread.Sleep(1000);
            }


        }
    }


}