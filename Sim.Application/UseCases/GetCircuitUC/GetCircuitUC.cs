using Microsoft.Extensions.Logging;
using Sim.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.UseCases.GetCircuitUC;

public interface IGetCircuitUC : IUseCase
{
    public CircuitResult GetCircuit(string fileName);
}

public class GetCircuitUC(IRepository repo, ILogger<IUseCase> logger) : IGetCircuitUC
{
    private readonly ILogger<IUseCase> _logger = logger;
    private readonly IRepository _repo = repo;

    public CircuitResult GetCircuit(string fileName)
    {
         var content = _repo.GetSchemeContent(fileName);

        return new CircuitResult
        {
            Name = fileName,
            Content = content
        };
    }
}