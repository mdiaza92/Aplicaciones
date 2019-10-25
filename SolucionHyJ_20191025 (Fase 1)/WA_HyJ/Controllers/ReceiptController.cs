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
using WA_HyJ.Models.DAO;
using WA_HyJ.Models.Request;
using WA_HyJ.Models.Response;
using WA_HyJ.Models.ViewModels;

namespace WA_HyJ.Controllers
{
    public class ReceiptController : Controller
    {
        private IOption _Option;
        private IReceipt _Receipt;
        private IOrder _Order;

        public ReceiptController()
        {
            _Option = new OptionDAO();
            _Receipt = new ReceiptDAO();
            _Order = new OrderDAO();
        }

        private async Task<IEnumerable<SelectListItem>> ListarTipoPago(string TipoPago = null)
        {
            var selectList = new List<SelectListItem>();

            var get = await _Option.GetTipoPago();

            foreach (var item in get)
            {
                selectList.Add(new SelectListItem()
                {
                    Text = item.Key,
                    Value = item.Value.ToString(),
                    Selected = (item.Value.ToString() == TipoPago)
                });
            }

            return selectList;
        }

        // GET: Receipt
        public async Task<ActionResult> Index()
        {
            var receipts = await _Receipt.GetAllAsync();

            List<ListarComprobantes> lista = new List<ListarComprobantes>();

            foreach (var item in receipts)
            {
                lista.Add(new ListarComprobantes()
                {
                    FechaRegistro = item.FechaFactura,
                    Estado = item.Estado,
                    Id = item.Id,
                    Comprobante = item.NumeroFactura,
                    Tipo = item.Tipo,
                    Validado = item.Pedido != null ? item.Pedido.Validado : false,
                    TieneMedioPago = item.TieneMedioPago
                });
            }

            return View(lista);
        }

        // GET: Receipt/Details/5
        public async Task<ActionResult> Details(string id)
        {
            try
            {
                DetalleComprobante dp = new DetalleComprobante();

                var result = await _Receipt.DetailsAsync(id);

                if (result == null)
                    return PartialView("_404");

                IMapper iMapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ReceiptResponse, DetalleComprobante>()
                        .ForMember(des => des.Id, opt => opt.MapFrom(source => source.Id))
                        .ForMember(des => des.Comprobante, opt => opt.MapFrom(source => source.NumeroFactura));
                }).CreateMapper();

                dp = iMapper.Map<ReceiptResponse, DetalleComprobante>(result);

                dp.IGV = await _Option.GetIGVAsync();

