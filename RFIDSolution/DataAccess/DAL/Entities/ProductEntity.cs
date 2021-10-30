using RFIDSolution.DataAccess.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.Shared.DAL.Entities
{
    public class ProductEntity : BaseEntity
    {
        private string pRODUCT_ARTICLE;

        public ProductEntity() : base()
        {
        }
        [Key]
        public int PRODUCT_ID { get; set; }

        [StringLength(50)]
        public string PRODUCT_NAME { get; set; }

        [StringLength(50)]
        public string PRODUCT_CODE { get; set; }

        [StringLength(150)]
        public string EPC { get; set; } = "";

        public int? MODEL_ID { get; set; }

        public string PRODUCT_ARTICLE { get => pRODUCT_ARTICLE; set => pRODUCT_ARTICLE = value; }

        [StringLength(10)]
        public string PRODUCT_SIZE { get; set; }

        [StringLength(100)]
        public string PRODUCT_CATEGORY { get; set; }

        public ProductStatus PRODUCT_STATUS { get; set; } = ProductStatus.Available;

        public ProductSide LR { get; set; } = ProductSide.Left;

        [StringLength(50)]
        public string SAMPLE_NO { get; set; }

        [StringLength(100)]
        public string SAMPLE_REQUEST { get; set; }

        [StringLength(100)]
        public string SAMPLE_SIZE { get; set; }

        [StringLength(100)]
        public string PRODUCT_POC { get; set; }

        [StringLength(50)]
        public string PRODUCT_LOCATION { get; set; }

        [StringLength(400)]
        public string CURRENT_LOCATION { get; set; }

        public string PRODUCT_REMARKS { get; set; }

        [StringLength(100)]
        public string DEV_TEAM { get; set; }

        [StringLength(100)]
        public string DEV_NAME { get; set; }

        [StringLength(100)]
        public string PRODUCT_SEASON { get; set; }

        [StringLength(100)]
        public string PRODUCT_STAGE { get; set; }

        [StringLength(200)]
        public string COLOR_NAME { get; set; }

        [StringLength(100)]
        public string PRODUCT_GENDER { get; set; }

        public DateTime? COMPLETED_DATE { get; set; }

        [StringLength(100)]
        public string PRODUCT_WHQDEVELOPER { get; set; }

        [StringLength(100)]
        public string UPPER_MATERIAL { get; set; }

        [StringLength(100)]
        public string PRODUCT_MSMATERIAL { get; set; }

        [StringLength(100)]
        public string OUTSOLE_MATERIAL { get; set; }

        public int LAST_ID { get; set; }

        [StringLength(100)]
        public string PRODUCT_PATTERN { get; set; }

        [StringLength(100)]
        public string PRODUCT_STLFILE { get; set; }

        public DateTime INPUT_DATE { get; set; }

        [StringLength(100)]
        public string REF_DOC_NO { get; set; }

        public string REF_DOC_DATE { get; set; }

        [StringLength(100)]
        public string PAIR_CODE { get; set; }

        public int? CATEGORY_ID { get; set; }

        [ForeignKey(nameof(CATEGORY_ID))]
        public CategoryEntity Category { get; set; }

        [ForeignKey(nameof(MODEL_ID))]
        public ModelEntity Model { get; set; }

        public ICollection<TransferDetailEntity> TransferDetails { get; set; } = new HashSet<TransferDetailEntity>();

        public ICollection<InventoryDetailEntity> InventoryDetails { get; set; } = new HashSet<InventoryDetailEntity>();
    }
}
