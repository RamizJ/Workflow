using Microsoft.AspNetCore.Mvc;
using Workflow.VM.Common;

#pragma warning disable 1591

namespace WorkflowService.Extensions
{
    public static class ActionResultExtension
    {
        public static ActionResult<TVm> ToActionResult<TVm>(this OperationResult<TVm> operationResult)
        {
            if (operationResult == null)
                return new NoContentResult();

            return operationResult.Succeeded
                ? (ActionResult<TVm>) new OkObjectResult(operationResult.Data)
                : new BadRequestObjectResult(operationResult.Errors);
        }

        public static IActionResult ToIActionResult(this OperationResult operationResult)
        {
            if (operationResult == null)
                return new NoContentResult();

            return operationResult.Succeeded
                ? (IActionResult)new OkResult()
                : new BadRequestObjectResult(operationResult.Errors);
        }
    }
}
