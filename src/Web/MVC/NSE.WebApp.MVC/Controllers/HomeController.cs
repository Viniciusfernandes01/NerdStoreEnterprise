using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Models;

namespace NSE.WebApp.MVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //not-available
        [Route("not-available")]
        public IActionResult NotAvailable()
        {
            var modelErro = new ErrorViewModel
            {
                Message = "system temporarily unavailable, try again later",
                ErrorCode = 500,
                Title = "Not Available"
            };

            return View("Error", modelErro);
        }

        [Route("erro/{id:length(3,3)}")]
        public IActionResult Error(int id)
        {
            var modelError = new ErrorViewModel();

            if (id == 500)
            {
                modelError.Message = "Erro! Tente novamente ou contato nosso suporte.";
                modelError.Title = "Erro!";
                modelError.ErrorCode = id;
            }

            if (id == 404)
            {
                modelError.Message = "Página não existe! Tente novamente ou contato nosso suporte.";
                modelError.Title = "Página não encontrada!";
                modelError.ErrorCode = id;
            }
            else if(id == 403)
            {
                modelError.Message = "Você não tem permissão para acessar está página! Tente novamente ou contato nosso suporte.";
                modelError.Title = "Permissão negada!";
                modelError.ErrorCode = id;
            }
            else
            {
                return StatusCode(404);
            }

            return View("Error", modelError);
        }
    }
}
