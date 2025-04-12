using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace MinhaEmpresa.Utils
{
    /// <summary>
    /// Classe utilitária para validação de dados
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Resultado de uma validação
        /// </summary>
        public class ValidationResult
        {
            /// <summary>
            /// Indica se a validação foi bem-sucedida
            /// </summary>
            public bool IsValid { get; set; }
            
            /// <summary>
            /// Mensagem de erro, caso a validação falhe
            /// </summary>
            public string? ErrorMessage { get; set; }
            
            /// <summary>
            /// Cria um resultado de validação bem-sucedido
            /// </summary>
            public static ValidationResult Success() => new ValidationResult { IsValid = true };
            
            /// <summary>
            /// Cria um resultado de validação com falha
            /// </summary>
            public static ValidationResult Error(string message) => new ValidationResult { IsValid = false, ErrorMessage = message };
        }
        
        /// <summary>
        /// Valida se um campo de texto não está vazio
        /// </summary>
        /// <param name="value">Valor a ser validado</param>
        /// <param name="fieldName">Nome do campo para mensagem de erro</param>
        /// <returns>Resultado da validação</returns>
        public static ValidationResult ValidateRequired(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return ValidationResult.Error($"O campo {fieldName} é obrigatório.");
            }
            
            return ValidationResult.Success();
        }
        
        /// <summary>
        /// Valida se um valor numérico está dentro de um intervalo
        /// </summary>
        /// <param name="value">Valor a ser validado</param>
        /// <param name="min">Valor mínimo</param>
        /// <param name="max">Valor máximo</param>
        /// <param name="fieldName">Nome do campo para mensagem de erro</param>
        /// <returns>Resultado da validação</returns>
        public static ValidationResult ValidateRange(decimal value, decimal min, decimal max, string fieldName)
        {
            if (value < min || value > max)
            {
                return ValidationResult.Error($"O campo {fieldName} deve estar entre {min} e {max}.");
            }
            
            return ValidationResult.Success();
        }
        
        /// <summary>
        /// Valida se uma data está dentro de um intervalo
        /// </summary>
        /// <param name="value">Data a ser validada</param>
        /// <param name="min">Data mínima</param>
        /// <param name="max">Data máxima</param>
        /// <param name="fieldName">Nome do campo para mensagem de erro</param>
        /// <returns>Resultado da validação</returns>
        public static ValidationResult ValidateDateRange(DateTime value, DateTime min, DateTime max, string fieldName)
        {
            if (value < min || value > max)
            {
                return ValidationResult.Error($"A data no campo {fieldName} deve estar entre {min.ToShortDateString()} e {max.ToShortDateString()}.");
            }
            
            return ValidationResult.Success();
        }
        
        /// <summary>
        /// Valida se um email está em formato válido
        /// </summary>
        /// <param name="email">Email a ser validado</param>
        /// <returns>Resultado da validação</returns>
        public static ValidationResult ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return ValidationResult.Error("O email é obrigatório.");
            }
            
            // Padrão de regex para validação de email
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            
            if (!Regex.IsMatch(email, pattern))
            {
                return ValidationResult.Error("O formato do email é inválido.");
            }
            
            return ValidationResult.Success();
        }
        
        /// <summary>
        /// Valida se um CPF está em formato válido
        /// </summary>
        /// <param name="cpf">CPF a ser validado</param>
        /// <returns>Resultado da validação</returns>
        public static ValidationResult ValidateCPF(string cpf)
        {
            // Remover caracteres não numéricos
            cpf = Regex.Replace(cpf, "[^0-9]", "");
            
            if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11)
            {
                return ValidationResult.Error("O CPF deve conter 11 dígitos.");
            }
            
            // Verificar se todos os dígitos são iguais
            bool allDigitsEqual = true;
            for (int i = 1; i < cpf.Length; i++)
            {
                if (cpf[i] != cpf[0])
                {
                    allDigitsEqual = false;
                    break;
                }
            }
            
            if (allDigitsEqual)
            {
                return ValidationResult.Error("CPF inválido.");
            }
            
            // Cálculo do primeiro dígito verificador
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += int.Parse(cpf[i].ToString()) * (10 - i);
            }
            
            int remainder = sum % 11;
            int digit1 = remainder < 2 ? 0 : 11 - remainder;
            
            if (int.Parse(cpf[9].ToString()) != digit1)
            {
                return ValidationResult.Error("CPF inválido.");
            }
            
            // Cálculo do segundo dígito verificador
            sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += int.Parse(cpf[i].ToString()) * (11 - i);
            }
            
            remainder = sum % 11;
            int digit2 = remainder < 2 ? 0 : 11 - remainder;
            
            if (int.Parse(cpf[10].ToString()) != digit2)
            {
                return ValidationResult.Error("CPF inválido.");
            }
            
            return ValidationResult.Success();
        }
        
        /// <summary>
        /// Valida se um CNPJ está em formato válido
        /// </summary>
        /// <param name="cnpj">CNPJ a ser validado</param>
        /// <returns>Resultado da validação</returns>
        public static ValidationResult ValidateCNPJ(string cnpj)
        {
            // Remover caracteres não numéricos
            cnpj = Regex.Replace(cnpj, "[^0-9]", "");
            
            if (string.IsNullOrWhiteSpace(cnpj) || cnpj.Length != 14)
            {
                return ValidationResult.Error("O CNPJ deve conter 14 dígitos.");
            }
            
            // Verificar se todos os dígitos são iguais
            bool allDigitsEqual = true;
            for (int i = 1; i < cnpj.Length; i++)
            {
                if (cnpj[i] != cnpj[0])
                {
                    allDigitsEqual = false;
                    break;
                }
            }
            
            if (allDigitsEqual)
            {
                return ValidationResult.Error("CNPJ inválido.");
            }
            
            // Cálculo do primeiro dígito verificador
            int[] multiplier1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int sum = 0;
            
            for (int i = 0; i < 12; i++)
            {
                sum += int.Parse(cnpj[i].ToString()) * multiplier1[i];
            }
            
            int remainder = sum % 11;
            int digit1 = remainder < 2 ? 0 : 11 - remainder;
            
            if (int.Parse(cnpj[12].ToString()) != digit1)
            {
                return ValidationResult.Error("CNPJ inválido.");
            }
            
            // Cálculo do segundo dígito verificador
            int[] multiplier2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            sum = 0;
            
            for (int i = 0; i < 13; i++)
            {
                sum += int.Parse(cnpj[i].ToString()) * multiplier2[i];
            }
            
            remainder = sum % 11;
            int digit2 = remainder < 2 ? 0 : 11 - remainder;
            
            if (int.Parse(cnpj[13].ToString()) != digit2)
            {
                return ValidationResult.Error("CNPJ inválido.");
            }
            
            return ValidationResult.Success();
        }
        
        /// <summary>
        /// Valida um formulário completo
        /// </summary>
        /// <param name="validations">Lista de validações a serem executadas</param>
        /// <returns>Lista de mensagens de erro, vazia se todas as validações passarem</returns>
        public static List<string> ValidateForm(params ValidationResult[] validations)
        {
            List<string> errors = new List<string>();
            
            foreach (var validation in validations)
            {
                if (!validation.IsValid)
                {
                    errors.Add(validation.ErrorMessage ?? "Erro de validação não especificado");
                }
            }
            
            return errors;
        }
        
        /// <summary>
        /// Formata um CPF com a máscara padrão
        /// </summary>
        /// <param name="cpf">CPF a ser formatado</param>
        /// <returns>CPF formatado</returns>
        public static string FormatCPF(string cpf)
        {
            // Remover caracteres não numéricos
            cpf = Regex.Replace(cpf, "[^0-9]", "");
            
            if (cpf.Length != 11)
            {
                return cpf;
            }
            
            return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
        }
        
        /// <summary>
        /// Formata um CNPJ com a máscara padrão
        /// </summary>
        /// <param name="cnpj">CNPJ a ser formatado</param>
        /// <returns>CNPJ formatado</returns>
        public static string FormatCNPJ(string cnpj)
        {
            // Remover caracteres não numéricos
            cnpj = Regex.Replace(cnpj, "[^0-9]", "");
            
            if (cnpj.Length != 14)
            {
                return cnpj;
            }
            
            return $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";
        }
        
        /// <summary>
        /// Formata um telefone com a máscara padrão
        /// </summary>
        /// <param name="phone">Telefone a ser formatado</param>
        /// <returns>Telefone formatado</returns>
        public static string FormatPhone(string phone)
        {
            // Remover caracteres não numéricos
            phone = Regex.Replace(phone, "[^0-9]", "");
            
            if (phone.Length < 10 || phone.Length > 11)
            {
                return phone;
            }
            
            if (phone.Length == 11) // Celular com DDD
            {
                return $"({phone.Substring(0, 2)}) {phone.Substring(2, 5)}-{phone.Substring(7, 4)}";
            }
            else // Telefone fixo com DDD
            {
                return $"({phone.Substring(0, 2)}) {phone.Substring(2, 4)}-{phone.Substring(6, 4)}";
            }
        }
    }
}
