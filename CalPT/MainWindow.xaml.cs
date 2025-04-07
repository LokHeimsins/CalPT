using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CalPT
{
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<Participante> participantes = new ObservableCollection<Participante>();
        private readonly Random random = new Random();

        private int valorBase = 500;
        private int extraPor = 200;

        public MainWindow()
        {
            InitializeComponent();
            ParticipantesList.ItemsSource = participantes;
            ValorBaseInput.Text = valorBase.ToString();
            ExtraPorInput.Text = extraPor.ToString();
        }

        private void Agregar_Click(object sender, RoutedEventArgs e)
        {
            string nombre = NombreInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("El nombre no puede estar vacío.");
                return;
            }

            if (!int.TryParse(ValorInput.Text, out int valor) || valor < 0)
            {
                MessageBox.Show("Valor inválido. Debe ser un número entero positivo.");
                return;
            }

            if (!int.TryParse(ValorBaseInput.Text, out valorBase) || valorBase < 0 ||
                !int.TryParse(ExtraPorInput.Text, out extraPor) || extraPor <= 0)
            {
                MessageBox.Show("Los parámetros de participación deben ser números positivos válidos.");
                return;
            }

            if (valor < valorBase)
            {
                MessageBox.Show($"Solo se pueden agregar participantes con valor mayor al base ({valorBase}).");
                return;
            }

            int participaciones = CalcularParticipaciones(valor);
            participantes.Add(new Participante { Nombre = nombre, Valor = valor, Participaciones = participaciones });

            NombreInput.Clear();
            ValorInput.Clear();
        }

        private int CalcularParticipaciones(int valor)
        {
            return (valor - valorBase) / extraPor + 1;
        }

        private void Mezclar_Click(object sender, RoutedEventArgs e)
        {
            var nombres = new List<string>();
            foreach (var p in participantes)
            {
                for (int i = 0; i < p.Participaciones; i++)
                {
                    nombres.Add(p.Nombre);
                }
            }

            var mezclados = nombres.OrderBy(_ => random.Next()).ToList();
            string texto = string.Join("\n", mezclados);
            MessageBox.Show(texto, "Lista mezclada de nombres");

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Archivo de texto (*.txt)|*.txt",
                Title = "Guardar lista mezclada",
                FileName = "nombres_mezclados.txt"
            };

            if (saveDialog.ShowDialog() == true)
            {
                File.WriteAllLines(saveDialog.FileName, mezclados);
                MessageBox.Show($"Archivo guardado en: {saveDialog.FileName}");
            }
        }


        private void MostrarLista_Click(object sender, RoutedEventArgs e)
        {
            var nombres = new List<string>();
            foreach (var p in participantes)
            {
                for (int i = 0; i < p.Participaciones; i++)
                {
                    nombres.Add(p.Nombre);
                }
            }

            string texto = string.Join("\n", nombres);
            MessageBox.Show(texto, "Lista de nombres copiable");
        }

        private void CerrarDialogo_Click(object sender, RoutedEventArgs e)
        {
            GanadorDialogHost.IsOpen = false;
        }
    }

    public class Participante
    {
        public string Nombre { get; set; } = string.Empty;
        public int Valor { get; set; }
        public int Participaciones { get; set; }
    }
}
