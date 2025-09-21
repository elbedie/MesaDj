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
  private ManualResetEventSlim _pausaEvento = new ManualResetEventSlim(true);

  private AudioFileReader _audioFile;
  private WaveOutEvent _outputDevice;

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
      if (_executando && !_pausado)
      {
        _outputDevice?.Pause();
        _pausado = true;
        _pausaEvento.Reset();
      }
    }
  }

  public void Retomar()
  {
    lock (_trava)
    {
      if (_executando && _pausado)
      {
        _outputDevice?.Play();
        _pausado = false;
        _pausaEvento.Set();
      }
    }
  }

  public void Parar()
  {
    lock (_trava)
    {
      _executando = false;
      _pausado = false;
    }

    _outputDevice?.Stop();
    _audioFile?.Dispose();
    _outputDevice?.Dispose();

    if (_thread != null && _thread.IsAlive)
      _thread.Join();
  }

  public string Estado
  {
    get
    {
      lock (_trava)
      {
        if (!_executando) return "Parado";
        return _pausado ? "Pausado" : "Tocando";
      }
    }
  }

  private void Executar()
  {
    string arquivo = System.IO.Path.Combine("Assets", _nome.ToLower() + ".wav");
    try
    {
      while (_executando)
      {
        using (var audioFile = new AudioFileReader(arquivo))
        using (var outputDevice = new WaveOutEvent())
        {
          _audioFile = audioFile;
          _outputDevice = outputDevice;
          outputDevice.Init(audioFile);
          outputDevice.Play();

          while (_executando && outputDevice.PlaybackState != PlaybackState.Stopped)
          {
            _pausaEvento.Wait();
            Thread.Sleep(100);
          }

          outputDevice.Stop();
        }
        _audioFile = null;
        _outputDevice = null;
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Erro ao tocar {arquivo}: {ex.Message}");
    }
    finally
    {
      _audioFile?.Dispose();
      _outputDevice?.Dispose();
      Console.WriteLine($"[{_nome}] foi parado.");
    }
  }
}
