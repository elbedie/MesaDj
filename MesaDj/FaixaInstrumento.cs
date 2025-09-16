using System;
using System.Threading;
using NAudio.Wave;
public class FaixaInstrumento
{
  private readonly string _nome;
  private Thread _thread;
  private bool _executando = false;
  private bool _pausado = false;
  private readonly object _trava = new object();
  public FaixaInstrumento(string nome)
  {
    _nome = nome;
  }
  public void Iniciar()
  {
    lock (_trava)
    {
      if (_thread == null)
      {
        _executando = true;
        _pausado = false;
        _thread = new Thread(Executar);
        _thread.IsBackground = true;
        _thread.Start();
      }
    }
  }
  public void Pausar()
  {
    lock (_trava)
    {
      if (_executando)
        _pausado = true;
    }
  }
  public void Retomar()
  {
    lock (_trava)
    {
      if (_executando && _pausado)
        _pausado = false;
    }
  }
  public void Parar()
  {
    lock (_trava)
    {
      _executando = false;
      _pausado = false;
    }
    if (_thread != null && _thread.IsAlive)
      _thread.Join();
  }
  public string Estado
  {
    get
    {
      lock (_trava)
      {
        if (!_executando)
          return "Parado";
        return _pausado ? "Pausado" : "Tocando";
      }
    }
  }
  private void Executar()
  {
    while (true)
    {
      lock (_trava)
      {
        if (!_executando)
          break;
        if (_pausado)
        {
          Monitor.Wait(_trava, 250);
          continue;
        }
      }
      TocarSomInstrumento();
      Thread.Sleep(1000);
    }
    Console.WriteLine("[{_nome}] foi parado.");
  }
  private void TocarSomInstrumento()
  {
    string arquivo = _nome.ToLower() + ".wav";
    try
    {
      using (var audioFile = new AudioFileReader(arquivo))
      using (var outputDevice = new WaveOutEvent())
      {
        outputDevice.Init(audioFile);
        outputDevice.Play();
        // Espera apenas o tempo do arquivo ou 500ms, o que for menor
        int duracao = (int)Math.Min(audioFile.TotalTime.TotalMilliseconds, 500);
        Thread.Sleep(duracao); // não bloqueia toda a thread, só a batida
        outputDevice.Stop();
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine("Erro ao tocar {arquivo}: {ex.Message}");
    }
  }
}