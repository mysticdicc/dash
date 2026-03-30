using NetTools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DashLib.Models.Network
{
    public class SubnetContainer : BaseMonitoringTargetContainer
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Id { get; set; }
        public byte[] Address { get; set; }
        public byte[] SubnetMask { get; set; }
        public byte[] StartAddress { get; set; }
        public byte[] EndAddress { get; set; }
        new public List<IpMonitoringTarget> Children { get; set; }

        public SubnetContainer() { }

        public SubnetContainer(string CIDR) : base()
        {
            var subnet = IPAddressRange.Parse(CIDR);

            StartAddress = IpMonitoringTarget.ConvertToByte(subnet.Begin);
            EndAddress = IpMonitoringTarget.ConvertToByte(subnet.End);

            var split = CIDR.Split('/');

            SubnetMask = IpMonitoringTarget.GetMaskFromCidr(Int32.Parse(split[1]));
            Address = IpMonitoringTarget.ConvertToByte(split[0]);

            var temp = new List<IpMonitoringTarget>();

            foreach (IPAddress ip in subnet)
            {
                temp.Add(
                    new IpMonitoringTarget(this)
                    {
                        Address = IpMonitoringTarget.ConvertToByte(ip)
                    }
                );
            }

            Children = temp;
        }

        public static string GetCidrString(SubnetContainer subnet)
        {
            var startAdr = IPAddress.Parse(IpMonitoringTarget.ConvertToString(subnet.StartAddress));
            var subMask = IPAddress.Parse(IpMonitoringTarget.ConvertToString(subnet.SubnetMask));

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
