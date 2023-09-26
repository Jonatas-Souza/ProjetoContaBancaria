/*
  Criar um console app para movimentações financeiras
  Criar clientes pessoas física e pessoa jurídica
  Criar uma lista de movimentações para cada cliente
  Descontar taxa de R$ 2,00 para PJ e R$ 1,00 para PF nas operações de saque e depósito
  
  Aplicar os conceitos de herança, polimorfisco, encapsulamento, etc.
  
 */


using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using _5._1_Desafio_do_curso.Entities;
using _5._1_Desafio_do_curso.Entities.Enums;
using System.Text.RegularExpressions;
using System.Globalization;
using _5._1_Desafio_do_curso.Data;
using _5._1_Desafio_do_curso.Entities.Exceptions;
using System.IO;

namespace _5._1_Desafio_do_curso
{
    internal class Program

    {
        static string CpfCnpj;
        static string Nome;
        static string Senha;
        static TipoCliente TipClient;
        static List<ContaPF> listaContaPF = new List<ContaPF>();
        static List<ContaPJ> listaContaPJ = new List<ContaPJ>();
        static List<ExtratoPF> listaMovimentacoesPF = new List<ExtratoPF>();
        static List<ExtratoPJ> listaMovimentacoesPJ = new List<ExtratoPJ>();



        static void Main(string[] args)
        {
            using (var dbContext = new AppDbContext())
            {
                //// Faça operações de banco de dados usando o contexto aqui
                //var produto = new Produto
                //{
                //    Nome = "Exemplo",
                //    Preco = 9.99m
                //};

                //dbContext.Produtos.Add(produto);
                dbContext.SaveChanges();
            }

            Login();
        }

