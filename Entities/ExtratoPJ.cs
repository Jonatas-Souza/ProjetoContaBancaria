using _5._1_Desafio_do_curso.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace _5._1_Desafio_do_curso.Entities
{
    public class ExtratoPJ : ContaPJ
    {
        #region Atributos e construtores
        public DateTime DateTime { get; private set; }
        public double Valor { get; private set; }
        public TipoTransacao TipoOp { get; private set; }

        public ExtratoPJ()
        {

        }



        public ExtratoPJ(string cnpj, DateTime dateTime, double valor, TipoTransacao tipoTransacao) : base(cnpj)
        {
            DateTime = dateTime;
            Valor = valor;
            TipoOp = tipoTransacao;
        }



        #endregion Atributos e construtores


    }
}
