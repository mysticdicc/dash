using NetTools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DashLib.Network
{
    public class Subnet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public byte[] Address { get; set; }
        public byte[] SubnetMask { get; set; }
        public byte[] StartAddress { get; set; }
        public byte[] EndAddress { get; set; }
        public List<IP> List { get; set; }

        public Subnet() { }

        public Subnet(string CIDR)
        {
            var subnet = IPAddressRange.Parse(CIDR);

            StartAddress = IP.ConvertToByte(subnet.Begin);
            EndAddress = IP.ConvertToByte(subnet.End);

            var split = CIDR.Split('/');

            SubnetMask = IP.GetMaskFromCidr(Int32.Parse(split[1]));
            Address = IP.ConvertToByte(split[0]);

            var temp = new List<IP>();

            foreach (IPAddress ip in subnet)
            {
                temp.Add(
                    new IP
                    {
                        Address = IP.ConvertToByte(ip)
                    }
                );
            }

            List = temp;
        }

        public static string GetCidrString(Subnet subnet)
        {
            var startAdr = IPAddress.Parse(IP.ConvertToString(subnet.StartAddress));
            var subMask = IPAddress.Parse(IP.ConvertToString(subnet.SubnetMask));

            if (null != startAdr && null != subMask)
            {
                var range = new IPAddressRange(startAdr, IPAddressRange.SubnetMaskLength(subMask));

                if (range != null)
                {
                    return range.ToCidrString();
                }
                else
                {
                    throw new InvalidCastException("Unable to create IP range");
                }
            }
            else
            {
                throw new InvalidCastException("Unable to create IP and Mask objects");
            }
        }
    }
}
