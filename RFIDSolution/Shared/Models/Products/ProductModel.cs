using RFIDSolution.Shared.Models.Shared;
using RFIDSolution.Shared.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.Shared.Models
{
    public class ProductModel
    {
        public ProductModel()
        {

        }

        public int ID { get; set; }
        public string SKU { get; set; }
        public string EPC { get; set; }
        public int ModelId { get; set; }
        public string Article { get; set; }
        public string Size { get; set; }
        public string POC { get; set; }
        public string Location { get; set; }
        public string Remarks { get; set; }
        public string DevStyleName { get; set; }
        public string Season { get; set; }
        public string Stage { get; set; }
        public string ColorWay { get; set; }
        public string Category { get; set; }
        public string ModelName { get; set; }
        public string RefDocNo { get; set; }
        public string RefDocDate { get; set; }
        public ProductSide LR { get; set; }
        public string LRStr { get; set; }
        public ProductStatus ProductStatus { get; set; }
        public string StatusColor => ProductStatus == ProductStatus.Available ? "badge bg-success" : "badge bg-danger";
        public string CreatedUser { get; set; }

        public List<TransferInoutModel> TransferHistory { get; set; } = new List<TransferInoutModel>(); 

        public string Note { get; set; }

        public string WarningClass => IsWarning? "bg-warning" : "";

        public bool IsWarning = false;

        public bool ShowDetail = false;

        public int activeTab = 0;

        public RFTagResponse TagResponse { get; set; } = new RFTagResponse();
    }
}
