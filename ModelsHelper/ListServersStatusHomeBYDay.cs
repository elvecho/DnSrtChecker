using DnSrtChecker.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DnSrtChecker.ModelsHelper
{
    public class ListServersStatusHomeBYDay
    {
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? OperationClosureDatetime { get; set; }
        public IEnumerable<RtServersHome>  ListRtServersHome { get; set; }
    }
}
