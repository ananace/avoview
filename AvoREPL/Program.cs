using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lextm.SharpSnmpLib;

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

                                // Console.WriteLine(appliance.SystemName);
                            }
                            break;

                        case "snmpnext":
                            goto case "snmp";
                        case "snmp":
                            {
                                var ip = System.Net.IPAddress.Parse(parts[1].ToLower());
                                var vbl = new List<Variable>();

                                foreach (var part in parts.Skip(2))
                                {
                                    var oid = new ObjectIdentifier(part);
                                    Console.WriteLine($"Adding Oid: {oid}");
                                    var vb = new Variable(oid);

                                    vbl.Add(vb);
                                }

                                var aidp = new AvoCommLib.Protocols.AIDP(ip);

                                Task<List<Variable>> ret;
                                if (parts.First().ToLower() == "snmp")
                                    ret = aidp.SnmpGet(vbl);
                                else
                                    ret = aidp.SnmpGetNext(vbl);

                                ret.Wait();

                                foreach (var vb in ret.Result)
                                    Console.WriteLine($"{AvoCommLib.MIB.MIBCollection.GetFullNameFromOid(vb.Id, false)}: {vb.Data}");
                            }
                            break;
                        
                        case "loadmib":
                            {
                                AvoCommLib.MIB.MIBCollection.LoadXML(parts[1]);
                            }
                            break;

                        default:
                            Console.WriteLine($"Unknown input: \"{parts.First()}\"");
                            goto case "help";

                        case "help":
                            Console.WriteLine("Available commands:");
                            Console.WriteLine();
                            Console.WriteLine("- help");
                            Console.WriteLine("- loadmib FILE.XML");
                            Console.WriteLine("- discover IP");
                            Console.WriteLine("- snmp IP OID...");
                            Console.WriteLine("- snmpnext IP OID...");
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
