using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaClient.GCModels
{
    public class MachineInfoFactory
    {
        public static MachineInfo GetSimpleNetworkMachineInfo(string info)
        {
            string[] rs = info.Split(',');
            if (rs.Count() != 3)
            {
                throw new Exception(String.Format("machine config info is incorrect, {0}", info));
            }
            //MachineInfo machine = new MachineInfo() { MachineName = rs[0].Trim() };
            MachineInfo machine = new MachineInfo(rs[0].Trim());
            machine.NetworkCompent = GCFacilities.NetworkManager.NetworkComponentFactory.GetSimpleNetworkComponent(rs[1].Trim(), rs[2].Trim());
            return machine;
        }
    }
}
