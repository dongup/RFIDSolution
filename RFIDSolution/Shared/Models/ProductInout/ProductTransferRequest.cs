using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.Shared.Models.ProductInout
{
    /// <summary>
    /// Chứa thông tin cơ bản của 1 product khi transfer in/out
    /// </summary>
    public class ProductTransferRequest
    {
        public ProductTransferRequest()
        {

        }

        public ProductTransferRequest(ProductModel product)
        {
            ProductId = product.ID;
            EPC = product.EPC;
            Note = product.Note;
        }

        public int ProductId { get; set; }

        public string EPC { get; set; }

        public ProductStatus ProductStatus { get; set; }

        public string Note { get; set; }
    }
}