                return PartialView("_Details", dp);
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return PartialView("_500");
            }
        }

        // GET: Receipt/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Receipt/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Receipt/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Receipt/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
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

        // GET: Receipt/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            var model = await _Receipt.DetailsAsync(id);

            EliminarComprobante ep = new EliminarComprobante()
            {
                Id = model.Id,
                Pedido = model.Pedido,
                Comprobante = model.NumeroFactura,
                Tipo = model.Tipo,
                FechaRegistro = model.FechaFactura
            };

            return PartialView("_Delete", ep);
        }

        // POST: Receipt/Delete/5
        [HttpPost, ValidateJSAntiForgeryToken]
        public async Task<JsonResult> DeleteReceipt(string id)
        {
            object respuesta = null;

            HttpStatusCode result = await _Receipt.DeleteAsync(id);

            if (result == HttpStatusCode.OK)
                respuesta = Url.Action("Index");
            else
                respuesta = result;

            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }
        
        public async Task<ActionResult> MedioPago(string id)
        {
            var model = await _Receipt.DetailsAsync(id);

            ElegirMedioPago ep = new ElegirMedioPago()
            {
                IdComprobante = model.Id,
                IdPedido = model.Pedido.Id,
                MediosPago = await ListarTipoPago()
            };

            return PartialView("_MedioPago", ep);
        }

        public async Task<ActionResult> Evaluar(int idmetodopago, string idpedido, string idcomprobante)
        {
            var pedido = await _Order.DetailsAsync(idpedido);           

            if (idmetodopago != 3)
            {
                return PartialView("_GenerarEfectivoCheque");
            }

            var comprobante = await _Receipt.DetailsAsync(idcomprobante);

            GenerarLetras model = new GenerarLetras()
            {
                Cantidad = 1, //valor por default
                Total = pedido.Total.Value,
                Fecha = comprobante.FechaFactura
            };

            return PartialView("_GenerarLetras", model);
        }

        public ActionResult DetalleLetra(int cantidad, decimal total, string fechafactura)
        {
            if (cantidad < 1 || cantidad > 4)
                return null;

            List<LetraDetalle> model = new List<LetraDetalle>();

            int numero = 1;
            var temp = total / cantidad;

            int[] diasinicial = new int[] { 7, 15, 30, 60, 90 };

            var FF = DateTime.Parse(fechafactura);

            for (int i = 0; i < cantidad; i++)
            {
                model.Add(new LetraDetalle()
                {
                    Numero = numero,
                    Monto = temp,
                    Dias = diasinicial[i],
                    Fecha = FF.AddDays(diasinicial[i])
                });

                numero++;
            }

            return PartialView("_DetalleLetra", model);
        }

        [HttpPost, ValidateJSAntiForgeryToken]
        public async Task<JsonResult> GenerarMedioPago(GenerarLetras modelletra, string idcomprobante, string idpedido, int idmediopago, HttpPostedFileBase image)
        {
            try
            {
                object respuesta = null;

                if (idmediopago == 3)
                    respuesta = await EvaluarLetra(idcomprobante, idmediopago, modelletra);
                else
                    respuesta = await EvaluarOtros(idcomprobante, idmediopago, image);

                return Json(respuesta, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ErrorViewModel() { _ErrorMessage = ex.Message, _StatusCode = HttpStatusCode.InternalServerError });
            }
        }

        private async Task<object> EvaluarOtros(string idcomprobante, int idmediopago, HttpPostedFileBase image)
        {
            if (image == null) return new ErrorViewModel() { _ErrorMessage = "No se encontró la imagen a cargar.", _StatusCode = HttpStatusCode.NoContent };

            OtherMethodPaymentRequest request = new OtherMethodPaymentRequest()
            {
                TipoPago = idmediopago,
                Imagen = image
            };

            var response1 = await _Receipt.AddOtherPaymentMethodAsync(idcomprobante, request);

            Type t1 = response1.GetType();

            if (t1 == typeof(HttpStatusCode) && (HttpStatusCode)response1 == HttpStatusCode.Created)
                return Url.Action("Index");
            else
                return new ErrorViewModel() { _ErrorMessage = "Hubo un problema al actualizar el método de pago.", _StatusCode = HttpStatusCode.InternalServerError };
        }        

        private async Task<object> EvaluarLetra(string idcomprobante, int idmediopago, GenerarLetras modelletra)
        {
            if (modelletra == null) return new ErrorViewModel() { _ErrorMessage = "No se encontró la lista de letras a cargar.", _StatusCode = HttpStatusCode.NoContent };

            List<LetterDetailRequest> letters = new List<LetterDetailRequest>();

            foreach (var item in modelletra.Detalle)
            {
                letters.Add(new LetterDetailRequest()
                {
                    Fecha = item.Fecha,
                    Monto = item.Monto,
                    Dias = item.Dias
                });
            }

            LetterMethodPaymentRequest request = new LetterMethodPaymentRequest()
            {
                TipoPago = idmediopago,
                CantidadLetra = modelletra.Cantidad,
                DetalleLetra = letters
            };

            var response = await _Receipt.AddLetterPaymentMethodAsync(idcomprobante, request);

            Type t = response.GetType();

            if (t == typeof(HttpStatusCode) && (HttpStatusCode)response == HttpStatusCode.Created)
                return Url.Action("Index");
            else
                return new ErrorViewModel() { _ErrorMessage = "Hubo un problema al actualizar el método de pago.", _StatusCode = (HttpStatusCode)response };
        }
    }
}
