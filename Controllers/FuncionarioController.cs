using Microsoft.AspNetCore.Mvc;
using MyEmployeeProject.DAO;
using MyEmployeeProject.Models;

namespace MyEmployeeProject.Controllers
{
    public class FuncionarioController : Controller
    {
        private readonly FuncionarioDAO _funcionarioDAO;
        private readonly CargoDAO _cargoDAO;
        private readonly DepartamentoDAO _departamentoDAO;

        public FuncionarioController(FuncionarioDAO funcionarioDAO, CargoDAO cargoDAO, DepartamentoDAO departamentoDAO)
        {
            _funcionarioDAO = funcionarioDAO;
            _cargoDAO = cargoDAO;
            _departamentoDAO = departamentoDAO;
        }

        public IActionResult Index()
        {
            try
            {
                var funcionarios = _funcionarioDAO.ListarFuncionarios();
                return View(funcionarios);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Erro ao carregar funcionários: {ex.Message}";
                return View(new List<Funcionario>());
            }
        }

        public IActionResult Create()
        {
            try
            {
                ViewBag.Cargos = _cargoDAO.ListarCargos();
                ViewBag.Departamentos = _departamentoDAO.ListarDepartamentos();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Erro ao carregar dados: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Create(Funcionario funcionario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    funcionario.DataCriacao = DateTime.Now;
                    funcionario.DataAtualizacao = DateTime.Now;
                    _funcionarioDAO.InserirFuncionario(funcionario);
                    
                    TempData["Success"] = "Funcionário cadastrado com sucesso!";
                    return RedirectToAction("Index");
                }
                
                ViewBag.Cargos = _cargoDAO.ListarCargos();
                ViewBag.Departamentos = _departamentoDAO.ListarDepartamentos();
                return View(funcionario);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Erro ao salvar funcionário: {ex.Message}";
                ViewBag.Cargos = _cargoDAO.ListarCargos();
                ViewBag.Departamentos = _departamentoDAO.ListarDepartamentos();
                return View(funcionario);
            }
        }

        public IActionResult Edit(int id)
        {
            try
            {
                var funcionario = _funcionarioDAO.ObterFuncionarioPorId(id);
                if (funcionario == null)
                {
                    TempData["Error"] = "Funcionário não encontrado.";
                    return RedirectToAction("Index");
                }

                ViewBag.Cargos = _cargoDAO.ListarCargos();
                ViewBag.Departamentos = _departamentoDAO.ListarDepartamentos();
                return View(funcionario);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erro ao carregar funcionário: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Edit(Funcionario funcionario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    funcionario.DataAtualizacao = DateTime.Now;
                    _funcionarioDAO.AtualizarFuncionario(funcionario);
                    
                    TempData["Success"] = "Funcionário atualizado com sucesso!";
                    return RedirectToAction("Index");
                }
                
                ViewBag.Cargos = _cargoDAO.ListarCargos();
                ViewBag.Departamentos = _departamentoDAO.ListarDepartamentos();
                return View(funcionario);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Erro ao atualizar funcionário: {ex.Message}";
                ViewBag.Cargos = _cargoDAO.ListarCargos();
                ViewBag.Departamentos = _departamentoDAO.ListarDepartamentos();
                return View(funcionario);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _funcionarioDAO.ExcluirFuncionario(id);
                TempData["Success"] = "Funcionário excluído com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erro ao excluir funcionário: {ex.Message}";
            }
            
            return RedirectToAction("Index");
        }
    }
}
