using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using MinhaEmpresa.Models;

namespace MinhaEmpresa.Utils
{
    /// <summary>
    /// Gerencia a exportação de dados para diferentes formatos
    /// </summary>
    public static class ExportManager
    {
        /// <summary>
        /// Exporta uma lista de funcionários para CSV
        /// </summary>
        /// <param name="funcionarios">Lista de funcionários a exportar</param>
        /// <param name="caminhoArquivo">Caminho do arquivo de destino</param>
        /// <param name="incluirCabecalho">Se deve incluir cabeçalho com nomes das colunas</param>
        /// <returns>True se a exportação foi bem-sucedida</returns>
        public static bool ExportarParaCSV(List<Funcionario> funcionarios, string caminhoArquivo, bool incluirCabecalho = true)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(caminhoArquivo, false, Encoding.UTF8))
                {
                    // Escrever cabeçalho
                    if (incluirCabecalho)
                    {
                        writer.WriteLine("ID,Nome,Cargo,Departamento,Salario,DataContratacao");
                    }

                    // Escrever dados
                    foreach (var funcionario in funcionarios)
                    {
                        // Escapar campos com vírgula
                        string nome = EscaparCampoCSV(funcionario.Nome);
                        string cargo = EscaparCampoCSV(funcionario.Cargo?.Nome ?? "");
                        string departamento = EscaparCampoCSV(funcionario.Departamento?.Nome ?? "");

                        writer.WriteLine($"{funcionario.Id},{nome},{cargo},{departamento},{funcionario.Salario.ToString("F2").Replace(',', '.')},{funcionario.DataContratacao.ToString("yyyy-MM-dd")}");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao exportar para CSV: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Exporta uma lista de funcionários para JSON
        /// </summary>
        /// <param name="funcionarios">Lista de funcionários a exportar</param>
        /// <param name="caminhoArquivo">Caminho do arquivo de destino</param>
        /// <returns>True se a exportação foi bem-sucedida</returns>
        public static bool ExportarParaJSON(List<Funcionario> funcionarios, string caminhoArquivo)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(funcionarios, options);
                File.WriteAllText(caminhoArquivo, jsonString, Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao exportar para JSON: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Exporta uma lista de funcionários para Excel (formato CSV compatível)
        /// </summary>
        /// <param name="funcionarios">Lista de funcionários a exportar</param>
        /// <param name="caminhoArquivo">Caminho do arquivo de destino</param>
        /// <returns>True se a exportação foi bem-sucedida</returns>
        public static bool ExportarParaExcel(List<Funcionario> funcionarios, string caminhoArquivo)
        {
            try
            {
                // Usar ponto-e-vírgula como separador para melhor compatibilidade com Excel
                using (StreamWriter writer = new StreamWriter(caminhoArquivo, false, Encoding.UTF8))
                {
                    // Escrever cabeçalho com BOM para Excel reconhecer UTF-8
                    writer.WriteLine("ID;Nome;Cargo;Departamento;Salario;DataContratacao");

                    // Escrever dados
                    foreach (var funcionario in funcionarios)
                    {
                        string nome = funcionario.Nome.Replace(";", ",");
                        string cargo = (funcionario.Cargo?.Nome ?? "").Replace(";", ",");
                        string departamento = (funcionario.Departamento?.Nome ?? "").Replace(";", ",");

                        writer.WriteLine($"{funcionario.Id};{nome};{cargo};{departamento};{funcionario.Salario.ToString("F2").Replace('.', ',')};{funcionario.DataContratacao.ToString("dd/MM/yyyy")}");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao exportar para Excel: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Escapa um campo para formato CSV
        /// </summary>
        /// <param name="campo">Campo a ser escapado</param>
        /// <returns>Campo escapado</returns>
        private static string EscaparCampoCSV(string campo)
        {
            if (string.IsNullOrEmpty(campo))
                return string.Empty;

            // Se o campo contém vírgula, aspas ou quebra de linha, envolve em aspas duplas
            if (campo.Contains(",") || campo.Contains("\"") || campo.Contains("\n") || campo.Contains("\r"))
            {
                // Substituir aspas por aspas duplas (regra do CSV)
                campo = campo.Replace("\"", "\"\"");
                return $"\"{campo}\"";
            }

            return campo;
        }

        /// <summary>
        /// Mostra uma caixa de diálogo para salvar arquivo e exporta os dados no formato selecionado
        /// </summary>
        /// <param name="funcionarios">Lista de funcionários a exportar</param>
        /// <param name="formatoInicial">Formato inicial selecionado</param>
        /// <returns>True se a exportação foi bem-sucedida</returns>
        public static bool ExportarComDialogo(List<Funcionario> funcionarios, string formatoInicial = "CSV")
        {
            using (var saveDialog = new System.Windows.Forms.SaveFileDialog())
            {
                saveDialog.Filter = "Arquivo CSV (*.csv)|*.csv|Arquivo JSON (*.json)|*.json|Arquivo Excel (*.xlsx)|*.xlsx";
                saveDialog.Title = "Exportar dados";
                saveDialog.FileName = $"Funcionarios_{DateTime.Now.ToString("yyyyMMdd")}";
                
                // Definir formato inicial
                switch (formatoInicial.ToUpper())
                {
                    case "JSON":
                        saveDialog.FilterIndex = 2;
                        break;
                    case "EXCEL":
                        saveDialog.FilterIndex = 3;
                        break;
                    default: // CSV
                        saveDialog.FilterIndex = 1;
                        break;
                }

                if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string caminhoArquivo = saveDialog.FileName;
                    string extensao = Path.GetExtension(caminhoArquivo).ToLower();

                    switch (extensao)
                    {
                        case ".json":
                            return ExportarParaJSON(funcionarios, caminhoArquivo);
                        case ".xlsx":
                            return ExportarParaExcel(funcionarios, caminhoArquivo);
                        default: // .csv
                            return ExportarParaCSV(funcionarios, caminhoArquivo);
                    }
                }
            }

            return false; // Usuário cancelou
        }
    }
}