        /// <summary>
        /// Método responsável pelo processo de cadastro inicial e login
        /// </summary>
        public static void Login()
        {

            Console.Clear();
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine(" Olá, seja bem vindo!");
            Console.WriteLine();
            Console.Write(" Para começarmos informe por gentileza o seu CPF ou CNPJ, digite apenas os números: ");
            CpfCnpj = Console.ReadLine();
            bool validacao = false;
            try
            {
                validacao = Clientes.ValidaCpfCnpj(CpfCnpj);
            }
            catch (DomainException e)
            {
                Console.WriteLine();
                Console.WriteLine(" AppError: " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(" SystemError: " + e.Message);
            }


            while (!validacao)
            {
                Console.WriteLine();
                Console.Write(" Informe somente os números do seu CPF ou CNPJ: ");
                CpfCnpj = Console.ReadLine();
                try
                {
                    validacao = Clientes.ValidaCpfCnpj(CpfCnpj);
                }
                catch (DomainException e)
                {
                    Console.WriteLine();
                    Console.WriteLine(" AppError: " + e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine();
                    Console.WriteLine(" SystemError: " + e.Message);
                }

            }

            bool registro;

            TipClient = Clientes.ClassificaCliente(CpfCnpj);

            if (TipClient == TipoCliente.PF)
            {
                registro = listaContaPF.Exists(x => x.Cpf == CpfCnpj);
            }
            else
            {
                registro = listaContaPJ.Exists(x => x.Cnpj == CpfCnpj);
            }


            if (!registro)
            {
                Console.Clear();
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine();
                Console.Write($@" Você ainda não possui uma conta conosco, para abrir uma nova conta digite ""S"", para sair digite ""N"": ");
                string opcao = Console.ReadLine();
                Console.WriteLine();
                while (opcao.ToUpper() != "N" && opcao.ToUpper() != "S")
                {
                    Console.Write($@" Opção inválida, digite ""S"" ou ""N"": ");
                    opcao = Console.ReadLine();
                    Console.WriteLine();
                }

                if (opcao.ToUpper() == "N")
                {
                    Login();
                }

                if (TipClient == TipoCliente.PF)
                {
                    Console.Write(" Informe seu nome: ");
                    Nome = Console.ReadLine();
                    Console.WriteLine();
                    while (String.IsNullOrWhiteSpace(Nome))
                    {
                        Console.Write(" Informe seu nome: ");
                        Nome = Console.ReadLine();
                        Console.WriteLine();
                    }
                    Console.Write(" Cadastre uma senha: ");
                    Senha = LerSenha();
                    Console.WriteLine();
                    while (String.IsNullOrWhiteSpace(Senha))
                    {
                        Console.Write(" Cadastre uma senha: ");
                        Senha = LerSenha();
                        Console.WriteLine();
                    }
                    Console.Write(" Confirme a senha criada: ");
                    string confereSenha = LerSenha();

                    while (confereSenha != Senha)
                    {
                        Console.WriteLine();
                        Console.WriteLine(" A senha digitada não confere, tente novamente!");
                        Console.WriteLine();
                        Console.Write(" Cadastre uma senha: ");
                        Senha = LerSenha();
                        Console.WriteLine();
                        while (String.IsNullOrWhiteSpace(Senha))
                        {
                            Console.Write(" Cadastre uma senha: ");
                            Senha = LerSenha();
                            Console.WriteLine();
                        }
                        Console.Write(" Confirme a senha criada: ");
                        confereSenha = LerSenha();

                    }

                    listaContaPF.Add(new ContaPF(Nome, TipClient, Senha, 0.00, CpfCnpj));

                    Console.Clear();
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine();

                    Console.WriteLine(" *** Usuário cadastrado com sucesso! ***");

                    Console.WriteLine();

                    PartDay();
                    Console.WriteLine();
                    Console.WriteLine($" {listaContaPF[listaContaPF.FindIndex(x => x.Cpf == CpfCnpj)].Nome} seja bem vindo!");
                    Console.WriteLine();
                    Console.Write(" Informe sua senha: ");
                    Senha = LerSenha();
                    string senha2 = listaContaPF[listaContaPF.FindIndex(x => x.Cpf == CpfCnpj)].Senha;

                    while (senha2 != Senha)
                    {
                        Console.WriteLine();
                        Console.Write(" Senha incorreta, tente novamente: ");
                        Senha = LerSenha();
                        senha2 = listaContaPF[listaContaPF.FindIndex(x => x.Cpf == CpfCnpj)].Senha;

                    }


                }
                else
                {
                    Console.Write(" Informe sua razão social: ");
                    Nome = Console.ReadLine();
                    Console.WriteLine();
                    while (String.IsNullOrWhiteSpace(Nome))
                    {
                        Console.Write(" Informe sua razão social: ");
                        Nome = Console.ReadLine();
                        Console.WriteLine();
                    }
                    Console.Write(" Cadastre uma senha: ");
                    Senha = LerSenha();
                    Console.WriteLine();
                    while (String.IsNullOrWhiteSpace(Senha))
                    {
                        Console.Write(" Cadastre uma senha: ");
                        Senha = LerSenha();
                        Console.WriteLine();
                    }
                    Console.Write(" Confirme a senha criada: ");
                    string confereSenha = LerSenha();

                    while (confereSenha != Senha)
                    {
                        Console.WriteLine();
                        Console.WriteLine(" A senha digitada não confere, tente novamente!");
                        Console.WriteLine();
                        Console.Write(" Cadastre uma senha: ");
                        Senha = LerSenha();
                        Console.WriteLine();
                        while (String.IsNullOrWhiteSpace(Senha))
                        {
                            Console.Write(" Cadastre uma senha: ");
                            Senha = LerSenha();
                            Console.WriteLine();
                        }
                        Console.Write(" Confirme a senha criada: ");
                        confereSenha = LerSenha();

                    }

                    listaContaPJ.Add(new ContaPJ(Nome, TipClient, Senha, 0.00, CpfCnpj));

                    Console.Clear();
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine();

                    Console.WriteLine(" *** Usuário cadastrado com sucesso! ***");

                    Console.WriteLine();

                    PartDay();
                    Console.WriteLine();
                    Console.WriteLine($" {listaContaPJ[listaContaPJ.FindIndex(x => x.Cnpj == CpfCnpj)].Nome} seja bem vindo!");
                    Console.WriteLine();
                    Console.Write(" Informe sua senha: ");
                    Senha = LerSenha();
                    string senha2 = listaContaPJ[listaContaPJ.FindIndex(x => x.Cnpj == CpfCnpj)].Senha;

                    while (senha2 != Senha)
                    {
                        Console.WriteLine();
                        Console.Write(" Senha incorreta, tente novamente: ");
                        Senha = LerSenha();
                        senha2 = listaContaPJ[listaContaPJ.FindIndex(x => x.Cnpj == CpfCnpj)].Senha;
                    }


                }

                Console.Clear();
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine(" *** Login realizado com sucesso! ***");
                Console.WriteLine();

                Menu();

            }
            else
            {
                Console.Clear();
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine();

                PartDay();
                Console.WriteLine();

                if (TipClient == TipoCliente.PF)
                {
                    Console.WriteLine($" {listaContaPF[listaContaPF.FindIndex(x => x.Cpf == CpfCnpj)].Nome} seja bem vindo!");
                    Console.WriteLine();
                    Console.Write(" Informe sua senha: ");
                    Senha = LerSenha();
                    string senha2 = listaContaPF[listaContaPF.FindIndex(x => x.Cpf == CpfCnpj)].Senha;

                    while (senha2 != Senha)
                    {
                        Console.WriteLine();
                        Console.Write(" Senha incorreta, tente novamente: ");
                        Senha = LerSenha();
                        senha2 = listaContaPF[listaContaPF.FindIndex(x => x.Cpf == CpfCnpj)].Senha;
                    }
                }
                else
                {
                    Console.WriteLine($" {listaContaPJ[listaContaPJ.FindIndex(x => x.Cnpj == CpfCnpj)].Nome} seja bem vindo!");
                    Console.WriteLine();
                    Console.Write(" Informe sua senha: ");
                    Senha = LerSenha();
                    string senha2 = listaContaPJ[listaContaPJ.FindIndex(x => x.Cnpj == CpfCnpj)].Senha;

                    while (senha2 != Senha)
                    {
                        Console.WriteLine();
                        Console.Write(" Senha incorreta, tente novamente: ");
                        Senha = LerSenha();
                        senha2 = listaContaPJ[listaContaPJ.FindIndex(x => x.Cnpj == CpfCnpj)].Senha;
                    }
                }

                Console.Clear();
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine(" *** Login realizado com sucesso! ***");
                Console.WriteLine();

                Menu();
            }

        }

        /// <summary>
        /// Método responsável pelo menu principal e direcionamento conforme opções selecionadas
        /// </summary>
        public static void Menu()
        {
            if (TipClient == TipoCliente.PF)
            {
                string cpfCnpjFormatado = CpfCnpj.Substring(0, 3) + '.' + CpfCnpj.Substring(3, 3) + '.' + CpfCnpj.Substring(6, 3) + '-' + CpfCnpj.Substring(9, 2);
                string nome = listaContaPF[listaContaPF.FindIndex(x => x.Cpf == CpfCnpj)].Nome;
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine($"                     CONTA: {cpfCnpjFormatado}                 CLIENTE: {nome}");
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine();
            }
            else
            {
                string cpfCnpjFormatado = CpfCnpj.Substring(0, 2) + '.' + CpfCnpj.Substring(2, 3) + '.' + CpfCnpj.Substring(5, 3) + '/' + CpfCnpj.Substring(8, 4) + '-' + CpfCnpj.Substring(12, 2);
                string nome = listaContaPJ[listaContaPJ.FindIndex(x => x.Cnpj == CpfCnpj)].Nome;
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine($"                     CONTA: {cpfCnpjFormatado}                 CLIENTE: {nome}");
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine();
            }
            Console.WriteLine("--------------------------------------------------- Menu Principal -----------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("                                           Selecione uma das seguintes opções:");
            Console.WriteLine();
            Console.WriteLine("                                                     1 - Extrato");
            Console.WriteLine("                                                     2 - Saque");
            Console.WriteLine("                                                     3 - Depósito");
            Console.WriteLine("                                                     4 - Sair");
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine();
            Console.Write("                                                        Opção: ");
            string opcao = Console.ReadLine();

            Console.WriteLine();

            bool soNumeros = Regex.IsMatch(opcao, "^[0-9]+$");

            if (!soNumeros || !(int.Parse(opcao) >= 1 && int.Parse(opcao) <= 4))
            {
                Console.Clear();
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine(" OPÇÃO INVÁLIDA! ");
                Console.WriteLine();
                Menu();
            }

            switch (opcao)
            {
                case "1":
                    Extrato();
                    break;
                case "2":
                    Saque();
                    break;
                case "3":
                    Deposito();
                    break;
                case "4":
                    Login();
                    break;
            }

        }

        /// <summary>
        /// Método responsável por demonstrar as transações da conta
        /// </summary>
        public static void Extrato()
        {
            Console.Clear();
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine();
            if (TipClient == TipoCliente.PF)
            {
                string cpfCnpjFormatado = CpfCnpj.Substring(0, 3) + '.' + CpfCnpj.Substring(3, 3) + '.' + CpfCnpj.Substring(6, 3) + '-' + CpfCnpj.Substring(9, 2);
                string nome = listaContaPF[listaContaPF.FindIndex(x => x.Cpf == CpfCnpj)].Nome;
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine($"                     CONTA: {cpfCnpjFormatado}                 CLIENTE: {nome}");
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine();
            }
            else
            {
                string cpfCnpjFormatado = CpfCnpj.Substring(0, 2) + '.' + CpfCnpj.Substring(2, 3) + '.' + CpfCnpj.Substring(5, 3) + '/' + CpfCnpj.Substring(8, 4) + '-' + CpfCnpj.Substring(12, 2);
                string nome = listaContaPJ[listaContaPJ.FindIndex(x => x.Cnpj == CpfCnpj)].Nome;
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine($"                     CONTA: {cpfCnpjFormatado}                 CLIENTE: {nome}");
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine();
            }
            Console.WriteLine("------------------------------------------------------- Extrato -------------------------------------------------------");
            Console.WriteLine();

            if (TipClient == TipoCliente.PF)
            {
                int count = listaMovimentacoesPF.Count(x => x.Cpf == CpfCnpj);
                string opcao;
                bool soNumeros;
                if (count == 0)
                {
                    Console.WriteLine(@" Sua conta ainda não possui nenhuma movimentação, volte no menu principal e realize um depósito!");
                    Console.WriteLine();
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine();
                    Console.Write(@" Para retornar ao menu principal digite ""1"", para sair digite ""2"": ");
                    opcao = Console.ReadLine();
                    soNumeros = Regex.IsMatch(opcao, "^[0-9]+$");

                    while (!soNumeros || !(int.Parse(opcao) >= 1 && int.Parse(opcao) <= 2))
                    {
                        Console.WriteLine();
                        Console.WriteLine(" OPÇÃO INVÁLIDA!");
                        Console.WriteLine();
                        Console.Write(@" Para retornar ao menu principal digite ""1"", para sair digite ""2"": ");
                        opcao = Console.ReadLine();
                        soNumeros = Regex.IsMatch(opcao, "^[0-9]+$");
                    }

                    if (int.Parse(opcao) == 1)
                    {
                        Console.Clear();
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine();
                        Menu();
                    }
                    else
                    {
                        Login();
                    }
                }

                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("                            |            DATA            |     |    VALOR    |     |    TIPO DE TRANSAÇÃO    |");
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                foreach (ExtratoPF extratoPF in listaMovimentacoesPF)
                {
                    if (extratoPF.Cpf == CpfCnpj)
                    {
                        Console.WriteLine($"                                  {extratoPF.DateTime}             R$ {extratoPF.Valor.ToString("F2", CultureInfo.InvariantCulture)}                   {extratoPF.TipoOp}");
                    }

                }
                Console.WriteLine();
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine($"                                             SALDO DA CONTA: R$ {listaContaPF[listaContaPF.FindIndex(x => x.Cpf == CpfCnpj)].Saldo.ToString("F2", CultureInfo.InvariantCulture)}");
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine();
                Console.Write($@" Se deseja gerar um arquivo .TXT com as transações acima do seu extrato digite ""S"" para sim, ou ""N"" para não: ");
                opcao = Console.ReadLine();
                Console.WriteLine();
                while (opcao.ToUpper() != "N" && opcao.ToUpper() != "S")
                {
                    Console.Write($@" Opção inválida, digite ""S"" ou ""N"": ");
                    opcao = Console.ReadLine();
                    Console.WriteLine();
                }
                if (opcao.ToUpper() == "S")
                {
                    try
                    {
                        string filePath = "D:\\Jônatas - SAP\\DEV\\ESTUDOS\\CURSOS - LINKEDIN\\C#\\C_Sharp_Basico_Linkedin_Thaise_Medeiros\\5.1-Desafio do curso\\AccoutnsFiles";
                        string fileName = $"Account_Transactions_{CpfCnpj}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.txt";
                        string fullFilePath = filePath + "\\" + fileName;
                        
                        List<string> fileLines = new List<string>();
                        fileLines.Add("------------------------------------------------------------------------------------------------------------------------");
                        fileLines.Add("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                        fileLines.Add("------------------------------------------------------------------------------------------------------------------------");
                        fileLines.Add("");
                        string cpfCnpjFormatado = CpfCnpj.Substring(0, 3) + '.' + CpfCnpj.Substring(3, 3) + '.' + CpfCnpj.Substring(6, 3) + '-' + CpfCnpj.Substring(9, 2);
                        string nome = listaContaPF[listaContaPF.FindIndex(x => x.Cpf == CpfCnpj)].Nome;
                        fileLines.Add("------------------------------------------------------------------------------------------------------------------------");
                        fileLines.Add($"                     CONTA: {cpfCnpjFormatado}                 CLIENTE: {nome}");
                        fileLines.Add("------------------------------------------------------------------------------------------------------------------------");
                        fileLines.Add("");
                        fileLines.Add("------------------------------------------------------- Extrato -------------------------------------------------------");
                        fileLines.Add("------------------------------------------------------------------------------------------------------------------------");
                        fileLines.Add("                            |            DATA            |     |    VALOR    |     |    TIPO DE TRANSAÇÃO    |");
                        fileLines.Add("------------------------------------------------------------------------------------------------------------------------");
                        foreach (ExtratoPF extratoPF in listaMovimentacoesPF)
                        {
                            if (extratoPF.Cpf == CpfCnpj)
                            {
                                fileLines.Add($"                                  {extratoPF.DateTime}             R$ {extratoPF.Valor.ToString("F2", CultureInfo.InvariantCulture)}                   {extratoPF.TipoOp}");
                            }

                        }
                        fileLines.Add("");
                        fileLines.Add("------------------------------------------------------------------------------------------------------------------------");
                        fileLines.Add($"                                             SALDO DA CONTA: R$ {listaContaPF[listaContaPF.FindIndex(x => x.Cpf == CpfCnpj)].Saldo.ToString("F2", CultureInfo.InvariantCulture)}");
                        fileLines.Add("------------------------------------------------------------------------------------------------------------------------");
                        fileLines.Add("");

                        File.AppendAllLines(fullFilePath, fileLines);

                        Console.WriteLine($"O arquivo {fileName} foi gerado com sucesso no diretório: {filePath}");

                        Console.WriteLine();

                    }
                    catch (IOException e)
                    {
                        Console.WriteLine("Erro na geração do arquivo:");
                        Console.WriteLine(e.Message);
                    }
                }
                Console.WriteLine();
                Console.Write(@" Para retornar ao menu principal digite ""1"", para sair digite ""2"": ");
                opcao = Console.ReadLine();
                soNumeros = Regex.IsMatch(opcao, "^[0-9]+$");

                while (!soNumeros || !(int.Parse(opcao) >= 1 && int.Parse(opcao) <= 2))
                {
                    Console.WriteLine();
                    Console.WriteLine(" OPÇÃO INVÁLIDA!");
                    Console.WriteLine();
                    Console.Write(@" Para retornar ao menu principal digite ""1"", para sair digite ""2"": ");
                    opcao = Console.ReadLine();
                    soNumeros = Regex.IsMatch(opcao, "^[0-9]+$");
                }

                if (int.Parse(opcao) == 1)
                {
                    Console.Clear();
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine();
                    Menu();
                }
                else
                {
                    Login();
                }
            }
            else
            {
                int count = listaMovimentacoesPJ.Count(x => x.Cnpj == CpfCnpj);
                string opcao;
                bool soNumeros;
                if (count == 0)
                {
                    Console.WriteLine(@" Sua conta ainda não possui nenhuma movimentação, volte no menu principal e realize um depósito!");
                    Console.WriteLine();
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine();
                    Console.Write(@" Para retornar ao menu principal digite ""1"", para sair digite ""2"": ");
                    opcao = Console.ReadLine();
                    soNumeros = Regex.IsMatch(opcao, "^[0-9]+$");

                    while (!soNumeros || !(int.Parse(opcao) >= 1 && int.Parse(opcao) <= 2))
                    {
                        Console.WriteLine();
                        Console.WriteLine(" OPÇÃO INVÁLIDA!");
                        Console.WriteLine();
                        Console.Write(@" Para retornar ao menu principal digite ""1"", para sair digite ""2"": ");
                        opcao = Console.ReadLine();
                        soNumeros = Regex.IsMatch(opcao, "^[0-9]+$");
                    }

                    if (int.Parse(opcao) == 1)
                    {
                        Console.Clear();
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine();
                        Menu();
                    }
                    else
                    {
                        Login();
                    }
                }

                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("                            |            DATA            |     |    VALOR    |     |    TIPO DE TRANSAÇÃO    |");
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                foreach (ExtratoPJ extratoPJ in listaMovimentacoesPJ)
                {
                    if (extratoPJ.Cnpj == CpfCnpj)
                    {
                        Console.WriteLine($"                                  {extratoPJ.DateTime}             R$ {extratoPJ.Valor.ToString("F2", CultureInfo.InvariantCulture)}                   {extratoPJ.TipoOp}");
                    }

                }
                Console.WriteLine();
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine($"                                             SALDO DA CONTA: R$ {listaContaPJ[listaContaPJ.FindIndex(x => x.Cnpj == CpfCnpj)].Saldo.ToString("F2", CultureInfo.InvariantCulture)}");
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine();
                Console.Write($@" Se deseja gerar um arquivo .TXT com as transações acima do seu extrato digite ""S"" para sim, ou ""N"" para não: ");
                opcao = Console.ReadLine();
                Console.WriteLine();
                while (opcao.ToUpper() != "N" && opcao.ToUpper() != "S")
                {
                    Console.Write($@" Opção inválida, digite ""S"" ou ""N"": ");
                    opcao = Console.ReadLine();
                    Console.WriteLine();
                }
                if (opcao.ToUpper() == "S")
                {
                    try
                    {
                        string filePath = "D:\\Jônatas - SAP\\DEV\\ESTUDOS\\CURSOS - LINKEDIN\\C#\\C_Sharp_Basico_Linkedin_Thaise_Medeiros\\5.1-Desafio do curso\\AccoutnsFiles";
                        string fileName = $"Account_Transactions_{CpfCnpj}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.txt";
                        string fullFilePath = filePath + "\\" + fileName;
                       
                        List<string> fileLines = new List<string>();
                        fileLines.Add("------------------------------------------------------------------------------------------------------------------------");
                        fileLines.Add("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                        fileLines.Add("------------------------------------------------------------------------------------------------------------------------");
                        fileLines.Add("");
                        string cpfCnpjFormatado = CpfCnpj.Substring(0, 2) + '.' + CpfCnpj.Substring(2, 3) + '.' + CpfCnpj.Substring(5, 3) + '/' + CpfCnpj.Substring(8, 4) + '-' + CpfCnpj.Substring(12, 2);
                        string nome = listaContaPJ[listaContaPJ.FindIndex(x => x.Cnpj == CpfCnpj)].Nome;
                        fileLines.Add("------------------------------------------------------------------------------------------------------------------------");
                        fileLines.Add($"                     CONTA: {cpfCnpjFormatado}                 CLIENTE: {nome}");
                        fileLines.Add("------------------------------------------------------------------------------------------------------------------------");
                        fileLines.Add("");
                        fileLines.Add("------------------------------------------------------- Extrato -------------------------------------------------------");
                        fileLines.Add("------------------------------------------------------------------------------------------------------------------------");
                        fileLines.Add("                            |            DATA            |     |    VALOR    |     |    TIPO DE TRANSAÇÃO    |");
                        fileLines.Add("------------------------------------------------------------------------------------------------------------------------");
                        foreach (ExtratoPJ extratoPJ in listaMovimentacoesPJ)
                        {
                            if (extratoPJ.Cnpj == CpfCnpj)
                            {
                                fileLines.Add($"                                  {extratoPJ.DateTime}             R$ {extratoPJ.Valor.ToString("F2", CultureInfo.InvariantCulture)}                   {extratoPJ.TipoOp}");
                            }

                        }
                        fileLines.Add("");
                        fileLines.Add("------------------------------------------------------------------------------------------------------------------------");
                        fileLines.Add($"                                             SALDO DA CONTA: R$ {listaContaPJ[listaContaPJ.FindIndex(x => x.Cnpj == CpfCnpj)].Saldo.ToString("F2", CultureInfo.InvariantCulture)}");
                        fileLines.Add("------------------------------------------------------------------------------------------------------------------------");
                        fileLines.Add("");

                        File.AppendAllLines(fullFilePath, fileLines);

                        Console.WriteLine($"O arquivo {fileName} foi gerado com sucesso no diretório: {filePath}");

                        Console.WriteLine();

                    }
                    catch (IOException e)
                    {
                        Console.WriteLine("Erro na geração do arquivo:");
                        Console.WriteLine(e.Message);
                    }
                    Console.Write(@" Para retornar ao menu principal digite ""1"", para sair digite ""2"": ");
                    opcao = Console.ReadLine();
                    soNumeros = Regex.IsMatch(opcao, "^[0-9]+$");

                    while (!soNumeros || !(int.Parse(opcao) >= 1 && int.Parse(opcao) <= 2))
                    {
                        Console.WriteLine();
                        Console.WriteLine(" OPÇÃO INVÁLIDA!");
                        Console.WriteLine();
                        Console.Write(@" Para retornar ao menu principal digite ""1"", para sair digite ""2"": ");
                        opcao = Console.ReadLine();
                        soNumeros = Regex.IsMatch(opcao, "^[0-9]+$");
                    }

                    if (int.Parse(opcao) == 1)
                    {
                        Console.Clear();
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine();
                        Menu();
                    }
                    else
                    {
                        Login();
                    }
                }

            }
        }
        /// <summary>
        /// Método responsável por realizar depósitos na conta
        /// </summary>
        public static void Deposito()
        {

            double valor;
            DateTime dateTime;
            if (TipClient == TipoCliente.PF)
            {
                string novoDeposito = "S";
                while (novoDeposito.ToUpper() == "S")
                {
                    valor = 0;
                    Console.Clear();
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine();

                    string cpfCnpjFormatado = CpfCnpj.Substring(0, 3) + '.' + CpfCnpj.Substring(3, 3) + '.' + CpfCnpj.Substring(6, 3) + '-' + CpfCnpj.Substring(9, 2);
                    string nome = listaContaPF[listaContaPF.FindIndex(x => x.Cpf == CpfCnpj)].Nome;
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine($"                     CONTA: {cpfCnpjFormatado}                 CLIENTE: {nome}");
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine();

                    Console.WriteLine("------------------------------------------------------- Depósito -------------------------------------------------------");
                    Console.WriteLine();
                    Console.WriteLine($" Caro cliente, sua conta é do tipo pessoa físisa e possui uma taxa de R$ {ContaPF.Taxa.ToString("F2", CultureInfo.InvariantCulture)} para cada depósito realizado!");
                    Console.WriteLine();

                    Console.Write(" Informe o valor do depósito: R$ ");
                    try
                    {
                        valor = listaContaPF[listaContaPF.FindIndex(x => x.Cpf == CpfCnpj)].ValidaValor(Console.ReadLine());
                    }
                    catch (DomainException e)
                    {
                        Console.WriteLine();
                        Console.WriteLine(" AppError: " + e.Message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine();
                        Console.WriteLine(" SystemError: " + e.Message);
                    }
                    while (valor == 0)
                    {
                        Console.WriteLine();
                        Console.Write(" Informe o valor do depósito: R$ ");
                        try
                        {
                            valor = listaContaPF[listaContaPF.FindIndex(x => x.Cpf == CpfCnpj)].ValidaValor(Console.ReadLine());
                        }
                        catch (DomainException e)
                        {
                            Console.WriteLine();
                            Console.WriteLine(" AppError: " + e.Message);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine();
                            Console.WriteLine(" SystemError: " + e.Message);
                        }
                    }

                    dateTime = DateTime.Now;
                    listaMovimentacoesPF.Add(new ExtratoPF(CpfCnpj, dateTime, valor, TipoTransacao.Deposito));
                    listaMovimentacoesPF.Add(new ExtratoPF(CpfCnpj, dateTime, -ContaPF.Taxa, TipoTransacao.Tarifa));
                    listaContaPF[listaContaPF.FindIndex(x => x.Cpf == CpfCnpj)].DepositoConta(valor);
                    Console.WriteLine();
                    Console.WriteLine(" *** Seu depósito foi realizado com sucesso! ***");
                    Console.WriteLine();
                    Console.Write(@" Deseja realizar um novo depósito? Informe ""S"" para sim ou ""N"" para não: ");
                    novoDeposito = Console.ReadLine();
                    Console.WriteLine();
                    while (novoDeposito.ToUpper() != "S" && novoDeposito.ToUpper() != "N")
                    {
                        Console.WriteLine(" OPÇÃO INVÁLIDA!");
                        Console.WriteLine();
                        Console.Write(@" Deseja realizar um novo depósito? Informe ""S"" para sim ou ""N"" para não: ");
                        novoDeposito = Console.ReadLine();
                        Console.WriteLine();
                    }

                }

                Console.Write(@" Para retornar ao menu principal digite ""1"", para sair digite ""2"": ");
                string opcao = Console.ReadLine();
                bool soNumeros = Regex.IsMatch(opcao, "^[0-9]+$");

                while (!soNumeros || !(int.Parse(opcao) >= 1 && int.Parse(opcao) <= 2))
                {
                    Console.WriteLine();
                    Console.WriteLine(" OPÇÃO INVÁLIDA!");
                    Console.WriteLine();
                    Console.Write(@" Para retornar ao menu principal digite ""1"", para sair digite ""2"": ");
                    opcao = Console.ReadLine();
                    soNumeros = Regex.IsMatch(opcao, "^[0-9]+$");
                }

                if (int.Parse(opcao) == 1)
                {
                    Console.Clear();
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine();
                    Menu();
                }
                else
                {
                    Login();
                }

            }
            else
            {
                string novoDeposito = "S";
                while (novoDeposito.ToUpper() == "S")
                {
                    valor = 0;
                    Console.Clear();
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine();

                    string cpfCnpjFormatado = CpfCnpj.Substring(0, 2) + '.' + CpfCnpj.Substring(2, 3) + '.' + CpfCnpj.Substring(5, 3) + '/' + CpfCnpj.Substring(8, 4) + '-' + CpfCnpj.Substring(12, 2);
                    string nome = listaContaPJ[listaContaPJ.FindIndex(x => x.Cnpj == CpfCnpj)].Nome;
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine($"                     CONTA: {cpfCnpjFormatado}                 CLIENTE: {nome}");
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine();

                    Console.WriteLine("------------------------------------------------------- Depósito -------------------------------------------------------");
                    Console.WriteLine();
                    Console.WriteLine($" Caro cliente, sua conta é do tipo pessoa jurídica e possui uma taxa de R$ {ContaPJ.Taxa.ToString("F2", CultureInfo.InvariantCulture)} para cada depósito realizado!");
                    Console.WriteLine();

                    Console.Write(" Informe o valor do depósito: R$ ");
                    try
                    {
                        valor = listaContaPJ[listaContaPJ.FindIndex(x => x.Cnpj == CpfCnpj)].ValidaValor(Console.ReadLine());
                    }
                    catch (DomainException e)
                    {
                        Console.WriteLine();
                        Console.WriteLine(" AppError: " + e.Message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine();
                        Console.WriteLine(" SystemError: " + e.Message);
                    }
                    while (valor == 0)
                    {
                        Console.WriteLine();
                        Console.Write(" Informe o valor do depósito: R$ ");
                        try
                        {
                            valor = listaContaPJ[listaContaPJ.FindIndex(x => x.Cnpj == CpfCnpj)].ValidaValor(Console.ReadLine());
                        }
                        catch (DomainException e)
                        {
                            Console.WriteLine();
                            Console.WriteLine(" AppError: " + e.Message);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine();
                            Console.WriteLine(" SystemError: " + e.Message);
                        }
                    }

                    dateTime = DateTime.Now;
                    listaMovimentacoesPJ.Add(new ExtratoPJ(CpfCnpj, dateTime, valor, TipoTransacao.Deposito));
                    listaMovimentacoesPJ.Add(new ExtratoPJ(CpfCnpj, dateTime, -ContaPJ.Taxa, TipoTransacao.Tarifa));
                    listaContaPJ[listaContaPJ.FindIndex(x => x.Cnpj == CpfCnpj)].DepositoConta(valor);
                    Console.WriteLine();
                    Console.WriteLine(" *** Seu depósito foi realizado com sucesso! ***");
                    Console.WriteLine();
                    Console.Write(@" Deseja realizar um novo depósito? Informe ""S"" para sim ou ""N"" para não: ");
                    novoDeposito = Console.ReadLine();
                    Console.WriteLine();
                    while (novoDeposito.ToUpper() != "S" && novoDeposito.ToUpper() != "N")
                    {
                        Console.WriteLine(" OPÇÃO INVÁLIDA!");
                        Console.WriteLine();
                        Console.Write(@" Deseja realizar um novo depósito? Informe ""S"" para sim ou ""N"" para não: ");
                        novoDeposito = Console.ReadLine();
                        Console.WriteLine();
                    }

                }

                Console.Write(@" Para retornar ao menu principal digite ""1"", para sair digite ""2"": ");
                string opcao = Console.ReadLine();
                bool soNumeros = Regex.IsMatch(opcao, "^[0-9]+$");

                while (!soNumeros || !(int.Parse(opcao) >= 1 && int.Parse(opcao) <= 2))
                {
                    Console.WriteLine();
                    Console.WriteLine(" OPÇÃO INVÁLIDA!");
                    Console.WriteLine();
                    Console.Write(@" Para retornar ao menu principal digite ""1"", para sair digite ""2"": ");
                    opcao = Console.ReadLine();
                    soNumeros = Regex.IsMatch(opcao, "^[0-9]+$");
                }

                if (int.Parse(opcao) == 1)
                {
                    Console.Clear();
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine();
                    Menu();
                }
                else
                {
                    Login();
                }
            }
        }

        /// <summary>
        /// Método responsável por realizar saques na conta
        /// </summary>
        static void Saque()
        {
            double saldoConta;
            double valor;
            DateTime dateTime;
            if (TipClient == TipoCliente.PF)
            {
                saldoConta = listaContaPF[listaContaPF.FindIndex(x => x.Cpf == CpfCnpj)].Saldo;
                string novoSaque = "S";
                string opcao;
                bool soNumeros;
                if (saldoConta <= ContaPF.Taxa)
                {
                    Console.Clear();
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine();
                    if (TipClient == TipoCliente.PF)
                    {
                        string cpfCnpjFormatado = CpfCnpj.Substring(0, 3) + '.' + CpfCnpj.Substring(3, 3) + '.' + CpfCnpj.Substring(6, 3) + '-' + CpfCnpj.Substring(9, 2);
                        string nome = listaContaPF[listaContaPF.FindIndex(x => x.Cpf == CpfCnpj)].Nome;
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine($"                     CONTA: {cpfCnpjFormatado}                 CLIENTE: {nome}");
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine();
                    }
                    else
                    {
                        string cpfCnpjFormatado = CpfCnpj.Substring(0, 2) + '.' + CpfCnpj.Substring(2, 3) + '.' + CpfCnpj.Substring(5, 3) + '/' + CpfCnpj.Substring(8, 4) + '-' + CpfCnpj.Substring(12, 2);
                        string nome = listaContaPJ[listaContaPJ.FindIndex(x => x.Cnpj == CpfCnpj)].Nome;
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine($"                     CONTA: {cpfCnpjFormatado}                 CLIENTE: {nome}");
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine();
                    }
                    Console.WriteLine("------------------------------------------------------- Saque -------------------------------------------------------");
                    Console.WriteLine();
                    Console.WriteLine($" No momento não é possível realizar saques em sua conta pois o saldo é de R$ {saldoConta.ToString("F2", CultureInfo.InvariantCulture)}!");
                    Console.WriteLine();
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine();
                    Console.Write(@" Para retornar ao menu principal digite ""1"", para sair digite ""2"": ");
                    opcao = Console.ReadLine();
                    soNumeros = Regex.IsMatch(opcao, "^[0-9]+$");

                    while (!soNumeros || !(int.Parse(opcao) >= 1 && int.Parse(opcao) <= 2))
                    {
                        Console.WriteLine();
                        Console.WriteLine(" OPÇÃO INVÁLIDA!");
                        Console.WriteLine();
                        Console.Write(@" Para retornar ao menu principal digite ""1"", para sair digite ""2"": ");
                        opcao = Console.ReadLine();
                        soNumeros = Regex.IsMatch(opcao, "^[0-9]+$");
                    }

                    if (int.Parse(opcao) == 1)
                    {
                        Console.Clear();
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine();
                        Menu();
                    }
                    else
                    {
                        Login();
                    }
                }

                while (novoSaque.ToUpper() == "S" && saldoConta > ContaPF.Taxa)
                {
                    Console.Clear();
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine();
                    if (TipClient == TipoCliente.PF)
                    {
                        string cpfCnpjFormatado = CpfCnpj.Substring(0, 3) + '.' + CpfCnpj.Substring(3, 3) + '.' + CpfCnpj.Substring(6, 3) + '-' + CpfCnpj.Substring(9, 2);
                        string nome = listaContaPF[listaContaPF.FindIndex(x => x.Cpf == CpfCnpj)].Nome;
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine($"                     CONTA: {cpfCnpjFormatado}                 CLIENTE: {nome}");
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine();
                    }
                    else
                    {
                        string cpfCnpjFormatado = CpfCnpj.Substring(0, 2) + '.' + CpfCnpj.Substring(2, 3) + '.' + CpfCnpj.Substring(5, 3) + '/' + CpfCnpj.Substring(8, 4) + '-' + CpfCnpj.Substring(12, 2);
                        string nome = listaContaPJ[listaContaPJ.FindIndex(x => x.Cnpj == CpfCnpj)].Nome;
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine($"                     CONTA: {cpfCnpjFormatado}                 CLIENTE: {nome}");
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine();
                    }
                    Console.WriteLine("------------------------------------------------------- Saque -------------------------------------------------------");
                    Console.WriteLine();
                    Console.WriteLine($" Caro cliente, sua conta é do tipo pessoa físisa e possui uma taxa de R$ {ContaPF.Taxa.ToString("F2", CultureInfo.InvariantCulture)} para cada saque realizado!");
                    Console.WriteLine();
                    Console.WriteLine($" SALDO DA CONTA: R$ {saldoConta.ToString("F2", CultureInfo.InvariantCulture)}");
                    Console.WriteLine();
                    Console.Write(" Informe o valor do saque: R$ ");
                    bool validaVal = double.TryParse(Console.ReadLine(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out valor);
                    while (!validaVal)
                    {
                        Console.WriteLine();
                        Console.WriteLine(" VALOR INVÁLIDO!");
                        Console.WriteLine();
                        Console.Write(" Informe o valor do saque: R$ ");
                        validaVal = double.TryParse(Console.ReadLine(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out valor);
                    }
                    while (valor <= ContaPF.Taxa)
                    {
                        Console.WriteLine();
                        Console.WriteLine(" Você informou um valor de saque menor ou igual a taxa cobraba por saque, tente novamente!");
                        Console.WriteLine();
                        Console.Write(" Informe o valor do saque: R$ ");
                        validaVal = double.TryParse(Console.ReadLine(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out valor);
                        while (!validaVal)
                        {
                            Console.WriteLine();
                            Console.WriteLine(" VALOR INVÁLIDO!");
                            Console.WriteLine();
                            Console.Write(" Informe o valor do saque: R$ ");
                            validaVal = double.TryParse(Console.ReadLine(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out valor);
                        }
                    }
                    while ((saldoConta - valor - ContaPF.Taxa) < 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine(" Você informou um valor de saque que fará sua conta ficar com saldo negativo, tente novamente!");
                        Console.WriteLine();
                        Console.Write(" Informe o valor do saque: R$ ");
                        validaVal = double.TryParse(Console.ReadLine(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out valor);
                        while (!validaVal)
                        {
                            Console.WriteLine();
                            Console.WriteLine(" VALOR INVÁLIDO!");
                            Console.WriteLine();
                            Console.Write(" Informe o valor do saque: R$ ");
                            validaVal = double.TryParse(Console.ReadLine(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out valor);
                        }
                    }
                    dateTime = DateTime.Now;
                    listaMovimentacoesPF.Add(new ExtratoPF(CpfCnpj, dateTime, -valor, TipoTransacao.Saque));
                    listaMovimentacoesPF.Add(new ExtratoPF(CpfCnpj, dateTime, -ContaPF.Taxa, TipoTransacao.Tarifa));
                    listaContaPF[listaContaPF.FindIndex(x => x.Cpf == CpfCnpj)].SaqueConta(valor);
                    saldoConta = listaContaPF[listaContaPF.FindIndex(x => x.Cpf == CpfCnpj)].Saldo;
                    Console.WriteLine();
                    Console.WriteLine(" *** Seu saque foi realizado com sucesso! ***");
                    Console.WriteLine();
                    Console.Write(@" Deseja realizar um novo saque? Informe ""S"" para sim ou ""N"" para não: ");
                    novoSaque = Console.ReadLine();
                    Console.WriteLine();
                    while (novoSaque.ToUpper() != "S" && novoSaque.ToUpper() != "N")
                    {
                        Console.WriteLine(" OPÇÃO INVÁLIDA!");
                        Console.WriteLine();
                        Console.Write(@" Deseja realizar um novo saque? Informe ""S"" para sim ou ""N"" para não: ");
                        novoSaque = Console.ReadLine();
                        Console.WriteLine();
                    }

                }

                if (novoSaque.ToUpper() == "S" && saldoConta <= ContaPF.Taxa)
                {
                    Saque();
                }

                Console.Write(@" Para retornar ao menu principal digite ""1"", para sair digite ""2"": ");
                opcao = Console.ReadLine();
                soNumeros = Regex.IsMatch(opcao, "^[0-9]+$");

                while (!soNumeros || !(int.Parse(opcao) >= 1 && int.Parse(opcao) <= 2))
                {
                    Console.WriteLine();
                    Console.WriteLine(" OPÇÃO INVÁLIDA!");
                    Console.WriteLine();
                    Console.Write(@" Para retornar ao menu principal digite ""1"", para sair digite ""2"": ");
                    opcao = Console.ReadLine();
                    soNumeros = Regex.IsMatch(opcao, "^[0-9]+$");
                }

                if (int.Parse(opcao) == 1)
                {
                    Console.Clear();
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine();
                    Menu();
                }
                else
                {
                    Login();
                }

            }
            else
            {
                saldoConta = listaContaPJ[listaContaPJ.FindIndex(x => x.Cnpj == CpfCnpj)].Saldo;
                string novoSaque = "S";
                string opcao;
                bool soNumeros;
                if (saldoConta <= ContaPJ.Taxa)
                {
                    Console.Clear();
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine();
                    if (TipClient == TipoCliente.PF)
                    {
                        string cpfCnpjFormatado = CpfCnpj.Substring(0, 3) + '.' + CpfCnpj.Substring(3, 3) + '.' + CpfCnpj.Substring(6, 3) + '-' + CpfCnpj.Substring(9, 2);
                        string nome = listaContaPF[listaContaPF.FindIndex(x => x.Cpf == CpfCnpj)].Nome;
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine($"                     CONTA: {cpfCnpjFormatado}                 CLIENTE: {nome}");
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine();
                    }
                    else
                    {
                        string cpfCnpjFormatado = CpfCnpj.Substring(0, 2) + '.' + CpfCnpj.Substring(2, 3) + '.' + CpfCnpj.Substring(5, 3) + '/' + CpfCnpj.Substring(8, 4) + '-' + CpfCnpj.Substring(12, 2);
                        string nome = listaContaPJ[listaContaPJ.FindIndex(x => x.Cnpj == CpfCnpj)].Nome;
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine($"                     CONTA: {cpfCnpjFormatado}                 CLIENTE: {nome}");
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine();
                    }
                    Console.WriteLine("------------------------------------------------------- Saque -------------------------------------------------------");
                    Console.WriteLine();
                    Console.WriteLine($" No momento não é possível realizar saques em sua conta pois o saldo é de R$ {saldoConta.ToString("F2", CultureInfo.InvariantCulture)}!");
                    Console.WriteLine();
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine();
                    Console.Write(@" Para retornar ao menu principal digite ""1"", para sair digite ""2"": ");
                    opcao = Console.ReadLine();
                    soNumeros = Regex.IsMatch(opcao, "^[0-9]+$");

                    while (!soNumeros || !(int.Parse(opcao) >= 1 && int.Parse(opcao) <= 2))
                    {
                        Console.WriteLine();
                        Console.WriteLine(" OPÇÃO INVÁLIDA!");
                        Console.WriteLine();
                        Console.Write(@" Para retornar ao menu principal digite ""1"", para sair digite ""2"": ");
                        opcao = Console.ReadLine();
                        soNumeros = Regex.IsMatch(opcao, "^[0-9]+$");
                    }

                    if (int.Parse(opcao) == 1)
                    {
                        Console.Clear();
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine();
                        Menu();
                    }
                    else
                    {
                        Login();
                    }
                }

                while (novoSaque.ToUpper() == "S" && saldoConta > ContaPJ.Taxa)
                {
                    Console.Clear();
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine();
                    if (TipClient == TipoCliente.PF)
                    {
                        string cpfCnpjFormatado = CpfCnpj.Substring(0, 3) + '.' + CpfCnpj.Substring(3, 3) + '.' + CpfCnpj.Substring(6, 3) + '-' + CpfCnpj.Substring(9, 2);
                        string nome = listaContaPF[listaContaPF.FindIndex(x => x.Cpf == CpfCnpj)].Nome;
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine($"                     CONTA: {cpfCnpjFormatado}                 CLIENTE: {nome}");
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine();
                    }
                    else
                    {
                        string cpfCnpjFormatado = CpfCnpj.Substring(0, 2) + '.' + CpfCnpj.Substring(2, 3) + '.' + CpfCnpj.Substring(5, 3) + '/' + CpfCnpj.Substring(8, 4) + '-' + CpfCnpj.Substring(12, 2);
                        string nome = listaContaPJ[listaContaPJ.FindIndex(x => x.Cnpj == CpfCnpj)].Nome;
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine($"                     CONTA: {cpfCnpjFormatado}                 CLIENTE: {nome}");
                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine();
                    }
                    Console.WriteLine("------------------------------------------------------- Saque -------------------------------------------------------");
                    Console.WriteLine();
                    Console.WriteLine($" Caro cliente, sua conta é do tipo pessoa jurídica e possui uma taxa de R$ {ContaPJ.Taxa.ToString("F2", CultureInfo.InvariantCulture)} para cada saque realizado!");
                    Console.WriteLine();
                    Console.WriteLine($" SALDO DA CONTA: R$ {saldoConta.ToString("F2", CultureInfo.InvariantCulture)}");
                    Console.WriteLine();
                    Console.Write(" Informe o valor do saque: R$ ");
                    bool validaVal = double.TryParse(Console.ReadLine(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out valor);
                    while (!validaVal)
                    {
                        Console.WriteLine();
                        Console.WriteLine(" VALOR INVÁLIDO!");
                        Console.WriteLine();
                        Console.Write(" Informe o valor do saque: R$ ");
                        validaVal = double.TryParse(Console.ReadLine(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out valor);
                    }
                    while (valor <= ContaPJ.Taxa)
                    {
                        Console.WriteLine();
                        Console.WriteLine(" Você informou um valor de saque menor ou igual a taxa cobraba por saque, tente novamente!");
                        Console.WriteLine();
                        Console.Write(" Informe o valor do saque: R$ ");
                        validaVal = double.TryParse(Console.ReadLine(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out valor);
                        while (!validaVal)
                        {
                            Console.WriteLine();
                            Console.WriteLine(" VALOR INVÁLIDO!");
                            Console.WriteLine();
                            Console.Write(" Informe o valor do saque: R$ ");
                            validaVal = double.TryParse(Console.ReadLine(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out valor);
                        }
                    }

                    while ((saldoConta - valor - ContaPJ.Taxa) < 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine(" Você informou um valor de saque que fará sua conta ficar com saldo negativo, tente novamente!");
                        Console.WriteLine();
                        Console.Write(" Informe o valor do saque: R$ ");
                        validaVal = double.TryParse(Console.ReadLine(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out valor);
                        while (!validaVal)
                        {
                            Console.WriteLine();
                            Console.WriteLine(" VALOR INVÁLIDO!");
                            Console.WriteLine();
                            Console.Write(" Informe o valor do saque: R$ ");
                            validaVal = double.TryParse(Console.ReadLine(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out valor);
                        }
                    }
                    dateTime = DateTime.Now;
                    listaMovimentacoesPJ.Add(new ExtratoPJ(CpfCnpj, dateTime, -valor, TipoTransacao.Saque));
                    listaMovimentacoesPJ.Add(new ExtratoPJ(CpfCnpj, dateTime, -ContaPJ.Taxa, TipoTransacao.Tarifa));
                    listaContaPJ[listaContaPJ.FindIndex(x => x.Cnpj == CpfCnpj)].SaqueConta(valor);
                    saldoConta = listaContaPJ[listaContaPJ.FindIndex(x => x.Cnpj == CpfCnpj)].Saldo;
                    Console.WriteLine();
                    Console.WriteLine(" *** Seu saque foi realizado com sucesso! ***");
                    Console.WriteLine();
                    Console.Write(@" Deseja realizar um novo saque? Informe ""S"" para sim ou ""N"" para não: ");
                    novoSaque = Console.ReadLine();
                    Console.WriteLine();
                    while (novoSaque.ToUpper() != "S" && novoSaque.ToUpper() != "N")
                    {
                        Console.WriteLine(" OPÇÃO INVÁLIDA!");
                        Console.WriteLine();
                        Console.Write(@" Deseja realizar um novo saque? Informe ""S"" para sim ou ""N"" para não: ");
                        novoSaque = Console.ReadLine();
                        Console.WriteLine();
                    }

                }

                if (novoSaque.ToUpper() == "S" && saldoConta <= ContaPJ.Taxa)
                {
                    Saque();
                }

                Console.Write(@" Para retornar ao menu principal digite ""1"", para sair digite ""2"": ");
                opcao = Console.ReadLine();
                soNumeros = Regex.IsMatch(opcao, "^[0-9]+$");

                while (!soNumeros || !(int.Parse(opcao) >= 1 && int.Parse(opcao) <= 2))
                {
                    Console.WriteLine();
                    Console.WriteLine(" OPÇÃO INVÁLIDA!");
                    Console.WriteLine();
                    Console.Write(@" Para retornar ao menu principal digite ""1"", para sair digite ""2"": ");
                    opcao = Console.ReadLine();
                    soNumeros = Regex.IsMatch(opcao, "^[0-9]+$");
                }

                if (int.Parse(opcao) == 1)
                {
                    Console.Clear();
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("     $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$          INTERNATIONAL BANK          $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine();
                    Menu();
                }
                else
                {
                    Login();
                }
            }
        }

        #region [Métodos Auxiliares]

        public static void PartDay()
        {
            int hora = DateTime.Now.Hour;
            DateTime dateTime = DateTime.Now;

            if (hora < 12)
            {
                Console.WriteLine(" " + dateTime);
                Console.WriteLine();
                Console.WriteLine(" Bom dia!");
            }
            else if (hora < 18)
            {
                Console.WriteLine(" " + dateTime);
                Console.WriteLine();
                Console.WriteLine(" Boa tarde!");
            }
            else
            {
                Console.WriteLine(" " + dateTime);
                Console.WriteLine();
                Console.WriteLine(" Boa Noite!");
            }

        }

        public static string LerSenha()
        {
            StringBuilder pw = new StringBuilder();
            bool caracterApagado = false;

            while (true)
            {
                ConsoleKeyInfo cki = Console.ReadKey(true);

                if (cki.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (deletarTexto(cki))
                {
                    if (pw.Length != 0)
                    {
                        Console.Write("\b \b");
                        pw.Length--;

                        caracterApagado = true;
                    }
                }
                else
                {
                    caracterApagado = false;
                }

                if (!caracterApagado && verificarCaracterValido(cki))
                {
                    Console.Write('*');
                    pw.Append(cki.KeyChar);
                }
            }

            return pw.ToString();
        }

        private static bool verificarCaracterValido(ConsoleKeyInfo tecla)
        {
            if (char.IsLetterOrDigit(tecla.KeyChar) || char.IsPunctuation(tecla.KeyChar) ||
                char.IsSymbol(tecla.KeyChar))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private static bool deletarTexto(ConsoleKeyInfo tecla)
        {
            if (tecla.Key == ConsoleKey.Backspace || tecla.Key == ConsoleKey.Delete)
                return true;
            else
                return false;
        }

        #endregion Métodos Auxiliares


    }
}
