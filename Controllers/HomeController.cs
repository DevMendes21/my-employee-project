using Microsoft.AspNetCore.Mvc;
using MyEmployeeProject.DAO;
using MyEmployeeProject.Models;

namespace MyEmployeeProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly FuncionarioDAO _funcionarioDAO;
        private readonly CargoDAO _cargoDAO;
        private readonly DepartamentoDAO _departamentoDAO;

        public HomeController(FuncionarioDAO funcionarioDAO, CargoDAO cargoDAO, DepartamentoDAO departamentoDAO)
        {
            _funcionarioDAO = funcionarioDAO;
            _cargoDAO = cargoDAO;
            _departamentoDAO = departamentoDAO;
        }

        public IActionResult Index()
        {
            try
            {
                var funcionarios = _funcionarioDAO.ListarFuncionarios().Take(10).ToList();
                var totalFuncionarios = _funcionarioDAO.ContarFuncionarios();
                var cargos = _cargoDAO.ListarCargos();
                var departamentos = _departamentoDAO.ListarDepartamentos();

                ViewBag.TotalFuncionarios = totalFuncionarios;
                ViewBag.TotalCargos = cargos.Count();
                ViewBag.TotalDepartamentos = departamentos.Count();
                ViewBag.FuncionariosAtivos = funcionarios.Count(f => f.Status == StatusFuncionario.Ativo);

                return View(funcionarios);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Erro ao carregar dashboard: {ex.Message}";
                return View(new List<Funcionario>());
            }
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
