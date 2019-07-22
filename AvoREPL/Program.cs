using System;
using System.Linq;
using SnmpSharpNet;

namespace AvoREPL
{
    class Program
    {
        static void Main(string[] args)
        {

            while (true)
            {
                Console.Write("> ");
                var cmd = Console.ReadLine();
                if (string.IsNullOrEmpty(cmd))
                    break;

                var parts = cmd.Split();

                try
                {
                    switch (parts.First().ToLower())
                    {
                    case "discover":
                        {
                            var ip = System.Net.IPAddress.Parse(parts[1].ToLower());
                            var appliance = new AvoCommLib.Appliance();
                            appliance.IPAddress = ip;
                            appliance.Discover();
                        }
                        break;

                    case "snmp":
                        {
                            var ip = System.Net.IPAddress.Parse(parts[1].ToLower());
                            var oid = new Oid(parts[2]);

                            var vb = new Vb(oid);

                            var aidp = new AvoCommLib.Protocols.AIDP(ip);

                            var ret = aidp.SnmpGet(oid);
                            ret.Wait();

                            Console.WriteLine($"{oid}: {ret.Result.Value}");
                        }
                        break;

                    case "snmpnext":
                        {
                            var ip = System.Net.IPAddress.Parse(parts[1].ToLower());
                            var oid = new Oid(parts[2]);

                            var vb = new Vb(oid);

                            var aidp = new AvoCommLib.Protocols.AIDP(ip);

                            var ret = aidp.SnmpGetNext(oid);
                            ret.Wait();

                            Console.WriteLine($"{oid}: {ret.Result.Value}");
                        }
                        break;

                    default:
                        Console.WriteLine($"Unknown input: \"{parts.First()}\"");
                        goto case "help";

                    case "help":
                        Console.WriteLine("Available commands:");
                        Console.WriteLine();
                        Console.WriteLine("- help");
                        Console.WriteLine("- discover IP");
                        Console.WriteLine("- snmp IP OID");
                        Console.WriteLine("- snmpnext IP OID");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            Console.WriteLine();
        }
    }
}
