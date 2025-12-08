using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace danklibrary.Monitoring
{
    public class PingState
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("MonitorState")]
        public int MonitorID { get; set; }
        required public bool Response { get; set; }
        [JsonIgnore]
        public MonitorState? MonitorState { get; set; }
    }
}
