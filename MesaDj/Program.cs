using System;
using System.Collections.Generic;

class Program
{
  static void Main(string[] args)
  {
    var nomesInstrumentos = new List<string> { "bateria", "baixo", "sintetizador" };
    var faixas = new Dictionary<string, FaixaInstrumento>();
    foreach (var nome in nomesInstrumentos)
      faixas[nome] = new FaixaInstrumento(nome.First().ToString().ToUpper() + nome.Substring(1));

    foreach (var faixa in faixas.Values)
      faixa.Iniciar();

    int escolha = 0;
    do
    {
      Console.WriteLine("\n==== MESA DE DJ ====");
      Console.WriteLine("1 - Pausar instrumento");
      Console.WriteLine("2 - Retomar instrumento");
      Console.WriteLine("3 - Ver status de instrumentos");
      Console.WriteLine("4 - Sair");
      Console.Write("Escolha: ");
      if (!int.TryParse(Console.ReadLine(), out escolha))
        continue;

      if (escolha == 1 || escolha == 2)
      {
        Console.WriteLine("Qual instrumento?");
        for (int i = 0; i < nomesInstrumentos.Count; i++)
          Console.WriteLine($"{i + 1} - {nomesInstrumentos[i].Substring(0, 1).ToUpper() + nomesInstrumentos[i].Substring(1)}");

        Console.Write("Escolha: ");
        if (int.TryParse(Console.ReadLine(), out int instr) &&
            instr >= 1 && instr <= nomesInstrumentos.Count)
        {
          var nome = nomesInstrumentos[instr - 1];
          if (escolha == 1)
          {
            faixas[nome].Pausar();
            Console.WriteLine($"{nome} pausado.");
          }
          else if (escolha == 2)
          {
            faixas[nome].Retomar();
            Console.WriteLine($"{nome} retomado.");
          }
        }
        else
        {
          Console.WriteLine("Instrumento inválido!");
        }
      }
      else if (escolha == 3)
      {
        Console.WriteLine("-- Estado das faixas --");
        foreach (var kvp in faixas)
          Console.WriteLine($"{kvp.Key.ToUpper(),14}: {kvp.Value.Estado}");
      }
      else if (escolha == 4)
      {
        Console.WriteLine("Encerrando faixas...");
      }
      else
      {
        Console.WriteLine("Opção inválida!");
      }

    } while (escolha != 4);

    foreach (var faixa in faixas.Values)
      faixa.Parar();

    Console.WriteLine("Todos os instrumentos foram encerrados. Até logo, DJ!");
  }
}