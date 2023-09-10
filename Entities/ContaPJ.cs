using System;
using _5._1_Desafio_do_curso.Entities.Enums;

namespace _5._1_Desafio_do_curso.Entities
{
    public class ContaPJ : Clientes
    {
        public string Cnpj { get; protected set; }
        
        public static double Taxa = 2.00;

        public ContaPJ()
        {

        }

        public ContaPJ(string cnpj)
        {
            Cnpj = cnpj;
        }

        public ContaPJ(string nome, TipoCliente tipo, string senha, double saldo, string cnpj) : base(nome, tipo, senha, saldo)
        {
            Cnpj = cnpj;
        }

        public override void DepositoConta(double valor)
        {
            base.DepositoConta(valor);
            Saldo -= Taxa;
        }

        public override void SaqueConta(double valor)
        {
            base.SaqueConta(valor);
            Saldo -= Taxa;
        }
    }
}
