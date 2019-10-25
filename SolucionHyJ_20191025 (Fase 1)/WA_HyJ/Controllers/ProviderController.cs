using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WA_HyJ.Filter;
using WA_HyJ.Interfaces;
using WA_HyJ.Models;
using WA_HyJ.Models.ViewModels;
using WA_HyJ.Providers;

namespace WA_HyJ.Controllers
{
    public class ProviderController : Controller
    {
        private IProvider _Provider;

        public ProviderController()
        {
            _Provider = new ProviderDAO();
        }

        // GET: Providers
        public async Task<ActionResult> Index()
        {
            var model = await _Provider.GetAllAsync();

            List<ListarProveedores> lista = new List<ListarProveedores>();

            foreach (var item in model)
            {
                lista.Add(new ListarProveedores(){
                    Celular = item.Celular,
                    Direccion = item.Direccion,
                    Id = item.Id,
                    RazonSocial = item.RazonSocial,
                    RUC = item.RUC,
                    Telefono = item.Telefono,
                    Contacto = item.Contacto
                });
            }

            return View(lista);
        }

        // GET: Providers/Details/5
        public async Task<ActionResult> Details(string id)
        {
            DetalleProveedor model = new DetalleProveedor();
            try
            {
                var result = await _Provider.DetailsAsync(id);

                if (result == null)
                    return PartialView("_404");

                var config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<ProviderModel, DetalleProveedor>();
                });

                IMapper iMapper = config.CreateMapper();

                model = iMapper.Map<ProviderModel, DetalleProveedor>(result);

                return PartialView("_Details", model);
            }
            catch(Exception ex)
            {
                ViewBag.error = ex.Message;
                return PartialView("_500");
            }
        }

        // GET: Providers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Providers/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegistroProveedores model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var config = new MapperConfiguration(cfg => {
                        cfg.CreateMap<RegistroProveedores, ProviderModel>();
                    });

                    IMapper iMapper = config.CreateMapper();
                    var mp = iMapper.Map<RegistroProveedores, ProviderModel>(model);
                    var result = await _Provider.AddAsync(mp);

                    if (result != null)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError("", "No se logró crear el registro.");
                        return View(model);
                    }
                        
                }
                
                return View(model);
            }
            catch(Exception e)
            {
                ModelState.AddModelError("", e);
                return View(model);
            }
        }

        // GET: Providers/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            ActualizarProveedores model = new ActualizarProveedores();

            var result = await _Provider.DetailsAsync(id);

            if (result == null)
                return RedirectToAction("Index");

            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<ProviderModel, ActualizarProveedores>();
            });

            IMapper iMapper = config.CreateMapper();

            model = iMapper.Map<ProviderModel, ActualizarProveedores>(result);

            return View(model);
        }

        // POST: Providers/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ActualizarProveedores model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var config = new MapperConfiguration(cfg => {
                        cfg.CreateMap<ActualizarProveedores, ProviderModel>();
                    });

                    IMapper iMapper = config.CreateMapper();
                    var mp = iMapper.Map<ActualizarProveedores, ProviderModel>(model);
                    HttpStatusCode result = await _Provider.UpdateAsync(mp);

                    if (result == HttpStatusCode.OK)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError("", "No se logró actualizar el registro.");
                        return View(model);
                    }
                }

                return View(model);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e);
                return View(model);
            }
        }
        
        public async Task<ActionResult> Delete(string id)
        {
            var model = await _Provider.DetailsAsync(id);

            EliminarProveedores modelResult = new EliminarProveedores() { Id = id, RazonSocial = model.RazonSocial, RUC = model.RUC };
            
            return PartialView("_Delete", modelResult);
        }

        [HttpPost, ValidateJSAntiForgeryToken]
        public async Task<JsonResult> DeleteProvider(string id)
        {
            object respuesta = null;

            HttpStatusCode result = await _Provider.DeleteAsync(id);

            if (result == HttpStatusCode.OK)
                respuesta = Url.Action("Index");
            else
                respuesta = result;

            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }
    }
}
