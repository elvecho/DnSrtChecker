using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.Models.ViewModels.ModelsFunctions
{
    public class RtServerByUser
    {
        //rtx.rtserverparam
        public int LStoreGroupId { get; set; }//RtServer.lStoreGroupID,
        public string SzGroupDescription { get; set; }//Store_Group.szDescription AS szGroupDescription, // SzGroupDescription 
        public int LRetailStoreId { get; set; }//RtServer.lRetailStoreID,//
        public string szRetailStoreDescription { get; set; }//Store.szDescription AS szStoreDescription,//szRetailStoreDescription
        public string SzLocation { get; set; }//RtServer.szLocation,
        public string SzRtServerId { get; set; }//RtServer.szRtServerID,
        public string SzIpAddress { get; set; }//RtServer.szIpAddress,
        public bool? BOnDutyFlag { get; set; }//RtServer.bOnDutyFlag
    }
}
