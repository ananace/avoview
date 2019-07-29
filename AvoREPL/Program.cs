using System;
using System.IO;
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
            AvoCommLib.MIB.MIBCollection.EnsureRoot();

            for (int i = 0; i < args.Length; ++i)
            {
                var arg = args[i];

                if (arg == "-m")
                {
                    var mibsFolder = args[++i];

                    var files = Directory.GetFiles(mibsFolder, "*.xml");
                    foreach (var file in files)
                    {
                        var cBefore = AvoCommLib.MIB.MIBCollection.RegisteredMIBs.Count;
                        AvoCommLib.MIB.MIBCollection.LoadXML(file);
                        var cAfter = AvoCommLib.MIB.MIBCollection.RegisteredMIBs.Count;

                        Console.WriteLine($"Loaded {cAfter - cBefore} MIB Nodes from {file}");
                    }
                }
            }

            while (true)
            {
                Console.WriteLine();
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
                                var appliance = new AvoCommLib.Appliance {
                                    IPAddress = ip
                                };
                                appliance.Discover();

                                Console.WriteLine("Discovered data:");
                                Console.WriteLine();
                                Console.WriteLine($"          Model: {appliance.Model}");
                                Console.WriteLine($"    MAC Address: {string.Join(":", appliance.MACAddress.Select(b => b.ToString("X2")))}");
                                Console.WriteLine($"     IP Address: {appliance.IPAddress}");
                                Console.WriteLine($" Subnet Address: {appliance.SubnetAddress}");
                                Console.WriteLine($"Gateway Address: {appliance.GatewayAddress}");
                                Console.WriteLine($"       Hostname: {appliance.Hostname}");
                                Console.WriteLine($"           Mode: {appliance.Mode}");
                            }
                            break;

                        case "snmpnext":
                            goto case "snmp";
                        case "snmp":
                            {
                                var ip = System.Net.IPAddress.Parse(parts[1].ToLower());
                                var appliance = new AvoCommLib.Appliance {
                                    IPAddress = ip
                                };
                                var vbl = new List<Variable>();

                                foreach (var part in parts.Skip(2))
                                {
                                    var oid = new ObjectIdentifier(part);
                                    var vb = new Variable(oid);

                                    vbl.Add(vb);
                                }

                                Task<AvoCommLib.Protocols.AIDP.BaseCommand> ret;
                                if (parts.First().ToLower() == "snmp")
                                    ret = appliance.AIDPSession.SendRequest(new AvoCommLib.Protocols.AIDP.SNMPGetRequest { Variables = vbl });
                                else
                                    ret = appliance.AIDPSession.SendRequest(new AvoCommLib.Protocols.AIDP.SNMPGetNextRequest { Variables = vbl });

                                ret.Wait();

                                IEnumerable<Variable> variables = (ret.Result as AvoCommLib.Protocols.AIDP.SNMPResponse).Variables;

                                foreach (var vb in variables)
                                {
                                    AvoCommLib.MIB.MIBNode node;

                                    if (!AvoCommLib.MIB.MIBCollection.HasOid(vb.Id))
                                    {
                                        try {
                                            node = AvoCommLib.MIB.MIBCollection.GetNode(new ObjectIdentifier(vb.Id.ToNumerical().SkipLast(1).ToArray()));
                                        } catch (KeyNotFoundException) {
                                            node = null;
                                        }
                                    }
                                    else
                                        node = AvoCommLib.MIB.MIBCollection.GetNode(vb.Id);

                                    if (node != null && node.Type == AvoCommLib.MIB.MIBType.Int && node.ValueMappings.Any())
                                        Console.WriteLine($"{vb.Id} ({node.Name}): {node.ValueMappings.GetValueOrDefault((vb.Data as Integer32).ToInt32(), vb.Data.ToString())} ({vb.Data})");
                                    else if (node != null)
                                        Console.WriteLine($"{vb.Id} ({node.Name}): {vb.Data}");
                                    else
                                        Console.WriteLine($"{vb.Id}: {vb.Data}");
                                }
                            }
                            break;
                        
                        case "loadmib":
                            {
                                var cBefore = AvoCommLib.MIB.MIBCollection.RegisteredMIBs.Count;
                                AvoCommLib.MIB.MIBCollection.LoadXML(parts[1]);
                                var cAfter = AvoCommLib.MIB.MIBCollection.RegisteredMIBs.Count;

                                Console.WriteLine($"Loaded {cAfter - cBefore} MIB Nodes from {parts[1]}");
                            }
                            break;

                        case "oid":
                            {
                                foreach (var part in parts.Skip(1))
                                {
                                    var oid = new ObjectIdentifier(part);
                                    Console.WriteLine($"Oid {oid}:");

                                    var node = AvoCommLib.MIB.MIBCollection.GetNode(oid);

                                    Console.WriteLine($"Type: {node.Type}");
                                    if (node.Parent != null)
                                        Console.WriteLine($"Parent: {node.Parent.Oid}");
                                    Console.WriteLine($"Full name: {AvoCommLib.MIB.MIBCollection.GetFullNameFromOid(oid)}");

                                    if (node.Type == AvoCommLib.MIB.MIBType.Int)
                                    {
                                        var mappings = (node as AvoCommLib.MIB.MIBNode).ValueMappings;
                                        if (mappings.Any())
                                        {
                                            Console.WriteLine("Values:");
                                            foreach (var kv in mappings)
                                                Console.WriteLine($"- {kv.Key} == {kv.Value}");
                                        }
                                    }

                                    if (node.Children.Any())
                                    {
                                        Console.WriteLine("Children:");
                                        foreach (var child in node.Children)
                                            Console.WriteLine($"- {child.Oid} - {child.Name}");
                                    }

                                    if (part != parts.Last())
                                        Console.WriteLine();
                                }
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
                            Console.WriteLine("- loadmib FILE.XML");
                            Console.WriteLine("- oid OID...");
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
