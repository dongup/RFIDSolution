using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RFIDSolution.Shared.Models;
using System.Linq;

namespace RFIDSolution.Utils
{
    public class ModelStateValidator
    {
        public static IActionResult ValidateModelState(ActionContext context)
        {
            (string fieldName, ModelStateEntry entry) = context.ModelState
                .First(x => x.Value.Errors.Count > 0);
            string errorSerialized = entry.Errors.First().ErrorMessage;

            ResponseModel<ModelErrorCollection> rspns = new ResponseModel<ModelErrorCollection>();
            rspns.Failed(errorSerialized);
            rspns.Result = entry.Errors;

            return new OkObjectResult(rspns);
        }
    }
}
