using System;
using System.Text.RegularExpressions;
using _5._1_Desafio_do_curso.Entities.Enums;

namespace _5._1_Desafio_do_curso.Entities
{
    public abstract class Clientes
    {

        #region Atributos e construtores
        public string Nome { get; protected set; }
        public TipoCliente Tipo { get; protected set; }
        public string Senha { get; protected set; }
        public double Saldo { get; protected set; }

        public Clientes()
        {
            
        }

        public Clientes(string nome, TipoCliente tipo, string senha, double saldo)
        {
            Nome = nome;
            Tipo = tipo;
            Senha = senha;
            Saldo = saldo;
        }



        #endregion Atributos e construtores


        /// <summary>
        /// Método responsável por realizar a validação das numerações de CPF e CNPJ
        /// </summary>
        /// <param name="cpf_cnpj"></param>
        /// <returns></returns>
        public static bool ValidaCpfCnpj(string cpf_cnpj)
        {

            bool soNumeros = Regex.IsMatch(cpf_cnpj, "^[0-9]+$");

            if (!soNumeros)
            {
                return false;
            }

            int tamanho = cpf_cnpj.Length;

            if (tamanho != 11 && tamanho != 14)
            {
                return false;
            }

            TipoCliente tipoCliente = ClassificaCliente(cpf_cnpj);

            int[] arrayCpfCnpj = new int[tamanho];
            for (int x = 0; x < tamanho; x++)
            {
                arrayCpfCnpj[x] = int.Parse(cpf_cnpj.Substring(x, 1));
            }


            string digitoVerificador1;
            string digitoVerificador2;
            string dvCpf;
            string dvCnpj;
            int somaDv1 = 0;
            int SomaDv2 = 0;
            int cont = 10;
            int cont2 = 11;
            int[] seq1 = new int[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] seq2 = new int[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3 };

            if (tipoCliente == TipoCliente.PF)
            {
                #region Validações de CPF

                for (int x = 0; x < 9; x++)
                {
                    int digitoCpf = arrayCpfCnpj[x];
                    somaDv1 = somaDv1 + digitoCpf * cont;
                    cont--;
                }

                int resto1 = somaDv1 * 10 % 11;

                if (resto1 == 10)
                {
                    resto1 = 0;
                }

                digitoVerificador1 = resto1.ToString();


                for (int x = 0; x < 10; x++)
                {
                    int digitoCpf = arrayCpfCnpj[x];
                    SomaDv2 = SomaDv2 + digitoCpf * cont2;
                    cont2--;
                }

                int resto2 = SomaDv2 * 10 % 11;

                if (resto2 == 10)
                {
                    resto2 = 0;
                }

                digitoVerificador2 = resto2.ToString();

                dvCpf = digitoVerificador1 + digitoVerificador2;

                if (cpf_cnpj.Substring(9, 2) != dvCpf)
                {
                    return false;
                }

                #endregion Validações de CPF
            }
            else
            {
                #region Validações de CNPJ

                for (int x = 0; x < 12; x++)
                {
                    int digitoCnpj = arrayCpfCnpj[x];
                    somaDv1 = somaDv1 + digitoCnpj * seq1[x];
                }

                int resto1 = somaDv1 % 11;

                if (resto1 < 2)
                {
                    digitoVerificador1 = "0";
                }

                digitoVerificador1 = (11 - resto1).ToString();

                for (int x = 0; x < 12; x++)
                {
                    int digitoCnpj = arrayCpfCnpj[x];
                    SomaDv2 = SomaDv2 + digitoCnpj * seq2[x];
                }

                SomaDv2 = SomaDv2 + int.Parse(digitoVerificador1) * 2;

                int resto2 = SomaDv2 % 11;

                if (resto2 < 2)
                {
                    digitoVerificador2 = "0";
                }

                digitoVerificador2 = (11 - resto2).ToString();

                dvCnpj = digitoVerificador1 + digitoVerificador2;

                if (cpf_cnpj.Substring(12, 2) != dvCnpj)
                {
                    return false;
                }

                #endregion Validações de CNPJ
            }

            return true;

        }

        /// <summary>
        /// Método responsável por classificar o tipo de cliente em CPF ou CNPJ
        /// </summary>
        /// <param name="cpf_cnpj"></param>
        /// <returns></returns>
        public static TipoCliente ClassificaCliente(string cpf_cnpj)
        {
            if (cpf_cnpj.Length == 11)
            {
                return TipoCliente.PF;
            }
            else
            {
                return TipoCliente.PJ;
            }
        }

        public virtual void DepositoConta(double valor)
        {
            Saldo += valor;

        }

        public virtual void SaqueConta(double valor)
        {
            Saldo -= valor;
        }

    }
}
