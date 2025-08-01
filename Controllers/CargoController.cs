using Microsoft.AspNetCore.Mvc;
using MyEmployeeProject.DAO;
using MyEmployeeProject.Models;

namespace MyEmployeeProject.Controllers
{
    public class CargoController : Controller
    {
        private readonly CargoDAO _cargoDAO;
        private readonly DepartamentoDAO _departamentoDAO;

        public CargoController(CargoDAO cargoDAO, DepartamentoDAO departamentoDAO)
        {
            _cargoDAO = cargoDAO;
            _departamentoDAO = departamentoDAO;
        }

        public IActionResult Index()
        {
            try
            {
                var cargos = _cargoDAO.ListarCargos();
                return View(cargos);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Erro ao carregar cargos: {ex.Message}";
                return View(new List<Cargo>());
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Cargo cargo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    cargo.DataCriacao = DateTime.Now;
                    _cargoDAO.InserirCargo(cargo);
                    
                    TempData["Success"] = "Cargo cadastrado com sucesso!";
                    return RedirectToAction("Index");
                }
                
                return View(cargo);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Erro ao salvar cargo: {ex.Message}";
                return View(cargo);
            }
        }

        public IActionResult Edit(int id)
        {
            try
            {
                var cargo = _cargoDAO.ObterCargoPorId(id);
                if (cargo == null)
                {
                    TempData["Error"] = "Cargo não encontrado.";
                    return RedirectToAction("Index");
                }

                return View(cargo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erro ao carregar cargo: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Edit(Cargo cargo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _cargoDAO.AtualizarCargo(cargo);
                    
                    TempData["Success"] = "Cargo atualizado com sucesso!";
                    return RedirectToAction("Index");
                }
                
                return View(cargo);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Erro ao atualizar cargo: {ex.Message}";
                return View(cargo);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _cargoDAO.ExcluirCargo(id);
                TempData["Success"] = "Cargo excluído com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erro ao excluir cargo: {ex.Message}";
            }
            
            return RedirectToAction("Index");
        }
    }
}
