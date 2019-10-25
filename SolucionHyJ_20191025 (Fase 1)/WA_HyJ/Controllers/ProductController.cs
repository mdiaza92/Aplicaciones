using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using WA_HyJ.Filter;
using WA_HyJ.Interfaces;
using WA_HyJ.Models;
using WA_HyJ.Models.DAO;
using WA_HyJ.Models.InternalModels;
using WA_HyJ.Models.ViewModels;
using WA_HyJ.Providers;

namespace WA_HyJ.Controllers
{
    public class ProductController : Controller
    {
        private IProduct _Product;
        private IProvider _Provider;
        private IOption _Option;
        private IBrand _Brand;

        public ProductController()
        {
            _Product = new ProductDAO();
            _Provider = new ProviderDAO();
            _Option = new OptionDAO();
            _Brand = new BrandDAO();
        }

        #region Select List Item
        private IEnumerable<SelectListItem> ListarTipos(string Tipo = null)
        {
            var selectList = new List<SelectListItem>();

            string listaKey = WebConfigurationManager.AppSettings["listTipoProductos"];
            string[] split = listaKey.Split(',');

            foreach (var item in split)
            {
                if (string.IsNullOrEmpty(Tipo))
                {
                    selectList.Add(new SelectListItem()
                    {
                        Value = item,
                        Text = item
                    });
                }
                else
                {
                    selectList.Add(new SelectListItem()
                    {
                        Value = item,
                        Text = item,
                        Selected = (Tipo == item)
                    });
                }
            }

            return selectList;
        }
        
        /// <param name="TipoMoneda">Moneda (Enum) como entero</param>
        private async Task<IEnumerable<SelectListItem>> ListarTipoMonedas(string TipoMoneda = null)
        {
            var selectList = new List<SelectListItem>();

            var getTM = await _Option.GetTipoMoneda();

            foreach (var item in getTM)
            {
                selectList.Add(new SelectListItem()
                {
                    Text = item.Key,
                    Value = item.Value.ToString(),
                    Selected = (item.Value.ToString() == TipoMoneda)
                });
            }

            return selectList;
        }

        private async Task<IEnumerable<SelectListItem>> ListarTipoUnidades(string TipoUnidad = null)
        {
            var selectList = new List<SelectListItem>();

            var getUM = await _Option.GetUnidadMedida();

            foreach (var item in getUM)
            {
                selectList.Add(new SelectListItem()
                {
                    Text = item.Key,
                    Value = item.Value.ToString(),
                    Selected = (item.Value.ToString() == TipoUnidad)
                });
            }

            return selectList;
        }

        private async Task<IEnumerable<SelectListItem>> ListarMarcas(string IdMarca = null)
        {
            var selectList = new List<SelectListItem>();

            var getAll = await _Brand.GetAllAsync();

            foreach (var item in getAll)
            {
                selectList.Add(new SelectListItem()
                {
                    Text = item.Nombre,
                    Value = item.Id,
                    Selected = (item.Id == IdMarca)
                });
            }

            return selectList;
        }

        private async Task<IEnumerable<SelectListItem>> GetListProveedoresAsync(string IdProveedor = null)
        {
            var selectList = new List<SelectListItem>();

            var result = await _Provider.GetAllAsync();

            foreach (ProviderModel item in result)
            {
                selectList.Add(new SelectListItem()
                {
                    Text = item.RazonSocial,
                    Value = item.Id,
                    Selected = (item.Id == IdProveedor)
                });
            }

            return selectList;
        }
        #endregion

        private async Task<string> GetProveedorAsync(ProductModel source)
        {
            var model = await _Provider.GetAllAsync();
            var _respuesta = model.SingleOrDefault(x => x.Id == source.IdProveedor);
            return _respuesta != null ? _respuesta.RazonSocial : "-";

        }

        // GET: Product
        public async Task<ActionResult> Index()
        {
            var listaInicial = await _Product.GetAllAsync();

            List<ListarProductos> model = new List<ListarProductos>();

            foreach (ProductModel item in listaInicial)
            {
                model.Add(new ListarProductos()
                {
                    Id = item.Id,
                    Codigo = item.Codigo,
                    Costo = item.PrecioUnitario,
                    Descripcion = item.Descripcion ?? "-",
                    Moneda = item.Moneda ?? "-",
                    Nombre = item.Nombre,
                    Peso = item.Peso,
                    Proveedor = await GetProveedorAsync(item),
                    Tipo = item.Tipo ?? "-",
                    Unidad = item.UM ?? "-",
                    Marca = item.Marca != null ? item.Marca.Nombre : "-"
                });
            }

            return View(model);
        }

        // GET: Product/Details/5
        public async Task<ActionResult> Details(string id)
        {
            try
            {
                DetalleProducto dp = new DetalleProducto();

                var result = await _Product.DetailsAsync(id);

                if (result == null)
                    return PartialView("_404");

                IMapper iMapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ProductModel, DetalleProducto>()
                        .ForMember(des => des.Unidad, opt => opt.MapFrom(source => source.UM))
                        .ForMember(des => des.Moneda, opt => opt.MapFrom(source => source.Moneda));
                }).CreateMapper();

