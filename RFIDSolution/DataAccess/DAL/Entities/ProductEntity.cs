using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RFIDSolution.WebAdmin.Enums.AppEnums;

namespace RFIDSolution.WebAdmin.DAL.Entities
{
    public class ProductEntity : BaseEntity
    {
        public ProductEntity() : base()
        {
        }
        [Key]
        public int PRODUCT_ID { get; set; }

        [StringLength(50)]
        [Required]
        public string PRODUCT_NAME { get; set; }

        [StringLength(50)]
        [Required]
        public string PRODUCT_CODE { get; set; }

        [StringLength(150)]
        [Required]
        public string EPC { get; set; }

        [Required]
        public int MODEL_ID { get; set; }

        public string PRODUCT_ARTICLE { get; set; }

        [StringLength(10)]
        public string PRODUCT_SIZE { get; set; }

        public ProductStatus PRODUCT_STATUS { get; set; } = ProductStatus.Available;

        [StringLength(50)]
        public string SAMPLE_NO { get; set; }

        [StringLength(100)]
        public string SAMPLE_REQUEST { get; set; }

        [StringLength(100)]
        public string SAMPLE_SIZE { get; set; }

        [StringLength(100)]
        public string PRODUCT_CUSTODIAN { get; set; }

        public ProductSide LR { get; set; } = ProductSide.Left;

        [StringLength(50)]
        public string PRODUCT_LOCATION { get; set; }

        public string PRODUCT_DESCRIPTION { get; set; }

        [StringLength(100)]
        public string DEV_TEAM { get; set; }

        [StringLength(100)]
        public string DEV_NAME { get; set; }

        [StringLength(100)]
        public string PRODUCT_SEASON { get; set; }

        [StringLength(100)]
        public string PRODUCT_TDCODE { get; set; }

        [StringLength(200)]
        public string COLOR_NAME { get; set; }

        [StringLength(100)]
        public string PRODUCT_GENDER { get; set; }

        public DateTime COMPLETED_DATE { get; set; }

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

        public DateTime REF_DOC_DATE { get; set; }

        [StringLength(100)]
        public string PAIR_CODE { get; set; }

        [ForeignKey(nameof(MODEL_ID))]
        public ModelEntity Model { get; set; }
    }
}
