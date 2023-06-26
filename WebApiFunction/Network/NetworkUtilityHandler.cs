

using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Security.Cryptography.Xml;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace WebApiFunction.Network
{
    public class NetworkUtilityHandler
    {
        public class NetworkClassDescriptor
        {
            public IPAddress From { get; set; }
            public IPAddress To { get; set; }
            public IPAddress Mask { get; set; }
        }
        public readonly Dictionary<string, NetworkClassDescriptor> NetworkClasses = new Dictionary<string, NetworkClassDescriptor>()
        {
            {"A",new NetworkClassDescriptor{ From =IPAddress.Parse("0.0.0.0"),To= IPAddress.Parse("127.255.255.255"), Mask=IPAddress.Parse("255.0.0.0")}},
            {"B",new NetworkClassDescriptor{ From =IPAddress.Parse("128.0.0.0"),To= IPAddress.Parse("191.255.255.255"), Mask=IPAddress.Parse("255.255.0.0") }},
            {"C",new NetworkClassDescriptor{ From =IPAddress.Parse("192.0.0.0"),To= IPAddress.Parse("223.255.255.255"), Mask=IPAddress.Parse("255.255.255.0")}},
            {"D",new NetworkClassDescriptor{ From =IPAddress.Parse("224.0.0.0"),To= IPAddress.Parse("239.255.255.255") }},
            {"E",new NetworkClassDescriptor{ From =IPAddress.Parse("224.0.0.0"),To= IPAddress.Parse("239.255.255.255") }}
        };

        public NetworkUtilityHandler()
        {

        }

        public class NetworkCardProperties
        {
            public static readonly int[] Exp2ToExp10Sys = new int[8] { 128, 64, 32, 16, 8, 4, 2, 1 };
            public bool[] MaskBits
            {
                get
                {
                    return CalcMaskBits(Mask);

                }
            }
            public bool[] CalcMaskBits(IPAddress mask)
            {
                int[] maskOktetesInt = GetIPv4DecimalBlocks(mask);
                var bitArray = GetIpv4DecimalBlocksAsBitArray(maskOktetesInt);
                return bitArray;
            }
            public int CIDRv4
            {
                get
                {
                    return CalcCIDRv4(MaskBits);
                }
            }
            public int CalcCIDRv4(bool[] maskBits)
            {
                return maskBits.ToList().FindAll(x => x).Count();
            }
            public int DeviceRangev4Bits
            {
                get
                {

                    return 32 - CIDRv4;
                }
            }
            public long DeviceRangev4Count
            {
                get
                {
                    var val = Math.Pow(2.0, DeviceRangev4Bits).ToString();
                    long result = long.Parse(val);
                    return result - 2;
                }
            }
            public double DeviceRangeReservedOctettesFromRight
            {
                get
                {

                    var result = DeviceRangev4Bits / 8.0;
                    return result;
                }
            }
            public IPAddress[] NetworkIdAndBroadcast
            {
                get
                {
                    return CalcNetworkAndBroadcastByGivenIpAndMask(Ip, CIDRv4);
                }
            }
            public IPAddress[] CalcNetworkAndBroadcastByGivenIpAndMask(IPAddress ip, int cidr)
            {
                int[] decBlockIpv4 = GetIPv4DecimalBlocks(ip);
                bool[] ipV4BitPresentation = GetIpv4DecimalBlocksAsBitArray(decBlockIpv4);
                bool[] lowestNetworkIp = new bool[ipV4BitPresentation.Length];
                bool[] highestNetworkIp = new bool[ipV4BitPresentation.Length];
                Array.Copy(ipV4BitPresentation, lowestNetworkIp, ipV4BitPresentation.Length);
                Array.Copy(ipV4BitPresentation, highestNetworkIp, ipV4BitPresentation.Length);
                int networkIpRange = cidr;
                if (decBlockIpv4.Length == 4)
                {
                    for (int i = 0; i < 32; i++)
                    {

                        if (i >= networkIpRange)
                        {
                            lowestNetworkIp[i] = false;
                            highestNetworkIp[i] = true;
                        }
                    }
                }

                IPAddress lowestIp = GetIp4vFromBitPresentation(lowestNetworkIp);
                IPAddress highestIp = GetIp4vFromBitPresentation(highestNetworkIp);
                return new IPAddress[] { lowestIp, highestIp };
            }
            public bool IsIPv4InSubnet(IPAddress addr, IPAddress networkId, int cidr)
            {
                IPAddress[] networkBroadcast = CalcNetworkAndBroadcastByGivenIpAndMask(networkId, cidr);
                int[] addrDecimalPresentation = GetIPv4DecimalBlocks(addr);
                int[] decimalNetworkId = GetIPv4DecimalBlocks(networkBroadcast[0]);
                int[] decimalBroadcast = GetIPv4DecimalBlocks(networkBroadcast[1]);
                bool a = addrDecimalPresentation[0] <= decimalBroadcast[0] && addrDecimalPresentation[0] >= decimalNetworkId[0];
                bool b = addrDecimalPresentation[1] <= decimalBroadcast[1] && addrDecimalPresentation[1] >= decimalNetworkId[1];
                bool c = addrDecimalPresentation[2] <= decimalBroadcast[2] && addrDecimalPresentation[2] >= decimalNetworkId[2];
                bool d = addrDecimalPresentation[3] <= decimalBroadcast[3] && addrDecimalPresentation[3] >= decimalNetworkId[3];
                bool match = a && b
                     && c
                     && d
                    ;
                if (match)
                {

                }
                return match;
            }
            public async Task<ConcurrentDictionary<IPAddress, string>> DnsLookup(List<IPAddress> addr)
            {
                ConcurrentDictionary<IPAddress, string> response = new ConcurrentDictionary<IPAddress, string>();
                List<Task> tasks = new List<Task>();
                Func<IPAddress, Task<bool>> func = new Func<IPAddress, Task<bool>>(async (ip) =>
                {
                    IPHostEntry hostEntry = null;
                    try
                    {
                        hostEntry = await Dns.GetHostEntryAsync(ip);
                    }
                    catch (Exception ex)
                    {

                        System.Diagnostics.Debug.WriteLine("resolving exception: " + ex.Message + "");
                    }
                    response.TryAdd(ip, hostEntry == null ? null : hostEntry.HostName);

                    System.Diagnostics.Debug.WriteLine("dns-resolve result for ip '" + ip.ToString() + "': " + (hostEntry == null ? null : hostEntry.Aliases.ToList().Count) + " resolves");

                    return !(hostEntry == null);
                });
                int i = 0;
                foreach (var item in addr)
                {

                    Task<bool> task = func(item);
                    tasks.Add(task);
                    System.Diagnostics.Debug.WriteLine("try to get hostname: " + item.ToString() + "");
                    i++;
                }


                Task.WaitAll(tasks.ToArray());
                return response;
            }
            public async Task<List<IPAddress>> CalcIPv4RangeList()
            {
                var networkAndBroadcast = NetworkIdAndBroadcast;
                var lowestIp = networkAndBroadcast[0];
                var highestIp = networkAndBroadcast[1];
                int[] lowestIpDecBlocks = GetIPv4DecimalBlocks(lowestIp);
                int[] highestIpDecBlocks = GetIPv4DecimalBlocks(highestIp);
                int[] ipBlockCalc = new int[4];
                Array.Copy(lowestIpDecBlocks, ipBlockCalc, lowestIpDecBlocks.Length);
                Array.Reverse(ipBlockCalc);
                Func<int, int, int, int, bool> fCheckMatch = new Func<int, int, int, int, bool>((a, b, c, d) =>
                {
                    bool match = a == highestIpDecBlocks[0] && b == highestIpDecBlocks[1] && c == highestIpDecBlocks[2] && d == highestIpDecBlocks[3];
                    if (match)
                    {

                    }
                    return match;
                });
                var t = Task.Run(() =>
                {

                    long index = 0;
                    bool matchBlocks = false;

                    List<IPAddress> response = new List<IPAddress>();
                    response.Add(lowestIp);
                    while (index < DeviceRangev4Count + 1)
                    {

                        ipBlockCalc[0]++;
                        if (ipBlockCalc[0] == 256)
                        {
                            ipBlockCalc[1]++;
                            ipBlockCalc[0] = 0;
                        }
                        if (ipBlockCalc[1] == 256)
                        {
                            ipBlockCalc[2]++;
                            ipBlockCalc[1] = 0;
                        }
                        if (ipBlockCalc[2] == 256)
                        {
                            ipBlockCalc[3]++;
                            ipBlockCalc[2] = 0;
                        }

                        matchBlocks = fCheckMatch(ipBlockCalc[3], ipBlockCalc[2], ipBlockCalc[1], ipBlockCalc[0]);
                        if (matchBlocks)
                            break;
                        string ipStr = "" + ipBlockCalc[3] + "." + ipBlockCalc[2] + "." + ipBlockCalc[1] + "." + ipBlockCalc[0] + "";
                        System.Diagnostics.Debug.WriteLine(ipStr);

                        var ip = IPAddress.Parse(ipStr);
                        response.Add(ip);

                        index++;
                    }
                    response.Add(highestIp);
                    return response;
                });

                return await t;
            }
            public IPAddress Mask { get; set; }
            public IPAddress Ip { get; set; }
            public IPAddress NetId
            {
                get
                {
                    return NetworkIdAndBroadcast[0];
                }
            }
            public IPAddress BroadcastIp
            {
                get
                {
                    return NetworkIdAndBroadcast[1];
                }
            }
            public List<GatewayIPAddressInformation> Gateway { get; set; }
            public List<IPAddress> DnsServers { get; set; }

            public IPAddress GetIp4vFromBitPresentation(bool[] bitArrayIpv4)
            {
                IPAddress addr = null;
                if (bitArrayIpv4 != null && bitArrayIpv4.Length == 32)
                {

                    int[] octectDecValues = new int[4];
                    bool[] octetValues = new bool[8];
                    for (int i = 0; i < 4; i++)
                    {
                        Array.Copy(bitArrayIpv4, i * 8, octetValues, 0, 8);
                        for (int o = 0; o < octetValues.Length; o++)
                        {
                            if (octetValues[o])
                                octectDecValues[i] += Exp2ToExp10Sys[o];
                        }

                    }
                    string ipStr = null;
                    for (int i = 0; i < octectDecValues.Length; i++)
                    {
                        ipStr += octectDecValues[i] + (i < 3 ? "." : string.Empty);
                    }
                    addr = IPAddress.Parse(ipStr);
                }
                return addr;
            }
            private byte ConvertBoolArrayToByte(bool[] source)
            {
                byte result = 0;
                int index = 8 - source.Length;

                foreach (bool b in source)
                {
                    if (b)
                        result |= (byte)(1 << 7 - index);

                    index++;
                }

                return result;
            }
            public bool[] GetIpv4DecimalBlocksAsBitArray(int[] decimalBlocks)
            {
                bool[] bitArray = new bool[8 * 4];
                if (decimalBlocks.Length == 4)
                {

                    for (int i = 0; i < decimalBlocks.Length; i++)
                    {
                        var tmp = new BitArray(new int[] { decimalBlocks[i] });
                        bool[] bools = new bool[8];
                        for (int j = 0; j < 8; j++)
                        {
                            bitArray[7 - j + i * 8] = tmp[j];

                        }

                    }
                }
                return bitArray;
            }
            public int[] GetIPv4DecimalBlocks(IPAddress addr)
            {
                int[] response = new int[0];
                if (addr != null)
                {
                    string ipStr = addr.ToString();
                    if (ipStr != null)
                    {
                        string[] splitIpStrArr = ipStr.Split('.');
                        if (splitIpStrArr.Length == 4)
                        {
                            response = new int[splitIpStrArr.Length];
                            for (int i = 0; i < splitIpStrArr.Length; i++)
                            {
                                response[i] = int.TryParse(splitIpStrArr[i], out int val) ? val : int.MaxValue;
                            }
                        }
                        if (response.ToList().IndexOf(int.MaxValue) != -1)
                            return new int[0];
                    }
                }
                return response;
            }
        }
        public async void Test()
        {

            var iptest = new NetworkCardProperties { Ip = IPAddress.Parse("8.8.8.8"), Mask = IPAddress.Parse("255.255.255.0") };
            List<IPAddress> iTmp = await iptest.CalcIPv4RangeList();
            bool checkValidIp = iptest.IsIPv4InSubnet(IPAddress.Parse("1.0.0.0"), IPAddress.Parse("10.0.0.0"), 1);
            var dns = await iptest.DnsLookup(iTmp);
            System.Diagnostics.Debug.WriteLine("networkinformation: CIDRv4=" + iptest.CIDRv4 + "");
            System.Diagnostics.Debug.WriteLine("networkinformation: hit=" + iptest.DeviceRangev4Count + "");
        }
        public static NetworkCardProperties GetPhysicalEthernetIPAdress()
        {
            NetworkCardProperties response = new NetworkCardProperties();
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {

                var addr = ni.GetIPProperties().GatewayAddresses.FirstOrDefault();
                if (addr != null && !addr.Address.ToString().Equals("0.0.0.0"))
                {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet || ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                    {
                        var props = ni.GetIPProperties();
                        foreach (var item in props.UnicastAddresses)
                        {
                            if (item.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                response.Ip = item.Address;
                                response.Mask = item.IPv4Mask;
                                break;
                            }
                        }

                        var gwList = new List<GatewayIPAddressInformation>(props.GatewayAddresses.Count);
                        foreach (var item in props.GatewayAddresses)
                        {
                            gwList.Add(item);
                        }
                        var dnsList = new List<IPAddress>(props.DnsAddresses.Count);
                        foreach (var item in props.DnsAddresses)
                        {
                            dnsList.Add(item);
                        }
                        response.DnsServers = dnsList;
                        response.Gateway = gwList;
                    }
                }
            }
            return response;
        }
    }
}