                dp = iMapper.Map<ProductModel, DetalleProducto>(result);

                dp.Proveedor = await GetProveedorAsync(result);

                return PartialView("_Details", dp);
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return PartialView("_500");
            }
        }

        // GET: Product/Create
        public async Task<ActionResult> Create()
        {
            var model = new RegistrarProductos();

            await LoadCombos(model);

            return View(model);
        }

        public async Task LoadCombos(RegistrarProductos model)
        {
            model.Tipos = ListarTipos();
            model.Monedas = await ListarTipoMonedas();
            model.Unidades = await ListarTipoUnidades();
            model.Proveedores = await GetListProveedoresAsync();
            model.Marcas = await ListarMarcas();
        }

        // POST: Product/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegistrarProductos model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ProductModel pm = new ProductModel
                    {
                        PrecioUnitario = model.PrecioUnitario,
                        Descripcion = model.Descripcion,
                        IdProveedor = model.IdProveedor,
                        Moneda = model.Moneda.ToString(),
                        Nombre = model.Nombre,
                        Peso = model.Peso,
                        Tipo = model.Tipo,
                        UM = model.UnidadMedida.ToString(),
                        IdMarca = model.IdMarca
                    };

                    var result = await _Product.AddAsync(pm);

                    if (result != null)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError("", "No se logró crear el registro.");

                        await LoadCombos(model);

                        return View(model);
                    }
                }

                await LoadCombos(model);

                return View(model);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e);

                await LoadCombos(model);

                return View(model);
            }
        }

        // GET: Product/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            ActualizarProductos model = new ActualizarProductos();

            var result = await _Product.DetailsAsync(id);

            if (result == null)
                return RedirectToAction("Index");

            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<ProductModel, ActualizarProductos>();
            });

            IMapper iMapper = config.CreateMapper();

            model = iMapper.Map<ProductModel, ActualizarProductos>(result);

            model.Proveedores = await GetListProveedoresAsync(result.IdProveedor);
            model.Tipos = ListarTipos(result.Tipo);
            model.Monedas = await ListarTipoMonedas(result.Moneda);
            model.Unidades = await ListarTipoUnidades(result.UM);
            model.Marcas = await ListarMarcas(result.Marca.Id);

            model.UnidadMedida = int.Parse(result.UM);

            return View(model);
        }

        // POST: Product/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ActualizarProductos model)
        {
            ProductModel pm = new ProductModel
            {
                Id = model.Id,
                PrecioUnitario = model.PrecioUnitario,
                Descripcion = model.Descripcion,
                IdProveedor = model.IdProveedor,
                Moneda = model.Moneda.ToString(),
                Nombre = model.Nombre,
                Peso = model.Peso,
                Tipo = model.Tipo,
                UM = model.UnidadMedida.ToString(),
                IdMarca = model.IdMarca
            };

            try
            {
                if (ModelState.IsValid)
                {
                    HttpStatusCode result = await _Product.UpdateAsync(pm);

                    if (result == HttpStatusCode.OK)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError("", "No se logró actualizar el registro.");
                     
                        model.Proveedores = await GetListProveedoresAsync(pm.IdProveedor);
                        model.Tipos = ListarTipos(pm.Tipo);
                        model.Monedas = await ListarTipoMonedas(pm.Moneda);
                        model.Unidades = await ListarTipoUnidades(pm.UM);
                        model.Marcas = await ListarMarcas(pm.IdMarca);

                        return View(model);
                    }
                }

                model.Proveedores = await GetListProveedoresAsync(pm.IdProveedor);
                model.Tipos = ListarTipos(pm.Tipo);
                model.Monedas = await ListarTipoMonedas(pm.Moneda);
                model.Unidades = await ListarTipoUnidades(pm.UM);
                model.Marcas = await ListarMarcas(pm.IdMarca);

                return View(model);
            }
            catch(Exception e)
            {
                ModelState.AddModelError("", e);

                model.Proveedores = await GetListProveedoresAsync(pm.IdProveedor);
                model.Tipos = ListarTipos(pm.Tipo);
                model.Monedas = await ListarTipoMonedas(pm.Moneda);
                model.Unidades = await ListarTipoUnidades(pm.UM);
                model.Marcas = await ListarMarcas(pm.IdMarca);

                return View(model);
            }
        }

        // GET: Product/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            var model = await _Product.DetailsAsync(id);

            EliminarProducto ep = new EliminarProducto()
            {
                Id = model.Id,
                Codigo = model.Codigo,
                Nombre = model.Nombre
            };

            return PartialView("_Delete", ep);
        }

        // POST: Product/Delete/5
        [HttpPost, ValidateJSAntiForgeryToken]
        public async Task<JsonResult> DeleteProduct(string id)
        {
            object respuesta = null;

            HttpStatusCode result = await _Product.DeleteAsync(id);

            if (result == HttpStatusCode.OK)
                respuesta = Url.Action("Index");
            else
                respuesta = result;

            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }
    }
}
