using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVendas.Libraries.Texto
{
    public class Mascara
    {
        public static string Remover(string valor)
        {
            return valor.Replace("(", "").Replace(")", "").Replace("-", "").Replace(".", "").Replace("R$", "").Replace(",", "").Replace(" ", "");
        }
        /*
         * PagarMe
         * - O PagarMe recebe o valor no seguinte formato: 3310, que representa R$ 33,10
         */
        
        public static int ConverterValorPagarMe(decimal valor)
        {
            string valorString = valor.ToString("C");
            valorString = Remover(valorString);
            int valorInt = Convert.ToInt32(valorString);

            return valorInt;
        }

        public static decimal ConverterPagarMeIntToDecimal(int valor)
        {
            //10000 -> "10000" -> "100.00" -> 100.00
            string valorPagarMeString = valor.ToString();
            string valorDecimalString = valorPagarMeString.Substring(0, valorPagarMeString.Length - 2) + "," + valorPagarMeString.Substring(valorPagarMeString.Length - 2);

            var dec = decimal.Parse(valorDecimalString);

            return dec;
        }

        public static int ExtrairCodigoPedido(string codigoPedido, out string transactionId)
        {
            string[] resultadoSeparacao = codigoPedido.Split("-");

            transactionId = resultadoSeparacao[1];

            return int.Parse(resultadoSeparacao[0]);
        }
        public static string PrimeiroNome(string nomeCompleto)
        {
            string nome = nomeCompleto.Split(' ')[0];
            //if (nome.Length > 7)
            //{
            //    nome = nome.Substring(0, 7);
            //}
            return nome;
        }
        public static string mascaraCep(string valor)
        {
            return valor.Replace("-", "").Replace(".", "").Replace(" ", "");
        }
        public static string RemoverDoisPontosTraco(string valor)
        {
            return valor.Replace("-", "").Replace(":", "").Replace(" ", "");
        }
    }
}
