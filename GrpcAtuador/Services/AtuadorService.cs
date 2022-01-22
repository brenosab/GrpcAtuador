using Grpc.Core;
using GrpcGreeter;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GrpcAtuador
{
    public class Atuador
    {
        public Atuador(string nome)
        {
            Nome = nome;
            Ligado = false;
        }
        public string Nome { get; set; }
        public bool Ligado { get; set; }
    }
    public enum CodAtuador
    {
        lampada = 27,
        ar_condicionado = 12,
        incendio = 71
    }
    public class AtuadorService : Greeter.GreeterBase
    {
        private readonly ILogger<AtuadorService> _logger;
        public AtuadorService(ILogger<AtuadorService> logger)
        {
            _logger = logger;
        }

        public override Task<MsgResponse> Ligar(MsgRequest request,
            ServerCallContext context)
        {
            StreamReader r = new($"Atuadores/{request.Tipo}.json");
            string json = r.ReadToEnd();
            r.Dispose();
            r.Close();

            Atuador atuador = JsonConvert.DeserializeObject<Atuador>(json);
            bool lastState = atuador.Ligado;
            atuador.Ligado = true;

            var jsonString = JsonConvert.SerializeObject(atuador);
            File.WriteAllText($@"Atuadores/{request.Tipo}.json", jsonString);

            _logger.LogInformation($"Ligando {request.Tipo}");
            return Task.FromResult(new MsgResponse() { 
                Cod = request.Cod,
                Valid = true,
                Tipo = request.Tipo,
                Ligado = lastState
            });    
        }
        public override Task<MsgResponse> Desligar(MsgRequest request,
            ServerCallContext context)
        {
            StreamReader r = new($"Atuadores/{request.Tipo}.json");
            string json = r.ReadToEnd();
            r.Dispose();
            r.Close();

            Atuador atuador = JsonConvert.DeserializeObject<Atuador>(json);
            bool lastState = atuador.Ligado;
            atuador.Ligado = false;

            var jsonString = JsonConvert.SerializeObject(atuador);
            File.WriteAllText($@"Atuadores/{request.Tipo}.json", jsonString);

            _logger.LogInformation($"Desligando {request.Tipo}");
            return Task.FromResult(new MsgResponse()
            {
                Cod = request.Cod,
                Valid = true,
                Tipo = request.Tipo,
                Ligado = lastState
            });
        }   
    }
}
