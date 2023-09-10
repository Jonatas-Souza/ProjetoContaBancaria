using System;
using _5._1_Desafio_do_curso.Entities.Enums;

namespace _5._1_Desafio_do_curso.Entities
{
    public class ExtratoPF : ContaPF
    {
        #region Atributos e construtores
        public DateTime DateTime { get; private set; }
        public double Valor { get; private set; }
        public TipoTransacao TipoOp { get; private set; }

        public ExtratoPF()
        {
            
        }



        public ExtratoPF(string cpf, DateTime dateTime, double valor, TipoTransacao tipoTransacao) : base(cpf)
        {
            DateTime = dateTime;
            Valor = valor; 
            TipoOp = tipoTransacao;
        }



        #endregion Atributos e construtores

    }
}
