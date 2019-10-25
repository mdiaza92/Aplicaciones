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
using WA_HyJ.Models.DAO;
using WA_HyJ.Models.Request;
using WA_HyJ.Models.Response;
using WA_HyJ.Models.ViewModels;
using WA_HyJ.Providers;

namespace WA_HyJ.Controllers
{
    public class OrderController : Controller
    {
        private IProvider _Provider;
        private IKardex _Kardex;
        private IOption _Option;
        private IOrder _Order;
        private IReceipt _Receipt;

        public OrderController()
        {
            _Provider = new ProviderDAO();
            _Kardex = new KardexDAO();
            _Option = new OptionDAO();
            _Order = new OrderDAO();
            _Receipt = new ReceiptDAO();
        }

        #region Combo Box
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

        private async Task<IEnumerable<SelectListItem>> ListarTipoRecibo(string TipoComprobante = null)
        {
            var selectList = new List<SelectListItem>();

            var get = await _Option.GetTipoRecibo();

            foreach (var item in get)
            {
                selectList.Add(new SelectListItem()
                {
                    Text = item.Key,
                    Value = item.Value.ToString(),
                    Selected = (item.Value.ToString() == TipoComprobante)
                });
            }

            return selectList;
        }
        #endregion

        // GET: Pedido
        public async Task<ActionResult> Index()
        {
            var ordenes = await _Order.GetAllAsync();

            List<ListarPedidos> lista = new List<ListarPedidos>();

            foreach (var item in ordenes)
            {
                lista.Add(new ListarPedidos()
                {
                    Codigo = item.Codigo,
                    CreatedOn = item.CreatedOn,
                    Estado = item.Estado,
                    Id = item.Id,
                    Comprobante = item.NumeroFactura,
                    Total = item.Total,
                    Validado = item.Validado
                });
            }

            return View(lista);
        }

        // GET: Pedido/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public async Task<ActionResult> GenerarComprobante(string id, string codigo)
        {
            RegistrarFactura r = new RegistrarFactura()
            {
                IdOrden = id,
                CodigoOrden = codigo,
                TipoComprobante = await ListarTipoRecibo(),
                FechaComprobante = DateTime.Today
            };

            return PartialView("_GenerarComprobante", r);
        }

        [HttpPost, ValidateJSAntiForgeryToken]
        public async Task<JsonResult> RegistrarComprobante([Bind(Include = "IdOrden,FechaComprobante,Registro,IdTipoComprobante")]RegistrarFactura model)
        {
            if (!ModelState.IsValid)
            {
                string msj = string.Join(Environment.NewLine, GetErrors(ModelState));
                return Json(new ErrorViewModel() { _ErrorMessage = msj, _StatusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                ReceiptRequest request = new ReceiptRequest()
                {
                    IdOrden = model.IdOrden,
                    Tipo = model.IdTipoComprobante,
                    NumeroFactura = model.Registro,
                    FechaFactura = model.FechaComprobante
                };

                var response = await _Receipt.AddAsync(request);

                if (response != null)
                    return Json(Url.Action("Index"), JsonRequestBehavior.AllowGet);
                else
                    return Json("No se pudo generar el comprobante", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ErrorViewModel() { _ErrorMessage = ex.Message, _StatusCode = HttpStatusCode.InternalServerError }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost, ValidateJSAntiForgeryToken]
        public async Task<JsonResult> ActualizarPedido(List<ProductosSeleccionados> model, string idpedido, string idcomprobante)
        {
            if (!ModelState.IsValid)
            {
                string msj = string.Join(Environment.NewLine, GetErrors(ModelState));
                return Json(new ErrorViewModel() { _ErrorMessage = msj, _StatusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                List<OrderDetailRequest> orderDetails = new List<OrderDetailRequest>();

                foreach (var item in model)
                {
                    orderDetails.Add(new OrderDetailRequest()
                    {
                        IdProducto = item.IdProducto,
                        Cantidad = item.Cantidad
                    });
                }

                //comparar listas para ver si hay diferencia entre cantidad y subtotal
                var response = await _Order.UpdateOrderDetailsAsync(idpedido, orderDetails);

                if (response == HttpStatusCode.OK)
                {
                    var URL = Url.Action("MedioPago", "Receipt", new { id = idcomprobante });
                    return Json(URL, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json("No se pudo actualizar el comprobante", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ErrorViewModel() { _ErrorMessage = ex.Message, _StatusCode = HttpStatusCode.InternalServerError }, JsonRequestBehavior.AllowGet);
            }
        }

        public IEnumerable<string> GetErrors(ModelStateDictionary modelState)
        {
            return modelState.Values.SelectMany(v => v.Errors)
                                    .Select(v => v.ErrorMessage + " " + v.Exception).ToList();

        }

        // GET: Pedido/Create
        public async Task<ActionResult> Create()
        {
            var model = new RegistrarPedidos() {
                Proveedores = await GetListProveedoresAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> GetPedidos(string idprovider)
        {
            List<ListarProductosPorProveedor> model = new List<ListarProductosPorProveedor>();

            var lista = await _Kardex.GetStockAsync(idprovider);

            foreach (var item in lista)
            {
                model.Add(new ListarProductosPorProveedor()
                {
                    IdProducto = item.Product.Id,
                    Cantidad = item.CurrentAmount,
                    Detalle = item.Product.Descripcion ?? "-",
                    PrecioUnitario = item.Product.PrecioUnitario,
                    Producto = item.Product.Nombre
                });
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> OrderSelected(List<ListarProductosPorProveedor> model)
        {
            ViewBag.IGV = await _Option.GetIGVAsync();

            return PartialView("_OrderSelected", model);
        }

        // POST: Pedido/Create
        [HttpPost, ValidateJSAntiForgeryToken]
        public async Task<JsonResult> Create(List<ProductosSeleccionados> model)
        {
            List<OrderDetailRequest> orderDetails = new List<OrderDetailRequest>();

            foreach (var item in model)
            {
                orderDetails.Add(new OrderDetailRequest() {
                    IdProducto = item.IdProducto,
                    Cantidad = item.Cantidad
                });
            }

            OrderRequest orderRequest = new OrderRequest()
            {
                IdUsuario = null,
                DetallePedido = orderDetails
            };

            var result = await _Order.AddAsync(orderRequest);

            if (result != null)
                return Json(Url.Action("Index", "Order"), JsonRequestBehavior.AllowGet);
            else
                return Json(new OkResponse() { Response = "Hubo un problema al intentar crear la órden." });
        }

        // GET: Pedido/Edit/5
        public ActionResult Edit(string id)
        {
            return View();
        }

        // POST: Pedido/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Pedido/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            var model = await _Order.DetailsAsync(id);

            EliminarOrden ep = new EliminarOrden()
            {
                Id = model.Id,
                Codigo = model.Codigo,
                Total = model.Total
            };

            return PartialView("_Delete", ep);
        }

        // POST: Pedido/Delete/5
        [HttpPost, ValidateJSAntiForgeryToken]
        public async Task<JsonResult> DeleteOrder(string id)
        {
            object respuesta = null;

            HttpStatusCode result = await _Order.DeleteAsync(id);

            if (result == HttpStatusCode.OK)
                respuesta = Url.Action("Index");
            else
                respuesta = result;

            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetAllPedidos()
        {
            var lista = await _Order.GetAllAsync();

            return Json(lista.Count(), JsonRequestBehavior.AllowGet);
        }
    }
}
