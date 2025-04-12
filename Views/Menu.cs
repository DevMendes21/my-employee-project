using System;
using System.Windows.Forms;

namespace MyEmployeeProject.Views
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            this.Text = "Sistema de Gestão de Funcionários";
            this.Width = 400;
            this.Height = 300;
            this.StartPosition = FormStartPosition.CenterScreen;

            Button btnCadastrar = new Button
            {
                Text = "Cadastrar Funcionário",
                Width = 200,
                Height = 40,
                Location = new System.Drawing.Point(100, 50)
            };
            btnCadastrar.Click += (s, e) => new CadastroFuncionario().ShowDialog();

            Button btnListar = new Button
            {
                Text = "Listar Funcionários",
                Width = 200,
                Height = 40,
                Location = new System.Drawing.Point(100, 120)
            };
            btnListar.Click += (s, e) => new ListagemFuncionarios().ShowDialog();

            this.Controls.AddRange(new Control[] { btnCadastrar, btnListar });
        }
    }
}
