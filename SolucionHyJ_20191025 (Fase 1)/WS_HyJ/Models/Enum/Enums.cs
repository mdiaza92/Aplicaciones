using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace WS_HyJ.Models.Enum
{
    public static class Enums
    {
        /// <summary>
        /// Hace referencia al tipo de moneda
        /// </summary>
        public enum ECurrencyType : byte
        {
            [Description("S/.")]
            Soles = 1,

            [Description("US$")]
            Dolares = 2,
        }

        /// <summary>
        /// Hace referencia a las unidades de medida
        /// </summary>
        public enum EUnitOfMeasurement : byte
        {
            [Description("UN")]
            Unidad = 1,

            [Description("MG")]
            Miligramo = 2,

            [Description("G")]
            Gramo = 3,

            [Description("KG")]
            Kilogramo = 4,

            [Description("L")]
            Litro = 5
        }

        /// <summary>
        /// Hace referencia al estado de los pedidos
        /// </summary>
        public enum EOrderStatus : byte
        {
            [Description("Pendiente")]
            Pendiente = 1,

            [Description("Recibido")]
            Recibido = 2
        }

        /// <summary>
        /// Hace referencia al tipo de letra
        /// </summary>
        public enum ELetterStatus : byte
        {
            [Description("Cancelado")]
            Cancelado = 1
        }

        /// <summary>
        /// Hace referencia al tipo de comprobante
        /// </summary>
        public enum EReceiptType : byte
        {
            [Description("Boleta")]
            Boleta = 1,

            [Description("Factura")]
            Factura = 2
        }

        /// <summary>
        /// Hace referencia al estado de revisión del recibo
        /// </summary>
        public enum EReceiptStatus : byte
        {
            [Description("Pendiente")]
            Pendiente = 1,

            [Description("Conforme")]
            Conforme = 2
        }

        /// <summary>
        /// Hace referencia al método de pago
        /// </summary>
        public enum EPaymentType : byte
        {
            [Description("Efectivo")]
            Efectivo = 1,

            [Description("Cheque")]
            Cheque = 2,

            [Description("Letra")]
            Letra = 3
        }
    }
}
