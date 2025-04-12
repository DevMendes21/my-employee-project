using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using MyEmployeeProject.Models;

namespace MyEmployeeProject.Utils
{
    public static class Export
    {
        public static bool ExportarParaCSV(List<Funcionario> funcionarios, string caminhoArquivo, bool incluirCabecalho = true)
        {
            try
            {
                using (var writer = new StreamWriter(caminhoArquivo, false, Encoding.UTF8))
                {
                    // Escrever cabeçalho
                    if (incluirCabecalho)
                    {
                        writer.WriteLine("ID,Nome,Email,Telefone,Cargo,Departamento,Salário,Data Contratação");
                    }

                    // Escrever linhas de dados
                    foreach (var funcionario in funcionarios)
                    {
                        // Escapar campos com vírgulas e aspas
                        string nome = EscaparCampoCSV(funcionario.Nome);
                        string email = EscaparCampoCSV(funcionario.Email);
                        string telefone = EscaparCampoCSV(funcionario.Telefone);
                        string cargo = EscaparCampoCSV(funcionario.Cargo?.Nome ?? "");
                        string departamento = EscaparCampoCSV(funcionario.Departamento?.Nome ?? "");
                        string dataContratacao = funcionario.DataContratacao.ToString("dd/MM/yyyy");

                        // Escrever linha
                        writer.WriteLine($"{funcionario.Id},{nome},{email},{telefone},{cargo},{departamento},{funcionario.Salario},{dataContratacao}");
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string EscaparCampoCSV(string campo)
        {
            if (string.IsNullOrEmpty(campo))
                return "";

            // Se o campo contiver vírgula, aspas ou quebra de linha, coloque entre aspas
            bool precisaEscapar = campo.Contains(",") || campo.Contains("\"") || campo.Contains("\n") || campo.Contains("\r");

            if (precisaEscapar)
            {
                // Substituir aspas por aspas duplas e colocar entre aspas
                return $"\"{campo.Replace("\"", "\"\"")}\"";
            }

            return campo;
        }
    }
}
