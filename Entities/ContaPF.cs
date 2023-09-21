using System;
using System.Drawing;
using System.Globalization;
using _5._1_Desafio_do_curso.Entities.Enums;
using _5._1_Desafio_do_curso.Entities.Exceptions;

namespace _5._1_Desafio_do_curso.Entities
{
    public class ContaPF : Clientes
    {
        public string Cpf { get; protected set; }

        public static double Taxa = 1.00;

        public ContaPF()
        {
            
        }

        public ContaPF(string cpf)
        {
            Cpf = cpf;
        }

        public ContaPF(string nome, TipoCliente tipo, string senha, double saldo, string cpf) : base(nome, tipo, senha, saldo)
        {
            Cpf = cpf;
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

        public double ValidaValor(string v1)
        {
            double v2 = 0;

            if(!double.TryParse(v1, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out v2))
            {
                throw new DomainException("Valor inválido!");
            }

            if (v2 <= Taxa)
            {
                throw new DomainException("Valor informado menor ou igual a taxa cobrada por depósito!");
            }

            return v2;
        }


    }
}
