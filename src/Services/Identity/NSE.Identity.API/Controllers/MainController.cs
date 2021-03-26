using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace NSE.Identity.API.Controllers
{
  [ApiController]
  public abstract class MainController : Controller
  {
    protected ICollection<string> Errors = new List<string>();

    protected ActionResult CustomResponse(object result = null)
    {
      if (ValidOperation())
      {
        return Ok(result);
      }

      return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
      {
          {"Messages", Errors.ToArray()}
      }));
    }

    protected ActionResult CustomResponse(ModelStateDictionary modelState)
    {
      var errors = modelState.Values.Selectmany(e => e.Errors);
      foreach (var error in errors)
      {
        AddErrorsInProcessing(error.ErrorMessage);
      }

      return CustomResponse();
    }

    protected bool ValidOperation()
    {
      return !Errors.Any();
    }

    protected void AddErrorsInProcessing(string error)
    {
      Errors.Add(error);
    }

    protected void ClearErrorsInProcessing()
    {
      Errors.Clear();
    }
  }
}